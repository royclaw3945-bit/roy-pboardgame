// Stage 7 Tests — phase transitions + round cycle (v3)

import { describe, it, expect } from 'vitest';
import { createGame } from '../state/init';
import { dispatch } from '../dispatch';
import { executeSetup } from './setup';
import { checkAdvertiseComplete, getNextAdvertiser } from './advertise';
import { allPlayersAssigned, buildTurnQueue, advanceToPlacement, initAssignmentPhase } from './assignment';
import { advanceTurn } from './placement';
import { buildPerformanceOrder, advanceToEndTurn } from './performance';
import { payWages, cyclePerfCards, executeEndTurn } from './end-turn';
import { updatePlayer, updateCharacter, adjustFame } from '../state/helpers';
import type { PlayerId, PlayerConfig, GameState, AssignmentCardPlacement } from '../types';

const CONFIGS: readonly PlayerConfig[] = [
  { name: 'Alice', magicianId: 'MECHANIKER', isHuman: true },
  { name: 'Bob', magicianId: 'OPTICIAN', isHuman: false },
];

function setup(): GameState {
  return createGame(CONFIGS, { seed: 42 });
}

/** Helper: assign characters using v3 PLACE_ASSIGNMENT_CARD flow */
function assignBothPlayers(state: GameState): GameState {
  let s = { ...state, phase: 'ASSIGNMENT' as const };
  s = initAssignmentPhase(s);

  // Player 0: place card for DOWNTOWN with magician
  const p0card = s.players[0].assignmentCards.find(c => c.location === 'DOWNTOWN')!;
  const { state: s1 } = dispatch(s, {
    type: 'PLACE_ASSIGNMENT_CARD', playerId: 0 as PlayerId,
    cardId: p0card.id, characterIndices: [0],
  });
  const { state: s2 } = dispatch(s1, {
    type: 'SUBMIT_ASSIGNMENT', playerId: 0 as PlayerId,
  });

  // Player 1: place card for DOWNTOWN with magician
  const p1card = s2.players[1].assignmentCards.find(c => c.location === 'DOWNTOWN')!;
  const { state: s3 } = dispatch(s2, {
    type: 'PLACE_ASSIGNMENT_CARD', playerId: 1 as PlayerId,
    cardId: p1card.id, characterIndices: [0],
  });
  const { state: s4 } = dispatch(s3, {
    type: 'SUBMIT_ASSIGNMENT', playerId: 1 as PlayerId,
  });

  // Reveal assignments
  const { state: s5 } = dispatch(s4, { type: 'REVEAL_ASSIGNMENTS' });
  return s5;
}

describe('executeSetup', () => {
  it('rolls dice and advances to ADVERTISE', () => {
    const state = setup();
    const result = executeSetup(state);
    expect(result.phase).toBe('ADVERTISE');
    expect(result.downtownDice.DAHLGAARD.length).toBe(6);
    expect(result.downtownDice.INN.length).toBe(6);
    expect(result.downtownDice.BANK.length).toBe(6);
    expect(result.log.length).toBeGreaterThan(0);
  });

  it('determines initiative order', () => {
    const state = setup();
    const result = executeSetup(state);
    expect(result.initiativeOrder.length).toBe(2);
    expect(new Set(result.initiativeOrder).size).toBe(2);
  });

  it('resets player round state (v3)', () => {
    let state = setup();
    state = updatePlayer(state, 0 as PlayerId, () => ({
      hasAdvertised: true, usedAbilityThisTurn: true, chosenWeekday: 'FRIDAY' as const,
    }));
    const result = executeSetup(state);
    expect(result.players[0].hasAdvertised).toBe(false);
    expect(result.players[0].usedAbilityThisTurn).toBe(false);
    expect(result.players[0].chosenWeekday).toBeNull();
    expect(result.players[0].currentPlacements.length).toBe(0);
  });

  it('resets weekdayPerformers', () => {
    const result = executeSetup(setup());
    expect(result.theater.weekdayPerformers.THURSDAY).toBeNull();
    expect(result.theater.weekdayPerformers.FRIDAY).toBeNull();
    expect(result.theater.weekdayPerformers.SATURDAY).toBeNull();
    expect(result.theater.weekdayPerformers.SUNDAY).toBeNull();
  });

  it('clears assignmentPhase and performancePhase', () => {
    const result = executeSetup(setup());
    expect(result.assignmentPhase).toBeNull();
    expect(result.performancePhase).toBeNull();
  });

  it('is deterministic', () => {
    const a = executeSetup(setup());
    const b = executeSetup(setup());
    expect(a.downtownDice.DAHLGAARD).toEqual(b.downtownDice.DAHLGAARD);
    expect(a.initiativeOrder).toEqual(b.initiativeOrder);
  });
});

describe('advertise phase', () => {
  it('getNextAdvertiser returns first unadvertised player', () => {
    const state = executeSetup(setup());
    const next = getNextAdvertiser(state);
    expect(next).not.toBeNull();
    expect(state.players[next!].hasAdvertised).toBe(false);
  });

  it('checkAdvertiseComplete advances when all done', () => {
    let state = executeSetup(setup());
    state = updatePlayer(state, 0 as PlayerId, () => ({ hasAdvertised: true }));
    state = updatePlayer(state, 1 as PlayerId, () => ({ hasAdvertised: true }));
    const result = checkAdvertiseComplete(state);
    expect(result.phase).toBe('ASSIGNMENT');
  });

  it('checkAdvertiseComplete stays if not all done', () => {
    let state = executeSetup(setup());
    state = updatePlayer(state, 0 as PlayerId, () => ({ hasAdvertised: true }));
    const result = checkAdvertiseComplete(state);
    expect(result.phase).toBe('ADVERTISE');
  });
});

describe('assignment phase (v3)', () => {
  it('allPlayersAssigned returns false initially', () => {
    const state = setup();
    expect(allPlayersAssigned(state)).toBe(false);
  });

  it('allPlayersAssigned returns true after reveal', () => {
    const state = assignBothPlayers(executeSetup(setup()));
    expect(allPlayersAssigned(state)).toBe(true);
  });

  it('buildTurnQueue creates entries for assigned characters (v3)', () => {
    const state = assignBothPlayers(executeSetup(setup()));
    const queue = buildTurnQueue(state);
    expect(queue.length).toBe(2);
  });

  it('advanceToPlacement transitions correctly', () => {
    const state = assignBothPlayers(executeSetup(setup()));
    const result = advanceToPlacement(state);
    expect(result.phase).toBe('PLACEMENT');
    expect(result.turnQueue.length).toBe(2);
    expect(result.currentTurnIdx).toBe(0);
  });
});

describe('placement phase', () => {
  it('advanceTurn increments index', () => {
    let state = setup();
    state = {
      ...state,
      phase: 'PLACEMENT' as const,
      turnQueue: [
        { playerId: 0 as PlayerId, characterIdx: 0 },
        { playerId: 1 as PlayerId, characterIdx: 0 },
      ],
      currentTurnIdx: 0,
    };
    const result = advanceTurn(state);
    expect(result.currentTurnIdx).toBe(1);
    expect(result.phase).toBe('PLACEMENT');
  });

  it('advanceTurn transitions to PERFORMANCE when queue done', () => {
    let state = setup();
    state = {
      ...state,
      phase: 'PLACEMENT' as const,
      turnQueue: [{ playerId: 0 as PlayerId, characterIdx: 0 }],
      currentTurnIdx: 0,
    };
    const result = advanceTurn(state);
    expect(result.phase).toBe('PERFORMANCE');
  });
});

describe('performance phase (v3)', () => {
  it('buildPerformanceOrder uses weekdayPerformers', () => {
    let state = setup();
    state = {
      ...state,
      theater: {
        ...state.theater,
        weekdayPerformers: {
          THURSDAY: 1 as PlayerId,
          FRIDAY: 0 as PlayerId,
          SATURDAY: null,
          SUNDAY: null,
        },
      },
    };
    const order = buildPerformanceOrder(state);
    expect(order.length).toBe(2);
    expect(order[0].weekday).toBe('THURSDAY');
    expect(order[0].playerId).toBe(1);
    expect(order[1].weekday).toBe('FRIDAY');
    expect(order[1].playerId).toBe(0);
  });

  it('advanceToEndTurn transitions correctly', () => {
    let state = setup();
    state = { ...state, phase: 'PERFORMANCE' as const };
    const result = advanceToEndTurn(state);
    expect(result.phase).toBe('END_TURN');
    expect(result.performancePhase).toBeNull();
  });
});

describe('end-turn phase', () => {
  it('payWages deducts from coins', () => {
    const state = setup();
    const result = payWages(state);
    expect(result.players[0].coins).toBe(state.players[0].coins - 1);
  });

  it('payWages penalizes fame when can\'t pay', () => {
    let state = setup();
    state = updatePlayer(state, 0 as PlayerId, () => ({ coins: 0 }));
    const result = payWages(state);
    expect(result.players[0].fame).toBe(state.players[0].fame - 2);
  });

  it('cyclePerfCards rotates cards', () => {
    const state = setup();
    const initialCount = state.theater.perfCards.length;
    const result = cyclePerfCards(state);
    expect(result.theater.perfCards.length).toBe(initialCount);
    expect(result.theater.perfDiscard.length).toBe(1);
    expect(result.theater.perfDeck.length).toBe(state.theater.perfDeck.length - 1);
  });

  it('cyclePerfCards resets weekdayPerformers', () => {
    let state = setup();
    state = {
      ...state,
      theater: {
        ...state.theater,
        weekdayPerformers: {
          THURSDAY: 0 as PlayerId,
          FRIDAY: 1 as PlayerId,
          SATURDAY: null,
          SUNDAY: null,
        },
      },
    };
    const result = cyclePerfCards(state);
    expect(result.theater.weekdayPerformers.THURSDAY).toBeNull();
    expect(result.theater.weekdayPerformers.FRIDAY).toBeNull();
  });

  it('executeEndTurn advances round', () => {
    let state = setup();
    state = { ...state, phase: 'END_TURN' as const };
    const result = executeEndTurn(state);
    expect(result.round).toBe(2);
    expect(result.phase).toBe('SETUP');
  });

  it('executeEndTurn ends game at max rounds', () => {
    let state = setup();
    state = { ...state, phase: 'END_TURN' as const, round: 7 };
    const result = executeEndTurn(state);
    expect(result.gameOver).toBe(true);
    expect(result.phase).toBe('GAME_OVER');
  });
});

describe('full round cycle (v3)', () => {
  it('SETUP → ADVERTISE → ASSIGNMENT → PLACEMENT → PERFORMANCE → END_TURN', () => {
    let state = setup();

    // SETUP → ADVERTISE
    state = executeSetup(state);
    expect(state.phase).toBe('ADVERTISE');

    // Both players skip advertise
    const { state: s2 } = dispatch(state, { type: 'SKIP_ADVERTISE', playerId: 0 as PlayerId });
    const { state: s3 } = dispatch(s2, { type: 'SKIP_ADVERTISE', playerId: 1 as PlayerId });
    state = checkAdvertiseComplete(s3);
    expect(state.phase).toBe('ASSIGNMENT');

    // v3: PLACE_ASSIGNMENT_CARD + SUBMIT + REVEAL
    state = assignBothPlayers(state);
    expect(state.phase).toBe('ASSIGNMENT_REVEAL');

    // Advance to PLACEMENT
    state = advanceToPlacement(state);
    expect(state.phase).toBe('PLACEMENT');
    expect(state.turnQueue.length).toBe(2);

    // Place characters
    const { state: s6 } = dispatch(state, {
      type: 'PLACE_CHARACTER', playerId: state.turnQueue[0].playerId,
      characterIdx: state.turnQueue[0].characterIdx, slotIndex: 0,
    });
    const s7 = advanceTurn(s6);
    const { state: s8 } = dispatch(s7, {
      type: 'PLACE_CHARACTER', playerId: s7.turnQueue[1].playerId,
      characterIdx: s7.turnQueue[1].characterIdx, slotIndex: 1,
    });
    state = advanceTurn(s8);
    expect(state.phase).toBe('PERFORMANCE');

    // Skip to END_TURN
    state = advanceToEndTurn(state);
    expect(state.phase).toBe('END_TURN');

    // Execute end turn
    state = executeEndTurn(state);
    expect(state.round).toBe(2);
    expect(state.phase).toBe('SETUP');
  });
});
