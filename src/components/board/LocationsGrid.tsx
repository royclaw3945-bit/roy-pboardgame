'use client';

import { useGameStore } from '@/stores/game-store';
import { LocationCard } from './LocationCard';
import type { Location } from '@/core/types';

const LOCATIONS: Location[] = ['DOWNTOWN', 'MARKET_ROW', 'WORKSHOP', 'THEATER', 'DARK_ALLEY'];

export function LocationsGrid() {
  const state = useGameStore((s) => s.state);
  if (!state) return null;

  // v4: DA always enabled
  const shown = LOCATIONS;

  return (
    <div className="flex flex-col gap-1.5">
      {shown.map((loc) => (
        <LocationCard key={loc} location={loc} />
      ))}
    </div>
  );
}
