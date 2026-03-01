'use client';

import { useGameStore } from '@/stores/game-store';
import { PHASE_CONFIG } from '@/core/phases/registry';
import type { Phase } from '@/core/types';

const VISIBLE_PHASES: Phase[] = [
  'ADVERTISE', 'ASSIGNMENT', 'PLACEMENT', 'PERFORMANCE', 'END_TURN',
];

export function PhaseBanner() {
  const state = useGameStore((s) => s.state);
  if (!state) return null;

  const currentIdx = VISIBLE_PHASES.indexOf(
    state.phase === 'ASSIGNMENT_REVEAL' ? 'ASSIGNMENT' : state.phase,
  );

  return (
    <div className="phase-banner">
      <h2 style={{ fontSize: '1rem', marginBottom: 8 }}>
        라운드 {state.round} / {state.maxRounds}
      </h2>
      <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', gap: 0 }}>
        {VISIBLE_PHASES.map((p, i) => {
          const config = PHASE_CONFIG[p];
          const isActive = i === currentIdx;
          const isPast = i < currentIdx;
          return (
            <div key={p} style={{ display: 'flex', alignItems: 'center' }}>
              <div
                style={{
                  display: 'flex',
                  flexDirection: 'column',
                  alignItems: 'center',
                  gap: 4,
                }}
              >
                <div
                  style={{
                    width: isActive ? 36 : 28,
                    height: isActive ? 36 : 28,
                    borderRadius: '50%',
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    fontSize: isActive ? '0.75rem' : '0.65rem',
                    fontWeight: 700,
                    fontFamily: 'var(--font-heading)',
                    background: isActive
                      ? 'linear-gradient(135deg, var(--purple), var(--cyan))'
                      : isPast
                      ? 'rgba(124, 58, 237, 0.2)'
                      : 'var(--bg-secondary)',
                    border: `2px solid ${
                      isActive ? 'var(--cyan-light)' : isPast ? 'var(--purple)' : 'var(--border)'
                    }`,
                    color: isActive ? '#fff' : isPast ? 'var(--purple-light)' : 'var(--text-dim)',
                    boxShadow: isActive ? '0 0 16px var(--purple-glow), 0 0 8px var(--cyan-glow)' : 'none',
                    transition: 'all 0.4s ease',
                  }}
                >
                  {i + 1}
                </div>
                <span
                  style={{
                    fontSize: isActive ? '0.72rem' : '0.65rem',
                    color: isActive ? 'var(--cyan-light)' : isPast ? 'var(--purple-light)' : 'var(--text-dim)',
                    fontFamily: 'var(--font-heading)',
                    fontWeight: isActive ? 700 : 400,
                    transition: 'all 0.3s ease',
                  }}
                >
                  {config.nameKo}
                </span>
              </div>
              {i < VISIBLE_PHASES.length - 1 && (
                <div
                  style={{
                    width: 32,
                    height: 2,
                    background: isPast
                      ? 'linear-gradient(90deg, var(--purple), var(--cyan))'
                      : 'var(--border)',
                    margin: '0 4px',
                    marginBottom: 20,
                    transition: 'background 0.4s ease',
                  }}
                />
              )}
            </div>
          );
        })}
      </div>
    </div>
  );
}
