// Assignment phase (v3: 동시 비밀배치 → 공개 → 턴큐 생성)

import type { GameState, PlayerId, TurnQueueEntry, AssignmentPhaseState } from '../types';
import { addLog } from '../state/helpers';

/** Initialize assignment phase state */
export function initAssignmentPhase(state: GameState): GameState {
  const assignmentPhase: AssignmentPhaseState = {
    playersSubmitted: [],
    currentAssigner: state.players[0]?.id ?? null,
  };
  return { ...state, assignmentPhase };
}

/** Check if all players have submitted assignments */
export function allPlayersSubmitted(state: GameState): boolean {
  const submitted = state.assignmentPhase?.playersSubmitted ?? [];
  return submitted.length >= state.players.length;
}

/** Check if all players have assigned characters (after reveal) */
export function allPlayersAssigned(state: GameState): boolean {
  return state.players.every(p => p.characters.some(c => c.assigned));
}

/** Build turn queue from assignments — sorted by initiative */
export function buildTurnQueue(state: GameState): readonly TurnQueueEntry[] {
  const queue: TurnQueueEntry[] = [];

  for (const playerIdx of state.initiativeOrder) {
    const player = state.players[playerIdx];
    for (const placement of player.currentPlacements) {
      const card = player.assignmentCards.find(c => c.id === placement.cardId);
      if (!card) continue;
      for (const charIdx of placement.characterIndices) {
        queue.push({
          playerId: player.id,
          characterIdx: charIdx,
        });
      }
    }
  }
  return queue;
}

/** Advance from ASSIGNMENT_REVEAL to PLACEMENT */
export function advanceToPlacement(state: GameState): GameState {
  if (!allPlayersAssigned(state)) return state;

  const turnQueue = buildTurnQueue(state);

  return {
    ...state,
    phase: 'PLACEMENT',
    turnQueue,
    currentTurnIdx: 0,
    assignmentPhase: null,
  };
}
