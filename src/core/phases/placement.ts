// Placement phase — turn queue, sequential placement + actions

import type { GameState, Location, LocationSlot, PlayerId } from '../types';
import { addLog } from '../state/helpers';

/** Reset all location slots */
export function resetLocationSlots(state: GameState): GameState {
  const newSlots = { ...state.locationSlots };
  for (const loc of Object.keys(newSlots) as Location[]) {
    newSlots[loc] = newSlots[loc].map(s => ({ ...s, occupant: null }));
  }
  return { ...state, locationSlots: newSlots };
}

/** Advance to next turn in queue */
export function advanceTurn(state: GameState): GameState {
  const nextIdx = state.currentTurnIdx + 1;

  // All turns done → advance to PERFORMANCE
  if (nextIdx >= state.turnQueue.length) {
    return {
      ...state,
      phase: 'PERFORMANCE',
      currentTurnIdx: -1,
    };
  }

  return { ...state, currentTurnIdx: nextIdx };
}

/** Check if current turn's character has remaining AP */
export function currentCharacterAP(state: GameState): number {
  const turn = state.turnQueue[state.currentTurnIdx];
  if (!turn) return 0;
  const player = state.players.find(p => p.id === turn.playerId);
  if (!player) return 0;
  return player.characters[turn.characterIdx]?.ap ?? 0;
}

/** Get the location of the current turn's character */
export function currentTurnLocation(state: GameState): Location | null {
  const turn = state.turnQueue[state.currentTurnIdx];
  if (!turn) return null;
  const player = state.players.find(p => p.id === turn.playerId);
  if (!player) return null;
  return player.characters[turn.characterIdx]?.location ?? null;
}
