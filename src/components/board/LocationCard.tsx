'use client';

import { useGameStore } from '@/stores/game-store';
import { LOCATION_META, CHARACTER_META } from '@/core/data/constants';
import { GameIcon } from '../shared/GameIcon';
import type { Location } from '@/core/types';

interface Props {
  location: Location;
  fullWidth?: boolean;
}

export function LocationCard({ location, fullWidth }: Props) {
  const state = useGameStore((s) => s.state);
  if (!state) return null;

  const meta = LOCATION_META[location];
  const slots = state.locationSlots[location] ?? [];
  const occupiedCount = slots.filter(s => s.occupant).length;

  return (
    <div
      className="location-card has-bg"
      data-loc={location}
      style={{
        backgroundImage: `url(${meta.img})`,
        ...(fullWidth && { gridColumn: '1 / -1' }),
      }}
    >
      {/* Header */}
      <div className="location-header">
        <GameIcon type={location} size="md" color="var(--loc-color)" />
        <span className="location-name">{meta.name}</span>
        <span className="loc-slot-count">
          {occupiedCount > 0 && (
            <span style={{ color: 'var(--green)', marginRight: 2 }}>{occupiedCount}</span>
          )}
          {occupiedCount > 0 ? '/' : ''}{slots.length}
        </span>
      </div>

      {/* Worker Slots */}
      <div className="worker-slots">
        {slots.map((slot, i) => {
          const player = slot.occupant
            ? state.players.find(p => p.id === slot.occupant!.playerId)
            : null;
          const char = slot.occupant && player
            ? player.characters[slot.occupant.charIdx]
            : null;

          return (
            <div
              key={i}
              className={`worker-slot ${slot.occupant ? 'occupied' : ''}`}
              style={{
                borderColor: player ? player.color : undefined,
                ...(player && { boxShadow: `0 0 8px ${player.color}40` }),
              }}
              title={
                char
                  ? `${player!.name} - ${CHARACTER_META[char.type].name}`
                  : `AP 보정: ${slot.apMod >= 0 ? '+' : ''}${slot.apMod}`
              }
            >
              {slot.occupant && char ? (
                <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 1 }}>
                  <GameIcon type={char.type} size="sm" color={player?.color} />
                  <span style={{ fontSize: '0.55rem', color: player?.color, fontWeight: 700 }}>
                    {player?.name.slice(0, 3)}
                  </span>
                </div>
              ) : (
                <span className={`ws-ap ${slot.apMod > 0 ? 'positive' : 'zero'}`}>
                  {slot.apMod > 0 ? `+${slot.apMod}` : slot.apMod === 0 ? '0' : slot.apMod}
                </span>
              )}
            </div>
          );
        })}
      </div>
    </div>
  );
}
