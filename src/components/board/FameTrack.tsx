'use client';

import { useGameStore } from '@/stores/game-store';
import { getMagicianDef } from '@/core/data/magicians';

export function BoardFameTrack() {
  const state = useGameStore((s) => s.state);
  if (!state) return null;

  const maxFame = Math.max(...state.players.map((p) => p.fame), 50);

  return (
    <div className="board-fame-track">
      {state.players.map((p) => {
        const mag = getMagicianDef(p.magicianId);
        const pct = Math.min((p.fame / maxFame) * 100, 100);
        return (
          <div
            key={p.id as number}
            className="bft-token"
            style={{
              left: `${pct}%`,
              '--player-color': p.color,
            } as React.CSSProperties}
            title={`${p.name}: ${p.fame} 명성`}
          >
            <img
              src={mag.img}
              alt={p.name}
              className="bft-img"
              style={{ borderColor: p.color }}
              onError={(e) => {
                (e.target as HTMLImageElement).style.display = 'none';
              }}
            />
            <span className="bft-fame">{p.fame}</span>
          </div>
        );
      })}
    </div>
  );
}

// Legacy export for backward compatibility
export { BoardFameTrack as FameTrack };
