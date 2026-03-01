'use client';

import { useGameStore } from '@/stores/game-store';
import { PerformanceCardView } from './PerformanceCardView';
import { WeekdayColumn } from './WeekdayColumn';
import { WEEKDAYS } from '@/core/data/constants';
import { GameIcon } from '../shared/GameIcon';

export function TheaterSection() {
  const state = useGameStore((s) => s.state);
  if (!state) return null;

  return (
    <div className="theater-section">
      <h3 style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
        <GameIcon type="THEATER" size="md" color="var(--gold-primary)" />
        <span style={{ fontFamily: 'var(--font-heading)', color: 'var(--gold-primary)' }}>
          극장
        </span>
        <span style={{ fontSize: '0.75rem', color: 'var(--text-dim)', marginLeft: 'auto' }}>
          카드 {state.theater.perfCards.length}장 | 덱 {state.theater.perfDeck.length}장
        </span>
      </h3>

      {/* Performance Cards Row */}
      <div className="performance-cards-row">
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

      {/* Weekday Columns */}
      <div className="theater-weekdays">
        {WEEKDAYS.map((day) => (
          <WeekdayColumn key={day} weekday={day} />
        ))}
      </div>
    </div>
  );
}
