// End Turn phase — wages, reset, card cycling, round advance (v3)

import type { GameState, PerfCardState, Weekday } from '../types';
import { addLog, updatePlayer, adjustFame } from '../state/helpers';
import { WAGES, WEEKDAYS, CHARACTER_META } from '../data/constants';

/** Pay wages to all characters */
export function payWages(state: GameState): GameState {
  let s = state;
  for (const player of state.players) {
    let totalWage = 0;
    for (const char of player.characters) {
      totalWage += CHARACTER_META[char.type].wage;
    }
    if (totalWage > 0) {
      if (player.coins >= totalWage) {
        s = updatePlayer(s, player.id, p => ({
          coins: p.coins - totalWage,
        }));
        s = addLog(s, `${player.name}: 급여 -${totalWage}코인`);
      } else {
        const unpaid = totalWage - player.coins;
        const famePenalty = unpaid * WAGES.UNPAID_PENALTY;
        s = updatePlayer(s, player.id, () => ({ coins: 0 }));
        s = adjustFame(s, player.id, -famePenalty);
        s = addLog(s, `${player.name}: 급여 미지급 (-${famePenalty}명성)`);
      }
    }
  }
  return s;
}

/** Cycle performance cards — discard oldest, shift, draw new (v3: array-based slotMarkers) */
export function cyclePerfCards(state: GameState): GameState {
  const { perfCards, perfDeck, perfDiscard } = state.theater;
  if (perfCards.length === 0) return state;

  const oldest = perfCards[perfCards.length - 1];
  const remaining = perfCards.slice(0, -1);

  let newCards = [...remaining];
  let newDeck = [...perfDeck];
  const newDiscard = [...perfDiscard, oldest.cardId];

  if (newDeck.length > 0) {
    const newCardId = newDeck.shift()!;
    // v3: slotMarkers is array, weekday is null
    const newCard: PerfCardState = {
      cardId: newCardId,
      weekday: null,
      slotMarkers: [null, null, null, null, null, null],
    };
    newCards = [newCard, ...newCards];
  }

  // v3: Reset weekdayPerformers
  const weekdayPerformers = {} as Record<Weekday, null>;
  for (const day of WEEKDAYS) weekdayPerformers[day] = null;

  return {
    ...state,
    theater: {
      ...state.theater,
      perfCards: newCards,
      perfDeck: newDeck,
      perfDiscard: newDiscard,
      weekdayPerformers,
    },
  };
}

/** Reset location slots */
function resetLocations(state: GameState): GameState {
  const newSlots = { ...state.locationSlots };
  for (const loc of Object.keys(newSlots) as (keyof typeof newSlots)[]) {
    newSlots[loc] = newSlots[loc].map(s => ({ ...s, occupant: null }));
  }
  return { ...state, locationSlots: newSlots };
}

/** Execute full end-of-turn sequence */
export function executeEndTurn(state: GameState): GameState {
  let s = state;
  s = payWages(s);
  s = cyclePerfCards(s);
  s = resetLocations(s);

  const nextRound = s.round + 1;
  if (nextRound > s.maxRounds) {
    s = { ...s, phase: 'GAME_OVER', round: nextRound, gameOver: true };
    s = addLog(s, '게임 종료!');
  } else {
    s = { ...s, phase: 'SETUP', round: nextRound };
    s = addLog(s, `라운드 ${s.round - 1} 종료`);
  }

  return s;
}
