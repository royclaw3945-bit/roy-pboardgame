// Performance phase (v3: weekdayPerformers 기반, 목→일 순서)

import type { GameState, Weekday, PlayerId, PerfCardState } from '../types';
import { WEEKDAYS, WEEKDAY_MOD } from '../data/constants';
import { getPlayerMarkersOnCard } from '../state/selectors';

export interface PerformerEntry {
  readonly playerId: PlayerId;
  readonly weekday: Weekday;
}

/** Build performance order: Thursday → Sunday */
export function buildPerformanceOrder(state: GameState): readonly PerformerEntry[] {
  const entries: PerformerEntry[] = [];

  for (const day of WEEKDAYS) {
    const performer = state.theater.weekdayPerformers[day];
    if (performer !== null) {
      entries.push({ playerId: performer, weekday: day });
    }
  }

  return entries;
}

/** Get cards where a player has markers */
export function getPerformableCards(
  state: GameState,
  playerId: PlayerId,
): readonly PerfCardState[] {
  return state.theater.perfCards.filter(c =>
    getPlayerMarkersOnCard(c, playerId).length > 0,
  );
}

/** Get weekday modifier for yields */
export function getWeekdayMod(weekday: Weekday): { fameMod: number; coinMod: number } {
  return WEEKDAY_MOD[weekday];
}

/** Advance performance to END_TURN */
export function advanceToEndTurn(state: GameState): GameState {
  return {
    ...state,
    phase: 'END_TURN',
    performancePhase: null,
  };
}
