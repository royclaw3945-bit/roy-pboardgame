// Advertise phase — initiative order, each player advertises or skips

import type { GameState } from '../types';
import { addLog } from '../state/helpers';
import { initAssignmentPhase } from './assignment';

/** Check if all players have advertised and advance to ASSIGNMENT */
export function checkAdvertiseComplete(state: GameState): GameState {
  const allDone = state.players.every(p => p.hasAdvertised);
  if (!allDone) return state;

  let s: GameState = {
    ...state,
    phase: 'ASSIGNMENT',
    currentPlayerIdx: 0,
  };
  s = initAssignmentPhase(s);
  return s;
}

/** Get next player to advertise (in initiative order) */
export function getNextAdvertiser(state: GameState): number | null {
  for (const playerIdx of state.initiativeOrder) {
    if (!state.players[playerIdx].hasAdvertised) {
      return playerIdx;
    }
  }
  return null;
}
