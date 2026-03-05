// dispatch(state, action) → DispatchResult
// Exhaustive action handling with validate + apply pattern

import type { GameState, GameAction, ActionResult, Location, ValidationError } from './types';
import { success, failure, err } from './types';
import { getHandler } from './actions';
import { updateCharacter } from './state/helpers';
import { advanceTurn } from './phases/placement';
import { getTrickDef } from './data/tricks';

export interface DispatchResult {
  readonly result: ActionResult;
  readonly state: GameState;
}

/** Actions that cost AP — rulebook-accurate values */
const AP_COST: Readonly<Record<string, number>> = {
  // Downtown (main actions = 3 AP)
  LEARN_TRICK: 3,
  TAKE_COINS: 3,
  HIRE: 3,
  REROLL: 1,
  SET_DIE: 2,
  // Market
  BUY: 1,
  ORDER: 1,
  QUICK_ORDER: 2,
  // Workshop — PREPARE is dynamic, handled separately
  // Theater
  SETUP_TRICK: 1,
  RESCHEDULE: 1,
  CHOOSE_WEEKDAY: 0, // v5: 배치 시 자동 선택, AP 비용 없음
  // Dark Alley
  DRAW_CARDS: 1,
  CHOOSE_CARD: 0,
  FORTUNE_TELLING: 1,
};

/** Location required for each action */
const ACTION_LOCATION: Readonly<Record<string, Location>> = {
  LEARN_TRICK: 'DOWNTOWN',
  TAKE_COINS: 'DOWNTOWN',
  HIRE: 'DOWNTOWN',
  REROLL: 'DOWNTOWN',
  SET_DIE: 'DOWNTOWN',
  CHOOSE_DIE: 'DOWNTOWN',
  BUY: 'MARKET_ROW',
  ORDER: 'MARKET_ROW',
  QUICK_ORDER: 'MARKET_ROW',
  PREPARE: 'WORKSHOP',
  SETUP_TRICK: 'THEATER',
  RESCHEDULE: 'THEATER',
  CHOOSE_WEEKDAY: 'THEATER',
  DRAW_CARDS: 'DARK_ALLEY',
  CHOOSE_CARD: 'DARK_ALLEY',
  FORTUNE_TELLING: 'DARK_ALLEY',
};

/** Get AP cost for an action (PREPARE uses dynamic trick.prepareCost) */
function getApCost(state: GameState, action: GameAction): number {
  if (action.type === 'PREPARE') {
    const player = state.players.find(p => p.id === action.playerId);
    if (player) {
      const slot = player.tricks[action.trickIdx];
      if (slot) return getTrickDef(slot.trickId).prepareCost;
    }
    return 1; // fallback
  }
  return AP_COST[action.type] ?? 0;
}

/** Validate AP + location for location actions during PLACEMENT phase */
function validatePlacementAction(state: GameState, action: GameAction): readonly ValidationError[] {
  const errors: ValidationError[] = [];
  const cost = getApCost(state, action);
  const requiredLoc = ACTION_LOCATION[action.type];

  // Only validate during PLACEMENT phase
  if (state.phase !== 'PLACEMENT') return errors;
  if (!requiredLoc) return errors;

  const turn = state.turnQueue[state.currentTurnIdx];
  if (!turn) { errors.push(err('turn', '현재 턴 없음')); return errors; }

  const playerId = 'playerId' in action ? (action as { playerId: number }).playerId : null;
  if (playerId !== null && turn.playerId !== playerId) {
    errors.push(err('turn', '현재 턴이 아닙니다'));
    return errors;
  }

  if (turn.characterIdx < 0) {
    errors.push(err('character', '먼저 캐릭터를 선택하세요'));
    return errors;
  }

  const player = state.players.find(p => p.id === turn.playerId);
  if (!player) return errors;
  const char = player.characters[turn.characterIdx];
  if (!char || !char.placed) {
    errors.push(err('placed', '캐릭터가 아직 배치되지 않았습니다'));
    return errors;
  }
  if (char.location !== requiredLoc) {
    errors.push(err('location', `${requiredLoc}에 있는 캐릭터만 이 액션을 수행할 수 있습니다`));
    return errors;
  }
  if (cost > 0 && char.ap < cost) {
    errors.push(err('ap', `AP가 부족합니다 (필요: ${cost}, 현재: ${char.ap})`));
    return errors;
  }

  return errors;
}

export function dispatch(state: GameState, action: GameAction): DispatchResult {
  // Pre-validate AP/location for location actions during PLACEMENT
  if (state.phase === 'PLACEMENT' && (action.type in ACTION_LOCATION)) {
    const apErrors = validatePlacementAction(state, action);
    if (apErrors.length > 0) {
      return { result: failure(apErrors), state };
    }
  }

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

  let newState = handler.apply(state, action);

  // Post-apply: consume AP for location actions during PLACEMENT
  if (state.phase === 'PLACEMENT' && (action.type in AP_COST || action.type === 'PREPARE')) {
    const cost = getApCost(state, action);
    const turn = state.turnQueue[state.currentTurnIdx];
    if (turn && turn.characterIdx >= 0 && cost > 0) {
      newState = updateCharacter(newState, turn.playerId, turn.characterIdx, c => ({
        ap: c.ap - cost,
      }));
      // Auto-advance when AP hits 0
      const updatedPlayer = newState.players.find(p => p.id === turn.playerId);
      const updatedChar = updatedPlayer?.characters[turn.characterIdx];
      if (updatedChar && updatedChar.ap <= 0 && newState.phase === 'PLACEMENT') {
        newState = advanceTurn(newState);
      }
    }
  }

  return { result: success(), state: newState };
}

// Exhaustive type check helper
export function assertNever(x: never): never {
  throw new Error(`Unhandled action type: ${(x as GameAction).type}`);
}
