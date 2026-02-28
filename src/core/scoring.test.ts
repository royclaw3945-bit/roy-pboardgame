// Stage 7 Tests — scoring + 7-round simulation (v3)

import { describe, it, expect } from 'vitest';
import { createGame } from './state/init';
import { dispatch } from './dispatch';
import { executeSetup } from './phases/setup';
import { checkAdvertiseComplete } from './phases/advertise';
import { initAssignmentPhase, advanceToPlacement } from './phases/assignment';
import { advanceTurn } from './phases/placement';
import { advanceToEndTurn } from './phases/performance';
import { executeEndTurn } from './phases/end-turn';
import { calculateScores, applyFinalScoring } from './scoring';
import { updatePlayer } from './state/helpers';
import type { PlayerId, TrickId, SymbolIndex, PlayerConfig, GameState } from './types';

const CONFIGS: readonly PlayerConfig[] = [
  { name: 'Alice', magicianId: 'MECHANIKER', isHuman: true },
  { name: 'Bob', magicianId: 'OPTICIAN', isHuman: false },
];

/** Helper: v3 assignment — both players place DOWNTOWN card with magician */
function assignBothPlayers(state: GameState): GameState {
  let s = { ...state, phase: 'ASSIGNMENT' as const };
  s = initAssignmentPhase(s);

  // Player 0
  const p0card = s.players[0].assignmentCards.find(c => c.location === 'DOWNTOWN')!;
  const { state: s1 } = dispatch(s, {
    type: 'PLACE_ASSIGNMENT_CARD', playerId: 0 as PlayerId,
    cardId: p0card.id, characterIndices: [0],
  });
  const { state: s2 } = dispatch(s1, {
    type: 'SUBMIT_ASSIGNMENT', playerId: 0 as PlayerId,
  });

  // Player 1
  const p1card = s2.players[1].assignmentCards.find(c => c.location === 'DOWNTOWN')!;
  const { state: s3 } = dispatch(s2, {
    type: 'PLACE_ASSIGNMENT_CARD', playerId: 1 as PlayerId,
    cardId: p1card.id, characterIndices: [0],
  });
  const { state: s4 } = dispatch(s3, {
    type: 'SUBMIT_ASSIGNMENT', playerId: 1 as PlayerId,
  });

  // Reveal
  const { state: s5 } = dispatch(s4, { type: 'REVEAL_ASSIGNMENTS' });
  return s5;
}

describe('calculateScores', () => {
  it('calculates base scores correctly', () => {
    const state = createGame(CONFIGS, { seed: 42 });
    const scores = calculateScores(state);
    expect(scores.length).toBe(2);
    // Base fame = 10 for both
    expect(scores[0].baseFame).toBe(10);
    // Shard bonus: 1 shard × 1 = 1
    expect(scores[0].shardBonus).toBe(1);
    // Coin bonus: 10 coins / 3 = 3
    expect(scores[0].coinBonus).toBe(3);
    // Character bonus: 1 apprentice × 2 + 0 specialists × 3 = 2
    expect(scores[0].characterBonus).toBe(2);
    // Total: 10 + 1 + 3 + 2 = 16
    expect(scores[0].totalFame).toBe(16);
  });

  it('player 1 has more coins → higher coin bonus', () => {
    const state = createGame(CONFIGS, { seed: 42 });
    const scores = calculateScores(state);
    // Player 1 starts with 12 coins → 12/3 = 4
    expect(scores[1].coinBonus).toBe(4);
  });

  it('trick end bonuses work', () => {
    let state = createGame(CONFIGS, { seed: 42 });
    // Give Alice M3B (PER_SHARD, famePerUnit: 1) + 5 shards
    state = updatePlayer(state, 0 as PlayerId, () => ({
      tricks: [{
        trickId: 'M3B' as TrickId, symbolIndex: 0 as SymbolIndex,
        prepared: true, markersOnTrick: 0,
      }],
      shards: 5,
    }));
    const scores = calculateScores(state);
    expect(scores[0].trickEndBonus).toBe(5); // 5 shards × 1
  });

  it('ALL_SPECIALISTS bonus requires all 3', () => {
    let state = createGame(CONFIGS, { seed: 42 });
    state = updatePlayer(state, 0 as PlayerId, () => ({
      tricks: [{
        trickId: 'O3A' as TrickId, symbolIndex: 0 as SymbolIndex,
        prepared: true, markersOnTrick: 0,
      }],
      specialists: ['ENGINEER', 'MANAGER', 'ASSISTANT'],
    }));
    const scores = calculateScores(state);
    expect(scores[0].trickEndBonus).toBe(12);
  });

  it('FOUR_TRICKS bonus when has 4 tricks', () => {
    let state = createGame(CONFIGS, { seed: 42 });
    state = updatePlayer(state, 0 as PlayerId, () => ({
      tricks: [
        { trickId: 'O3C' as TrickId, symbolIndex: 0 as SymbolIndex, prepared: true, markersOnTrick: 0 },
        { trickId: 'M1A' as TrickId, symbolIndex: 1 as SymbolIndex, prepared: true, markersOnTrick: 0 },
        { trickId: 'E1A' as TrickId, symbolIndex: 2 as SymbolIndex, prepared: true, markersOnTrick: 0 },
        { trickId: 'S1A' as TrickId, symbolIndex: 3 as SymbolIndex, prepared: true, markersOnTrick: 0 },
      ],
    }));
    const scores = calculateScores(state);
    expect(scores[0].trickEndBonus).toBe(10);
  });
});

describe('applyFinalScoring', () => {
  it('applies scores and determines winner', () => {
    let state = createGame(CONFIGS, { seed: 42 });
    state = { ...state, gameOver: true };
    const result = applyFinalScoring(state);
    expect(result.winner).not.toBeNull();
  });
});

describe('7-round simulation (v3)', () => {
  it('runs a full game (7 rounds) without errors', () => {
    let state = createGame(CONFIGS, { seed: 42 });

    for (let round = 1; round <= 7; round++) {
      // SETUP → ADVERTISE
      state = executeSetup(state);
      expect(state.phase).toBe('ADVERTISE');
      expect(state.round).toBe(round);

      // ADVERTISE — both skip
      let { state: s } = dispatch(state, { type: 'SKIP_ADVERTISE', playerId: 0 as PlayerId });
      ({ state: s } = dispatch(s, { type: 'SKIP_ADVERTISE', playerId: 1 as PlayerId }));
      state = checkAdvertiseComplete(s);
      expect(state.phase).toBe('ASSIGNMENT');

      // v3 ASSIGNMENT — PLACE_ASSIGNMENT_CARD + SUBMIT + REVEAL
      state = assignBothPlayers(state);
      expect(state.phase).toBe('ASSIGNMENT_REVEAL');

      // Advance to PLACEMENT
      state = advanceToPlacement(state);
      expect(state.phase).toBe('PLACEMENT');

      // PLACEMENT — place characters
      for (let i = 0; i < state.turnQueue.length; i++) {
        const turn = state.turnQueue[state.currentTurnIdx];
        ({ state: s } = dispatch(state, {
          type: 'PLACE_CHARACTER',
          playerId: turn.playerId,
          characterIdx: turn.characterIdx,
          slotIndex: i,
        }));
        state = advanceTurn(s);
      }
      expect(state.phase).toBe('PERFORMANCE');

      // PERFORMANCE — skip (no performances)
      state = advanceToEndTurn(state);
      expect(state.phase).toBe('END_TURN');

      // END_TURN
      state = executeEndTurn(state);

      if (round < 7) {
        expect(state.phase).toBe('SETUP');
        expect(state.round).toBe(round + 1);
      }
    }

    // Game should be over
    expect(state.gameOver).toBe(true);
    expect(state.phase).toBe('GAME_OVER');

    // Apply final scoring
    state = applyFinalScoring(state);
    expect(state.winner).not.toBeNull();
    expect(state.log.length).toBeGreaterThan(0);
  });
});
