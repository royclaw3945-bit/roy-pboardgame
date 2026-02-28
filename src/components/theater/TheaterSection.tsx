'use client';

import { useGameStore } from '@/stores/game-store';
import { PerformanceCardView } from './PerformanceCardView';
import { WeekdayColumn } from './WeekdayColumn';
import { WEEKDAYS } from '@/core/data/constants';

export function TheaterSection() {
  const state = useGameStore((s) => s.state);
  if (!state) return null;

  return (
    <div className="rounded-lg bg-[var(--bg-card)] p-2">
      <h3 className="mb-2 text-sm font-bold text-[var(--gold)]">극장</h3>

      {/* Performance Cards (circular array) */}
      <div className="mb-3">
        <h4 className="mb-1 text-xs text-[var(--text-secondary)]">
          퍼포먼스 카드 ({state.theater.perfCards.length}장 | 덱: {state.theater.perfDeck.length})
        </h4>
        <div className="flex gap-1.5 overflow-x-auto pb-1">
          {state.theater.perfCards.map((card, i) => (
            <PerformanceCardView
              key={card.cardId as string}
              card={card}
              index={i}
              isNewest={i === 0}
              isOldest={i === state.theater.perfCards.length - 1}
            />
          ))}
        </div>
      </div>

      {/* Weekday Columns */}
      <div className="grid grid-cols-4 gap-1">
        {WEEKDAYS.map((day) => (
          <WeekdayColumn key={day} weekday={day} />
        ))}
      </div>
    </div>
  );
}
