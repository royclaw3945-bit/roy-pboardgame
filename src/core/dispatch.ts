// dispatch(state, action) → DispatchResult
// Exhaustive action handling with validate + apply pattern

import type { GameState, GameAction, ActionResult } from './types';
import { success, failure } from './types';
import { getHandler } from './actions';

export interface DispatchResult {
  readonly result: ActionResult;
  readonly state: GameState;
}

export function dispatch(state: GameState, action: GameAction): DispatchResult {
  const handler = getHandler(action.type);
  if (!handler) {
    return {
      result: failure([{ field: 'type', message: `알 수 없는 액션: ${action.type}` }]),
      state,
    };
  }

  const errors = handler.validate(state, action);
  if (errors.length > 0) {
    return { result: failure(errors), state };
  }

  const newState = handler.apply(state, action);
  return { result: success(), state: newState };
}

// Exhaustive type check helper — call this in a default case
// If a new action type is added but not handled, TypeScript will error
export function assertNever(x: never): never {
  throw new Error(`Unhandled action type: ${(x as GameAction).type}`);
}
