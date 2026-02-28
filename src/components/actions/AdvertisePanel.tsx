'use client';

import { useGameStore } from '@/stores/game-store';
import { getNextAdvertiser } from '@/core/phases/advertise';
import { ADVERTISE_COST, ADVERTISE_FAME } from '@/core/data/constants';
import type { PlayerId } from '@/core/types';

export function AdvertisePanel() {
  const state = useGameStore((s) => s.state);
  const dispatchAction = useGameStore((s) => s.dispatch);
  const finishAdvertise = useGameStore((s) => s.finishAdvertise);

  if (!state) return null;

  const nextIdx = getNextAdvertiser(state);
  const allDone = state.players.every((p) => p.hasAdvertised);

  if (allDone) {
    return (
      <div>
        <h3 className="mb-2 text-sm font-bold">광고 페이즈</h3>
        <p className="mb-2 text-xs text-[var(--text-secondary)]">모든 플레이어 완료</p>
        <button
          onClick={finishAdvertise}
          className="rounded bg-[var(--green)] px-4 py-2 text-sm font-bold hover:bg-green-700"
        >
          배정 페이즈로 →
        </button>
      </div>
    );
  }

  const player = nextIdx !== null ? state.players[nextIdx] : null;
  const cost = ADVERTISE_COST[Math.min(state.round - 1, ADVERTISE_COST.length - 1)];

  return (
    <div>
      <h3 className="mb-2 text-sm font-bold">광고 페이즈</h3>
      {player && (
        <div className="mb-2">
          <span className="font-bold" style={{ color: player.color }}>
            {player.name}
          </span>
          <span className="ml-2 text-xs text-[var(--text-secondary)]">
            비용: {cost}코인 / 보상: +{ADVERTISE_FAME}명성
          </span>
        </div>
      )}
      <div className="flex gap-2">
        <button
          onClick={() => player && dispatchAction({
            type: 'ADVERTISE', playerId: player.id,
          })}
          disabled={!player || player.coins < cost}
          className="rounded bg-[var(--purple)] px-4 py-2 text-sm font-bold
                     hover:bg-purple-700 disabled:opacity-50"
        >
          광고 ({cost}C)
        </button>
        <button
          onClick={() => player && dispatchAction({
            type: 'SKIP_ADVERTISE', playerId: player.id,
          })}
          className="rounded bg-[var(--bg-panel)] px-4 py-2 text-sm hover:bg-[var(--bg-dark)]"
        >
          건너뛰기
        </button>
      </div>
    </div>
  );
}
