// Stage 2 Tests — initialization, helpers, selectors (v4: DA only)

import { describe, it, expect } from 'vitest';
import { createGame } from './init';
import { updatePlayer, adjustFame, adjustCoins, addLog, pipe, currentPlayer } from './helpers';
import {
  getPlayer, getActivePerfCards, getRankings, isGameOver,
  countTotalMarkersOnBoard, getNextAvailableSymbol, countTotalMarkersOnTricks,
} from './selectors';
import type { PlayerId, PlayerConfig } from '../types';

const P2_CONFIGS: readonly PlayerConfig[] = [
  { name: 'Alice', magicianId: 'MECHANIKER', isHuman: true,
    startingSpecialist: 'ENGINEER', startingComponents: ['METAL', 'METAL'] },
  { name: 'Bob', magicianId: 'OPTICIAN', isHuman: false,
    startingSpecialist: 'MANAGER', startingComponents: ['FABRIC', 'FABRIC'] },
];

const P4_CONFIGS: readonly PlayerConfig[] = [
  { name: 'Alice', magicianId: 'MECHANIKER', isHuman: true,
    startingSpecialist: 'ENGINEER', startingComponents: ['METAL', 'METAL'] },
  { name: 'Bob', magicianId: 'OPTICIAN', isHuman: false,
    startingSpecialist: 'MANAGER', startingComponents: ['FABRIC', 'FABRIC'] },
  { name: 'Charlie', magicianId: 'ESCAPIST', isHuman: false,
    startingSpecialist: 'ASSISTANT', startingComponents: ['WOOD', 'WOOD'] },
  { name: 'Diana', magicianId: 'SPIRITUALIST', isHuman: false,
    startingSpecialist: 'ENGINEER', startingComponents: ['GLASS', 'GLASS'] },
];

describe('createGame (v4: DA only)', () => {
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

  it('starting fame is 5', () => {
    const state = createGame(P2_CONFIGS, { seed: 1 });
    expect(state.players[0].fame).toBe(5);
    expect(state.players[1].fame).toBe(5);
  });

  it('assigns starting resources correctly', () => {
    const state = createGame(P2_CONFIGS, { seed: 1 });
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

  it('each player starts with Magician + Apprentice + specialist', () => {
    const state = createGame(P2_CONFIGS, { seed: 1 });
    // Alice: MECHANIKER + APPRENTICE + ENGINEER = 3 chars
    expect(state.players[0].characters.length).toBe(3);
    expect(state.players[0].characters[0].type).toBe('MAGICIAN');
    expect(state.players[0].characters[1].type).toBe('APPRENTICE');
    expect(state.players[0].characters[2].type).toBe('ENGINEER');
  });

  it('assistant starting specialist gives extra apprentice', () => {
    const state = createGame(P4_CONFIGS, { seed: 1 });
    // Charlie has ASSISTANT → 4 chars: MAG + APP + ASS + APP
    const charlie = state.players[2];
    expect(charlie.characters.length).toBe(4);
    expect(charlie.characters.filter(c => c.type === 'APPRENTICE').length).toBe(2);
  });

  it('each player starts with a specialist', () => {
    const state = createGame(P2_CONFIGS, { seed: 1 });
    expect(state.players[0].specialists).toEqual(['ENGINEER']);
    expect(state.players[1].specialists).toEqual(['MANAGER']);
  });

  it('each player starts with specialist board', () => {
    const state = createGame(P2_CONFIGS, { seed: 1 });
    expect(state.players[0].specialistBoards.length).toBe(1);
    expect(state.players[0].specialistBoards[0].type).toBe('ENGINEER');
  });

  it('each player starts with Lv.1 trick from favorite category', () => {
    const state = createGame(P2_CONFIGS, { seed: 1 });
    // Alice = MECHANIKER → favorite MECHANICAL → should have a Lv.1 MECHANICAL trick
    expect(state.players[0].tricks.length).toBeGreaterThanOrEqual(1);
    expect(state.players[0].tricks[0].symbolIndex).toBe(0);
  });

  it('engineer gets extra Lv.1 trick', () => {
    const state = createGame(P2_CONFIGS, { seed: 1 });
    // Alice has ENGINEER → should have 2 tricks
    expect(state.players[0].tricks.length).toBe(2);
    expect(state.players[0].tricks[1].symbolIndex).toBe(1);
  });

  it('starting components are applied', () => {
    const state = createGame(P2_CONFIGS, { seed: 1 });
    // Alice: METAL x2
    expect(state.players[0].components.METAL).toBe(2);
    // Bob: FABRIC x2
    expect(state.players[1].components.FABRIC).toBe(2);
  });

  it('symbol 0 is assigned to first trick', () => {
    const state = createGame(P2_CONFIGS, { seed: 1 });
    expect(state.players[0].symbols[0].assigned).toBe(true);
    expect(state.players[0].symbols[0].trickId).not.toBeNull();
  });

  it('each player owns 6 assignment cards', () => {
    const state = createGame(P2_CONFIGS, { seed: 1 });
    expect(state.players[0].assignmentCards.length).toBe(6);
    expect(state.players[1].assignmentCards.length).toBe(6);
    const p0ids = state.players[0].assignmentCards.map(c => c.id);
    const p1ids = state.players[1].assignmentCards.map(c => c.id);
    expect(new Set([...p0ids, ...p1ids]).size).toBe(12);
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
    }
  });

  it('downtown dice: 2 per group (6 total)', () => {
    const state = createGame(P2_CONFIGS, { seed: 1 });
    expect(state.downtownDice.DAHLGAARD.length).toBe(2);
    expect(state.downtownDice.INN.length).toBe(2);
    expect(state.downtownDice.BANK.length).toBe(2);
    expect(state.downtownDice.marked.DAHLGAARD.length).toBe(2);
  });

  it('location slots: 2p blocks 2 slots on DOWNTOWN/MARKET/DA', () => {
    const s2 = createGame(P2_CONFIGS, { seed: 1 });
    expect(s2.locationSlots.DOWNTOWN.length).toBe(2); // 4-2=2
    expect(s2.locationSlots.MARKET_ROW.length).toBe(2);
    expect(s2.locationSlots.DARK_ALLEY.length).toBe(2);
  });

  it('location slots: WORKSHOP always 4, THEATER uses weekdaySlots', () => {
    const s2 = createGame(P2_CONFIGS, { seed: 1 });
    expect(s2.locationSlots.WORKSHOP.length).toBe(4);
    // Theater: locationSlots is empty, uses theater.weekdaySlots instead
    expect(s2.locationSlots.THEATER.length).toBe(0);
    expect(Object.keys(s2.theater.weekdaySlots).length).toBe(4);
    // 2p: 1 backstage slot per weekday
    expect(s2.theater.weekdaySlots.THURSDAY.backstage.length).toBe(1);
  });

  it('location slots: 4p has all 4 slots on DOWNTOWN', () => {
    const s4 = createGame(P4_CONFIGS, { seed: 1 });
    expect(s4.locationSlots.DOWNTOWN.length).toBe(4);
  });

  it('trick decks have fewer cards after player setup', () => {
    const state = createGame(P2_CONFIGS, { seed: 1 });
    // Each player consumes at least 1 trick (engineer consumes 2)
    const totalTricks = Object.values(state.trickDecks).reduce((s, d) => s + d.length, 0);
    expect(totalTricks).toBeLessThan(48);
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

  it('maxRounds is always 7 (DA only)', () => {
    const state = createGame(P2_CONFIGS, { seed: 1 });
    expect(state.maxRounds).toBe(7);
  });

  it('player starts with empty placements and no weekday', () => {
    const state = createGame(P2_CONFIGS, { seed: 1 });
    for (const p of state.players) {
      expect(p.currentPlacements.length).toBe(0);
      expect(p.chosenWeekday).toBeNull();
      expect(p.usedAbilityThisTurn).toBe(false);
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
    expect(updated.players[0].fame).toBe(10); // 5+5
    expect(updated.players[0].coins).toBe(7); // 10-3
    expect(updated.log.length).toBe(1);
  });

  it('currentPlayer returns correct player', () => {
    const state = createGame(P2_CONFIGS, { seed: 1 });
    expect(currentPlayer(state).id).toBe(0);
  });
});

describe('selectors (v4)', () => {
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

  it('countTotalMarkersOnTricks counts markers on trick cards', () => {
    const state = createGame(P2_CONFIGS, { seed: 1 });
    const count = countTotalMarkersOnTricks(state.players[0]);
    // Alice has prepared tricks → may have markers
    expect(count).toBeGreaterThanOrEqual(0);
  });

  it('getNextAvailableSymbol skips assigned symbols', () => {
    const state = createGame(P2_CONFIGS, { seed: 1 });
    // Alice (ENGINEER) has 2 tricks → symbols 0,1 assigned
    const nextSym = getNextAvailableSymbol(state.players[0]);
    expect(nextSym).toBe(2);
  });
});
