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

/** Build turn queue — interleaved round-robin by initiative, characterIdx=-1 (player chooses) */
export function buildTurnQueue(state: GameState): readonly TurnQueueEntry[] {
  // Count assigned characters per player (initiative order)
  const playerChars: { playerId: PlayerId; count: number }[] = [];
  for (const playerIdx of state.initiativeOrder) {
    const player = state.players[playerIdx];
    let count = 0;
    for (const placement of player.currentPlacements) {
      count += placement.characterIndices.length;
    }
    if (count > 0) playerChars.push({ playerId: player.id, count });
  }

  // Interleave round-robin: each round, each player places 1 character
  const queue: TurnQueueEntry[] = [];
  const maxChars = Math.max(...playerChars.map(c => c.count), 0);
  for (let round = 0; round < maxChars; round++) {
    for (const { playerId, count } of playerChars) {
      if (round < count) {
        queue.push({ playerId, characterIdx: -1 });
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
