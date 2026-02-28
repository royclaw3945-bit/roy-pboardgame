'use client';

import { useGameStore } from '@/stores/game-store';
import { AdvertisePanel } from './AdvertisePanel';
import { AssignmentPanel } from './AssignmentPanel';
import { PlacementPanel } from './PlacementPanel';

export function ActionPanel() {
  const state = useGameStore((s) => s.state);
  if (!state) return null;

  return (
    <div className="rounded-lg bg-[var(--bg-card)] p-3">
      {state.phase === 'ADVERTISE' && <AdvertisePanel />}
      {state.phase === 'ASSIGNMENT' && <AssignmentPanel />}
      {state.phase === 'ASSIGNMENT_REVEAL' && <AssignmentPanel />}
      {state.phase === 'PLACEMENT' && <PlacementPanel />}
      {state.phase === 'PERFORMANCE' && <PerformanceActions />}
      {state.phase === 'END_TURN' && <EndTurnActions />}
      {state.phase === 'GAME_OVER' && <GameOverPanel />}
    </div>
  );
}

function PerformanceActions() {
  const finishPerformance = useGameStore((s) => s.finishPerformance);
  return (
    <div>
      <h3 className="mb-2 text-sm font-bold">공연 페이즈</h3>
      <button
        onClick={finishPerformance}
        className="rounded bg-[var(--purple)] px-4 py-2 text-sm font-bold hover:bg-purple-700"
      >
        공연 완료 → 턴 종료
      </button>
    </div>
  );
}

function EndTurnActions() {
  const finishRound = useGameStore((s) => s.finishRound);
  return (
    <div>
      <h3 className="mb-2 text-sm font-bold">턴 종료</h3>
      <button
        onClick={finishRound}
        className="rounded bg-[var(--orange)] px-4 py-2 text-sm font-bold hover:bg-orange-700"
      >
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
    <div className="text-center">
      <h3 className="mb-2 text-xl font-bold text-[var(--gold)]">게임 종료!</h3>
      {state.winner !== null ? (
        <p className="text-lg">
          우승:{' '}
          <span style={{ color: state.players[state.winner].color }}>
            {state.players[state.winner].name}
          </span>
        </p>
      ) : (
        <button
          onClick={applyScoring}
          className="rounded bg-[var(--gold)] px-6 py-2 text-sm font-bold text-black hover:bg-yellow-400"
        >
          최종 점수 계산
        </button>
      )}
    </div>
  );
}
