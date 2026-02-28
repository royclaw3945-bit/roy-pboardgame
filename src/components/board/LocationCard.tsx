'use client';

import { useGameStore } from '@/stores/game-store';
import { LOCATION_META } from '@/core/data/constants';
import type { Location } from '@/core/types';

interface Props {
  location: Location;
}

export function LocationCard({ location }: Props) {
  const state = useGameStore((s) => s.state);
  if (!state) return null;

  const meta = LOCATION_META[location];
  const slots = state.locationSlots[location] ?? [];

  return (
    <div className="rounded-lg bg-[var(--bg-card)] p-2">
      <h3 className="text-sm font-bold text-[var(--gold)]">{meta.name}</h3>
      <div className="mt-1 flex gap-1">
        {slots.map((slot, i) => (
          <div
            key={i}
            className={`h-8 w-8 rounded border text-center text-xs leading-8
              ${slot.occupant
                ? 'border-white/30 bg-white/10'
                : 'border-white/10 bg-[var(--bg-dark)]'
              }`}
            title={`Row ${slot.row}, AP mod: ${slot.apMod > 0 ? '+' : ''}${slot.apMod}`}
          >
            {slot.occupant ? (
              <span style={{ color: state.players.find(p => p.id === slot.occupant!.playerId)?.color }}>
                P{(slot.occupant.playerId as number) + 1}
              </span>
            ) : (
              <span className="text-[var(--text-secondary)]">
                {slot.apMod > 0 ? `+${slot.apMod}` : slot.apMod}
              </span>
            )}
          </div>
        ))}
      </div>
    </div>
  );
}
