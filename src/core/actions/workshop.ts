// Workshop action: PREPARE (v3: 컴포넌트 검증만, 소모 안 함)

import type { GameState, GameAction, ValidationError } from '../types';
import { err } from '../types';
import { registerHandler } from './registry';
import { getPlayer, hasRequiredComponents } from '../state/selectors';
import { updateTrick, addLog } from '../state/helpers';
import { getTrickDef } from '../data/tricks';
import { MARKERS_PER_SYMBOL } from '../data/constants';

type PrepareAction = Extract<GameAction, { type: 'PREPARE' }>;

registerHandler<PrepareAction>('PREPARE', {
  validate(state, action) {
    const errors: ValidationError[] = [];
    const player = state.players.find(p => p.id === action.playerId);
    if (!player) { errors.push(err('playerId', '플레이어 없음')); return errors; }
    const slot = player.tricks[action.trickIdx];
    if (!slot) { errors.push(err('trickIdx', '트릭 인덱스 오류')); return errors; }
    if (slot.prepared) errors.push(err('prepared', '이미 준비된 트릭'));
    // v3: 컴포넌트 보유 여부만 검증, 소모하지 않음
    if (!hasRequiredComponents(player, slot.trickId)) {
      errors.push(err('components', '필요 컴포넌트 미보유'));
    }
    return errors;
  },
  apply(state, action) {
    const player = getPlayer(state, action.playerId);
    const slot = player.tricks[action.trickIdx];
    const trick = getTrickDef(slot.trickId);

    // v3: 컴포넌트 소모 없이, prepared=true + 마커 슬롯만큼 markersOnTrick 증가
    let s = updateTrick(state, action.playerId, action.trickIdx, t => ({
      prepared: true,
      markersOnTrick: Math.min(t.markersOnTrick + trick.markerSlots, MARKERS_PER_SYMBOL),
    }));
    s = addLog(s, `${player.name}이(가) ${trick.nameKo} 준비 완료 (+${trick.markerSlots} 마커)`);
    return s;
  },
});
