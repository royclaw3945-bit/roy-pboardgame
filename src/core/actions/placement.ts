// Placement phase actions: PLACE_CHARACTER, PASS_CHARACTER, CONVERT_SHARD

import type { GameState, GameAction, ValidationError, LocationSlot, PlayerId } from '../types';
import { err } from '../types';
import { registerHandler } from './registry';
import { getPlayer } from '../state/selectors';
import { updateCharacter, updatePlayer, addLog, pipe } from '../state/helpers';
import { CHARACTER_META } from '../data/constants';

type PlaceAction = Extract<GameAction, { type: 'PLACE_CHARACTER' }>;
type PassAction = Extract<GameAction, { type: 'PASS_CHARACTER' }>;
type ConvertAction = Extract<GameAction, { type: 'CONVERT_SHARD' }>;

registerHandler<PlaceAction>('PLACE_CHARACTER', {
  validate(state, action) {
    const errors: ValidationError[] = [];
    if (state.phase !== 'PLACEMENT') errors.push(err('phase', '배치 페이즈가 아닙니다'));
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
    s = addLog(s, `${player.name}의 ${char.type}을(를) ${loc} 슬롯 ${action.slotIndex}에 배치 (AP: ${baseAP + apMod})`);
    return s;
  },
});

registerHandler<PassAction>('PASS_CHARACTER', {
  validate(state, action) {
    const errors: ValidationError[] = [];
    if (state.phase !== 'PLACEMENT') errors.push(err('phase', '배치 페이즈가 아닙니다'));
    const player = state.players.find(p => p.id === action.playerId);
    if (!player) errors.push(err('playerId', '플레이어 없음'));
    return errors;
  },
  apply(state, action) {
    const player = getPlayer(state, action.playerId);
    return addLog(state, `${player.name}이(가) 패스`);
  },
});

registerHandler<ConvertAction>('CONVERT_SHARD', {
  validate(state, action) {
    const errors: ValidationError[] = [];
    if (state.phase !== 'PLACEMENT') errors.push(err('phase', '배치 페이즈가 아닙니다'));
    const player = state.players.find(p => p.id === action.playerId);
    if (!player) { errors.push(err('playerId', '플레이어 없음')); return errors; }
    if (player.shards < 1) errors.push(err('shards', '샤드가 없습니다'));
    // Check current turn character hasn't converted yet
    const turn = state.turnQueue[state.currentTurnIdx];
    if (turn) {
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
    if (turn) {
      s = updateCharacter(s, action.playerId, turn.characterIdx, () => ({
        shardConverted: true, ap: s.players.find(p => p.id === action.playerId)!.characters[turn.characterIdx].ap + 1,
      }));
    }
    s = addLog(s, `${player.name}이(가) 샤드 변환 (+1 AP)`);
    return s;
  },
});
