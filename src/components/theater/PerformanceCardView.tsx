'use client';

import { useGameStore } from '@/stores/game-store';
import { getPerfCardDef } from '@/core/data/perf-cards';
import { VENUE_META } from '@/core/data/constants';
import { GameIcon } from '../shared/GameIcon';
import type { PerfCardState, SlotPosition } from '@/core/types';

interface Props {
  card: PerfCardState;
  index: number;
  isNewest: boolean;
  isOldest: boolean;
}

export function PerformanceCardView({ card, isNewest, isOldest }: Props) {
  const state = useGameStore((s) => s.state);
  if (!state) return null;

  const def = getPerfCardDef(card.cardId);
  const venue = VENUE_META[def.venue];

  return (
    <div
      className={`perf-card ${isNewest ? 'selected' : ''}`}
      style={{ borderColor: isNewest ? 'var(--green)' : isOldest ? 'rgba(239,68,68,0.4)' : undefined }}
    >
      {/* Ribbon badges */}
      {isNewest && (
        <div style={{
          position: 'absolute', top: 6, right: 6,
          background: 'var(--green)', color: '#000', fontSize: '0.6rem',
          fontWeight: 700, padding: '1px 6px', borderRadius: 4,
          fontFamily: 'var(--font-heading)',
        }}>
          NEW
        </div>
      )}
      {isOldest && (
        <div style={{
          position: 'absolute', top: 6, right: 6,
          background: 'var(--red)', color: '#fff', fontSize: '0.6rem',
          fontWeight: 700, padding: '1px 6px', borderRadius: 4,
          fontFamily: 'var(--font-heading)',
        }}>
          OLD
        </div>
      )}

      {/* Card Title */}
      <div className="perf-card-title" style={{ color: venue.color }}>
        {card.cardId as string}
      </div>

      {/* Venue name */}
      <div className="perf-card-bonus">
        {venue.nameKo}
      </div>

      {/* 2x3 Slot Grid */}
      <div className="perf-card-grid">
        {([0, 3, 1, 4, 2, 5] as SlotPosition[]).map((pos) => {
          const marker = card.slotMarkers[pos];
          const isActive = def.activeSlots.includes(pos);
          if (!isActive) return <div key={pos} className="trick-slot" style={{ opacity: 0.15, cursor: 'default' }} />;

          const player = marker
            ? state.players.find(p => p.id === marker.playerId)
            : null;

          const hasLink = def.linkCircles.some(lc =>
            lc.between[0] === pos || lc.between[1] === pos,
          );
          const hasShard = def.linkCircles.some(lc =>
            lc.hasShard && (lc.between[0] === pos || lc.between[1] === pos),
          );

          return (
            <div
              key={pos}
              className={`trick-slot ${marker ? 'occupied' : ''}`}
              style={{
                borderColor: player ? player.color : undefined,
                borderWidth: player ? 2 : undefined,
              }}
            >
              {marker && player && (
                <span style={{ color: player.color, fontSize: '0.75rem', fontWeight: 700 }}>
                  S{marker.symbolIndex}
                </span>
              )}
              {!marker && (
                <span className="trick-slot-info">
                  {pos < 3 ? 'L' : 'R'}{(pos % 3) + 1}
                </span>
              )}
              {hasLink && (
                <div className="link-indicator">L</div>
              )}
              {hasShard && (
                <span className="shard-indicator">
                  <GameIcon type="shards" size="xs" />
                </span>
              )}
            </div>
          );
        })}
      </div>

      {/* Performer Bonus */}
      <div style={{
        marginTop: 8, textAlign: 'center', fontSize: '0.72rem',
        color: 'var(--text-dim)', fontFamily: 'var(--font-heading)',
        display: 'flex', gap: 6, justifyContent: 'center',
      }}>
        <span style={{ color: 'var(--gold-primary)' }}>+{def.performerBonus.fame}F</span>
        <span style={{ color: 'var(--gold-primary)' }}>+{def.performerBonus.coins}C</span>
        {def.performerBonus.shards > 0 && (
          <span style={{ color: 'var(--cyan-light)' }}>+{def.performerBonus.shards}S</span>
        )}
      </div>
    </div>
  );
}
