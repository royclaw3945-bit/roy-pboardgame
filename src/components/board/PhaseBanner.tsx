'use client';

import { useGameStore } from '@/stores/game-store';
import { PHASE_CONFIG } from '@/core/phases/registry';

export function PhaseBanner() {
  const state = useGameStore((s) => s.state);
  if (!state) return null;

  const phases = Object.entries(PHASE_CONFIG) as [string, { nameKo: string }][];

  return (
    <div className="flex gap-1 rounded-lg bg-[var(--bg-card)] p-2">
      {phases.map(([key, config]) => (
        <div
          key={key}
          className={`flex-1 rounded px-2 py-1 text-center text-xs font-bold transition-all
            ${state.phase === key
              ? 'bg-[var(--purple)] text-white scale-105'
              : 'bg-[var(--bg-dark)] text-[var(--text-secondary)]'
            }`}
        >
          {config.nameKo}
        </div>
      ))}
    </div>
  );
}
