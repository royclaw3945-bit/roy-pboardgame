'use client';

import { useGameStore } from '@/stores/game-store';

export function FameTrack() {
  const state = useGameStore((s) => s.state);
  if (!state) return null;

  const maxFame = Math.max(...state.players.map((p) => p.fame), 50);

  return (
    <div className="rounded-lg bg-[var(--bg-card)] p-2">
      <h3 className="mb-1 text-xs font-bold text-[var(--text-secondary)]">명성 트랙</h3>
      <div className="flex flex-col gap-1">
        {state.players.map((p) => (
          <div key={p.id as number} className="flex items-center gap-2">
            <span className="w-16 text-xs font-bold" style={{ color: p.color }}>
              {p.name}
            </span>
            <div className="flex-1 h-4 bg-[var(--bg-dark)] rounded overflow-hidden">
              <div
                className="h-full rounded transition-all duration-300"
                style={{
                  width: `${(p.fame / maxFame) * 100}%`,
                  backgroundColor: p.color,
                }}
              />
            </div>
            <span className="w-8 text-right text-xs font-bold">{p.fame}</span>
          </div>
        ))}
      </div>
    </div>
  );
}
