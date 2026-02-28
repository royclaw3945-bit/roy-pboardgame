// 28 Performance Card definitions (Standard only — Duel cards excluded for now)
// Each card has a 2×3 grid of slots. Phase 1: all 6 slots active.
// Phase 2: individual activeSlots from card images.

import type { CardId, Venue, SlotPosition } from '../types';

export interface LinkCircleDef {
  readonly between: readonly [SlotPosition, SlotPosition];
  readonly hasShard: boolean;
}

export interface PerfCardDef {
  readonly id: CardId;
  readonly venue: Venue;
  readonly activeSlots: readonly SlotPosition[];
  readonly linkCircles: readonly LinkCircleDef[];
  readonly performerBonus: { readonly fame: number; readonly coins: number; readonly shards: number };
}

// Default link circles by venue (Phase 1 placeholder)
const RIVERSIDE_LINKS: readonly LinkCircleDef[] = [
  { between: [0, 1], hasShard: false },
  { between: [1, 2], hasShard: true },
  { between: [3, 4], hasShard: true },
  { between: [4, 5], hasShard: false },
  { between: [0, 3], hasShard: false },
  { between: [1, 4], hasShard: false },
  { between: [2, 5], hasShard: false },
];

const GM_LINKS: readonly LinkCircleDef[] = [
  { between: [0, 1], hasShard: true },
  { between: [1, 2], hasShard: false },
  { between: [3, 4], hasShard: false },
  { between: [4, 5], hasShard: true },
  { between: [0, 3], hasShard: true },
  { between: [1, 4], hasShard: false },
  { between: [2, 5], hasShard: true },
];

const MP_LINKS: readonly LinkCircleDef[] = [
  { between: [0, 1], hasShard: true },
  { between: [1, 2], hasShard: true },
  { between: [3, 4], hasShard: true },
  { between: [4, 5], hasShard: true },
  { between: [0, 3], hasShard: true },
  { between: [1, 4], hasShard: true },
  { between: [2, 5], hasShard: true },
];

const ALL_SLOTS: readonly SlotPosition[] = [0, 1, 2, 3, 4, 5];

function makeCard(id: string, venue: Venue, links: readonly LinkCircleDef[],
  bonus: { fame: number; coins: number; shards: number },
): PerfCardDef {
  return {
    id: id as CardId,
    venue,
    activeSlots: ALL_SLOTS,
    linkCircles: links,
    performerBonus: bonus,
  };
}

const RIVERSIDE_BONUS = { fame: 1, coins: 1, shards: 0 };
const GM_BONUS = { fame: 2, coins: 2, shards: 0 };
const MP_BONUS = { fame: 3, coins: 3, shards: 1 };

const ALL_PERF_CARDS: readonly PerfCardDef[] = [
  // Riverside Theater — 12 cards
  makeCard('RS01', 'RIVERSIDE', RIVERSIDE_LINKS, RIVERSIDE_BONUS),
  makeCard('RS02', 'RIVERSIDE', RIVERSIDE_LINKS, RIVERSIDE_BONUS),
  makeCard('RS03', 'RIVERSIDE', RIVERSIDE_LINKS, RIVERSIDE_BONUS),
  makeCard('RS04', 'RIVERSIDE', RIVERSIDE_LINKS, RIVERSIDE_BONUS),
  makeCard('RS05', 'RIVERSIDE', RIVERSIDE_LINKS, RIVERSIDE_BONUS),
  makeCard('RS06', 'RIVERSIDE', RIVERSIDE_LINKS, RIVERSIDE_BONUS),
  makeCard('RS07', 'RIVERSIDE', RIVERSIDE_LINKS, RIVERSIDE_BONUS),
  makeCard('RS08', 'RIVERSIDE', RIVERSIDE_LINKS, RIVERSIDE_BONUS),
  makeCard('RS09', 'RIVERSIDE', RIVERSIDE_LINKS, RIVERSIDE_BONUS),
  makeCard('RS10', 'RIVERSIDE', RIVERSIDE_LINKS, RIVERSIDE_BONUS),
  makeCard('RS11', 'RIVERSIDE', RIVERSIDE_LINKS, RIVERSIDE_BONUS),
  makeCard('RS12', 'RIVERSIDE', RIVERSIDE_LINKS, RIVERSIDE_BONUS),
  // Grand Magorian — 8 cards
  makeCard('GM01', 'GRAND_MAGORIAN', GM_LINKS, GM_BONUS),
  makeCard('GM02', 'GRAND_MAGORIAN', GM_LINKS, GM_BONUS),
  makeCard('GM03', 'GRAND_MAGORIAN', GM_LINKS, GM_BONUS),
  makeCard('GM04', 'GRAND_MAGORIAN', GM_LINKS, GM_BONUS),
  makeCard('GM05', 'GRAND_MAGORIAN', GM_LINKS, GM_BONUS),
  makeCard('GM06', 'GRAND_MAGORIAN', GM_LINKS, GM_BONUS),
  makeCard('GM07', 'GRAND_MAGORIAN', GM_LINKS, GM_BONUS),
  makeCard('GM08', 'GRAND_MAGORIAN', GM_LINKS, GM_BONUS),
  // Magnus Pantheon — 8 cards
  makeCard('MP01', 'MAGNUS_PANTHEON', MP_LINKS, MP_BONUS),
  makeCard('MP02', 'MAGNUS_PANTHEON', MP_LINKS, MP_BONUS),
  makeCard('MP03', 'MAGNUS_PANTHEON', MP_LINKS, MP_BONUS),
  makeCard('MP04', 'MAGNUS_PANTHEON', MP_LINKS, MP_BONUS),
  makeCard('MP05', 'MAGNUS_PANTHEON', MP_LINKS, MP_BONUS),
  makeCard('MP06', 'MAGNUS_PANTHEON', MP_LINKS, MP_BONUS),
  makeCard('MP07', 'MAGNUS_PANTHEON', MP_LINKS, MP_BONUS),
  makeCard('MP08', 'MAGNUS_PANTHEON', MP_LINKS, MP_BONUS),
];

export const PERF_CARDS: ReadonlyMap<CardId, PerfCardDef> = new Map(
  ALL_PERF_CARDS.map(card => [card.id, card]),
);

export function getPerfCardDef(id: CardId): PerfCardDef {
  const def = PERF_CARDS.get(id);
  if (!def) throw new Error(`Unknown perf card: ${id}`);
  return def;
}

export function getAllPerfCardIds(): readonly CardId[] {
  return ALL_PERF_CARDS.map(c => c.id);
}

export function getPerfCardIdsByVenue(venue: Venue): readonly CardId[] {
  return ALL_PERF_CARDS.filter(c => c.venue === venue).map(c => c.id);
}
