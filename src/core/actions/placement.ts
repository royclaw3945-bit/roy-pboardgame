// Placement phase actions: SELECT_CHARACTER, PLACE_CHARACTER, PASS_CHARACTER, CONVERT_SHARD, FINISH_ACTIONS

import type { GameState, GameAction, ValidationError, LocationSlot, PlayerId } from '../types';
import { err } from '../types';
import { registerHandler } from './registry';
import { getPlayer } from '../state/selectors';
import { updateCharacter, updatePlayer, addLog } from '../state/helpers';
import { CHARACTER_META } from '../data/constants';
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
    const slots = state.locationSlots[loc];
    if (!slots) { errors.push(err('slots', '슬롯 없음')); return errors; }
    const slot = slots[action.slotIndex];
    if (!slot) errors.push(err('slotIndex', '슬롯 인덱스 오류'));
    else if (slot.occupant) errors.push(err('occupant', '이미 점유된 슬롯'));
    return errors;
  },
  apply(state, action) {
    const player = getPlayer(state, action.playerId);
    const char = player.characters[action.characterIdx];
    const loc = char.location!;
    const slot = state.locationSlots[loc][action.slotIndex];
    const apMod = slot.apMod;
    const baseAP = CHARACTER_META[char.type].baseAP;

    // Update location slot
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
      placed: true,
      slotIndex: action.slotIndex,
      slotApMod: apMod,
      ap: baseAP + apMod,
    }));
    s = addLog(s, `${player.name}의 ${CHARACTER_META[char.type].name}을(를) ${loc} 슬롯 ${action.slotIndex}에 배치 (AP: ${baseAP + apMod})`);
    return s;
  },
});

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
