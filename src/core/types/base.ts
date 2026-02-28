// Literal union types — all game enums as string literal unions (v3)

export type Phase =
  | 'SETUP'
  | 'ADVERTISE'
  | 'ASSIGNMENT'
  | 'ASSIGNMENT_REVEAL'
  | 'PLACEMENT'
  | 'PERFORMANCE'
  | 'END_TURN'
  | 'GAME_OVER';

export type Location =
  | 'DOWNTOWN'
  | 'MARKET_ROW'
  | 'WORKSHOP'
  | 'THEATER'
  | 'DARK_ALLEY';

export type Weekday = 'THURSDAY' | 'FRIDAY' | 'SATURDAY' | 'SUNDAY';

export type TrickCategory = 'MECHANICAL' | 'OPTICAL' | 'ESCAPE' | 'SPIRITUAL';
export type TrickLevel = 1 | 2 | 3;

export type ComponentType =
  | 'WOOD' | 'METAL' | 'GLASS' | 'FABRIC'
  | 'ROPE' | 'OIL' | 'SAW' | 'ANIMAL'
  | 'LOCK' | 'MIRROR' | 'DISGUISE' | 'GEAR';

export type ComponentTier = 'basic' | 'advanced' | 'superior';

export type CharacterType =
  | 'MAGICIAN' | 'ENGINEER' | 'MANAGER' | 'ASSISTANT' | 'APPRENTICE';

export type SpecialistType = 'ENGINEER' | 'MANAGER' | 'ASSISTANT';

export type MagicianId =
  | 'MECHANIKER' | 'OPTICIAN' | 'ESCAPIST' | 'SPIRITUALIST'
  | 'ELECTRA' | 'GENTLEMAN' | 'RED_LOTUS' | 'YORUBA';

export type SymbolIndex = 0 | 1 | 2 | 3;
export type SymbolShape = 'CIRCLE' | 'TRIANGLE' | 'SQUARE' | 'STAR';

export type Venue = 'RIVERSIDE' | 'GRAND_MAGORIAN' | 'MAGNUS_PANTHEON';

export type SlotPosition = 0 | 1 | 2 | 3 | 4 | 5;

export type DiceGroup = 'DAHLGAARD' | 'INN' | 'BANK';

export type LinkRewardChoice = 'FAME' | 'COINS';

// -- Branded types --
export type TrickId = string & { readonly __brand: 'TrickId' };
export type PlayerId = number & { readonly __brand: 'PlayerId' };
export type CardId = string & { readonly __brand: 'CardId' };
export type AssignmentCardId = string & { readonly __brand: 'AssignmentCardId' };

// -- DA Ability --
export type DaAbilityId =
  | 'MECHANIKER_DA' | 'OPTICIAN_DA' | 'ESCAPIST_DA' | 'SPIRITUALIST_DA'
  | 'ELECTRA_DA' | 'GENTLEMAN_DA' | 'RED_LOTUS_DA' | 'YORUBA_DA';

// -- End Bonus --
export type EndBonusType =
  | 'TRICK_MARKERS_ON_PERF'
  | 'PER_SHARD'
  | 'PER_L1_TRICK'
  | 'PER_L2_TRICK'
  | 'PER_L3_TRICK'
  | 'PER_APPRENTICE'
  | 'PER_3_COINS'
  | 'PER_BASIC_COMP'
  | 'PER_ADVANCED_COMP'
  | 'PER_SUPERIOR_COMP'
  | 'ALL_SPECIALISTS'
  | 'FOUR_TRICKS'
  | 'HAS_ENGINEER'
  | 'HAS_MANAGER'
  | 'HAS_ASSISTANT';
