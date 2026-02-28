// Stage 1 Tests — data integrity + RNG determinism (v3)

import { describe, it, expect } from 'vitest';
import { TRICKS, getTrickDef, getAllTrickIds, getCategoryTrickIds } from './tricks';
import { MAGICIANS, getBaseMagicianIds, getAllMagicianIds } from './magicians';
import { PERF_CARDS, getAllPerfCardIds, getPerfCardIdsByVenue } from './perf-cards';
import { createAssignmentCards, getBaseCardCount, getExpansionCardCount } from './assignment-cards';
import { CHARACTER_DEFS } from './characters';
import {
  WEEKDAYS, SLOT_ADJACENCY, LINK_REWARDS,
  MAX_ACTIVE_PERF_CARDS, COMPONENT_META,
  MARKERS_PER_SYMBOL, SYMBOL_COUNT, SYMBOL_SHAPES,
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
});

describe('Magicians', () => {
  it('has exactly 8 magicians (v3)', () => {
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

describe('Assignment Cards (v3: personal)', () => {
  it('base game creates 9 cards per player', () => {
    const cards = createAssignmentCards(0, false);
    expect(cards.length).toBe(9);
  });

  it('Dark Alley creates 15 cards per player', () => {
    const cards = createAssignmentCards(0, true);
    expect(cards.length).toBe(15);
  });

  it('card IDs include player index', () => {
    const p0cards = createAssignmentCards(0, false);
    const p1cards = createAssignmentCards(1, false);
    expect(p0cards[0].id).toContain('P0');
    expect(p1cards[0].id).toContain('P1');
  });

  it('base game has no DARK_ALLEY location', () => {
    const cards = createAssignmentCards(0, false);
    expect(cards.map(c => c.location)).not.toContain('DARK_ALLEY');
  });

  it('DA expansion includes DARK_ALLEY', () => {
    const cards = createAssignmentCards(0, true);
    expect(cards.map(c => c.location)).toContain('DARK_ALLEY');
  });

  it('different players have unique card IDs', () => {
    const p0 = createAssignmentCards(0, true);
    const p1 = createAssignmentCards(1, true);
    const allIds = [...p0.map(c => c.id), ...p1.map(c => c.id)];
    expect(new Set(allIds).size).toBe(allIds.length);
  });

  it('getBaseCardCount is 9', () => {
    expect(getBaseCardCount()).toBe(9);
  });

  it('getExpansionCardCount is 6', () => {
    expect(getExpansionCardCount()).toBe(6);
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

describe('Constants (v3)', () => {
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

  it('slot adjacency is symmetric', () => {
    for (const [slot, neighbors] of Object.entries(SLOT_ADJACENCY)) {
      for (const neighbor of neighbors) {
        expect(SLOT_ADJACENCY[neighbor]).toContain(Number(slot));
      }
    }
  });

  it('link rewards are ordered by threshold', () => {
    for (let i = 1; i < LINK_REWARDS.length; i++) {
      expect(LINK_REWARDS[i].fameThreshold).toBeGreaterThan(LINK_REWARDS[i - 1].fameThreshold);
    }
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
