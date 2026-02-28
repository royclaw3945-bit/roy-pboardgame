// Advertise phase actions: ADVERTISE, SKIP_ADVERTISE, FINISH_ADVERTISE

import type { GameState, GameAction, ValidationError } from '../types';
import { err } from '../types';
import { registerHandler } from './registry';
import { getPlayer } from '../state/selectors';
import { updatePlayer, adjustFame, adjustCoins, addLog, pipe } from '../state/helpers';
import { ADVERTISE_COST, ADVERTISE_FAME } from '../data/constants';

type AdvertiseAction = Extract<GameAction, { type: 'ADVERTISE' }>;
type SkipAction = Extract<GameAction, { type: 'SKIP_ADVERTISE' }>;
type FinishAction = Extract<GameAction, { type: 'FINISH_ADVERTISE' }>;

registerHandler<AdvertiseAction>('ADVERTISE', {
  validate(state, action) {
    const errors: ValidationError[] = [];
    if (state.phase !== 'ADVERTISE') errors.push(err('phase', '광고 페이즈가 아닙니다'));
    const player = state.players.find(p => p.id === action.playerId);
    if (!player) { errors.push(err('playerId', '플레이어를 찾을 수 없습니다')); return errors; }
    if (player.hasAdvertised) errors.push(err('advertise', '이미 광고했습니다'));
    const cost = ADVERTISE_COST[Math.min(state.round - 1, ADVERTISE_COST.length - 1)];
    if (player.coins < cost) errors.push(err('coins', `코인이 부족합니다 (필요: ${cost})`));
    return errors;
  },
  apply(state, action) {
    const cost = ADVERTISE_COST[Math.min(state.round - 1, ADVERTISE_COST.length - 1)];
    const player = getPlayer(state, action.playerId);
    return pipe(state,
      s => adjustCoins(s, action.playerId, -cost),
      s => adjustFame(s, action.playerId, ADVERTISE_FAME),
      s => updatePlayer(s, action.playerId, () => ({ hasAdvertised: true })),
      s => addLog(s, `${player.name}이(가) 광고함 (-${cost}코인, +${ADVERTISE_FAME}명성)`),
    );
  },
});

registerHandler<SkipAction>('SKIP_ADVERTISE', {
  validate(state, action) {
    const errors: ValidationError[] = [];
    if (state.phase !== 'ADVERTISE') errors.push(err('phase', '광고 페이즈가 아닙니다'));
    const player = state.players.find(p => p.id === action.playerId);
    if (!player) errors.push(err('playerId', '플레이어를 찾을 수 없습니다'));
    return errors;
  },
  apply(state, action) {
    const player = getPlayer(state, action.playerId);
    return pipe(state,
      s => updatePlayer(s, action.playerId, () => ({ hasAdvertised: true })),
      s => addLog(s, `${player.name}이(가) 광고를 건너뜀`),
    );
  },
});

registerHandler<FinishAction>('FINISH_ADVERTISE', {
  validate(state) {
    const errors: ValidationError[] = [];
    if (state.phase !== 'ADVERTISE') errors.push(err('phase', '광고 페이즈가 아닙니다'));
    const allDone = state.players.every(p => p.hasAdvertised);
    if (!allDone) errors.push(err('advertise', '모든 플레이어가 광고를 완료하지 않았습니다'));
    return errors;
  },
  apply(state) {
    return { ...state, phase: 'ASSIGNMENT' };
  },
});
