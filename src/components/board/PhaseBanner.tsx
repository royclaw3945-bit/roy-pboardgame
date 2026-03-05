'use client';

import { useGameStore } from '@/stores/game-store';
import { PHASE_CONFIG } from '@/core/phases/registry';
import { GameIcon } from '../shared/GameIcon';
import type { Phase } from '@/core/types';

const VISIBLE_PHASES: Phase[] = [
  'ADVERTISE', 'ASSIGNMENT', 'PLACEMENT', 'PERFORMANCE', 'END_TURN',
];

const PHASE_ICONS: Record<string, string> = {
  ADVERTISE: 'fame',
  ASSIGNMENT: 'MAGICIAN',
  PLACEMENT: 'ap',
  PERFORMANCE: 'THEATER',
  END_TURN: 'coins',
};

export function PhaseBanner() {
  const state = useGameStore((s) => s.state);
  if (!state) return null;

  const currentIdx = VISIBLE_PHASES.indexOf(
    state.phase === 'ASSIGNMENT_REVEAL' ? 'ASSIGNMENT' : state.phase,
  );

  return (
    <div className="phase-banner">
      <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', gap: 12, marginBottom: 10 }}>
        <span style={{
          fontSize: '0.85rem', color: 'var(--gold-primary)',
          fontFamily: 'var(--font-heading)', fontWeight: 700,
          background: 'rgba(251, 191, 36, 0.1)', padding: '4px 14px', borderRadius: 20,
          border: '1px solid rgba(251, 191, 36, 0.2)',
        }}>
          R{state.round}/{state.maxRounds}
        </span>
        <h2 style={{ fontSize: '1.1rem', marginBottom: 0 }}>
          {PHASE_CONFIG[state.phase].nameKo}
        </h2>
      </div>

      <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', gap: 0 }}>
        {VISIBLE_PHASES.map((p, i) => {
          const config = PHASE_CONFIG[p];
          const isActive = i === currentIdx;
          const isPast = i < currentIdx;
          return (
            <div key={p} style={{ display: 'flex', alignItems: 'center' }}>
              <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 4 }}>
                <div
                  style={{
                    width: isActive ? 42 : 32,
                    height: isActive ? 42 : 32,
                    borderRadius: '50%',
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    background: isActive
                      ? 'linear-gradient(135deg, var(--purple), var(--cyan))'
                      : isPast
                      ? 'rgba(124, 58, 237, 0.2)'
                      : 'var(--bg-secondary)',
                    border: `2px solid ${
                      isActive ? 'var(--cyan-light)' : isPast ? 'var(--purple)' : 'var(--border)'
                    }`,
                    boxShadow: isActive ? '0 0 16px var(--purple-glow), 0 0 8px var(--cyan-glow)' : 'none',
                    transition: 'all 0.4s ease',
                  }}
                >
                  <GameIcon
                    type={PHASE_ICONS[p] ?? 'ap'}
                    size={isActive ? 'md' : 'sm'}
                    color={isActive ? '#fff' : isPast ? 'var(--purple-light)' : 'var(--text-dim)'}
                  />
                </div>
                <span
                  style={{
                    fontSize: isActive ? '0.8rem' : '0.7rem',
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
                    width: 36,
                    height: 3,
                    borderRadius: 2,
                    background: isPast
                      ? 'linear-gradient(90deg, var(--purple), var(--cyan))'
                      : 'var(--border)',
                    margin: '0 6px',
                    marginBottom: 22,
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
