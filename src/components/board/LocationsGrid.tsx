'use client';

import { useGameStore } from '@/stores/game-store';
import { LocationCard } from './LocationCard';
import type { Location } from '@/core/types';

const LOCATIONS: Location[] = ['DOWNTOWN', 'MARKET_ROW', 'WORKSHOP', 'THEATER', 'DARK_ALLEY'];

export function LocationsGrid() {
  const state = useGameStore((s) => s.state);
  if (!state) return null;

  const shown = state.config.useDarkAlley
    ? LOCATIONS
    : LOCATIONS.filter((l) => l !== 'DARK_ALLEY');

  return (
    <div className="flex flex-col gap-1.5">
      {shown.map((loc) => (
        <LocationCard key={loc} location={loc} />
      ))}
    </div>
  );
}
