// Assignment Cards — DA only: 6 permanent cards per player

import type { Location, AssignmentCardId } from '../types';
import type { AssignmentCardState } from '../types';

export interface AssignmentCardDef {
  readonly location: Location;
}

// DA: Theater×2, Workshop×1, Market×1, Downtown×1, DarkAlley×1
const DA_CARDS: readonly AssignmentCardDef[] = [
  { location: 'THEATER' },
  { location: 'THEATER' },
  { location: 'WORKSHOP' },
  { location: 'MARKET_ROW' },
  { location: 'DOWNTOWN' },
  { location: 'DARK_ALLEY' },
];

export function createAssignmentCards(
  playerIdx: number,
): readonly AssignmentCardState[] {
  return DA_CARDS.map((def, i) => ({
    id: `P${playerIdx}_AC${i}` as AssignmentCardId,
    location: def.location,
    kind: 'PERMANENT' as const,
  }));
}

export function getCardCount(): number {
  return DA_CARDS.length;
}
