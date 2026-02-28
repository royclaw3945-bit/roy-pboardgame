'use client';

import { useGameStore } from '@/stores/game-store';
import { PhaseBanner } from './PhaseBanner';
import { LocationsGrid } from './LocationsGrid';
import { TheaterSection } from '../theater/TheaterSection';
import { FameTrack } from './FameTrack';

export function BoardCenter() {
  const state = useGameStore((s) => s.state);
  if (!state) return null;

  return (
    <div className="flex flex-col gap-2">
      <PhaseBanner />
      <div className="grid grid-cols-2 gap-2">
        <LocationsGrid />
        <TheaterSection />
      </div>
      <FameTrack />
    </div>
  );
}
