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

  return (
    <div
      className="location-card has-bg"
      style={{
        backgroundImage: `url(${meta.img})`,
        ...(fullWidth && { gridColumn: '1 / -1' }),
      }}
    >
      {/* Header */}
      <div className="location-header">
        <GameIcon type={location} size="md" color="var(--gold-primary)" />
        <span className="location-name">{meta.name}</span>
        <span style={{ marginLeft: 'auto', fontSize: '0.7rem', color: 'var(--text-dim)' }}>
          {slots.filter(s => s.occupant).length}/{slots.length}
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
                borderWidth: player ? 2 : undefined,
              }}
              title={
                char
                  ? `${player!.name} - ${CHARACTER_META[char.type].name}`
                  : `AP: ${slot.apMod >= 0 ? '+' : ''}${slot.apMod}`
              }
            >
              {slot.occupant && char ? (
                <GameIcon
                  type={char.type}
                  size="sm"
                  color={player?.color}
                />
              ) : (
                <>
                  <span className="ws-ap">
                    {slot.apMod > 0 ? `+${slot.apMod}` : slot.apMod}
                  </span>
                </>
              )}
            </div>
          );
        })}
      </div>
    </div>
  );
}
