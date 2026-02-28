// Stage 2 Tests — initialization, helpers, selectors (v3)

import { describe, it, expect } from 'vitest';
import { createGame } from './init';
import { updatePlayer, adjustFame, adjustCoins, addLog, pipe, currentPlayer } from './helpers';
import { getPlayer, getActivePerfCards, getRankings, isGameOver, countTotalMarkersOnBoard, getNextAvailableSymbol } from './selectors';
import type { PlayerId, PlayerConfig } from '../types';

const P2_CONFIGS: readonly PlayerConfig[] = [
  { name: 'Alice', magicianId: 'MECHANIKER', isHuman: true },
  { name: 'Bob', magicianId: 'OPTICIAN', isHuman: false },
];

const P4_CONFIGS: readonly PlayerConfig[] = [
  { name: 'Alice', magicianId: 'MECHANIKER', isHuman: true },
  { name: 'Bob', magicianId: 'OPTICIAN', isHuman: false },
  { name: 'Charlie', magicianId: 'ESCAPIST', isHuman: false },
  { name: 'Diana', magicianId: 'SPIRITUALIST', isHuman: false },
];

describe('createGame (v3)', () => {
  it('creates 2-player game', () => {
    const state = createGame(P2_CONFIGS, { seed: 1 });
    expect(state.players.length).toBe(2);
    expect(state.round).toBe(1);
    expect(state.phase).toBe('SETUP');
    expect(state.maxRounds).toBe(7);
    expect(state.gameOver).toBe(false);
  });

  it('creates 4-player game', () => {
    const state = createGame(P4_CONFIGS, { seed: 1 });
    expect(state.players.length).toBe(4);
  });

  it('rejects invalid player count', () => {
    expect(() => createGame([], { seed: 1 })).toThrow();
    expect(() => createGame([P2_CONFIGS[0]], { seed: 1 })).toThrow();
  });

  it('assigns starting resources correctly', () => {
    const state = createGame(P2_CONFIGS, { seed: 1 });
    expect(state.players[0].fame).toBe(10);
    expect(state.players[0].coins).toBe(10);
    expect(state.players[0].shards).toBe(1);
    expect(state.players[1].coins).toBe(12);
  });

  it('4-player extra coins scale correctly', () => {
    const state = createGame(P4_CONFIGS, { seed: 1 });
    expect(state.players[0].coins).toBe(10);
    expect(state.players[1].coins).toBe(12);
    expect(state.players[2].coins).toBe(14);
    expect(state.players[3].coins).toBe(16);
  });

  it('each player starts with MAGICIAN + APPRENTICE', () => {
    const state = createGame(P2_CONFIGS, { seed: 1 });
    for (const p of state.players) {
      expect(p.characters.length).toBe(2);
      expect(p.characters[0].type).toBe('MAGICIAN');
      expect(p.characters[1].type).toBe('APPRENTICE');
    }
  });

  it('each player starts with 4 unassigned symbols', () => {
    const state = createGame(P2_CONFIGS, { seed: 1 });
    for (const p of state.players) {
      expect(p.symbols.length).toBe(4);
      for (const s of p.symbols) {
        expect(s.assigned).toBe(false);
        expect(s.trickId).toBeNull();
      }
    }
  });

  it('each player owns personal assignment cards', () => {
    const state = createGame(P2_CONFIGS, { seed: 1 });
    expect(state.players[0].assignmentCards.length).toBe(15);
    expect(state.players[1].assignmentCards.length).toBe(15);
    // Cards are unique per player
    const p0ids = state.players[0].assignmentCards.map(c => c.id);
    const p1ids = state.players[1].assignmentCards.map(c => c.id);
    expect(new Set([...p0ids, ...p1ids]).size).toBe(30);
  });

  it('base game assignment cards = 9 per player', () => {
    const state = createGame(P2_CONFIGS, { useDarkAlley: false, seed: 1 });
    expect(state.players[0].assignmentCards.length).toBe(9);
  });

  it('perf cards initialized correctly for 2 players', () => {
    const state = createGame(P2_CONFIGS, { seed: 1 });
    expect(state.theater.perfCards.length).toBe(3);
    expect(state.theater.perfDeck.length).toBe(25);
    expect(state.theater.perfDiscard.length).toBe(0);
  });

  it('perf cards use array for slotMarkers', () => {
    const state = createGame(P2_CONFIGS, { seed: 1 });
    for (const card of state.theater.perfCards) {
      expect(card.slotMarkers.length).toBe(6);
      expect(card.weekday).toBeNull();
      for (const m of card.slotMarkers) {
        expect(m).toBeNull();
      }
    }
  });

  it('weekdayPerformers initialized to null', () => {
    const state = createGame(P2_CONFIGS, { seed: 1 });
    expect(state.theater.weekdayPerformers.THURSDAY).toBeNull();
    expect(state.theater.weekdayPerformers.FRIDAY).toBeNull();
    expect(state.theater.weekdayPerformers.SATURDAY).toBeNull();
    expect(state.theater.weekdayPerformers.SUNDAY).toBeNull();
  });

  it('assignmentPhase and performancePhase are null initially', () => {
    const state = createGame(P2_CONFIGS, { seed: 1 });
    expect(state.assignmentPhase).toBeNull();
    expect(state.performancePhase).toBeNull();
  });

  it('trick decks have 12 cards each', () => {
    const state = createGame(P2_CONFIGS, { seed: 1 });
    expect(state.trickDecks.MECHANICAL.length).toBe(12);
    expect(state.trickDecks.OPTICAL.length).toBe(12);
    expect(state.trickDecks.ESCAPE.length).toBe(12);
    expect(state.trickDecks.SPIRITUAL.length).toBe(12);
  });

  it('is deterministic with same seed', () => {
    const a = createGame(P2_CONFIGS, { seed: 42 });
    const b = createGame(P2_CONFIGS, { seed: 42 });
    expect(a.theater.perfCards.map(c => c.cardId))
      .toEqual(b.theater.perfCards.map(c => c.cardId));
    expect(a.trickDecks.MECHANICAL).toEqual(b.trickDecks.MECHANICAL);
  });

  it('different seeds produce different shuffles', () => {
    const a = createGame(P2_CONFIGS, { seed: 42 });
    const b = createGame(P2_CONFIGS, { seed: 99 });
    const aCards = a.theater.perfCards.map(c => c.cardId).join(',');
    const bCards = b.theater.perfCards.map(c => c.cardId).join(',');
    expect(aCards).not.toBe(bCards);
  });

  it('base game has 5 rounds', () => {
    const state = createGame(P2_CONFIGS, { useDarkAlley: false, seed: 1 });
    expect(state.maxRounds).toBe(5);
  });

  it('location slots scale with player count', () => {
    const s2 = createGame(P2_CONFIGS, { seed: 1 });
    const s4 = createGame(P4_CONFIGS, { seed: 1 });
    expect(s2.locationSlots.DOWNTOWN.length).toBe(3);
    expect(s4.locationSlots.DOWNTOWN.length).toBe(6);
  });

  it('player starts with empty placements and no weekday', () => {
    const state = createGame(P2_CONFIGS, { seed: 1 });
    for (const p of state.players) {
      expect(p.currentPlacements.length).toBe(0);
      expect(p.chosenWeekday).toBeNull();
      expect(p.usedAbilityThisTurn).toBe(false);
      expect(p.specialistBoards.length).toBe(0);
    }
  });
});

describe('helpers', () => {
  it('updatePlayer modifies specific player', () => {
    const state = createGame(P2_CONFIGS, { seed: 1 });
    const updated = updatePlayer(state, 0 as PlayerId, () => ({ coins: 99 }));
    expect(updated.players[0].coins).toBe(99);
    expect(updated.players[1].coins).toBe(state.players[1].coins);
  });

  it('adjustFame clamps to 0', () => {
    const state = createGame(P2_CONFIGS, { seed: 1 });
    const updated = adjustFame(state, 0 as PlayerId, -100);
    expect(updated.players[0].fame).toBe(0);
  });

  it('adjustCoins clamps to 0', () => {
    const state = createGame(P2_CONFIGS, { seed: 1 });
    const updated = adjustCoins(state, 0 as PlayerId, -100);
    expect(updated.players[0].coins).toBe(0);
  });

  it('addLog appends entry', () => {
    const state = createGame(P2_CONFIGS, { seed: 1 });
    const updated = addLog(state, 'test message');
    expect(updated.log.length).toBe(1);
    expect(updated.log[0].message).toBe('test message');
    expect(updated.log[0].round).toBe(1);
  });

  it('pipe chains transforms', () => {
    const state = createGame(P2_CONFIGS, { seed: 1 });
    const updated = pipe(
      state,
      s => adjustFame(s, 0 as PlayerId, 5),
      s => adjustCoins(s, 0 as PlayerId, -3),
      s => addLog(s, 'piped'),
    );
    expect(updated.players[0].fame).toBe(15);
    expect(updated.players[0].coins).toBe(7);
    expect(updated.log.length).toBe(1);
  });

  it('currentPlayer returns correct player', () => {
    const state = createGame(P2_CONFIGS, { seed: 1 });
    expect(currentPlayer(state).id).toBe(0);
  });
});

describe('selectors (v3)', () => {
  it('getPlayer returns player by id', () => {
    const state = createGame(P2_CONFIGS, { seed: 1 });
    const p = getPlayer(state, 1 as PlayerId);
    expect(p.name).toBe('Bob');
  });

  it('getPlayer throws for invalid id', () => {
    const state = createGame(P2_CONFIGS, { seed: 1 });
    expect(() => getPlayer(state, 99 as PlayerId)).toThrow();
  });

  it('getActivePerfCards returns active cards', () => {
    const state = createGame(P2_CONFIGS, { seed: 1 });
    const cards = getActivePerfCards(state);
    expect(cards.length).toBe(3);
  });

  it('getRankings sorts by fame desc', () => {
    let state = createGame(P2_CONFIGS, { seed: 1 });
    state = adjustFame(state, 1 as PlayerId, 10);
    const rankings = getRankings(state);
    expect(rankings[0].id).toBe(1);
  });

  it('isGameOver returns false initially', () => {
    const state = createGame(P2_CONFIGS, { seed: 1 });
    expect(isGameOver(state)).toBe(false);
  });

  it('countTotalMarkersOnBoard returns 0 initially', () => {
    const state = createGame(P2_CONFIGS, { seed: 1 });
    expect(countTotalMarkersOnBoard(state, 0 as PlayerId)).toBe(0);
  });

  it('getNextAvailableSymbol returns 0 for fresh player', () => {
    const state = createGame(P2_CONFIGS, { seed: 1 });
    expect(getNextAvailableSymbol(state.players[0])).toBe(0);
  });
});
