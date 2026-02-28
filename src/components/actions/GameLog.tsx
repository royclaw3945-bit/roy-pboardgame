'use client';

import { useGameStore } from '@/stores/game-store';
import { useUIStore } from '@/stores/ui-store';

export function GameLog() {
  const state = useGameStore((s) => s.state);
  const showLog = useUIStore((s) => s.showLog);
  const toggleLog = useUIStore((s) => s.toggleLog);

  if (!state) return null;

  const entries = state.log.slice(-20).reverse();

  return (
    <div className="rounded-lg bg-[var(--bg-card)] p-2">
      <button
        onClick={toggleLog}
        className="w-full text-left text-xs font-bold text-[var(--text-secondary)] hover:text-white"
      >
        게임 로그 ({state.log.length}) {showLog ? '▼' : '▶'}
      </button>
      {showLog && (
        <div className="mt-1 max-h-48 overflow-y-auto">
          {entries.map((entry, i) => (
            <p key={i} className="text-[10px] text-[var(--text-secondary)] leading-tight">
              <span className="text-[var(--gold)]">[R{entry.round}]</span> {entry.message}
            </p>
          ))}
        </div>
      )}
    </div>
  );
}
