// Assignment Cards — v3: 개인 소유, 플레이어별 생성
// base 9장 + DA 확장 6장 = 15장/플레이어

import type { Location, AssignmentCardId } from '../types';
import type { AssignmentCardState } from '../types';

export interface AssignmentCardDef {
  readonly location: Location;
  readonly isExpansion: boolean;
}

const BASE_CARDS: readonly AssignmentCardDef[] = [
  { location: 'THEATER', isExpansion: false },
  { location: 'THEATER', isExpansion: false },
  { location: 'THEATER', isExpansion: false },
  { location: 'WORKSHOP', isExpansion: false },
  { location: 'WORKSHOP', isExpansion: false },
  { location: 'MARKET_ROW', isExpansion: false },
  { location: 'MARKET_ROW', isExpansion: false },
  { location: 'DOWNTOWN', isExpansion: false },
  { location: 'DOWNTOWN', isExpansion: false },
];

const DA_EXPANSION_CARDS: readonly AssignmentCardDef[] = [
  { location: 'DOWNTOWN', isExpansion: true },
  { location: 'DARK_ALLEY', isExpansion: true },
  { location: 'THEATER', isExpansion: true },
  { location: 'THEATER', isExpansion: true },
  { location: 'WORKSHOP', isExpansion: true },
  { location: 'MARKET_ROW', isExpansion: true },
];

export function createAssignmentCards(
  playerIdx: number,
  useDarkAlley: boolean,
): readonly AssignmentCardState[] {
  const defs = useDarkAlley
    ? [...BASE_CARDS, ...DA_EXPANSION_CARDS]
    : BASE_CARDS;

  return defs.map((def, i) => ({
    id: `P${playerIdx}_AC${i}` as AssignmentCardId,
    location: def.location,
    isExpansion: def.isExpansion,
  }));
}

export function getBaseCardCount(): number {
  return BASE_CARDS.length;
}

export function getExpansionCardCount(): number {
  return DA_EXPANSION_CARDS.length;
}
