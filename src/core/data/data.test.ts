// Stage 1 Tests — data integrity + RNG determinism (v4: DA only)

import { describe, it, expect } from 'vitest';
import { TRICKS, getTrickDef, getAllTrickIds, getCategoryTrickIds } from './tricks';
import { MAGICIANS, getBaseMagicianIds, getAllMagicianIds } from './magicians';
import { PERF_CARDS, getAllPerfCardIds, getPerfCardIdsByVenue } from './perf-cards';
import { createAssignmentCards, getCardCount } from './assignment-cards';
import { CHARACTER_DEFS } from './characters';
import {
  WEEKDAYS, SLOT_ADJACENCY, LINK_REWARD_BY_LEVEL,
  MAX_ACTIVE_PERF_CARDS, COMPONENT_META,
  MARKERS_PER_SYMBOL, SYMBOL_COUNT, SYMBOL_SHAPES,
  LOCATION_SLOTS, SLOT_BLOCKS, PERFORMER_LINK_BONUS,
  END_SCORING, STARTING,
} from './constants';
import { nextFloat, nextInt, shuffle, pick } from '../state/random';
import type { TrickId, TrickCategory } from '../types';

describe('Tricks', () => {
  it('has exactly 48 tricks', () => {
    expect(TRICKS.size).toBe(48);
  });

  it('has 12 tricks per category', () => {
    const categories: TrickCategory[] = ['MECHANICAL', 'OPTICAL', 'ESCAPE', 'SPIRITUAL'];
    for (const cat of categories) {
      expect(getCategoryTrickIds(cat).length).toBe(12);
    }
  });

  it('has 4 tricks per level per category', () => {
    const allTricks = [...TRICKS.values()];
    const categories: TrickCategory[] = ['MECHANICAL', 'OPTICAL', 'ESCAPE', 'SPIRITUAL'];
    for (const cat of categories) {
      for (const level of [1, 2, 3] as const) {
        const count = allTricks.filter(t => t.category === cat && t.level === level).length;
        expect(count).toBe(4);
      }
    }
  });

  it('getTrickDef returns correct trick', () => {
    const trick = getTrickDef('M1A' as TrickId);
    expect(trick.name).toBe('Linking Rings');
    expect(trick.category).toBe('MECHANICAL');
    expect(trick.level).toBe(1);
  });

  it('getTrickDef throws for unknown id', () => {
    expect(() => getTrickDef('UNKNOWN' as TrickId)).toThrow();
  });

  it('fame thresholds match levels', () => {
    for (const trick of TRICKS.values()) {
      if (trick.level === 1) expect(trick.fameThreshold).toBe(1);
      if (trick.level === 2) expect(trick.fameThreshold).toBe(16);
      if (trick.level === 3) expect(trick.fameThreshold).toBe(36);
    }
  });

  it('all trick IDs are unique', () => {
    const ids = getAllTrickIds();
    expect(new Set(ids).size).toBe(ids.length);
  });

  it('all tricks have positive prepareCost', () => {
    for (const trick of TRICKS.values()) {
      expect(trick.prepareCost).toBeGreaterThan(0);
    }
  });

  it('all tricks have at least 1 component requirement', () => {
    for (const trick of TRICKS.values()) {
      expect(Object.keys(trick.components).length).toBeGreaterThan(0);
    }
  });

  it('Hellhound has PER_SPECIAL_CARD endBonus', () => {
    const trick = getTrickDef('M3D' as TrickId);
    expect(trick.endBonus).not.toBeNull();
    expect(trick.endBonus!.type).toBe('PER_SPECIAL_CARD');
    expect(trick.endBonus!.famePerUnit).toBe(2);
  });

  it('Aztec Lady has TRICK_MARKERS_ON_TRICK endBonus', () => {
    const trick = getTrickDef('M3A' as TrickId);
    expect(trick.endBonus).not.toBeNull();
    expect(trick.endBonus!.type).toBe('TRICK_MARKERS_ON_TRICK');
  });
});

describe('Magicians', () => {
  it('has exactly 8 magicians', () => {
    expect(MAGICIANS.size).toBe(8);
  });

  it('has 4 base magicians', () => {
    expect(getBaseMagicianIds().length).toBe(4);
  });

  it('has 8 total magicians', () => {
    expect(getAllMagicianIds().length).toBe(8);
  });

  it('each magician has unique color', () => {
    const colors = [...MAGICIANS.values()].map(m => m.color);
    expect(new Set(colors).size).toBe(8);
  });

  it('each magician has a daAbility', () => {
    for (const m of MAGICIANS.values()) {
      expect(m.daAbility).toBeTruthy();
      expect(m.daAbilityDesc).toBeTruthy();
    }
  });

  it('DA expansion magicians are marked', () => {
    const expansion = [...MAGICIANS.values()].filter(m => m.isExpansion);
    expect(expansion.length).toBe(4);
  });
});

describe('Performance Cards', () => {
  it('has exactly 28 cards', () => {
    expect(PERF_CARDS.size).toBe(28);
  });

  it('has 12 Riverside, 8 Grand Magorian, 8 Magnus Pantheon', () => {
    expect(getPerfCardIdsByVenue('RIVERSIDE').length).toBe(12);
    expect(getPerfCardIdsByVenue('GRAND_MAGORIAN').length).toBe(8);
    expect(getPerfCardIdsByVenue('MAGNUS_PANTHEON').length).toBe(8);
  });

  it('all cards have 6 active slots (Phase 1)', () => {
    for (const card of PERF_CARDS.values()) {
      expect(card.activeSlots.length).toBe(6);
    }
  });

  it('all card IDs are unique', () => {
    const ids = getAllPerfCardIds();
    expect(new Set(ids).size).toBe(ids.length);
  });

  it('performer bonus scales with venue tier', () => {
    const rs = [...PERF_CARDS.values()].find(c => c.venue === 'RIVERSIDE')!;
    const gm = [...PERF_CARDS.values()].find(c => c.venue === 'GRAND_MAGORIAN')!;
    const mp = [...PERF_CARDS.values()].find(c => c.venue === 'MAGNUS_PANTHEON')!;
    expect(rs.performerBonus.fame).toBeLessThan(gm.performerBonus.fame);
    expect(gm.performerBonus.fame).toBeLessThan(mp.performerBonus.fame);
  });
});

describe('Assignment Cards (v4: DA only)', () => {
  it('creates 6 cards per player', () => {
    const cards = createAssignmentCards(0);
    expect(cards.length).toBe(6);
  });

  it('card IDs include player index', () => {
    const p0cards = createAssignmentCards(0);
    const p1cards = createAssignmentCards(1);
    expect(p0cards[0].id).toContain('P0');
    expect(p1cards[0].id).toContain('P1');
  });

  it('includes DARK_ALLEY location', () => {
    const cards = createAssignmentCards(0);
    expect(cards.map(c => c.location)).toContain('DARK_ALLEY');
  });

  it('has 2 THEATER, 1 WORKSHOP, 1 MARKET_ROW, 1 DOWNTOWN, 1 DARK_ALLEY', () => {
    const cards = createAssignmentCards(0);
    const locs = cards.map(c => c.location);
    expect(locs.filter(l => l === 'THEATER').length).toBe(2);
    expect(locs.filter(l => l === 'WORKSHOP').length).toBe(1);
    expect(locs.filter(l => l === 'MARKET_ROW').length).toBe(1);
    expect(locs.filter(l => l === 'DOWNTOWN').length).toBe(1);
    expect(locs.filter(l => l === 'DARK_ALLEY').length).toBe(1);
  });

  it('different players have unique card IDs', () => {
    const p0 = createAssignmentCards(0);
    const p1 = createAssignmentCards(1);
    const allIds = [...p0.map(c => c.id), ...p1.map(c => c.id)];
    expect(new Set(allIds).size).toBe(allIds.length);
  });

  it('getCardCount is 6', () => {
    expect(getCardCount()).toBe(6);
  });
});

describe('Characters', () => {
  it('has 5 character types', () => {
    expect(Object.keys(CHARACTER_DEFS).length).toBe(5);
  });

  it('MAGICIAN has highest baseAP', () => {
    expect(CHARACTER_DEFS.MAGICIAN.baseAP).toBe(3);
  });

  it('MAGICIAN has 0 wage', () => {
    expect(CHARACTER_DEFS.MAGICIAN.wage).toBe(0);
  });
});

describe('Constants (v4)', () => {
  it('has 4 weekdays', () => {
    expect(WEEKDAYS.length).toBe(4);
  });

  it('has 12 component types', () => {
    expect(Object.keys(COMPONENT_META).length).toBe(12);
  });

  it('MARKERS_PER_SYMBOL is 4', () => {
    expect(MARKERS_PER_SYMBOL).toBe(4);
  });

  it('SYMBOL_COUNT is 4', () => {
    expect(SYMBOL_COUNT).toBe(4);
  });

  it('has 4 symbol shapes', () => {
    expect(SYMBOL_SHAPES.length).toBe(4);
  });

  it('STARTING fame is 5', () => {
    expect(STARTING.fame).toBe(5);
  });

  it('LOCATION_SLOTS has 4 slots', () => {
    expect(LOCATION_SLOTS.length).toBe(4);
    expect(LOCATION_SLOTS[0].apMod).toBe(2);
    expect(LOCATION_SLOTS[1].apMod).toBe(1);
    expect(LOCATION_SLOTS[2].apMod).toBe(1);
    expect(LOCATION_SLOTS[3].apMod).toBe(0);
  });

  it('SLOT_BLOCKS defined for 2/3/4 players', () => {
    expect(SLOT_BLOCKS[2]).toBe(2);
    expect(SLOT_BLOCKS[3]).toBe(1);
    expect(SLOT_BLOCKS[4]).toBe(0);
  });

  it('slot adjacency is symmetric', () => {
    for (const [slot, neighbors] of Object.entries(SLOT_ADJACENCY)) {
      for (const neighbor of neighbors) {
        expect(SLOT_ADJACENCY[neighbor]).toContain(Number(slot));
      }
    }
  });

  it('LINK_REWARD_BY_LEVEL scales with level', () => {
    expect(LINK_REWARD_BY_LEVEL[1]).toBe(1);
    expect(LINK_REWARD_BY_LEVEL[2]).toBe(2);
    expect(LINK_REWARD_BY_LEVEL[3]).toBe(3);
  });

  it('PERFORMER_LINK_BONUS is 1', () => {
    expect(PERFORMER_LINK_BONUS).toBe(1);
  });

  it('END_SCORING has SPECIAL_CARD_FAME and FAME_CAP', () => {
    expect(END_SCORING.SPECIAL_CARD_FAME).toBe(2);
    expect(END_SCORING.FAME_CAP).toBe(20);
  });

  it('MAX_ACTIVE_PERF_CARDS is 5', () => {
    expect(MAX_ACTIVE_PERF_CARDS).toBe(5);
  });
});

describe('RNG', () => {
  it('is deterministic', () => {
    const a1 = nextFloat(0, 42);
    const a2 = nextFloat(0, 42);
    expect(a1.value).toBe(a2.value);
    expect(a1.nextCounter).toBe(a2.nextCounter);
  });

  it('different seeds produce different values', () => {
    const a = nextFloat(0, 42);
    const b = nextFloat(0, 99);
    expect(a.value).not.toBe(b.value);
  });

  it('counter advances sequentially', () => {
    const r1 = nextFloat(0, 42);
    expect(r1.nextCounter).toBe(1);
    const r2 = nextFloat(r1.nextCounter, 42);
    expect(r2.nextCounter).toBe(2);
  });

  it('nextInt produces values in range', () => {
    for (let i = 0; i < 100; i++) {
      const { value } = nextInt(i, 42, 3, 7);
      expect(value).toBeGreaterThanOrEqual(3);
      expect(value).toBeLessThanOrEqual(7);
    }
  });

  it('shuffle preserves elements', () => {
    const arr = [1, 2, 3, 4, 5];
    const { value: shuffled } = shuffle(0, 42, arr);
    expect([...shuffled].sort()).toEqual([1, 2, 3, 4, 5]);
  });

  it('shuffle is deterministic', () => {
    const arr = [1, 2, 3, 4, 5];
    const a = shuffle(0, 42, arr);
    const b = shuffle(0, 42, arr);
    expect(a.value).toEqual(b.value);
  });

  it('pick returns element from array', () => {
    const arr = ['a', 'b', 'c'] as const;
    for (let i = 0; i < 50; i++) {
      const { value } = pick(i, 42, arr);
      expect(arr).toContain(value);
    }
  });
});
