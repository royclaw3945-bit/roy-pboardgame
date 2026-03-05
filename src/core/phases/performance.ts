// Performance phase (v4: weekdayPerformers 기반, 목→일 순서)
// executePerformance: 각 요일 퍼포머의 카드를 자동 공연 처리

import type { GameState, Weekday, PlayerId, PerfCardState } from '../types';
import { WEEKDAYS, WEEKDAY_MOD, SPECIALIST_THEATER_BONUS, PERFORMER_LINK_BONUS } from '../data/constants';
import { getPlayer, getPlayerMarkersOnCard, countPlayerLinksOnCard } from '../state/selectors';
import { adjustFame, adjustCoins, updatePlayer, addLog } from '../state/helpers';
import { getTrickDef } from '../data/tricks';
import { getPerfCardDef } from '../data/perf-cards';

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

/** Recover a player's markers from a card (clear from board → back to supply) */
function recoverMarkers(
  state: GameState,
  playerId: PlayerId,
  card: PerfCardState,
): GameState {
  const markersOnCard = getPlayerMarkersOnCard(card, playerId);
  if (markersOnCard.length === 0) return state;

  const newSlots = card.slotMarkers.map(m =>
    m && m.playerId === playerId ? null : m,
  );
  const newPerfCards = state.theater.perfCards.map(c =>
    c.cardId === card.cardId ? { ...c, slotMarkers: newSlots } : c,
  );
  return { ...state, theater: { ...state.theater, perfCards: newPerfCards } };
}

/** Award trick yields for a player's markers on a card */
function awardTrickYields(
  state: GameState,
  playerId: PlayerId,
  card: PerfCardState,
  weekdayMod: { fameMod: number; coinMod: number },
): GameState {
  const player = getPlayer(state, playerId);
  const markers = getPlayerMarkersOnCard(card, playerId);
  let s = state;

  for (const { marker } of markers) {
    const trickSlot = player.tricks.find(t => t.symbolIndex === marker.symbolIndex);
    if (!trickSlot) continue;
    const trick = getTrickDef(trickSlot.trickId);

    const adjustedFame = Math.max(0, trick.yields.fame + weekdayMod.fameMod);
    const adjustedCoins = Math.max(0, trick.yields.coins + weekdayMod.coinMod);

    s = adjustFame(s, playerId, adjustedFame);
    s = adjustCoins(s, playerId, adjustedCoins);
    if (trick.yields.shards > 0) {
      s = updatePlayer(s, playerId, p => ({ shards: p.shards + trick.yields.shards }));
    }
  }
  return s;
}

/** Award performer-only bonuses: card bonus + link bonus + specialist bonus */
function awardPerformerBonuses(
  state: GameState,
  playerId: PlayerId,
  card: PerfCardState,
): GameState {
  const def = getPerfCardDef(card.cardId);
  let s = state;

  // Card bonus
  s = adjustFame(s, playerId, def.performerBonus.fame);
  s = adjustCoins(s, playerId, def.performerBonus.coins);
  if (def.performerBonus.shards > 0) {
    s = updatePlayer(s, playerId, p => ({ shards: p.shards + def.performerBonus.shards }));
  }

  // Link bonus: fixed 1 fame per link
  const links = countPlayerLinksOnCard(card, playerId);
  if (links > 0) {
    s = adjustFame(s, playerId, links * PERFORMER_LINK_BONUS);
  }

  // Specialist theater bonus (only for specialists placed at theater)
  const player = getPlayer(s, playerId);
  const theaterSpecs = player.characters.filter(
    c => c.location === 'THEATER' && c.placed &&
      (c.type === 'ENGINEER' || c.type === 'MANAGER' || c.type === 'ASSISTANT'),
  );
  for (const char of theaterSpecs) {
    const specType = char.type as 'ENGINEER' | 'MANAGER' | 'ASSISTANT';
    const bonus = SPECIALIST_THEATER_BONUS[specType];
    if (bonus.fame > 0) s = adjustFame(s, playerId, bonus.fame);
    if (bonus.coins > 0) s = adjustCoins(s, playerId, bonus.coins);
    if (bonus.shards > 0) {
      s = updatePlayer(s, playerId, p => ({ shards: p.shards + bonus.shards }));
    }
  }

  return s;
}

/** Process a single performance card for a specific weekday performer */
function processCardPerformance(
  state: GameState,
  performerId: PlayerId,
  card: PerfCardState,
  weekday: Weekday,
): GameState {
  const weekdayMod = WEEKDAY_MOD[weekday];
  const performer = getPlayer(state, performerId);
  let s = state;

  // Assign weekday to card for this performance
  const updatedCard = { ...card, weekday };
  s = {
    ...s,
    theater: {
      ...s.theater,
      perfCards: s.theater.perfCards.map(c =>
        c.cardId === card.cardId ? updatedCard : c,
      ),
    },
  };

  // Performer trick yields
  const perfMarkers = getPlayerMarkersOnCard(card, performerId);
  if (perfMarkers.length > 0) {
    s = awardTrickYields(s, performerId, card, weekdayMod);
    s = awardPerformerBonuses(s, performerId, card);
    s = addLog(s, `${performer.name}이(가) 퍼포머로 ${perfMarkers.length}개 트릭 공연 (${WEEKDAY_MOD[weekday].name}, 카드: ${card.cardId})`);
  }

  // Guest yields (other players with markers on this card)
  for (const otherPlayer of s.players) {
    if (otherPlayer.id === performerId) continue;
    const guestMarkers = getPlayerMarkersOnCard(card, otherPlayer.id);
    if (guestMarkers.length === 0) continue;

    s = awardTrickYields(s, otherPlayer.id, card, weekdayMod);
    s = addLog(s, `${otherPlayer.name}이(가) 게스트로 ${guestMarkers.length}개 트릭 공연 (${WEEKDAY_MOD[weekday].name})`);
  }

  // Recover ALL markers from card (all players)
  const freshCard = s.theater.perfCards.find(c => c.cardId === card.cardId)!;
  for (const p of s.players) {
    s = recoverMarkers(s, p.id, freshCard);
  }

  return s;
}

/** Execute full performance phase: iterate weekdays Thu→Sun, process each performer's cards */
export function executePerformance(state: GameState): GameState {
  let s = addLog(state, '공연 페이즈 시작');
  const order = buildPerformanceOrder(s);

  if (order.length === 0) {
    s = addLog(s, '이번 라운드 공연 없음');
    return advanceToEndTurn(s);
  }

  for (const { playerId, weekday } of order) {
    const player = getPlayer(s, playerId);
    s = addLog(s, `${WEEKDAY_MOD[weekday].name} 공연 — ${player.name}`);

    // Find all cards where this performer has markers
    const cardsWithMarkers = s.theater.perfCards.filter(
      c => getPlayerMarkersOnCard(c, playerId).length > 0,
    );

    if (cardsWithMarkers.length === 0) {
      s = addLog(s, `${player.name}: 공연할 카드 없음`);
      continue;
    }

    for (const card of cardsWithMarkers) {
      s = processCardPerformance(s, playerId, card, weekday);
    }
  }

  s = addLog(s, '공연 페이즈 종료');
  return advanceToEndTurn(s);
}
