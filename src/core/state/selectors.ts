// Pure query functions over GameState (v3)

import type {
  GameState, PlayerState, PlayerId, TrickId, CardId,
  TrickCategory, SlotPosition, SymbolIndex,
} from '../types';
import type { PerfCardState, TrickMarker } from '../types';
import { getTrickDef } from '../data/tricks';
import { getPerfCardDef } from '../data/perf-cards';
import { SLOT_ADJACENCY, MARKERS_PER_SYMBOL } from '../data/constants';

/** Get player by id */
export function getPlayer(state: GameState, playerId: PlayerId): PlayerState {
  const player = state.players.find(p => p.id === playerId);
  if (!player) throw new Error(`Player not found: ${playerId}`);
  return player;
}

/** Get player index */
export function getPlayerIndex(state: GameState, playerId: PlayerId): number {
  const idx = state.players.findIndex(p => p.id === playerId);
  if (idx === -1) throw new Error(`Player not found: ${playerId}`);
  return idx;
}

/** Get active performance cards on board */
export function getActivePerfCards(state: GameState): readonly PerfCardState[] {
  return state.theater.perfCards;
}

/** Get all trick markers on a specific card belonging to a player */
export function getPlayerMarkersOnCard(
  card: PerfCardState,
  playerId: PlayerId,
): readonly { slot: number; marker: TrickMarker }[] {
  const result: { slot: number; marker: TrickMarker }[] = [];
  for (let i = 0; i < card.slotMarkers.length; i++) {
    const marker = card.slotMarkers[i];
    if (marker && marker.playerId === playerId) {
      result.push({ slot: i, marker });
    }
  }
  return result;
}

/** Count links on a performance card between adjacent trick markers */
export function countLinksOnCard(card: PerfCardState): number {
  const def = getPerfCardDef(card.cardId);
  let links = 0;
  for (const link of def.linkCircles) {
    const [a, b] = link.between;
    const markerA = card.slotMarkers[a];
    const markerB = card.slotMarkers[b];
    if (markerA && markerB) links++;
  }
  return links;
}

/** Count links belonging to a specific player on a card */
export function countPlayerLinksOnCard(
  card: PerfCardState,
  playerId: PlayerId,
): number {
  const def = getPerfCardDef(card.cardId);
  let links = 0;
  for (const link of def.linkCircles) {
    const [a, b] = link.between;
    const markerA = card.slotMarkers[a];
    const markerB = card.slotMarkers[b];
    if (markerA && markerB && markerA.playerId === playerId && markerB.playerId === playerId) {
      links++;
    }
  }
  return links;
}

/** Check if a slot is adjacent to another on the 2x3 grid */
export function isAdjacentSlot(a: SlotPosition, b: SlotPosition): boolean {
  return SLOT_ADJACENCY[a]?.includes(b) ?? false;
}

/** Get player's learned trick defs */
export function getPlayerTrickDefs(player: PlayerState) {
  return player.tricks.map(slot => getTrickDef(slot.trickId));
}

/** Check if player has required components for a trick (v3: validation only, no consume) */
export function hasRequiredComponents(
  player: PlayerState,
  trickId: TrickId,
): boolean {
  const trick = getTrickDef(trickId);
  for (const [comp, needed] of Object.entries(trick.components)) {
    if ((player.components[comp as keyof typeof player.components] ?? 0) < (needed ?? 0)) {
      return false;
    }
  }
  return true;
}

/** Get allowed trick categories from Dahlgaard dice */
export function getAllowedCategories(state: GameState): readonly TrickCategory[] {
  const dice = state.downtownDice;
  const result: TrickCategory[] = [];
  for (let i = 0; i < dice.DAHLGAARD.length; i++) {
    if (dice.marked.DAHLGAARD[i]) continue;
    const face = dice.DAHLGAARD[i];
    if (face === 'X') continue;
    if (face === 'ANY') {
      return ['MECHANICAL', 'OPTICAL', 'ESCAPE', 'SPIRITUAL'];
    }
    if (!result.includes(face)) result.push(face);
  }
  return result;
}

/** Get next available symbol index for a player */
export function getNextAvailableSymbol(player: PlayerState): SymbolIndex | null {
  for (let i = 0; i < player.symbols.length; i++) {
    if (!player.symbols[i].assigned) return i as SymbolIndex;
  }
  return null;
}

/** Get symbol marker supply (not on trick, not on board) */
export function getSymbolSupply(
  state: GameState,
  playerId: PlayerId,
  symbolIndex: SymbolIndex,
): number {
  const player = getPlayer(state, playerId);
  const trick = player.tricks.find(t => t.symbolIndex === symbolIndex);
  if (!trick) return MARKERS_PER_SYMBOL;

  let onBoard = 0;
  for (const card of state.theater.perfCards) {
    for (const marker of card.slotMarkers) {
      if (marker && marker.playerId === playerId && marker.symbolIndex === symbolIndex) {
        onBoard++;
      }
    }
  }
  return MARKERS_PER_SYMBOL - trick.markersOnTrick - onBoard;
}

/** Check if game is over */
export function isGameOver(state: GameState): boolean {
  return state.gameOver;
}

/** Get rankings sorted by fame (desc), then initiative order */
export function getRankings(state: GameState): readonly PlayerState[] {
  return [...state.players].sort((a, b) => {
    if (b.fame !== a.fame) return b.fame - a.fame;
    const aInit = state.initiativeOrder.indexOf(state.players.indexOf(a));
    const bInit = state.initiativeOrder.indexOf(state.players.indexOf(b));
    return aInit - bInit;
  });
}

/** Get total trick markers placed on all performance cards for a player */
export function countTotalMarkersOnBoard(state: GameState, playerId: PlayerId): number {
  let count = 0;
  for (const card of state.theater.perfCards) {
    for (const marker of card.slotMarkers) {
      if (marker && marker.playerId === playerId) count++;
    }
  }
  return count;
}
