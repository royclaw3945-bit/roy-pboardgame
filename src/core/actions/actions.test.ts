// Stage 5 Tests — action handlers + dispatch (v3)

import { describe, it, expect } from 'vitest';
import { dispatch } from '../dispatch';
import { createGame } from '../state/init';
import { updatePlayer, updateCharacter, updateTrick, updateSymbol } from '../state/helpers';
import { getTrickDef } from '../data/tricks';
import type { PlayerId, TrickId, CardId, PlayerConfig, GameState, SlotPosition, SymbolIndex, AssignmentCardId } from '../types';

const CONFIGS: readonly PlayerConfig[] = [
  { name: 'Alice', magicianId: 'MECHANIKER', isHuman: true },
  { name: 'Bob', magicianId: 'OPTICIAN', isHuman: false },
];

function setup(overrides?: (s: GameState) => GameState): GameState {
  let s = createGame(CONFIGS, { seed: 42 });
  if (overrides) s = overrides(s);
  return s;
}

describe('dispatch', () => {
  it('rejects unknown action type', () => {
    const state = setup();
    const result = dispatch(state, { type: 'UNKNOWN_ACTION' } as any);
    expect(result.result.ok).toBe(false);
  });
});

describe('ADVERTISE', () => {
  it('succeeds in ADVERTISE phase with enough coins', () => {
    const state = setup(s => ({ ...s, phase: 'ADVERTISE' as const }));
    const { result, state: ns } = dispatch(state, {
      type: 'ADVERTISE', playerId: 0 as PlayerId,
    });
    expect(result.ok).toBe(true);
    expect(ns.players[0].coins).toBe(state.players[0].coins - 1);
    expect(ns.players[0].fame).toBe(state.players[0].fame + 2);
    expect(ns.players[0].hasAdvertised).toBe(true);
  });

  it('fails in wrong phase', () => {
    const state = setup();
    const { result } = dispatch(state, { type: 'ADVERTISE', playerId: 0 as PlayerId });
    expect(result.ok).toBe(false);
  });

  it('fails if already advertised', () => {
    let state = setup(s => ({ ...s, phase: 'ADVERTISE' as const }));
    state = updatePlayer(state, 0 as PlayerId, () => ({ hasAdvertised: true }));
    const { result } = dispatch(state, { type: 'ADVERTISE', playerId: 0 as PlayerId });
    expect(result.ok).toBe(false);
  });
});

describe('SKIP_ADVERTISE', () => {
  it('marks player as done without cost', () => {
    const state = setup(s => ({ ...s, phase: 'ADVERTISE' as const }));
    const { result, state: ns } = dispatch(state, {
      type: 'SKIP_ADVERTISE', playerId: 0 as PlayerId,
    });
    expect(result.ok).toBe(true);
    expect(ns.players[0].hasAdvertised).toBe(true);
    expect(ns.players[0].coins).toBe(state.players[0].coins);
  });
});

describe('PLACE_ASSIGNMENT_CARD (v3)', () => {
  it('places card with characters', () => {
    const state = setup(s => ({
      ...s,
      phase: 'ASSIGNMENT' as const,
      assignmentPhase: { playersSubmitted: [], currentAssigner: null },
    }));
    const cardId = state.players[0].assignmentCards[0].id;
    const { result, state: ns } = dispatch(state, {
      type: 'PLACE_ASSIGNMENT_CARD',
      playerId: 0 as PlayerId,
      cardId,
      characterIndices: [0, 1],
    });
    expect(result.ok).toBe(true);
    expect(ns.players[0].currentPlacements.length).toBe(1);
    expect(ns.players[0].currentPlacements[0].cardId).toBe(cardId);
    expect(ns.players[0].currentPlacements[0].characterIndices).toEqual([0, 1]);
  });

  it('fails for duplicate card placement', () => {
    let state = setup(s => ({
      ...s,
      phase: 'ASSIGNMENT' as const,
      assignmentPhase: { playersSubmitted: [], currentAssigner: null },
    }));
    const cardId = state.players[0].assignmentCards[0].id;
    const { state: s1 } = dispatch(state, {
      type: 'PLACE_ASSIGNMENT_CARD',
      playerId: 0 as PlayerId, cardId, characterIndices: [0],
    });
    const { result } = dispatch(s1, {
      type: 'PLACE_ASSIGNMENT_CARD',
      playerId: 0 as PlayerId, cardId, characterIndices: [1],
    });
    expect(result.ok).toBe(false);
  });
});

describe('SUBMIT_ASSIGNMENT (v3)', () => {
  it('submits assignment after placing cards', () => {
    let state = setup(s => ({
      ...s,
      phase: 'ASSIGNMENT' as const,
      assignmentPhase: { playersSubmitted: [], currentAssigner: null },
    }));
    const cardId = state.players[0].assignmentCards[0].id;
    const { state: s1 } = dispatch(state, {
      type: 'PLACE_ASSIGNMENT_CARD',
      playerId: 0 as PlayerId, cardId, characterIndices: [0],
    });
    const { result, state: ns } = dispatch(s1, {
      type: 'SUBMIT_ASSIGNMENT', playerId: 0 as PlayerId,
    });
    expect(result.ok).toBe(true);
    expect(ns.assignmentPhase!.playersSubmitted).toContain(0);
  });

  it('fails with no characters placed', () => {
    const state = setup(s => ({
      ...s,
      phase: 'ASSIGNMENT' as const,
      assignmentPhase: { playersSubmitted: [], currentAssigner: null },
    }));
    const { result } = dispatch(state, {
      type: 'SUBMIT_ASSIGNMENT', playerId: 0 as PlayerId,
    });
    expect(result.ok).toBe(false);
  });
});

describe('PLACE_CHARACTER', () => {
  it('places character on slot', () => {
    let state = setup(s => ({ ...s, phase: 'PLACEMENT' as const }));
    state = updateCharacter(state, 0 as PlayerId, 0, () => ({
      assigned: true, location: 'DOWNTOWN',
    }));
    const { result, state: ns } = dispatch(state, {
      type: 'PLACE_CHARACTER', playerId: 0 as PlayerId,
      characterIdx: 0, slotIndex: 0,
    });
    expect(result.ok).toBe(true);
    expect(ns.players[0].characters[0].placed).toBe(true);
    expect(ns.players[0].characters[0].ap).toBe(4);
    expect(ns.locationSlots.DOWNTOWN[0].occupant).not.toBeNull();
  });

  it('fails on occupied slot', () => {
    let state = setup(s => ({ ...s, phase: 'PLACEMENT' as const }));
    state = updateCharacter(state, 0 as PlayerId, 0, () => ({
      assigned: true, location: 'DOWNTOWN',
    }));
    const { state: s2 } = dispatch(state, {
      type: 'PLACE_CHARACTER', playerId: 0 as PlayerId,
      characterIdx: 0, slotIndex: 0,
    });
    const s3 = updateCharacter(s2, 1 as PlayerId, 0, () => ({
      assigned: true, location: 'DOWNTOWN',
    }));
    const { result } = dispatch(s3, {
      type: 'PLACE_CHARACTER', playerId: 1 as PlayerId,
      characterIdx: 0, slotIndex: 0,
    });
    expect(result.ok).toBe(false);
  });
});

describe('LEARN_TRICK (v3)', () => {
  it('learns a trick with symbolIndex', () => {
    const state = setup(s => ({ ...s, phase: 'PLACEMENT' as const }));
    const trickId = state.trickDecks.MECHANICAL.find(
      (id: TrickId) => getTrickDef(id).fameThreshold <= state.players[0].fame,
    )!;
    expect(trickId).toBeDefined();
    const { result, state: ns } = dispatch(state, {
      type: 'LEARN_TRICK', playerId: 0 as PlayerId, trickId, symbolIndex: 0 as SymbolIndex,
    });
    expect(result.ok).toBe(true);
    expect(ns.players[0].tricks.length).toBe(1);
    expect(ns.players[0].tricks[0].trickId).toBe(trickId);
    expect(ns.players[0].tricks[0].symbolIndex).toBe(0);
    expect(ns.players[0].symbols[0].assigned).toBe(true);
    expect(ns.players[0].symbols[0].trickId).toBe(trickId);
    expect(ns.trickDecks.MECHANICAL.length).toBe(11);
  });

  it('fails if symbol already assigned', () => {
    let state = setup();
    const trickId = state.trickDecks.MECHANICAL[0];
    state = updateSymbol(state, 0 as PlayerId, 0, () => ({
      assigned: true, trickId,
    }));
    const newTrickId = state.trickDecks.MECHANICAL[1];
    const { result } = dispatch(state, {
      type: 'LEARN_TRICK', playerId: 0 as PlayerId, trickId: newTrickId, symbolIndex: 0 as SymbolIndex,
    });
    expect(result.ok).toBe(false);
  });
});

describe('PREPARE (v3: no component consumption)', () => {
  it('prepares a trick without consuming components', () => {
    const trickId = 'M1A' as TrickId; // needs METAL: 2
    let state = setup();
    state = updatePlayer(state, 0 as PlayerId, p => ({
      tricks: [{ trickId, symbolIndex: 0 as SymbolIndex, prepared: false, markersOnTrick: 0 }],
      components: { ...p.components, METAL: 3 },
    }));
    const { result, state: ns } = dispatch(state, {
      type: 'PREPARE', playerId: 0 as PlayerId, trickIdx: 0,
    });
    expect(result.ok).toBe(true);
    expect(ns.players[0].tricks[0].prepared).toBe(true);
    // v3: 컴포넌트 소모 안 됨
    expect(ns.players[0].components.METAL).toBe(3);
    // markersOnTrick = markerSlots (M1A has 2)
    expect(ns.players[0].tricks[0].markersOnTrick).toBe(2);
  });

  it('fails if components insufficient', () => {
    const trickId = 'M1A' as TrickId;
    let state = setup();
    state = updatePlayer(state, 0 as PlayerId, () => ({
      tricks: [{ trickId, symbolIndex: 0 as SymbolIndex, prepared: false, markersOnTrick: 0 }],
    }));
    const { result } = dispatch(state, {
      type: 'PREPARE', playerId: 0 as PlayerId, trickIdx: 0,
    });
    expect(result.ok).toBe(false);
  });
});

describe('SETUP_TRICK (v3)', () => {
  it('places trick marker on perf card using symbolIndex', () => {
    const trickId = 'M1A' as TrickId;
    let state = setup();
    state = updatePlayer(state, 0 as PlayerId, () => ({
      tricks: [{ trickId, symbolIndex: 0 as SymbolIndex, prepared: true, markersOnTrick: 2 }],
    }));
    const cardId = state.theater.perfCards[0].cardId;
    const { result, state: ns } = dispatch(state, {
      type: 'SETUP_TRICK', playerId: 0 as PlayerId,
      trickIdx: 0, cardId, slotPosition: 0 as SlotPosition,
    });
    expect(result.ok).toBe(true);
    const marker = ns.theater.perfCards[0].slotMarkers[0];
    expect(marker).not.toBeNull();
    expect(marker!.playerId).toBe(0);
    expect(marker!.symbolIndex).toBe(0);
    expect(ns.players[0].tricks[0].markersOnTrick).toBe(1);
  });

  it('fails for same symbol on same card', () => {
    const trickId = 'M1A' as TrickId;
    let state = setup();
    state = updatePlayer(state, 0 as PlayerId, () => ({
      tricks: [{ trickId, symbolIndex: 0 as SymbolIndex, prepared: true, markersOnTrick: 3 }],
    }));
    const cardId = state.theater.perfCards[0].cardId;
    // Place first marker
    const { state: s2 } = dispatch(state, {
      type: 'SETUP_TRICK', playerId: 0 as PlayerId,
      trickIdx: 0, cardId, slotPosition: 0 as SlotPosition,
    });
    // Try second marker from same trick/symbol on same card
    const { result } = dispatch(s2, {
      type: 'SETUP_TRICK', playerId: 0 as PlayerId,
      trickIdx: 0, cardId, slotPosition: 1 as SlotPosition,
    });
    expect(result.ok).toBe(false);
  });
});

describe('CONVERT_SHARD', () => {
  it('converts shard to +1 AP', () => {
    let state = setup(s => ({
      ...s, phase: 'PLACEMENT' as const,
      turnQueue: [{ playerId: 0 as PlayerId, characterIdx: 0 }],
      currentTurnIdx: 0,
    }));
    state = updateCharacter(state, 0 as PlayerId, 0, () => ({
      ap: 3, assigned: true, placed: true, shardConverted: false,
    }));
    const { result, state: ns } = dispatch(state, {
      type: 'CONVERT_SHARD', playerId: 0 as PlayerId,
    });
    expect(result.ok).toBe(true);
    expect(ns.players[0].shards).toBe(0);
    expect(ns.players[0].characters[0].ap).toBe(4);
  });
});

describe('BUY', () => {
  it('buys component from market', () => {
    let state = setup();
    state = { ...state, market: { ...state.market, stock: ['WOOD', 'METAL'] } };
    const { result, state: ns } = dispatch(state, {
      type: 'BUY', playerId: 0 as PlayerId, componentType: 'WOOD',
    });
    expect(result.ok).toBe(true);
    expect(ns.players[0].components.WOOD).toBe(1);
    expect(ns.players[0].coins).toBe(state.players[0].coins - 1);
    expect(ns.market.stock).toEqual(['METAL']);
  });
});

describe('CHOOSE_WEEKDAY (v3)', () => {
  it('reserves weekday for performer', () => {
    const state = setup(s => ({ ...s, phase: 'PLACEMENT' as const }));
    const { result, state: ns } = dispatch(state, {
      type: 'CHOOSE_WEEKDAY', playerId: 0 as PlayerId, weekday: 'FRIDAY',
    });
    expect(result.ok).toBe(true);
    expect(ns.theater.weekdayPerformers.FRIDAY).toBe(0);
    expect(ns.players[0].chosenWeekday).toBe('FRIDAY');
  });

  it('fails if opponent already on that weekday', () => {
    let state = setup(s => ({ ...s, phase: 'PLACEMENT' as const }));
    const { state: s1 } = dispatch(state, {
      type: 'CHOOSE_WEEKDAY', playerId: 0 as PlayerId, weekday: 'FRIDAY',
    });
    const { result } = dispatch(s1, {
      type: 'CHOOSE_WEEKDAY', playerId: 1 as PlayerId, weekday: 'FRIDAY',
    });
    expect(result.ok).toBe(false);
  });
});
