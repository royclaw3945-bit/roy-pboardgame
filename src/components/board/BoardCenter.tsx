'use client';

import { useGameStore } from '@/stores/game-store';
import { GameBoard } from './GameBoard';
import { TheaterSection } from '../theater/TheaterSection';

export function BoardCenter() {
  const state = useGameStore((s) => s.state);
  if (!state) return null;

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 12, width: '100%' }}>
      <GameBoard />
      <TheaterSection />
    </div>
  );
}
