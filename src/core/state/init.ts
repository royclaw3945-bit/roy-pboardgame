// Game initialization — createGame(configs, options) → GameState (v4: DA only)

import type {
  GameState, GameConfig, PlayerState, PlayerConfig, CharacterState,
  PlayerId, TrickId, CardId, Location, TrickCategory, Weekday,
  ComponentType, PerfCardState, LocationSlot, SymbolMarkerState,
  SpecialistType, SpecialistBoard, TrickSlot,
} from '../types';
import type { DowntownDice, DarkAlleyState, MarketState, TheaterState, TheaterWeekdaySlot } from '../types';
import { MAGICIANS, getMagicianDef } from '../data/magicians';
import { getCategoryTrickIds, getTrickDef } from '../data/tricks';
import { getAllPerfCardIds } from '../data/perf-cards';
import { createAssignmentCards } from '../data/assignment-cards';
import {
  STARTING, DICE_FACES, WEEKDAYS, LOCATION_SLOTS, SLOT_BLOCKS,
  DA_ROUNDS, MAX_ACTIVE_PERF_CARDS,
} from '../data/constants';
import { hasRequiredComponents } from './selectors';
import { shuffle } from './random';

const LOCATIONS_WITH_SLOTS: readonly Location[] = [
  'DOWNTOWN', 'MARKET_ROW', 'DARK_ALLEY',
];
const LOCATIONS_NO_BLOCK: readonly Location[] = ['WORKSHOP'];
// Theater는 별도 weekdaySlots로 관리 (locationSlots 사용 안 함)

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
    usedDrawFirstCard: false,
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

function createSpecialistBoard(type: SpecialistType): SpecialistBoard {
  switch (type) {
    case 'ENGINEER': return { type: 'ENGINEER', extraTrickSlot: false };
    case 'MANAGER': return { type: 'MANAGER', multiSlots: [null, null] };
    case 'ASSISTANT': return { type: 'ASSISTANT', freeApprentice: false };
  }
}

function createPlayer(
  config: PlayerConfig, index: number,
  trickDecks: Record<TrickCategory, readonly TrickId[]>,
): { player: PlayerState; updatedDecks: Record<TrickCategory, readonly TrickId[]> } {
  const magician = getMagicianDef(config.magicianId);
  const spec = config.startingSpecialist;

  // Characters: Magician + Apprentice + starting specialist
  const characters: CharacterState[] = [
    createCharacter('MAGICIAN'),
    createCharacter('APPRENTICE'),
    createCharacter(spec),
  ];

  // Specialist bonus: assistant gives extra apprentice
  if (spec === 'ASSISTANT') {
    characters.push(createCharacter('APPRENTICE'));
  }

  // Starting components
  const components = emptyComponents();
  for (const comp of config.startingComponents) {
    components[comp] = Math.min((components[comp] ?? 0) + 1, STARTING.maxComponentsPerType);
  }
  // Manager bonus: extra components go into multi-component slots
  // (components on multi slots count as +1 of that type)
  const managerMultiSlots: (ComponentType | null)[] = [null, null];
  if (spec === 'MANAGER' && config.managerBonusComponents) {
    for (let i = 0; i < config.managerBonusComponents.length && i < 2; i++) {
      const comp = config.managerBonusComponents[i];
      components[comp] = Math.min((components[comp] ?? 0) + 1, STARTING.maxComponentsPerType);
      managerMultiSlots[i] = comp;
    }
  }

  // Starting Lv.1 trick from favorite category (player-selected or auto)
  const favCat = magician.favoriteCategory;
  const decks = { ...trickDecks };
  const favDeck = [...decks[favCat]];
  const lv1Trick = config.startingTrickId
    ? (favDeck.includes(config.startingTrickId) ? config.startingTrickId : favDeck.find(id => getTrickDef(id).level === 1))
    : favDeck.find(id => getTrickDef(id).level === 1);

  const tricks: TrickSlot[] = [];
  const symbols = [...createInitialSymbols()];

  if (lv1Trick) {
    // Assign symbol 0 to this trick
    symbols[0] = { assigned: true, trickId: lv1Trick };
    const trickDef = getTrickDef(lv1Trick);

    // Check free Prepare (include specialist boards for Manager multi-slot bonus)
    const tempBoards: SpecialistBoard[] = [spec === 'MANAGER'
      ? { type: 'MANAGER' as const, multiSlots: managerMultiSlots }
      : createSpecialistBoard(spec)];
    const tempPlayer = {
      components: components as Readonly<Record<ComponentType, number>>,
      specialistBoards: tempBoards,
    } as unknown as PlayerState;
    const canPrepare = hasRequiredComponents(tempPlayer, lv1Trick);

    tricks.push({
      trickId: lv1Trick,
      symbolIndex: 0,
      prepared: canPrepare,
      markersOnTrick: canPrepare ? trickDef.markerSlots : 0,
    });

    // Remove from deck
    decks[favCat] = favDeck.filter(id => id !== lv1Trick);
  }

  // Engineer bonus: extra Lv.1 trick from any category (player-selected or auto)
  if (spec === 'ENGINEER') {
    let extraTrick: TrickId | undefined;
    if (config.engineerBonusTrickId) {
      const td = getTrickDef(config.engineerBonusTrickId);
      const deck = decks[td.category];
      if (deck.includes(config.engineerBonusTrickId)) {
        extraTrick = config.engineerBonusTrickId;
      }
    }
    // Fallback: auto-pick from non-favorite category
    if (!extraTrick) {
      const categories: TrickCategory[] = ['MECHANICAL', 'OPTICAL', 'ESCAPE', 'SPIRITUAL'];
      for (const cat of categories) {
        if (cat === favCat) continue;
        const deck = [...decks[cat]];
        const found = deck.find(id => getTrickDef(id).level === 1);
        if (found) { extraTrick = found; break; }
      }
    }
    if (extraTrick) {
      const td = getTrickDef(extraTrick);
      symbols[1] = { assigned: true, trickId: extraTrick };
      const tp = {
        components: components as Readonly<Record<ComponentType, number>>,
        specialistBoards: [createSpecialistBoard(spec)],
      } as unknown as PlayerState;
      const cp = hasRequiredComponents(tp, extraTrick);
      tricks.push({
        trickId: extraTrick, symbolIndex: 1,
        prepared: cp, markersOnTrick: cp ? td.markerSlots : 0,
      });
      decks[td.category] = [...decks[td.category]].filter(id => id !== extraTrick);
    }
  }

  const player: PlayerState = {
    id: index as PlayerId,
    name: config.name,
    magicianId: config.magicianId,
    color: MAGICIANS.get(config.magicianId)!.color,
    fame: STARTING.fame,
    coins: STARTING.coins + STARTING.extraCoinsByPosition[index],
    shards: STARTING.shards,
    tricks,
    components: components as Readonly<Record<ComponentType, number>>,
    characters,
    specialists: [spec],
    symbols,
    specialistBoards: [spec === 'MANAGER'
      ? { type: 'MANAGER' as const, multiSlots: managerMultiSlots }
      : createSpecialistBoard(spec)],
    assignmentCards: createAssignmentCards(index),
    currentPlacements: [],
    chosenWeekday: null,
    usedAbilityThisTurn: false,
    hasAdvertised: false,
    isHuman: config.isHuman,
  };

  return { player, updatedDecks: decks };
}

function createLocationSlots(numPlayers: number): Record<Location, readonly LocationSlot[]> {
  const blocked = SLOT_BLOCKS[numPlayers] ?? 0;
  const result = {} as Record<Location, LocationSlot[]>;

  // Locations with player-count blocking
  for (const loc of LOCATIONS_WITH_SLOTS) {
    result[loc] = LOCATION_SLOTS.slice(0, LOCATION_SLOTS.length - blocked)
      .map((s, i) => ({ row: i, apMod: s.apMod, occupant: null }));
  }

  // Workshop: always all 4 slots, no blocking
  for (const loc of LOCATIONS_NO_BLOCK) {
    result[loc] = LOCATION_SLOTS.map((s, i) => ({ row: i, apMod: s.apMod, occupant: null }));
  }

  // Theater: managed via theater.weekdaySlots, not locationSlots
  result['THEATER'] = [];

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
  // DA: 2 dice per group (6 total)
  return {
    DAHLGAARD: [DICE_FACES.DAHLGAARD[0], DICE_FACES.DAHLGAARD[1]],
    INN: [DICE_FACES.INN[0], DICE_FACES.INN[1]],
    BANK: [DICE_FACES.BANK[0], DICE_FACES.BANK[1]],
    marked: {
      DAHLGAARD: [false, false],
      INN: [false, false],
      BANK: [false, false],
    },
  };
}

function createMarket(): MarketState {
  return {
    buyArea: ['WOOD', 'METAL', 'GLASS', 'FABRIC'],
    orderArea: [],
    quickOrderSlot: null,
  };
}

/** 극장 요일 슬롯 AP 보정: 상단 슬롯은 +1, +0 (2인 시 +1만) */
const THEATER_BACKSTAGE_AP = [1, 0] as const;

function createTheaterWeekdaySlot(numPlayers: number): TheaterWeekdaySlot {
  const numBackstage = numPlayers <= 2 ? 1 : 2;
  return {
    backstage: Array.from({ length: numBackstage }, (_, i) => ({
      occupant: null,
      apMod: THEATER_BACKSTAGE_AP[i] ?? 0,
    })),
    performSlot: { occupant: null },
  };
}

function createTheater(
  activeCards: readonly PerfCardState[],
  deck: readonly CardId[],
  numPlayers: number,
): TheaterState {
  const weekdayPerformers = {} as Record<Weekday, PlayerId | null>;
  const weekdaySlots = {} as Record<Weekday, TheaterWeekdaySlot>;
  for (const day of WEEKDAYS) {
    weekdayPerformers[day] = null;
    weekdaySlots[day] = createTheaterWeekdaySlot(numPlayers);
  }
  return {
    perfCards: activeCards,
    perfDeck: deck,
    perfDiscard: [],
    weekdayPerformers,
    weekdaySlots,
  };
}

function createDarkAlley(): DarkAlleyState {
  return {
    specialDecks: {},
    drawnChoices: null,
    drawnFromLocation: null,
  };
}

export function createGame(
  configs: readonly PlayerConfig[],
  options: { seed?: number } = {},
): GameState {
  const numPlayers = configs.length;
  if (numPlayers < 2 || numPlayers > 4) {
    throw new Error(`Invalid player count: ${numPlayers}. Must be 2-4.`);
  }

  const seed = options.seed ?? 42;
  let counter = 0;

  const { decks, nextCounter: c1 } = createTrickDecks(counter, seed);
  counter = c1;

  // Create players (may consume tricks from decks)
  let currentDecks = decks;
  const players: PlayerState[] = [];
  for (let i = 0; i < configs.length; i++) {
    const { player, updatedDecks } = createPlayer(configs[i], i, currentDecks);
    players.push(player);
    currentDecks = updatedDecks;
  }

  const { activeCards, deck, nextCounter: c2 } = createPerfCardDeck(counter, seed, numPlayers);
  counter = c2;

  const initiativeOrder = Array.from({ length: numPlayers }, (_, i) => i);

  const config: GameConfig = { numPlayers, seed };

  return {
    config,
    round: 1,
    maxRounds: DA_ROUNDS,
    phase: 'SETUP',
    players,
    initiativeOrder,
    currentPlayerIdx: 0,
    turnQueue: [],
    currentTurnIdx: -1,
    theater: createTheater(activeCards, deck, numPlayers),
    downtownDice: createDowntownDice(),
    market: createMarket(),
    darkAlley: createDarkAlley(),
    prophecy: { active: null, pending: [], deck: [], discard: [] },
    advertisedPlayers: [],
    trickDecks: currentDecks,
    locationSlots: createLocationSlots(numPlayers),
    assignmentPhase: null,
    performancePhase: null,
    log: [],
    rngCounter: counter,
    gameOver: false,
    winner: null,
  };
}
