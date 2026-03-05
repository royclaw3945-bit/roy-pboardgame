// Setup phase — roll dice + determine initiative (v4: DA only)

import type { GameState, Weekday, TheaterWeekdaySlot } from '../types';
import type { DahlgaardFace, InnFace, BankFace } from '../types';
import { rngRollDie, rngShuffle } from '../state/random';
import { addLog } from '../state/helpers';
import { DICE_FACES, WEEKDAYS } from '../data/constants';

function rollAllDice(state: GameState): GameState {
  let s = state;
  // v4: 2 dice per group (6 total)
  const dahlgaard: DahlgaardFace[] = [];
  for (let i = 0; i < 2; i++) {
    const { value, state: ns } = rngRollDie(s, DICE_FACES.DAHLGAARD);
    dahlgaard.push(value);
    s = ns;
  }
  const inn: InnFace[] = [];
  for (let i = 0; i < 2; i++) {
    const { value, state: ns } = rngRollDie(s, DICE_FACES.INN);
    inn.push(value);
    s = ns;
  }
  const bank: BankFace[] = [];
  for (let i = 0; i < 2; i++) {
    const { value, state: ns } = rngRollDie(s, DICE_FACES.BANK);
    bank.push(value);
    s = ns;
  }
  return {
    ...s,
    downtownDice: {
      DAHLGAARD: dahlgaard, INN: inn, BANK: bank,
      marked: {
        DAHLGAARD: [false, false],
        INN: [false, false],
        BANK: [false, false],
      },
    },
  };
}

function determineInitiative(state: GameState): GameState {
  if (state.round === 1) {
    const { value: order, state: ns } = rngShuffle(
      state, Array.from({ length: state.config.numPlayers }, (_, i) => i),
    );
    return { ...ns, initiativeOrder: order as readonly number[] };
  }
  // v4: sort by fame asc, ties → reverse existing order
  const prevOrder = state.initiativeOrder;
  const sorted = [...state.players]
    .sort((a, b) => {
      if (a.fame !== b.fame) return a.fame - b.fame;
      // Tie: reverse existing initiative order
      const aIdx = prevOrder.indexOf(state.players.indexOf(a));
      const bIdx = prevOrder.indexOf(state.players.indexOf(b));
      return bIdx - aIdx; // reverse
    })
    .map(p => state.players.indexOf(p));
  return { ...state, initiativeOrder: sorted };
}

function resetWeekdaySlots(state: GameState): Record<Weekday, TheaterWeekdaySlot> {
  const result = {} as Record<Weekday, TheaterWeekdaySlot>;
  for (const day of WEEKDAYS) {
    const existing = state.theater.weekdaySlots[day];
    result[day] = {
      backstage: existing.backstage.map(bs => ({ ...bs, occupant: null })),
      performSlot: { occupant: null },
    };
  }
  return result;
}

function resetPlayerRoundState(state: GameState): GameState {
  const weekdayPerformers = {} as Record<Weekday, null>;
  for (const day of WEEKDAYS) weekdayPerformers[day] = null;

  return {
    ...state,
    theater: { ...state.theater, weekdayPerformers, weekdaySlots: resetWeekdaySlots(state) },
    players: state.players.map(p => ({
      ...p,
      hasAdvertised: false,
      currentPlacements: [],
      chosenWeekday: null,
      usedAbilityThisTurn: false,
      characters: p.characters.map(c => ({
        ...c,
        ap: 0, assigned: false, location: null,
        placed: false, slotIndex: null, slotApMod: 0, shardConverted: false,
      })),
    })),
  };
}

export function executeSetup(state: GameState): GameState {
  let s = state;
  s = rollAllDice(s);
  s = determineInitiative(s);
  s = resetPlayerRoundState(s);
  s = {
    ...s,
    phase: 'ADVERTISE',
    currentPlayerIdx: 0,
    assignmentPhase: null,
    performancePhase: null,
  };
  s = addLog(s, `라운드 ${s.round} 시작`);
  return s;
}
