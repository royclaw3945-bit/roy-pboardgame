'use client';

import { useGameStore } from '@/stores/game-store';
import { WEEKDAY_MOD } from '@/core/data/constants';
import { GameIcon } from '../shared/GameIcon';
import type { Weekday } from '@/core/types';

interface Props {
  weekday: Weekday;
}

export function WeekdayColumn({ weekday }: Props) {
  const state = useGameStore((s) => s.state);
  if (!state) return null;

  const mod = WEEKDAY_MOD[weekday];
  const performer = state.theater.weekdayPerformers[weekday];
  const player = performer !== null
    ? state.players.find(p => p.id === performer)
    : null;

  return (
    <div className="weekday-col">
      <h4>{mod.name}</h4>
      <div style={{
        display: 'flex', gap: 4, justifyContent: 'center',
        fontSize: '0.7rem', marginBottom: 6,
      }}>
        <span style={{ color: mod.fameMod >= 0 ? 'var(--gold-primary)' : 'var(--red)' }}>
          <GameIcon type="fame" size="xs" /> {mod.fameMod >= 0 ? '+' : ''}{mod.fameMod}
        </span>
        <span style={{ color: mod.coinMod >= 0 ? 'var(--gold-primary)' : 'var(--red)' }}>
          <GameIcon type="coins" size="xs" /> {mod.coinMod >= 0 ? '+' : ''}{mod.coinMod}
        </span>
      </div>
      <div className={`weekday-slot ${performer !== null ? 'occupied' : ''}`}>
        {player ? (
          <div style={{
            display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 2,
          }}>
            <GameIcon type="MAGICIAN" size="sm" color={player.color} />
            <span style={{ color: player.color, fontSize: '0.68rem', fontWeight: 700 }}>
              {player.name}
            </span>
          </div>
        ) : (
          <span style={{ fontSize: '0.7rem', color: 'var(--text-dim)' }}>빈 슬롯</span>
        )}
      </div>
    </div>
  );
}
