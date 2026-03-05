'use client';

import { useGameStore } from '@/stores/game-store';
import { useUIStore } from '@/stores/ui-store';
import { PHASE_CONFIG } from '@/core/phases/registry';
import { ResourceBadge } from '../shared/ResourceBadge';
import { getMagicianDef } from '@/core/data/magicians';
import { Undo2, Redo2 } from 'lucide-react';
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
      <div style={{ display: 'flex', alignItems: 'center', gap: '2px' }}>
        {PHASE_ORDER.map((p, i) => {
          const isActive = p === state.phase;
          const isPast = i < currentPhaseIdx;
          return (
            <div key={p} style={{ display: 'flex', alignItems: 'center' }}>
              <div
                style={{
                  width: 24,
                  height: 24,
                  borderRadius: '50%',
                  display: 'flex',
                  alignItems: 'center',
                  justifyContent: 'center',
                  fontSize: '0.6rem',
                  fontWeight: 700,
                  fontFamily: 'var(--font-heading)',
                  background: isActive
                    ? 'linear-gradient(135deg, var(--purple), #6d28d9)'
                    : isPast
                    ? 'var(--purple-glow)'
                    : 'var(--bg-card)',
                  border: `2px solid ${isActive ? 'var(--purple-light)' : isPast ? 'var(--purple)' : 'var(--border)'}`,
                  color: isActive ? '#fff' : isPast ? 'var(--purple-light)' : 'var(--text-dim)',
                  boxShadow: isActive ? '0 0 10px var(--purple-glow)' : 'none',
                  transition: 'all 0.3s ease',
                }}
                title={PHASE_CONFIG[p].nameKo}
              >
                {PHASE_SHORT[p]?.[0] ?? ''}
              </div>
              {i < PHASE_ORDER.length - 1 && (
                <div
                  style={{
                    width: 12,
                    height: 2,
                    background: isPast ? 'var(--purple)' : 'var(--border)',
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
          title="되돌리기 (Undo)"
        >
          <Undo2 size={14} />
        </button>
        <button
          onClick={() => redo()}
          disabled={futureStates.length === 0}
          className="btn btn-sm"
          style={{ padding: '4px 8px', opacity: futureStates.length === 0 ? 0.3 : 1 }}
          title="다시하기 (Redo)"
        >
          <Redo2 size={14} />
        </button>
        <div style={{ width: 1, height: 20, background: 'var(--border)', margin: '0 4px' }} />
        {state.players.map((p, i) => {
          const mag = getMagicianDef(p.magicianId);
          return (
            <button
              key={i}
              onClick={() => setPlayerTab(i)}
              className={`player-tab ${selectedTab === i ? 'active' : ''}`}
              style={{
                borderColor: selectedTab === i ? p.color : 'transparent',
                display: 'flex',
                alignItems: 'center',
                gap: '6px',
              }}
            >
              {/* Portrait thumbnail */}
              <img
                src={mag.img}
                alt={mag.nameKo}
                style={{
                  width: 24,
                  height: 24,
                  borderRadius: '50%',
                  objectFit: 'cover',
                  border: `2px solid ${p.color}`,
                }}
                onError={(e) => {
                  (e.target as HTMLImageElement).style.display = 'none';
                }}
              />
              <span style={{ color: selectedTab === i ? p.color : undefined, fontWeight: 700 }}>
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
