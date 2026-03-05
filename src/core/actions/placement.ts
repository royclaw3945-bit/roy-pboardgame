// Placement phase actions: SELECT_CHARACTER, PLACE_CHARACTER, PASS_CHARACTER, CONVERT_SHARD, FINISH_ACTIONS

import type { GameState, GameAction, ValidationError, LocationSlot, PlayerId, Weekday, CharacterState } from '../types';
import { err } from '../types';
import { registerHandler } from './registry';
import { getPlayer } from '../state/selectors';
import { updateCharacter, updatePlayer, addLog } from '../state/helpers';
import { CHARACTER_META, WEEKDAYS, WEEKDAY_MOD } from '../data/constants';
import { advanceTurn } from '../phases/placement';

type SelectAction = Extract<GameAction, { type: 'SELECT_CHARACTER' }>;
type PlaceAction = Extract<GameAction, { type: 'PLACE_CHARACTER' }>;
type PassAction = Extract<GameAction, { type: 'PASS_CHARACTER' }>;
type ConvertAction = Extract<GameAction, { type: 'CONVERT_SHARD' }>;
type FinishAction = Extract<GameAction, { type: 'FINISH_ACTIONS' }>;

/** SELECT_CHARACTER — player chooses which assigned character to place this turn */
registerHandler<SelectAction>('SELECT_CHARACTER', {
  validate(state, action) {
    const errors: ValidationError[] = [];
    if (state.phase !== 'PLACEMENT') errors.push(err('phase', '배치 페이즈가 아닙니다'));
    const turn = state.turnQueue[state.currentTurnIdx];
    if (!turn) { errors.push(err('turn', '현재 턴 없음')); return errors; }
    if (turn.playerId !== action.playerId) errors.push(err('turn', '현재 턴이 아닙니다'));
    if (turn.characterIdx !== -1) errors.push(err('character', '이미 캐릭터를 선택했습니다'));
    const player = state.players.find(p => p.id === action.playerId);
    if (!player) { errors.push(err('playerId', '플레이어 없음')); return errors; }
    const char = player.characters[action.characterIdx];
    if (!char) { errors.push(err('charIdx', '캐릭터 인덱스 오류')); return errors; }
    if (!char.assigned) errors.push(err('assigned', '배정되지 않은 캐릭터'));
    if (char.placed) errors.push(err('placed', '이미 배치된 캐릭터'));
    return errors;
  },
  apply(state, action) {
    const player = getPlayer(state, action.playerId);
    const char = player.characters[action.characterIdx];
    // Update the current turn queue entry with the chosen characterIdx
    const newQueue = state.turnQueue.map((entry, i) =>
      i === state.currentTurnIdx
        ? { ...entry, characterIdx: action.characterIdx }
        : entry,
    );
    const s = addLog(
      { ...state, turnQueue: newQueue },
      `${player.name}이(가) ${CHARACTER_META[char.type].name}을(를) 선택`,
    );
    return s;
  },
});

registerHandler<PlaceAction>('PLACE_CHARACTER', {
  validate(state, action) {
    const errors: ValidationError[] = [];
    if (state.phase !== 'PLACEMENT') errors.push(err('phase', '배치 페이즈가 아닙니다'));
    const turn = state.turnQueue[state.currentTurnIdx];
    if (!turn || turn.playerId !== action.playerId) {
      errors.push(err('turn', '현재 턴이 아닙니다'));
      return errors;
    }
    if (turn.characterIdx === -1) errors.push(err('character', '먼저 캐릭터를 선택하세요'));
    if (turn.characterIdx !== action.characterIdx) {
      errors.push(err('charIdx', '선택한 캐릭터와 다릅니다'));
    }
    const player = state.players.find(p => p.id === action.playerId);
    if (!player) { errors.push(err('playerId', '플레이어 없음')); return errors; }
    const char = player.characters[action.characterIdx];
    if (!char) { errors.push(err('charIdx', '캐릭터 인덱스 오류')); return errors; }
    if (!char.assigned) errors.push(err('assigned', '배정되지 않은 캐릭터'));
    if (char.placed) errors.push(err('placed', '이미 배치된 캐릭터'));
    const loc = char.location;
    if (!loc) { errors.push(err('location', '로케이션 없음')); return errors; }

    // Theater: special validation
    if (loc === 'THEATER') {
      return validateTheaterPlacement(state, action, player, char, errors);
    }

    // Non-theater: use locationSlots
    const slots = state.locationSlots[loc];
    if (!slots || slots.length === 0) { errors.push(err('slots', '슬롯 없음')); return errors; }
    const slot = slots[action.slotIndex];
    if (!slot) errors.push(err('slotIndex', '슬롯 인덱스 오류'));
    else if (slot.occupant) errors.push(err('occupant', '이미 점유된 슬롯'));
    return errors;
  },
  apply(state, action) {
    const player = getPlayer(state, action.playerId);
    const char = player.characters[action.characterIdx];
    const loc = char.location!;

    // Theater: special apply
    if (loc === 'THEATER') {
      return applyTheaterPlacement(state, action, player, char);
    }

    // Non-theater: standard slot placement
    const slot = state.locationSlots[loc][action.slotIndex];
    const apMod = slot.apMod;
    const baseAP = CHARACTER_META[char.type].baseAP;
    const newSlots = state.locationSlots[loc].map((s, i) =>
      i === action.slotIndex
        ? { ...s, occupant: { playerId: action.playerId, charIdx: action.characterIdx } }
        : s,
    );
    let s: GameState = {
      ...state,
      locationSlots: { ...state.locationSlots, [loc]: newSlots },
    };
    s = updateCharacter(s, action.playerId, action.characterIdx, () => ({
      placed: true, slotIndex: action.slotIndex, slotApMod: apMod, ap: baseAP + apMod,
    }));
    s = addLog(s, `${player.name}의 ${CHARACTER_META[char.type].name}을(를) ${loc} 슬롯 ${action.slotIndex}에 배치 (AP: ${baseAP + apMod})`);
    return s;
  },
});

/** Theater placement validation */
function validateTheaterPlacement(
  state: GameState, action: Extract<GameAction, { type: 'PLACE_CHARACTER' }>,
  player: { id: PlayerId; chosenWeekday: Weekday | null }, char: CharacterState, errors: ValidationError[],
): ValidationError[] {
  const weekday = action.weekday;
  if (!weekday) { errors.push(err('weekday', '극장 배치 시 요일을 지정해야 합니다')); return errors; }
  if (!WEEKDAYS.includes(weekday)) { errors.push(err('weekday', '잘못된 요일')); return errors; }

  const slotType = action.theaterSlotType ?? 'BACKSTAGE';

  // 한 플레이어는 한 요일에만 배치 가능
  if (player.chosenWeekday !== null && player.chosenWeekday !== weekday) {
    errors.push(err('weekday', '이미 다른 요일에 캐릭터를 배치했습니다'));
    return errors;
  }

  // 다른 플레이어가 이미 점유한 요일인지
  const currentPerformer = state.theater.weekdayPerformers[weekday];
  if (currentPerformer !== null && currentPerformer !== action.playerId) {
    errors.push(err('weekday', '상대 플레이어가 이미 해당 요일을 점유'));
    return errors;
  }

  const daySlot = state.theater.weekdaySlots[weekday];

  if (slotType === 'PERFORM') {
    // 퍼포먼스 슬롯: 마법사만
    if (char.type !== 'MAGICIAN') {
      errors.push(err('character', '퍼포먼스 슬롯은 마법사만 배치 가능'));
    }
    if (daySlot.performSlot.occupant) {
      errors.push(err('occupant', '퍼포먼스 슬롯이 이미 점유됨'));
    }
  } else {
    // 백스테이지: slotIndex로 어느 백스테이지 슬롯인지
    const bsSlot = daySlot.backstage[action.slotIndex];
    if (!bsSlot) errors.push(err('slotIndex', '백스테이지 슬롯 인덱스 오류'));
    else if (bsSlot.occupant) errors.push(err('occupant', '이미 점유된 슬롯'));
  }

  return errors;
}

/** Theater placement apply */
function applyTheaterPlacement(
  state: GameState, action: Extract<GameAction, { type: 'PLACE_CHARACTER' }>,
  player: { id: PlayerId; name: string }, char: CharacterState,
): GameState {
  const weekday = action.weekday!;
  const slotType = action.theaterSlotType ?? 'BACKSTAGE';
  const baseAP = CHARACTER_META[char.type].baseAP;
  const daySlot = state.theater.weekdaySlots[weekday];
  const occupantInfo = { playerId: action.playerId, charIdx: action.characterIdx };

  let apMod = 0;
  let newDaySlot = daySlot;

  if (slotType === 'PERFORM') {
    // 퍼포먼스 슬롯: AP 보정 없음 (마법사 3AP 기본)
    newDaySlot = { ...daySlot, performSlot: { occupant: occupantInfo } };
  } else {
    // 백스테이지
    apMod = daySlot.backstage[action.slotIndex]?.apMod ?? 0;
    const newBackstage = daySlot.backstage.map((bs, i) =>
      i === action.slotIndex ? { ...bs, occupant: occupantInfo } : bs,
    );
    newDaySlot = { ...daySlot, backstage: newBackstage };
  }

  let s: GameState = {
    ...state,
    theater: {
      ...state.theater,
      weekdayPerformers: { ...state.theater.weekdayPerformers, [weekday]: action.playerId },
      weekdaySlots: { ...state.theater.weekdaySlots, [weekday]: newDaySlot },
    },
  };

  s = updateCharacter(s, action.playerId, action.characterIdx, () => ({
    placed: true, slotIndex: action.slotIndex, slotApMod: apMod, ap: baseAP + apMod,
  }));
  s = updatePlayer(s, action.playerId, () => ({ chosenWeekday: weekday }));

  const dayName = WEEKDAY_MOD[weekday].name;
  const slotName = slotType === 'PERFORM' ? '퍼포먼스' : '백스테이지';
  s = addLog(s, `${player.name}의 ${CHARACTER_META[char.type].name}을(를) 극장 ${dayName} ${slotName}에 배치 (AP: ${baseAP + apMod})`);
  return s;
}

/** FINISH_ACTIONS — end current character's action phase and advance turn */
registerHandler<FinishAction>('FINISH_ACTIONS', {
  validate(state, action) {
    const errors: ValidationError[] = [];
    if (state.phase !== 'PLACEMENT') errors.push(err('phase', '배치 페이즈가 아닙니다'));
    const turn = state.turnQueue[state.currentTurnIdx];
    if (!turn || turn.playerId !== action.playerId) {
      errors.push(err('turn', '현재 턴이 아닙니다'));
    }
    return errors;
  },
  apply(state, action) {
    const turn = state.turnQueue[state.currentTurnIdx];
    let s = state;
    // Zero out remaining AP
    if (turn && turn.characterIdx >= 0) {
      s = updateCharacter(s, action.playerId, turn.characterIdx, () => ({ ap: 0 }));
    }
    const player = getPlayer(s, action.playerId);
    s = addLog(s, `${player.name}이(가) 액션 종료`);
    return advanceTurn(s);
  },
});

registerHandler<PassAction>('PASS_CHARACTER', {
  validate(state, action) {
    const errors: ValidationError[] = [];
    if (state.phase !== 'PLACEMENT') errors.push(err('phase', '배치 페이즈가 아닙니다'));
    const turn = state.turnQueue[state.currentTurnIdx];
    if (!turn || turn.playerId !== action.playerId) {
      errors.push(err('turn', '현재 턴이 아닙니다'));
    }
    return errors;
  },
  apply(state, action) {
    const player = getPlayer(state, action.playerId);
    let s = addLog(state, `${player.name}이(가) 패스`);
    return advanceTurn(s);
  },
});

registerHandler<ConvertAction>('CONVERT_SHARD', {
  validate(state, action) {
    const errors: ValidationError[] = [];
    if (state.phase !== 'PLACEMENT') errors.push(err('phase', '배치 페이즈가 아닙니다'));
    const player = state.players.find(p => p.id === action.playerId);
    if (!player) { errors.push(err('playerId', '플레이어 없음')); return errors; }
    if (player.shards < 1) errors.push(err('shards', '샤드가 없습니다'));
    const turn = state.turnQueue[state.currentTurnIdx];
    if (turn && turn.characterIdx >= 0) {
      const char = player.characters[turn.characterIdx];
      if (char?.shardConverted) errors.push(err('shard', '이미 샤드 변환함'));
      // 극장에서는 샤드 강화 불가
      if (char?.location === 'THEATER') errors.push(err('location', '극장에서는 샤드 강화 불가'));
    }
    return errors;
  },
  apply(state, action) {
    const player = getPlayer(state, action.playerId);
    const turn = state.turnQueue[state.currentTurnIdx];
    let s = updatePlayer(state, action.playerId, p => ({
      shards: p.shards - 1,
    }));
    if (turn && turn.characterIdx >= 0) {
      s = updateCharacter(s, action.playerId, turn.characterIdx, c => ({
        shardConverted: true, ap: c.ap + 1,
      }));
    }
    s = addLog(s, `${player.name}이(가) 샤드 변환 (+1 AP)`);
    return s;
  },
});
