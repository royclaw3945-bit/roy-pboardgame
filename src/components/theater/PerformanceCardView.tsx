'use client';

import { useGameStore } from '@/stores/game-store';
import { getPerfCardDef } from '@/core/data/perf-cards';
import { VENUE_META } from '@/core/data/constants';
import type { PerfCardState, SlotPosition } from '@/core/types';

interface Props {
  card: PerfCardState;
  index: number;
  isNewest: boolean;
  isOldest: boolean;
}

export function PerformanceCardView({ card, index, isNewest, isOldest }: Props) {
  const state = useGameStore((s) => s.state);
  if (!state) return null;

  const def = getPerfCardDef(card.cardId);
  const venue = VENUE_META[def.venue];

  return (
    <div
      className={`min-w-[100px] rounded border p-1.5 text-xs
        ${isNewest ? 'border-green-400/50' : isOldest ? 'border-red-400/50' : 'border-white/10'}`}
      style={{ backgroundColor: venue.color + '15' }}
    >
      <div className="mb-1 flex items-center justify-between">
        <span className="font-bold" style={{ color: venue.color }}>
          {card.cardId as string}
        </span>
        {isNewest && <span className="text-[8px] text-green-400">NEW</span>}
        {isOldest && <span className="text-[8px] text-red-400">OLD</span>}
      </div>

      {/* 2x3 Slot Grid */}
      <div className="grid grid-cols-2 gap-0.5">
        {([0, 3, 1, 4, 2, 5] as SlotPosition[]).map((pos) => {
          const marker = card.slotMarkers[pos];
          const isActive = def.activeSlots.includes(pos);
          if (!isActive) return <div key={pos} className="h-5 w-full" />;

          return (
            <div
              key={pos}
              className={`h-5 w-full rounded-sm border text-center text-[8px] leading-5
                ${marker
                  ? 'border-white/30 bg-white/20'
                  : 'border-white/10 bg-[var(--bg-dark)]'
                }`}
            >
              {marker && (
                <span style={{
                  color: state.players.find(p => p.id === marker.playerId)?.color,
                }}>
                  S{marker.symbolIndex}
                </span>
              )}
            </div>
          );
        })}
      </div>

      <div className="mt-1 text-[8px] text-[var(--text-secondary)]">
        +{def.performerBonus.fame}F +{def.performerBonus.coins}C
        {def.performerBonus.shards > 0 && ` +${def.performerBonus.shards}S`}
      </div>
    </div>
  );
}
