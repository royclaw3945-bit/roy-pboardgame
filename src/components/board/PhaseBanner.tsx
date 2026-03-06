'use client';

import { useGameStore } from '@/stores/game-store';
import { PHASE_CONFIG } from '@/core/phases/registry';

// Minimal phase label — full stepper is in TopBar
export function PhaseBanner() {
  const state = useGameStore((s) => s.state);
  if (!state) return null;

  const phase = PHASE_CONFIG[state.phase];

  return (
    <div className="phase-banner-mini">
      <span className="pbm-round">R{state.round}</span>
      <span className="pbm-phase">{phase.nameKo}</span>
    </div>
  );
}
