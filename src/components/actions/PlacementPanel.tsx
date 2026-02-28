'use client';

import { useGameStore } from '@/stores/game-store';
import { LOCATION_META } from '@/core/data/constants';

export function PlacementPanel() {
  const state = useGameStore((s) => s.state);
  const dispatchAction = useGameStore((s) => s.dispatch);
  const nextTurn = useGameStore((s) => s.nextTurn);

  if (!state) return null;

  const turn = state.turnQueue[state.currentTurnIdx];
  if (!turn) {
    return (
      <div>
        <h3 className="mb-2 text-sm font-bold">배치 페이즈</h3>
        <p className="text-xs text-[var(--text-secondary)]">모든 캐릭터 배치 완료</p>
      </div>
    );
  }

  const player = state.players.find((p) => p.id === turn.playerId);
  if (!player) return null;
  const char = player.characters[turn.characterIdx];
  if (!char) return null;

  const location = char.location;
  const slots = location ? state.locationSlots[location] : [];

  return (
    <div>
      <h3 className="mb-2 text-sm font-bold">배치 페이즈</h3>
      <p className="mb-2 text-xs">
        <span className="font-bold" style={{ color: player.color }}>{player.name}</span>
        {' '}의 {char.type} → {location && LOCATION_META[location]?.name}
      </p>

      {!char.placed && location && slots.length > 0 && (
        <div className="mb-2 flex gap-1">
          {slots.map((slot, i) => (
            <button
              key={i}
              onClick={() => dispatchAction({
                type: 'PLACE_CHARACTER',
                playerId: turn.playerId,
                characterIdx: turn.characterIdx,
                slotIndex: i,
              })}
              disabled={!!slot.occupant}
              className={`rounded px-3 py-1.5 text-xs
                ${slot.occupant ? 'bg-red-900/30 opacity-50' : 'bg-[var(--bg-dark)] hover:bg-[var(--bg-panel)]'}`}
            >
              슬롯{i + 1} ({slot.apMod >= 0 ? '+' : ''}{slot.apMod}AP)
            </button>
          ))}
        </div>
      )}

      <div className="flex gap-2">
        {char.placed && (
          <button
            onClick={nextTurn}
            className="rounded bg-[var(--green)] px-4 py-2 text-sm font-bold hover:bg-green-700"
          >
            다음 턴 →
          </button>
        )}
        <button
          onClick={() => dispatchAction({ type: 'PASS_CHARACTER', playerId: turn.playerId })}
          className="rounded bg-[var(--bg-panel)] px-3 py-1.5 text-xs hover:bg-[var(--bg-dark)]"
        >
          패스
        </button>
      </div>
    </div>
  );
}
