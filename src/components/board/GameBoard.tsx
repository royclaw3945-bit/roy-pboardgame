'use client';

import { useGameStore } from '@/stores/game-store';
import { LocationZone } from './LocationZone';
import { BoardFameTrack } from './FameTrack';
import type { Location } from '@/core/types';

const LOCATIONS: Location[] = [
  'DOWNTOWN', 'THEATER', 'MARKET_ROW', 'WORKSHOP', 'DARK_ALLEY',
];

export function GameBoard() {
  const state = useGameStore((s) => s.state);
  if (!state) return null;

  return (
    <div className="game-board">
      <img
        src="/img/board_main.jpg"
        alt="Trickerion Board"
        className="game-board-img"
        draggable={false}
      />
      {LOCATIONS.map((loc) => (
        <LocationZone key={loc} location={loc} />
      ))}
      <BoardFameTrack />
    </div>
  );
}
