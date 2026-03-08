'use client';

import { useCallback } from 'react';
import { useGameStore } from '@/stores/game-store';
import { useUIStore } from '@/stores/ui-store';
import { LOCATION_META, LOCATION_BOARD_POSITIONS } from '@/core/data/constants';
import { GameIcon } from '../shared/GameIcon';
import { WorkerToken } from './WorkerToken';
import type { Location } from '@/core/types';

interface Props {
  location: Location;
}

export function LocationZone({ location }: Props) {
  const state = useGameStore((s) => s.state);
  const openPopover = useUIStore((s) => s.openLocationPopover);
  const popoverLocation = useUIStore((s) => s.popoverLocation);

  const handleClick = useCallback((e: React.MouseEvent<HTMLDivElement>) => {
    if (!state || state.phase !== 'PLACEMENT') return;

    const turn = state.turnQueue[state.currentTurnIdx];
    if (!turn) return;

    const player = state.players.find((p) => p.id === turn.playerId);
    if (!player) return;

    const charIdx = turn.characterIdx;
    const char = charIdx >= 0 ? player.characters[charIdx] : null;
    const subPhase = charIdx === -1 ? 'CHOOSING' : !char?.placed ? 'PLACING' : 'ACTING';

    // Only open popover for PLACING/ACTING when character is at this location
    if (subPhase === 'PLACING' && char?.location === location) {
      openPopover(location, e.currentTarget.getBoundingClientRect());
    } else if (subPhase === 'ACTING' && char?.location === location) {
      openPopover(location, e.currentTarget.getBoundingClientRect());
    }
  }, [state, location, openPopover]);

  if (!state) return null;

  const meta = LOCATION_META[location];
  const pos = LOCATION_BOARD_POSITIONS[location];
  const slots = state.locationSlots[location] ?? [];
  const occupiedCount = slots.filter((s) => s.occupant).length;

  // Compute visual state classes for PLACEMENT phase
  let zoneClass = 'location-zone';
  if (state.phase === 'PLACEMENT') {
    const turn = state.turnQueue[state.currentTurnIdx];
    if (turn) {
      const player = state.players.find((p) => p.id === turn.playerId);
      if (player) {
        const charIdx = turn.characterIdx;
        const char = charIdx >= 0 ? player.characters[charIdx] : null;
        const subPhase = charIdx === -1 ? 'CHOOSING' : !char?.placed ? 'PLACING' : 'ACTING';

        if (subPhase === 'PLACING' || subPhase === 'ACTING') {
          if (char?.location === location) {
            // This is the active location
            zoneClass += popoverLocation === location
              ? ' location-zone--selected'
              : ' location-zone--pulse';
          } else {
            // Dim non-relevant locations
            zoneClass += ' location-zone--dimmed';
          }
        }
      }
    }
  }

  return (
    <div
      className={zoneClass}
      data-loc={location}
      onClick={handleClick}
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
