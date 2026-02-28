'use client';

import { useGameStore } from '@/stores/game-store';
import { useUIStore } from '@/stores/ui-store';
import { PHASE_CONFIG } from '@/core/phases/registry';

export function TopBar() {
  const state = useGameStore((s) => s.state);
  const openModal = useUIStore((s) => s.openModal);
  const setPlayerTab = useUIStore((s) => s.setPlayerTab);
  const selectedTab = useUIStore((s) => s.selectedPlayerTab);

  if (!state) return null;

  const phase = PHASE_CONFIG[state.phase];

  return (
    <header className="flex items-center justify-between bg-[var(--bg-card)] px-4 py-2 rounded-lg">
      <div className="flex items-center gap-3">
        <span className="text-[var(--gold)] font-bold text-lg">
          R{state.round}/{state.maxRounds}
        </span>
        <span className="bg-[var(--purple)] px-3 py-1 rounded text-sm font-bold">
          {phase.nameKo}
        </span>
      </div>

      <div className="flex gap-1">
        {state.players.map((p, i) => (
          <button
            key={i}
            onClick={() => setPlayerTab(i)}
            className={`flex items-center gap-1 rounded px-3 py-1.5 text-sm transition-all
              ${selectedTab === i ? 'ring-2 ring-white/50' : 'opacity-70 hover:opacity-100'}`}
            style={{ backgroundColor: p.color + '33', borderColor: p.color }}
          >
            <span className="font-bold" style={{ color: p.color }}>{p.name}</span>
            <span className="text-xs text-[var(--text-secondary)]">
              {p.fame}F {p.coins}C {p.shards}S
            </span>
          </button>
        ))}
      </div>

      <button
        onClick={() => openModal('SAVE_LOAD')}
        className="rounded bg-[var(--bg-panel)] px-3 py-1.5 text-sm hover:bg-[var(--bg-dark)]"
      >
        저장/불러오기
      </button>
    </header>
  );
}
