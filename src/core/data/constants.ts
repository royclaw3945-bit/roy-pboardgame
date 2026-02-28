// Game constants & configuration (v3)

import type {
  ComponentType, ComponentTier, Location, Weekday,
  CharacterType, TrickCategory, Venue, SymbolIndex, SymbolShape,
} from '../types';

// -- Symbol Markers --
export const MARKERS_PER_SYMBOL = 4;
export const SYMBOL_COUNT = 4;

export const SYMBOL_SHAPES: readonly SymbolShape[] = [
  'CIRCLE', 'TRIANGLE', 'SQUARE', 'STAR',
];

export const SYMBOL_INDEX_TO_SHAPE: Readonly<Record<SymbolIndex, SymbolShape>> = {
  0: 'CIRCLE',
  1: 'TRIANGLE',
  2: 'SQUARE',
  3: 'STAR',
};

// -- Trick Category Metadata --
export const TRICK_CATEGORY_META: Readonly<Record<TrickCategory, {
  readonly name: string;
  readonly color: string;
}>> = {
  MECHANICAL: { name: '기계', color: '#e67e22' },
  OPTICAL: { name: '광학', color: '#9b59b6' },
  ESCAPE: { name: '탈출', color: '#27ae60' },
  SPIRITUAL: { name: '영혼', color: '#3498db' },
};

// -- Component Metadata --
export const COMPONENT_META: Readonly<Record<ComponentType, {
  readonly name: string;
  readonly tier: ComponentTier;
  readonly cost: number;
}>> = {
  WOOD:     { name: '나무', tier: 'basic', cost: 1 },
  METAL:    { name: '금속', tier: 'basic', cost: 1 },
  GLASS:    { name: '유리', tier: 'basic', cost: 1 },
  FABRIC:   { name: '천',   tier: 'basic', cost: 1 },
  ROPE:     { name: '밧줄', tier: 'advanced', cost: 2 },
  OIL:      { name: '석유', tier: 'advanced', cost: 2 },
  SAW:      { name: '톱',   tier: 'advanced', cost: 2 },
  ANIMAL:   { name: '동물', tier: 'advanced', cost: 2 },
  LOCK:     { name: '자물쇠', tier: 'superior', cost: 3 },
  MIRROR:   { name: '거울',   tier: 'superior', cost: 3 },
  DISGUISE: { name: '변장',   tier: 'superior', cost: 3 },
  GEAR:     { name: '톱니',   tier: 'superior', cost: 3 },
};

export const COMPONENT_POOL: Readonly<Record<ComponentTier, number>> = {
  basic: 10, advanced: 8, superior: 6,
};

// -- Location Metadata --
export const LOCATION_META: Readonly<Record<Location, {
  readonly name: string;
  readonly img: string;
}>> = {
  DOWNTOWN:   { name: '다운타운',     img: '/img/loc_downtown.jpg' },
  MARKET_ROW: { name: '시장',         img: '/img/loc_market.jpg' },
  WORKSHOP:   { name: '작업장',       img: '/img/loc_workshop.jpg' },
  THEATER:    { name: '극장',         img: '/img/loc_theater.jpg' },
  DARK_ALLEY: { name: '어둠의 골목', img: '/img/loc_darkalley.jpg' },
};

// -- Location Slot Pyramid (per player count) --
export const LOCATION_SLOT_PYRAMID = [
  { row: 1, apMod: 1 },
  { row: 2, apMod: 0 }, { row: 2, apMod: 0 },
  { row: 3, apMod: -1 }, { row: 3, apMod: -1 }, { row: 3, apMod: -1 },
] as const;

// -- Downtown Dice Faces --
export const DICE_FACES = {
  DAHLGAARD: ['MECHANICAL', 'OPTICAL', 'ESCAPE', 'SPIRITUAL', 'ANY', 'X'] as const,
  INN: ['ENGINEER', 'MANAGER', 'ASSISTANT', 'APPRENTICE', 'APPRENTICE', 'X'] as const,
  BANK: [1, 2, 3, 4, 5, 'X'] as const,
} as const;

// -- Weekday Modifiers --
export const WEEKDAY_MOD: Readonly<Record<Weekday, {
  readonly name: string;
  readonly fameMod: number;
  readonly coinMod: number;
}>> = {
  THURSDAY: { name: '목요일', fameMod: -1, coinMod: -1 },
  FRIDAY:   { name: '금요일', fameMod: 0,  coinMod: 0 },
  SATURDAY: { name: '토요일', fameMod: 0,  coinMod: 0 },
  SUNDAY:   { name: '일요일', fameMod: 1,  coinMod: 1 },
};

export const WEEKDAYS: readonly Weekday[] = [
  'THURSDAY', 'FRIDAY', 'SATURDAY', 'SUNDAY',
];

// -- Character Type Metadata --
export const CHARACTER_META: Readonly<Record<CharacterType, {
  readonly name: string;
  readonly baseAP: number;
  readonly wage: number;
}>> = {
  MAGICIAN:   { name: '마법사',       baseAP: 3, wage: 0 },
  ENGINEER:   { name: '기술자',       baseAP: 2, wage: 2 },
  MANAGER:    { name: '매니저',       baseAP: 2, wage: 2 },
  ASSISTANT:  { name: '어시스턴트',   baseAP: 2, wage: 2 },
  APPRENTICE: { name: '견습생',       baseAP: 1, wage: 1 },
};

// -- Starting Setup --
export const STARTING = {
  fame: 10,
  coins: 10,
  shards: 1,
  extraCoinsByPosition: [0, 2, 4, 6] as readonly number[],
  maxTricks: 4,
  maxComponentsPerType: 3,
} as const;

// -- Advertise --
export const ADVERTISE_COST = [1, 2, 3, 4] as const;
export const ADVERTISE_FAME = 2;

// -- Wages --
export const WAGES = {
  APPRENTICE: 1,
  SPECIALIST: 2,
  MAGICIAN: 0,
  UNPAID_PENALTY: 2,
} as const;

// -- Game Rounds --
export const BASE_ROUNDS = 5;
export const DA_ROUNDS = 7;

// -- Hire Limits --
export const HIRE_LIMITS = {
  ENGINEER: 1, MANAGER: 1, ASSISTANT: 1, APPRENTICE: Infinity,
} as const;

// -- Specialist Theater Bonus --
export const SPECIALIST_THEATER_BONUS = {
  ENGINEER:  { fame: 0, coins: 0, shards: 1 },
  MANAGER:   { fame: 0, coins: 2, shards: 0 },
  ASSISTANT: { fame: 1, coins: 0, shards: 0 },
} as const;

// -- Venue Metadata --
export const VENUE_META: Readonly<Record<Venue, {
  readonly name: string;
  readonly nameKo: string;
  readonly color: string;
}>> = {
  RIVERSIDE:       { name: 'Riverside Theater',  nameKo: '리버사이드 극장', color: '#f1c40f' },
  GRAND_MAGORIAN:  { name: 'Grand Magorian',     nameKo: '그랜드 마고리안', color: '#e67e22' },
  MAGNUS_PANTHEON: { name: 'Magnus Pantheon',     nameKo: '마그누스 판테온', color: '#e74c3c' },
};

// -- End Game Scoring --
export const END_SCORING = {
  SHARD_TO_FAME: 1,
  COINS_PER_FAME: 3,
  APPRENTICE_FAME: 2,
  SPECIALIST_FAME: 3,
} as const;

// -- Link Reward Thresholds --
export const LINK_REWARDS = [
  { fameThreshold: 1,  amount: 1 },
  { fameThreshold: 16, amount: 2 },
  { fameThreshold: 36, amount: 3 },
] as const;

// -- Performance Card Config --
export const MAX_ACTIVE_PERF_CARDS = 5;

// -- Slot Adjacency (2x3 grid) --
// Grid:  0  3
//        1  4
//        2  5
export const SLOT_ADJACENCY: Readonly<Record<number, readonly number[]>> = {
  0: [1, 3],
  1: [0, 2, 4],
  2: [1, 5],
  3: [0, 4],
  4: [1, 3, 5],
  5: [2, 4],
};
