// Action handler registry — each handler validates + applies its action

import type { GameState, GameAction, ActionResult } from '../types';

export interface ActionHandler<A extends GameAction = GameAction> {
  validate(state: GameState, action: A): readonly import('../types').ValidationError[];
  apply(state: GameState, action: A): GameState;
}

const handlers = new Map<string, ActionHandler>();

export function registerHandler<A extends GameAction>(
  type: A['type'],
  handler: ActionHandler<A>,
): void {
  handlers.set(type, handler as ActionHandler);
}

export function getHandler(type: string): ActionHandler | undefined {
  return handlers.get(type);
}

export function getAllHandlerTypes(): readonly string[] {
  return [...handlers.keys()];
}
