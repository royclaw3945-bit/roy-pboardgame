// End Turn phase — wages, reset, card cycling, round advance (v4)

import type { GameState, PerfCardState, Weekday, PlayerId, TheaterWeekdaySlot } from '../types';
import { addLog, updatePlayer, adjustFame } from '../state/helpers';
import { WAGES, WEEKDAYS, CHARACTER_META } from '../data/constants';

/** Pay wages — v4: Idle characters exempt, assistant board apprentices exempt */
export function payWages(state: GameState): GameState {
  let s = state;
  for (const player of state.players) {
    let totalWage = 0;
    for (const char of player.characters) {
      // v4: Idle = not assigned (placed === false AND assigned === false)
      if (!char.assigned && !char.placed) continue; // Idle → no wage
      // v4: assistant board apprentice → exempt
      // (simplified: check if APPRENTICE and assistant board has freeApprentice)
      if (char.type === 'APPRENTICE') {
        const assistantBoard = player.specialistBoards.find(b => b.type === 'ASSISTANT');
        if (assistantBoard && assistantBoard.freeApprentice) {
          // One apprentice is free — but only if it's on the board
          // For now, skip this apprentice's wage
          continue;
        }
      }
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

/** Cycle performance cards — v4: only discard from round 3+ */
export function cyclePerfCards(state: GameState): GameState {
  const { perfCards, perfDeck, perfDiscard } = state.theater;
  if (perfCards.length === 0) return state;

  let newCards = [...perfCards];
  let newDeck = [...perfDeck];
  let newDiscard = [...perfDiscard];

  // v4: Only discard oldest card from round 3+
  if (state.round >= 3) {
    const oldest = newCards[newCards.length - 1];

    // v4: Markers on discarded card → supply (just clear them, supply is computed)
    newCards = newCards.slice(0, -1);
    newDiscard = [...newDiscard, oldest.cardId];
  }

  // Draw new card
  if (newDeck.length > 0) {
    const newCardId = newDeck.shift()!;
    const newCard: PerfCardState = {
      cardId: newCardId,
      weekday: null,
      slotMarkers: [null, null, null, null, null, null],
    };
    newCards = [newCard, ...newCards];
  }

  // Reset weekdayPerformers and weekdaySlots
  const weekdayPerformers = {} as Record<Weekday, null>;
  const weekdaySlots = {} as Record<Weekday, TheaterWeekdaySlot>;
  for (const day of WEEKDAYS) {
    weekdayPerformers[day] = null;
    const existing = state.theater.weekdaySlots[day];
    weekdaySlots[day] = {
      backstage: existing.backstage.map(bs => ({ ...bs, occupant: null })),
      performSlot: { occupant: null },
    };
  }

  return {
    ...state,
    theater: {
      ...state.theater,
      perfCards: newCards,
      perfDeck: newDeck,
      perfDiscard: newDiscard,
      weekdayPerformers,
      weekdaySlots,
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

/** Cycle market: orderArea → buyArea, reset quickOrderSlot */
function cycleMarket(state: GameState): GameState {
  const { buyArea, orderArea, quickOrderSlot } = state.market;
  // Orders arrive: move orderArea items to buyArea
  const newBuyArea = [...buyArea, ...orderArea];
  return {
    ...state,
    market: {
      buyArea: newBuyArea,
      orderArea: [],
      quickOrderSlot: null, // Quick order returns to supply
    },
  };
}

/** Execute full end-of-turn sequence */
export function executeEndTurn(state: GameState): GameState {
  let s = state;
  s = payWages(s);
  s = cycleMarket(s);
  s = cyclePerfCards(s);
  s = resetLocations(s);

  const nextRound = s.round + 1;
  if (nextRound > s.maxRounds) {
    s = { ...s, phase: 'GAME_OVER', round: nextRound, gameOver: true };
    s = addLog(s, '게임 종료!');
  } else {
    s = { ...s, phase: 'SETUP', round: nextRound };
    s = addLog(s, `라운드 ${s.round - 1} 종료 → 라운드 ${s.round} 시작 준비`);
  }

  return s;
}
