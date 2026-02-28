'use client';

import { useGameStore } from '@/stores/game-store';
import { useUIStore } from '@/stores/ui-store';
import { getTrickDef } from '@/core/data/tricks';
import { CHARACTER_META, COMPONENT_META } from '@/core/data/constants';
import type { ComponentType } from '@/core/types';

const ALL_COMPS: ComponentType[] = [
  'WOOD', 'METAL', 'GLASS', 'FABRIC',
  'ROPE', 'OIL', 'SAW', 'ANIMAL',
  'LOCK', 'MIRROR', 'DISGUISE', 'GEAR',
];

export function PlayerBoard() {
  const state = useGameStore((s) => s.state);
  const selectedTab = useUIStore((s) => s.selectedPlayerTab);

  if (!state) return null;
  const player = state.players[selectedTab];
  if (!player) return null;

  return (
    <div className="rounded-lg bg-[var(--bg-card)] p-3">
      <h3 className="mb-2 text-sm font-bold" style={{ color: player.color }}>
        {player.name}
      </h3>

      {/* Resources */}
      <div className="mb-2 flex gap-3 text-xs">
        <span>Fame: <b>{player.fame}</b></span>
        <span>Coins: <b>{player.coins}</b></span>
        <span>Shards: <b>{player.shards}</b></span>
      </div>

      {/* Characters */}
      <div className="mb-2">
        <h4 className="text-xs font-bold text-[var(--text-secondary)]">캐릭터</h4>
        <div className="mt-1 flex flex-wrap gap-1">
          {player.characters.map((c, i) => (
            <div
              key={i}
              className={`rounded px-2 py-0.5 text-xs
                ${c.placed ? 'bg-green-900/30' : c.assigned ? 'bg-yellow-900/30' : 'bg-[var(--bg-dark)]'}`}
            >
              {CHARACTER_META[c.type].name}
              {c.location && <span className="ml-1 text-[var(--text-secondary)]">@{c.location}</span>}
              {c.ap > 0 && <span className="ml-1 text-[var(--gold)]">AP:{c.ap}</span>}
            </div>
          ))}
        </div>
      </div>

      {/* Tricks */}
      <div className="mb-2">
        <h4 className="text-xs font-bold text-[var(--text-secondary)]">
          트릭 ({player.tricks.length}/4)
        </h4>
        {player.tricks.length === 0 ? (
          <p className="mt-1 text-xs text-[var(--text-secondary)]">없음</p>
        ) : (
          <div className="mt-1 flex flex-col gap-0.5">
            {player.tricks.map((slot, i) => {
              const trick = getTrickDef(slot.trickId);
              return (
                <div key={i} className="flex items-center gap-1 text-xs">
                  <span className={slot.prepared ? 'text-green-400' : 'text-red-400'}>
                    {slot.prepared ? 'V' : 'X'}
                  </span>
                  <span>{trick.nameKo}</span>
                  <span className="text-[var(--text-secondary)]">Lv{trick.level}</span>
                  <span className="text-[var(--text-secondary)]">M:{slot.markersOnTrick}</span>
                </div>
              );
            })}
          </div>
        )}
      </div>

      {/* Components */}
      <div>
        <h4 className="text-xs font-bold text-[var(--text-secondary)]">컴포넌트</h4>
        <div className="mt-1 grid grid-cols-4 gap-0.5">
          {ALL_COMPS.map((comp) => {
            const count = player.components[comp];
            if (count === 0) return null;
            return (
              <div key={comp} className="text-xs">
                <span className="text-[var(--text-secondary)]">{COMPONENT_META[comp].name}:</span>
                <b className="ml-0.5">{count}</b>
              </div>
            );
          })}
        </div>
      </div>
    </div>
  );
}
