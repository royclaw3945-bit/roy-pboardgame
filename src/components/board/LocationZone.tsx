'use client';

import { useGameStore } from '@/stores/game-store';
import { LOCATION_META, LOCATION_BOARD_POSITIONS } from '@/core/data/constants';
import { GameIcon } from '../shared/GameIcon';
import { WorkerToken } from './WorkerToken';
import type { Location } from '@/core/types';

interface Props {
  location: Location;
}

export function LocationZone({ location }: Props) {
  const state = useGameStore((s) => s.state);
  if (!state) return null;

  const meta = LOCATION_META[location];
  const pos = LOCATION_BOARD_POSITIONS[location];
  const slots = state.locationSlots[location] ?? [];
  const occupiedCount = slots.filter((s) => s.occupant).length;

  return (
    <div
      className="location-zone"
      data-loc={location}
      style={{
        position: 'absolute',
        top: `${pos.top}%`,
        left: `${pos.left}%`,
        width: `${pos.width}%`,
        height: `${pos.height}%`,
      }}
    >
      <div className="lz-header">
        <GameIcon type={location} size="sm" color="var(--loc-color)" />
        <span className="lz-name">{meta.name}</span>
        <span className="lz-count">
          {occupiedCount > 0 && (
            <span style={{ color: 'var(--green)' }}>{occupiedCount}/</span>
          )}
          {occupiedCount === 0 && ''}{slots.length}
        </span>
      </div>
      <div className="lz-slots">
        {slots.map((slot, i) => {
          const player = slot.occupant
            ? state.players.find((p) => p.id === slot.occupant!.playerId)
            : null;
          const char =
            slot.occupant && player
              ? player.characters[slot.occupant.charIdx]
              : null;

          if (slot.occupant && char && player) {
            return (
              <WorkerToken
                key={i}
                occupied={{
                  charType: char.type,
                  playerColor: player.color,
                  playerName: player.name,
                }}
              />
            );
          }
          return <WorkerToken key={i} empty={{ apMod: slot.apMod }} />;
        })}
      </div>
    </div>
  );
}
