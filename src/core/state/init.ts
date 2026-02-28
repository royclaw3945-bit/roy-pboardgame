// Game initialization — createGame(configs, options) → GameState (v3)

import type {
  GameState, GameConfig, PlayerState, PlayerConfig, CharacterState,
  PlayerId, TrickId, CardId, Location, TrickCategory, Weekday,
  ComponentType, PerfCardState, LocationSlot, SymbolMarkerState,
  AssignmentPhaseState, PerformancePhaseState,
} from '../types';
import type { DowntownDice, DarkAlleyState, MarketState, TheaterState } from '../types';
import { MAGICIANS } from '../data/magicians';
import { getCategoryTrickIds } from '../data/tricks';
import { getAllPerfCardIds } from '../data/perf-cards';
import { createAssignmentCards } from '../data/assignment-cards';
import {
  STARTING, DICE_FACES, WEEKDAYS, LOCATION_SLOT_PYRAMID,
  BASE_ROUNDS, DA_ROUNDS, MAX_ACTIVE_PERF_CARDS,
} from '../data/constants';
import { shuffle } from './random';

const LOCATIONS_WITH_SLOTS: readonly Location[] = [
  'DOWNTOWN', 'MARKET_ROW', 'THEATER', 'DARK_ALLEY',
];

const ALL_COMPONENTS: readonly ComponentType[] = [
  'WOOD', 'METAL', 'GLASS', 'FABRIC',
  'ROPE', 'OIL', 'SAW', 'ANIMAL',
  'LOCK', 'MIRROR', 'DISGUISE', 'GEAR',
];

function emptyComponents(): Record<ComponentType, number> {
  const c = {} as Record<ComponentType, number>;
  for (const comp of ALL_COMPONENTS) c[comp] = 0;
  return c;
}

function createCharacter(type: CharacterState['type']): CharacterState {
  return {
    type, ap: 0, assigned: false, location: null,
    placed: false, slotIndex: null, slotApMod: 0, shardConverted: false,
  };
}

function createInitialSymbols(): readonly SymbolMarkerState[] {
  return [
    { assigned: false, trickId: null },
    { assigned: false, trickId: null },
    { assigned: false, trickId: null },
    { assigned: false, trickId: null },
  ];
}

function createPlayer(config: PlayerConfig, index: number, useDarkAlley: boolean): PlayerState {
  return {
    id: index as PlayerId,
    name: config.name,
    magicianId: config.magicianId,
    color: MAGICIANS.get(config.magicianId)!.color,
    fame: STARTING.fame,
    coins: STARTING.coins + STARTING.extraCoinsByPosition[index],
    shards: STARTING.shards,
    tricks: [],
    components: emptyComponents(),
    characters: [createCharacter('MAGICIAN'), createCharacter('APPRENTICE')],
    specialists: [],
    symbols: createInitialSymbols(),
    specialistBoards: [],
    assignmentCards: createAssignmentCards(index, useDarkAlley),
    currentPlacements: [],
    chosenWeekday: null,
    usedAbilityThisTurn: false,
    hasAdvertised: false,
    isHuman: config.isHuman,
  };
}

function createLocationSlots(numPlayers: number): Record<Location, readonly LocationSlot[]> {
  const maxSlots = numPlayers <= 2 ? 3 : numPlayers <= 3 ? 4 : 6;
  const slotsTemplate = LOCATION_SLOT_PYRAMID.slice(0, maxSlots);

  const result = {} as Record<Location, LocationSlot[]>;
  for (const loc of LOCATIONS_WITH_SLOTS) {
    result[loc] = slotsTemplate.map(s => ({ ...s, occupant: null }));
  }
  result.WORKSHOP = [];
  result.MARKET_ROW = slotsTemplate.map(s => ({ ...s, occupant: null }));
  return result;
}

function createTrickDecks(counter: number, seed: number) {
  const categories: TrickCategory[] = ['MECHANICAL', 'OPTICAL', 'ESCAPE', 'SPIRITUAL'];
  const decks = {} as Record<TrickCategory, readonly TrickId[]>;
  let c = counter;
  for (const cat of categories) {
    const ids = getCategoryTrickIds(cat);
    const { value: shuffled, nextCounter } = shuffle(c, seed, ids);
    decks[cat] = shuffled as readonly TrickId[];
    c = nextCounter;
  }
  return { decks, nextCounter: c };
}

function createPerfCardDeck(counter: number, seed: number, numPlayers: number) {
  const allIds = getAllPerfCardIds();
  const { value: shuffled, nextCounter } = shuffle(counter, seed, allIds);

  const numActive = Math.min(MAX_ACTIVE_PERF_CARDS, numPlayers + 1);
  const activeCards: PerfCardState[] = [];
  for (let i = 0; i < numActive; i++) {
    activeCards.push({
      cardId: shuffled[i],
      weekday: null,
      slotMarkers: [null, null, null, null, null, null],
    });
  }
  const remaining = shuffled.slice(numActive);
  return { activeCards, deck: remaining, nextCounter };
}

function createDowntownDice(): DowntownDice {
  return {
    DAHLGAARD: [...DICE_FACES.DAHLGAARD],
    INN: [...DICE_FACES.INN],
    BANK: [...DICE_FACES.BANK],
    marked: {
      DAHLGAARD: [false, false, false, false, false, false],
      INN: [false, false, false, false, false, false],
      BANK: [false, false, false, false, false, false],
    },
  };
}

function createMarket(): MarketState {
  return { stock: [], orders: [], quickOrder: null };
}

function createTheater(
  activeCards: readonly PerfCardState[],
  deck: readonly CardId[],
): TheaterState {
  const weekdayPerformers = {} as Record<Weekday, PlayerId | null>;
  for (const day of WEEKDAYS) weekdayPerformers[day] = null;
  return {
    perfCards: activeCards,
    perfDeck: deck,
    perfDiscard: [],
    weekdayPerformers,
  };
}

function createDarkAlley(): DarkAlleyState {
  return { specialDeck: [], drawnCards: {} };
}

export function createGame(
  configs: readonly PlayerConfig[],
  options: { useDarkAlley?: boolean; seed?: number } = {},
): GameState {
  const numPlayers = configs.length;
  if (numPlayers < 2 || numPlayers > 4) {
    throw new Error(`Invalid player count: ${numPlayers}. Must be 2-4.`);
  }

  const useDarkAlley = options.useDarkAlley ?? true;
  const seed = options.seed ?? 42;
  let counter = 0;

  const players = configs.map((c, i) => createPlayer(c, i, useDarkAlley));

  const { decks, nextCounter: c1 } = createTrickDecks(counter, seed);
  counter = c1;

  const { activeCards, deck, nextCounter: c2 } = createPerfCardDeck(counter, seed, numPlayers);
  counter = c2;

  const initiativeOrder = Array.from({ length: numPlayers }, (_, i) => i);

  const config: GameConfig = { numPlayers, useDarkAlley, seed };

  return {
    config,
    round: 1,
    maxRounds: useDarkAlley ? DA_ROUNDS : BASE_ROUNDS,
    phase: 'SETUP',
    players,
    initiativeOrder,
    currentPlayerIdx: 0,
    turnQueue: [],
    currentTurnIdx: -1,
    theater: createTheater(activeCards, deck),
    downtownDice: createDowntownDice(),
    market: createMarket(),
    darkAlley: createDarkAlley(),
    trickDecks: decks,
    locationSlots: createLocationSlots(numPlayers),
    assignmentPhase: null,
    performancePhase: null,
    log: [],
    rngCounter: counter,
    gameOver: false,
    winner: null,
  };
}
