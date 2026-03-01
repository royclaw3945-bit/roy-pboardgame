'use client';

import { useGameStore } from '@/stores/game-store';
import { getMagicianDef } from '@/core/data/magicians';
import { GameIcon } from '../shared/GameIcon';

export function FameTrack() {
  const state = useGameStore((s) => s.state);
  if (!state) return null;

  const maxFame = Math.max(...state.players.map((p) => p.fame), 50);

  return (
    <div className="fame-track">
      <h4>
        <GameIcon type="fame" size="sm" /> 명성 트랙
      </h4>
      {state.players.map((p) => {
        const mag = getMagicianDef(p.magicianId);
        return (
          <div key={p.id as number} className="fame-row">
            <div className="fame-name" style={{ display: 'flex', alignItems: 'center', gap: 4 }}>
              <img
                src={mag.img}
                alt={p.name}
                style={{
                  width: 18,
                  height: 18,
                  borderRadius: '50%',
                  objectFit: 'cover',
                  border: `1.5px solid ${p.color}`,
                }}
                onError={(e) => {
                  (e.target as HTMLImageElement).style.display = 'none';
                }}
              />
              <span style={{ color: p.color }}>{p.name}</span>
            </div>
            <div className="fame-bar-bg">
              <div
                className="fame-bar"
                style={{
                  width: `${Math.min((p.fame / maxFame) * 100, 100)}%`,
                  background: `linear-gradient(90deg, ${p.color}, ${p.color}88)`,
                }}
              />
            </div>
            <span className="fame-val">{p.fame}</span>
          </div>
        );
      })}
    </div>
  );
}
