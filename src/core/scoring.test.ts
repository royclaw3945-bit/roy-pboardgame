// Stage 7 Tests — scoring + 7-round simulation (v4: DA only)

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
  { name: 'Alice', magicianId: 'MECHANIKER', isHuman: true,
    startingSpecialist: 'ENGINEER', startingComponents: ['METAL', 'METAL'] },
  { name: 'Bob', magicianId: 'OPTICIAN', isHuman: false,
    startingSpecialist: 'MANAGER', startingComponents: ['FABRIC', 'FABRIC'] },
];

/** Helper: v4 assignment — both players place DOWNTOWN card with magician */
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

describe('calculateScores (v4)', () => {
  it('calculates base scores correctly', () => {
    const state = createGame(CONFIGS, { seed: 42 });
    const scores = calculateScores(state);
    expect(scores.length).toBe(2);
    // v4: base fame = 5
    expect(scores[0].baseFame).toBe(5);
    // Shard bonus: 1 shard × 1 = 1
    expect(scores[0].shardBonus).toBe(1);
    // Coin bonus: 10 coins / 3 = 3
    expect(scores[0].coinBonus).toBe(3);
    // v4: special card bonus (stub: 0 special cards)
    expect(scores[0].specialCardBonus).toBe(0);
  });

  it('trick end bonus: PER_SHARD', () => {
    let state = createGame(CONFIGS, { seed: 42 });
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

  it('trick end bonus: ALL_SPECIALISTS', () => {
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

  it('trick end bonus: FOUR_TRICKS', () => {
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

  it('trick end bonus: PER_BASIC_COMP counts total (not type count)', () => {
    let state = createGame(CONFIGS, { seed: 42 });
    // S3C Skeleton Dance has PER_BASIC_COMP endBonus (famePerUnit: 1)
    state = updatePlayer(state, 0 as PlayerId, p => ({
      tricks: [{
        trickId: 'S3C' as TrickId, symbolIndex: 0 as SymbolIndex,
        prepared: true, markersOnTrick: 0,
      }],
      components: { ...p.components, WOOD: 3, METAL: 2, GLASS: 0, FABRIC: 1 },
    }));
    const scores = calculateScores(state);
    // v4: total count = 3+2+0+1 = 6 (not 3 types)
    expect(scores[0].trickEndBonus).toBe(6);
  });

  it('trick end bonus: TRICK_MARKERS_ON_TRICK', () => {
    let state = createGame(CONFIGS, { seed: 42 });
    // M3A Aztec Lady: TRICK_MARKERS_ON_TRICK (famePerUnit: 2)
    state = updatePlayer(state, 0 as PlayerId, () => ({
      tricks: [
        { trickId: 'M3A' as TrickId, symbolIndex: 0 as SymbolIndex, prepared: true, markersOnTrick: 3 },
        { trickId: 'M1A' as TrickId, symbolIndex: 1 as SymbolIndex, prepared: true, markersOnTrick: 2 },
      ],
    }));
    const scores = calculateScores(state);
    // total markers on tricks = 3+2 = 5, × 2 = 10
    expect(scores[0].trickEndBonus).toBe(10);
  });

  it('fame cap: 20 per category', () => {
    let state = createGame(CONFIGS, { seed: 42 });
    // Give player extreme shard count to test cap
    state = updatePlayer(state, 0 as PlayerId, () => ({
      tricks: [{
        trickId: 'M3B' as TrickId, symbolIndex: 0 as SymbolIndex,
        prepared: true, markersOnTrick: 0,
      }],
      shards: 25, // 25 × 1 = 25 → capped to 20
    }));
    const scores = calculateScores(state);
    expect(scores[0].trickEndBonus).toBe(20); // capped
  });

  it('shard bonus capped at 20', () => {
    let state = createGame(CONFIGS, { seed: 42 });
    state = updatePlayer(state, 0 as PlayerId, () => ({ shards: 25 }));
    const scores = calculateScores(state);
    expect(scores[0].shardBonus).toBe(20);
  });

  it('coin bonus capped at 20', () => {
    let state = createGame(CONFIGS, { seed: 42 });
    state = updatePlayer(state, 0 as PlayerId, () => ({ coins: 100 }));
    const scores = calculateScores(state);
    // 100/3 = 33 → capped to 20
    expect(scores[0].coinBonus).toBe(20);
  });
});

describe('applyFinalScoring (v4)', () => {
  it('applies scores and determines winner', () => {
    let state = createGame(CONFIGS, { seed: 42 });
    state = { ...state, gameOver: true };
    const result = applyFinalScoring(state);
    expect(result.winner).not.toBeNull();
  });

  it('log includes 특수카드 not 캐릭터', () => {
    let state = createGame(CONFIGS, { seed: 42 });
    state = { ...state, gameOver: true };
    const result = applyFinalScoring(state);
    const scoreLogs = result.log.filter(l => l.message.includes('최종점수'));
    expect(scoreLogs.length).toBe(2);
    expect(scoreLogs[0].message).toContain('특수카드');
  });
});

describe('7-round simulation (v4)', () => {
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

      // v4 ASSIGNMENT — PLACE_ASSIGNMENT_CARD + SUBMIT + REVEAL
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

      // PERFORMANCE — skip
      state = advanceToEndTurn(state);
      expect(state.phase).toBe('END_TURN');

      // END_TURN
      state = executeEndTurn(state);

      if (round < 7) {
        expect(state.phase).toBe('SETUP');
        expect(state.round).toBe(round + 1);
      }
    }

    // Game over
    expect(state.gameOver).toBe(true);
    expect(state.phase).toBe('GAME_OVER');

    // Apply final scoring
    state = applyFinalScoring(state);
    expect(state.winner).not.toBeNull();
    expect(state.log.length).toBeGreaterThan(0);

    // v4: starting fame 5, verify reasonable final scores
    const scores = calculateScores(state);
    for (const score of scores) {
      expect(score.baseFame).toBeGreaterThanOrEqual(0);
      expect(score.totalFame).toBeGreaterThanOrEqual(score.baseFame);
    }
  });
});
