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
    <div style={{ display: 'flex', flexDirection: 'column', gap: 12 }}>
      <PhaseBanner />
      <LocationsGrid />
      <TheaterSection />
      <FameTrack />
    </div>
  );
}
