'use client';

import { useGameStore } from '@/stores/game-store';
import { getNextAdvertiser } from '@/core/phases/advertise';
import { ADVERTISE_COST, ADVERTISE_FAME } from '@/core/data/constants';
import { getMagicianDef } from '@/core/data/magicians';
import { ResourceBadge } from '../shared/ResourceBadge';
import { GameIcon } from '../shared/GameIcon';

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
        <p style={{ fontSize: '0.85rem', color: 'var(--text-dim)', marginBottom: 12 }}>
          모든 플레이어가 광고를 완료했습니다.
        </p>
        <button onClick={finishAdvertise} className="btn btn-primary">
          배정 페이즈로
        </button>
      </div>
    );
  }

  const player = nextIdx !== null ? state.players[nextIdx] : null;
  const cost = ADVERTISE_COST[Math.min(state.round - 1, ADVERTISE_COST.length - 1)];

  if (!player) return null;
  const mag = getMagicianDef(player.magicianId);

  return (
    <div>
      {/* Current player info */}
      <div style={{
        display: 'flex', alignItems: 'center', gap: 10, marginBottom: 12,
        padding: '8px 12px', background: 'var(--bg-card)', borderRadius: 'var(--radius)',
        border: `1px solid ${player.color}40`,
      }}>
        <img
          src={mag.img}
          alt={mag.nameKo}
          style={{
            width: 36, height: 36, borderRadius: '50%',
            objectFit: 'cover', border: `2px solid ${player.color}`,
          }}
          onError={(e) => { (e.target as HTMLImageElement).style.display = 'none'; }}
        />
        <div>
          <div style={{ fontWeight: 700, color: player.color, fontFamily: 'var(--font-heading)' }}>
            {player.name}
          </div>
          <div style={{ display: 'flex', gap: 6 }}>
            <ResourceBadge type="coins" value={player.coins} />
            <ResourceBadge type="fame" value={player.fame} />
          </div>
        </div>
      </div>

      {/* Cost / Reward info */}
      <div style={{
        display: 'flex', justifyContent: 'space-between', alignItems: 'center',
        padding: '8px 12px', background: 'var(--bg-secondary)', borderRadius: 'var(--radius)',
        marginBottom: 12, fontSize: '0.82rem',
      }}>
        <span style={{ color: 'var(--text-dim)' }}>
          비용: <span style={{ color: 'var(--gold-primary)', fontWeight: 700 }}>{cost}</span>
          <GameIcon type="coins" size="xs" />
        </span>
        <span style={{ color: 'var(--text-dim)' }}>
          보상: <span style={{ color: 'var(--gold-primary)', fontWeight: 700 }}>+{ADVERTISE_FAME}</span>
          <GameIcon type="fame" size="xs" />
        </span>
      </div>

      {/* Action buttons */}
      <div style={{ display: 'flex', gap: 8 }}>
        <button
          onClick={() => dispatchAction({ type: 'ADVERTISE', playerId: player.id })}
          disabled={player.coins < cost}
          className="btn btn-primary"
          style={{ flex: 1 }}
        >
          <GameIcon type="fame" size="sm" color="#fff" />
          광고 ({cost}C)
        </button>
        <button
          onClick={() => dispatchAction({ type: 'SKIP_ADVERTISE', playerId: player.id })}
          className="btn"
          style={{ flex: 1 }}
        >
          건너뛰기
        </button>
      </div>
    </div>
  );
}
