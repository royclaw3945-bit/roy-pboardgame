'use client';

import { useGameStore } from '@/stores/game-store';
import { WEEKDAY_MOD } from '@/core/data/constants';
import type { Weekday } from '@/core/types';

interface Props {
  weekday: Weekday;
}

export function WeekdayColumn({ weekday }: Props) {
  const state = useGameStore((s) => s.state);
  if (!state) return null;

  const mod = WEEKDAY_MOD[weekday];
  const performer = state.theater.weekdayPerformers[weekday];

  return (
    <div className="rounded bg-[var(--bg-dark)] p-1.5 text-center">
      <div className="text-xs font-bold">{mod.name}</div>
      <div className="text-[8px] text-[var(--text-secondary)]">
        {mod.fameMod >= 0 ? '+' : ''}{mod.fameMod}F
        {mod.coinMod >= 0 ? ' +' : ' '}{mod.coinMod}C
      </div>
      <div className="mt-1 min-h-[20px]">
        {performer !== null && (
          <div
            className="text-xs font-bold"
            style={{ color: state.players.find(p => p.id === performer)?.color }}
          >
            P{(performer as number) + 1}
          </div>
        )}
      </div>
    </div>
  );
}
