'use client';

import { useGameStore } from '@/stores/game-store';
import { useUIStore } from '@/stores/ui-store';
import { PHASE_CONFIG } from '@/core/phases/registry';
import { ResourceBadge } from '../shared/ResourceBadge';
import { getMagicianDef } from '@/core/data/magicians';
function Undo2Icon({ size = 14 }: { size?: number }) {
  return (
    <svg width={size} height={size} viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
      <path d="M9 14L4 9l5-5" /><path d="M4 9h10.5a5.5 5.5 0 0 1 0 11H11" />
    </svg>
  );
}
function Redo2Icon({ size = 14 }: { size?: number }) {
  return (
    <svg width={size} height={size} viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
      <path d="M15 14l5-5-5-5" /><path d="M20 9H9.5a5.5 5.5 0 0 0 0 11H13" />
    </svg>
  );
}
import { useStore } from 'zustand';
import type { Phase } from '@/core/types';

const PHASE_ORDER: Phase[] = [
  'ADVERTISE', 'ASSIGNMENT', 'ASSIGNMENT_REVEAL', 'PLACEMENT', 'PERFORMANCE', 'END_TURN',
];

const PHASE_SHORT: Record<string, string> = {
  ADVERTISE: '광고',
  ASSIGNMENT: '배정',
  ASSIGNMENT_REVEAL: '공개',
  PLACEMENT: '배치',
  PERFORMANCE: '공연',
  END_TURN: '종료',
};

export function TopBar() {
  const state = useGameStore((s) => s.state);
  const openModal = useUIStore((s) => s.openModal);
  const setPlayerTab = useUIStore((s) => s.setPlayerTab);
  const selectedTab = useUIStore((s) => s.selectedPlayerTab);
  const { undo, redo, pastStates, futureStates } = useStore(useGameStore.temporal);

  if (!state) return null;

  const phase = PHASE_CONFIG[state.phase];
  const currentPhaseIdx = PHASE_ORDER.indexOf(state.phase);

  return (
    <header className="top-bar">
      {/* Left: Round + Phase */}
      <div className="round-info">
        <span className="round-badge">R{state.round}/{state.maxRounds}</span>
        <span className="phase-badge">{phase.nameKo}</span>
      </div>

      {/* Center: Phase Stepper */}
      <div style={{ display: 'flex', alignItems: 'center', gap: 0 }}>
        {PHASE_ORDER.map((p, i) => {
          const isActive = p === state.phase;
          const isPast = i < currentPhaseIdx;
          return (
            <div key={p} style={{ display: 'flex', alignItems: 'center' }}>
              <div
                style={{
                  display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 2,
                }}
              >
                <div
                  style={{
                    width: isActive ? 30 : 24,
                    height: isActive ? 30 : 24,
                    borderRadius: '50%',
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    fontSize: isActive ? '0.7rem' : '0.6rem',
                    fontWeight: 700,
                    fontFamily: 'var(--font-heading)',
                    background: isActive
                      ? 'linear-gradient(135deg, var(--purple), var(--cyan))'
                      : isPast
                      ? 'rgba(124, 58, 237, 0.25)'
                      : 'var(--bg-card)',
                    border: `2px solid ${isActive ? 'var(--cyan-light)' : isPast ? 'var(--purple)' : 'var(--border)'}`,
                    color: isActive ? '#fff' : isPast ? 'var(--purple-light)' : 'var(--text-dim)',
                    boxShadow: isActive ? '0 0 12px var(--purple-glow), 0 0 6px var(--cyan-glow)' : 'none',
                    transition: 'all 0.3s ease',
                  }}
                  title={PHASE_CONFIG[p].nameKo}
                >
                  {i + 1}
                </div>
                <span style={{
                  fontSize: '0.58rem',
                  color: isActive ? 'var(--cyan-light)' : isPast ? 'var(--purple-light)' : 'var(--text-dim)',
                  fontFamily: 'var(--font-heading)',
                  fontWeight: isActive ? 700 : 400,
                  whiteSpace: 'nowrap',
                }}>
                  {PHASE_SHORT[p] ?? ''}
                </span>
              </div>
              {i < PHASE_ORDER.length - 1 && (
                <div
                  style={{
                    width: 16,
                    height: 2,
                    background: isPast
                      ? 'linear-gradient(90deg, var(--purple), var(--cyan))'
                      : 'var(--border)',
                    margin: '0 2px',
                    marginBottom: 14,
                    transition: 'background 0.3s ease',
                  }}
                />
              )}
            </div>
          );
        })}
      </div>

      {/* Right: Undo/Redo + Player Tabs */}
      <div className="player-tabs">
        <button
          onClick={() => undo()}
          disabled={pastStates.length === 0}
          className="btn btn-sm"
          style={{ padding: '4px 8px', opacity: pastStates.length === 0 ? 0.3 : 1 }}
          title="Undo"
        >
          <Undo2Icon size={14} />
        </button>
        <button
          onClick={() => redo()}
          disabled={futureStates.length === 0}
          className="btn btn-sm"
          style={{ padding: '4px 8px', opacity: futureStates.length === 0 ? 0.3 : 1 }}
          title="Redo"
        >
          <Redo2Icon size={14} />
        </button>
        <div style={{ width: 1, height: 20, background: 'var(--border)', margin: '0 4px' }} />
        {state.players.map((p, i) => {
          const mag = getMagicianDef(p.magicianId);
          const isActive = selectedTab === i;
          return (
            <button
              key={i}
              onClick={() => setPlayerTab(i)}
              className={`player-tab ${isActive ? 'active' : ''}`}
              style={{
                borderColor: isActive ? p.color : 'transparent',
                display: 'flex',
                alignItems: 'center',
                gap: '6px',
              }}
            >
              <img
                src={mag.img}
                alt={mag.nameKo}
                style={{
                  width: 26,
                  height: 26,
                  borderRadius: '50%',
                  objectFit: 'cover',
                  border: `2px solid ${p.color}`,
                  boxShadow: isActive ? `0 0 6px ${p.color}` : 'none',
                }}
                onError={(e) => {
                  (e.target as HTMLImageElement).style.display = 'none';
                }}
              />
              <span style={{ color: isActive ? p.color : undefined, fontWeight: 700, fontSize: '0.85rem' }}>
                {p.name}
              </span>
              <ResourceBadge type="fame" value={p.fame} />
            </button>
          );
        })}

        <button
          onClick={() => openModal('SAVE_LOAD')}
          className="btn btn-sm"
          style={{ marginLeft: 8 }}
        >
          저장
        </button>
      </div>
    </header>
  );
}
