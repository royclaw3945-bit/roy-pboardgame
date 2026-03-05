'use client';

import { motion, AnimatePresence } from 'framer-motion';
import { useGameStore } from '@/stores/game-store';
import { PHASE_CONFIG } from '@/core/phases/registry';
import { AdvertisePanel } from './AdvertisePanel';
import { AssignmentPanel } from './AssignmentPanel';
import { PlacementPanel } from './PlacementPanel';
import { GameIcon } from '../shared/GameIcon';

export function ActionPanel() {
  const state = useGameStore((s) => s.state);
  const lastError = useGameStore((s) => s.lastError);
  if (!state) return null;

  const phase = PHASE_CONFIG[state.phase];

  return (
    <div>
      {/* Error toast */}
      {lastError && (
        <div style={{
          padding: '8px 12px', marginBottom: 8,
          background: 'rgba(255,80,80,0.15)', border: '1px solid var(--red)',
          borderRadius: 'var(--radius)', fontSize: '0.78rem', color: '#ff6b6b',
        }}>
          {lastError}
        </div>
      )}

      {/* Phase header */}
      <h3 className="action-panel-title" style={{
        display: 'flex', alignItems: 'center', gap: 8,
        marginBottom: 12, fontFamily: 'var(--font-heading)',
      }}>
        <GameIcon type="ap" size="md" />
        <span>{phase.nameKo}</span>
      </h3>

      <AnimatePresence mode="wait">
        <motion.div
          key={state.phase}
          initial={{ opacity: 0, y: 12 }}
          animate={{ opacity: 1, y: 0 }}
          exit={{ opacity: 0, y: -12 }}
          transition={{ duration: 0.2 }}
        >
          {state.phase === 'ADVERTISE' && <AdvertisePanel />}
          {state.phase === 'ASSIGNMENT' && <AssignmentPanel />}
          {state.phase === 'ASSIGNMENT_REVEAL' && <AssignmentPanel />}
          {state.phase === 'PLACEMENT' && <PlacementPanel />}
          {state.phase === 'PERFORMANCE' && <PerformanceActions />}
          {state.phase === 'END_TURN' && <EndTurnActions />}
          {state.phase === 'GAME_OVER' && <GameOverPanel />}
        </motion.div>
      </AnimatePresence>
    </div>
  );
}

function PerformanceActions() {
  const finishPerformance = useGameStore((s) => s.finishPerformance);
  return (
    <div style={{ textAlign: 'center', padding: '20px 0' }}>
      <div style={{ fontSize: '2rem', marginBottom: 8 }}>
        <GameIcon type="THEATER" size="lg" color="var(--loc-theater)" />
      </div>
      <p style={{ fontSize: '0.95rem', color: 'var(--text)', marginBottom: 16, fontFamily: 'var(--font-heading)' }}>
        공연 진행 중
      </p>
      <button onClick={finishPerformance} className="btn btn-primary btn-lg" style={{ width: '100%' }}>
        <GameIcon type="THEATER" size="sm" color="#fff" />
        공연 완료
      </button>
    </div>
  );
}

function EndTurnActions() {
  const finishRound = useGameStore((s) => s.finishRound);
  return (
    <div style={{ textAlign: 'center', padding: '20px 0' }}>
      <p style={{ fontSize: '0.95rem', color: 'var(--text)', marginBottom: 16, fontFamily: 'var(--font-heading)' }}>
        라운드 종료
      </p>
      <button onClick={finishRound} className="btn btn-primary btn-lg" style={{ width: '100%' }}>
        다음 라운드
      </button>
    </div>
  );
}

function GameOverPanel() {
  const state = useGameStore((s) => s.state);
  const applyScoring = useGameStore((s) => s.applyScoring);
  if (!state) return null;

  return (
    <div style={{ textAlign: 'center', padding: '16px 0' }}>
      <h3 style={{
        fontFamily: 'var(--font-title)',
        fontSize: '1.5rem',
        color: 'var(--gold-primary)',
        marginBottom: 12,
      }}>
        게임 종료
      </h3>
      {state.winner !== null ? (
        <div>
          <p style={{ fontSize: '1.2rem', marginBottom: 12 }}>
            <GameIcon type="fame" size="lg" color="var(--gold-primary)" />
          </p>
          <p style={{
            fontSize: '1.1rem', fontFamily: 'var(--font-heading)', fontWeight: 700,
            color: state.players[state.winner].color,
          }}>
            {state.players[state.winner].name} 승리!
          </p>
          <div style={{ marginTop: 16 }}>
            {state.players
              .slice()
              .sort((a, b) => b.fame - a.fame)
              .map((p, i) => (
                <div key={p.id as number} className="fame-row" style={{ marginBottom: 4 }}>
                  <span style={{
                    width: 24, fontWeight: 700, fontFamily: 'var(--font-heading)',
                    color: i === 0 ? 'var(--gold-primary)' : 'var(--text-dim)',
                  }}>
                    #{i + 1}
                  </span>
                  <span className="fame-name" style={{ color: p.color }}>{p.name}</span>
                  <span className="fame-val">{p.fame}</span>
                </div>
              ))}
          </div>
        </div>
      ) : (
        <button onClick={applyScoring} className="btn btn-primary btn-lg">
          <GameIcon type="fame" size="md" color="#fff" />
          최종 점수 계산
        </button>
      )}
    </div>
  );
}
