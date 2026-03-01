'use client';

import { useGameStore } from '@/stores/game-store';
import { LocationCard } from './LocationCard';
import type { Location } from '@/core/types';

const LOCATIONS: Location[] = ['THEATER', 'DOWNTOWN', 'MARKET_ROW', 'WORKSHOP', 'DARK_ALLEY'];

export function LocationsGrid() {
  const state = useGameStore((s) => s.state);
  if (!state) return null;

  return (
    <div className="locations-grid">
      {LOCATIONS.map((loc) => (
        <LocationCard
          key={loc}
          location={loc}
          fullWidth={loc === 'THEATER'}
        />
      ))}
    </div>
  );
}
