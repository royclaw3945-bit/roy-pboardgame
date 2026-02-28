// Phase transition actions: ADVANCE_TURN, EXECUTE_END_TURN

import type { GameState, GameAction, ValidationError } from '../types';
import { err } from '../types';
import { registerHandler } from './registry';
import { advanceTurn } from '../phases/placement';
import { executeEndTurn } from '../phases/end-turn';
import { executeSetup } from '../phases/setup';

type AdvanceAction = Extract<GameAction, { type: 'ADVANCE_TURN' }>;
type EndTurnAction = Extract<GameAction, { type: 'EXECUTE_END_TURN' }>;

registerHandler<AdvanceAction>('ADVANCE_TURN', {
  validate(state) {
    const errors: ValidationError[] = [];
    if (state.phase !== 'PLACEMENT') {
      errors.push(err('phase', '배치 페이즈가 아닙니다'));
    }
    return errors;
  },
  apply(state) {
    return advanceTurn(state);
  },
});

registerHandler<EndTurnAction>('EXECUTE_END_TURN', {
  validate(state) {
    const errors: ValidationError[] = [];
    if (state.phase !== 'END_TURN') {
      errors.push(err('phase', '턴종료 페이즈가 아닙니다'));
    }
    return errors;
  },
  apply(state) {
    return executeEndTurn(state);
  },
});
