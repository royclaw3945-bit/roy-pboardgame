// Re-export all static data (v3)

export { TRICKS, getTrickDef, getTricksByCategory, getTricksByLevel, getAllTrickIds, getCategoryTrickIds } from './tricks';
export type { TrickDef, Yields, EndBonus } from './tricks';

export { MAGICIANS, getMagicianDef, getBaseMagicianIds, getAllMagicianIds } from './magicians';
export type { MagicianDef } from './magicians';

export { PERF_CARDS, getPerfCardDef, getAllPerfCardIds, getPerfCardIdsByVenue } from './perf-cards';
export type { PerfCardDef, LinkCircleDef } from './perf-cards';

export { CHARACTER_DEFS } from './characters';
export type { CharacterDef } from './characters';

export { createAssignmentCards, getBaseCardCount, getExpansionCardCount } from './assignment-cards';

export {
  TRICK_CATEGORY_META, COMPONENT_META, COMPONENT_POOL,
  LOCATION_META, LOCATION_SLOT_PYRAMID, DICE_FACES,
  WEEKDAY_MOD, WEEKDAYS, CHARACTER_META,
  STARTING, ADVERTISE_COST, ADVERTISE_FAME,
  WAGES, BASE_ROUNDS, DA_ROUNDS, HIRE_LIMITS,
  SPECIALIST_THEATER_BONUS, VENUE_META, END_SCORING,
  LINK_REWARDS, MAX_ACTIVE_PERF_CARDS, SLOT_ADJACENCY,
  MARKERS_PER_SYMBOL, SYMBOL_COUNT,
  SYMBOL_SHAPES, SYMBOL_INDEX_TO_SHAPE,
} from './constants';
