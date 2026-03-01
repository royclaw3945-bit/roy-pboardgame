'use client';

import { useGameStore } from '@/stores/game-store';
import { LOCATION_META, CHARACTER_META } from '@/core/data/constants';
import { getMagicianDef } from '@/core/data/magicians';
import { GameIcon } from '../shared/GameIcon';

export function PlacementPanel() {
  const state = useGameStore((s) => s.state);
  const dispatchAction = useGameStore((s) => s.dispatch);
  const nextTurn = useGameStore((s) => s.nextTurn);

  if (!state) return null;

  const turn = state.turnQueue[state.currentTurnIdx];
  if (!turn) {
    return (
      <div>
        <p style={{ fontSize: '0.85rem', color: 'var(--text-dim)' }}>
          모든 캐릭터 배치가 완료되었습니다.
        </p>
      </div>
    );
  }

  const player = state.players.find((p) => p.id === turn.playerId);
  if (!player) return null;
  const char = player.characters[turn.characterIdx];
  if (!char) return null;
  const mag = getMagicianDef(player.magicianId);

  const location = char.location;
  const slots = location ? state.locationSlots[location] : [];

  return (
    <div>
      {/* Current turn info */}
      <div className="turn-indicator" style={{
        background: `linear-gradient(135deg, ${player.color}, ${player.color}99)`,
      }}>
        {player.name}의 턴
      </div>

      {/* Character info */}
      <div style={{
        display: 'flex', alignItems: 'center', gap: 10, marginTop: 8, marginBottom: 12,
        padding: '10px 12px', background: 'var(--bg-card)', borderRadius: 'var(--radius)',
        border: `1px solid ${player.color}30`,
      }}>
        <img
          src={mag.img}
          alt={mag.nameKo}
          style={{
            width: 32, height: 32, borderRadius: '50%',
            objectFit: 'cover', border: `2px solid ${player.color}`,
          }}
          onError={(e) => { (e.target as HTMLImageElement).style.display = 'none'; }}
        />
        <div>
          <div style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
            <GameIcon type={char.type} size="sm" color={player.color} />
            <span style={{ fontWeight: 700, fontSize: '0.9rem' }}>
              {CHARACTER_META[char.type].name}
            </span>
          </div>
          {location && (
            <div style={{ fontSize: '0.78rem', color: 'var(--text-dim)', display: 'flex', alignItems: 'center', gap: 4 }}>
              <GameIcon type={location} size="xs" />
              {LOCATION_META[location].name}
            </div>
          )}
        </div>
      </div>

      {/* Slot selection */}
      {!char.placed && location && slots.length > 0 && (
        <div style={{ display: 'flex', gap: 6, marginBottom: 12, flexWrap: 'wrap' }}>
          {slots.map((slot, i) => {
            const occupied = !!slot.occupant;
            return (
              <button
                key={i}
                onClick={() => dispatchAction({
                  type: 'PLACE_CHARACTER',
                  playerId: turn.playerId,
                  characterIdx: turn.characterIdx,
                  slotIndex: i,
                })}
                disabled={occupied}
                className={`worker-slot ${occupied ? 'occupied' : ''}`}
                style={{
                  width: 52, height: 52, cursor: occupied ? 'not-allowed' : 'pointer',
                  ...(occupied && { opacity: 0.4 }),
                }}
              >
                {occupied ? (
                  <span style={{ fontSize: '0.6rem', color: 'var(--red)' }}>X</span>
                ) : (
                  <div style={{ textAlign: 'center' }}>
                    <span className="ws-ap" style={{ position: 'static', fontSize: '0.7rem' }}>
                      {slot.apMod > 0 ? `+${slot.apMod}` : slot.apMod}
                    </span>
                    <div style={{ fontSize: '0.55rem', color: 'var(--text-dim)' }}>AP</div>
                  </div>
                )}
              </button>
            );
          })}
        </div>
      )}

      {/* Actions */}
      <div style={{ display: 'flex', gap: 8 }}>
        {char.placed && (
          <button onClick={nextTurn} className="btn btn-primary" style={{ flex: 1 }}>
            다음 턴
          </button>
        )}
        <button
          onClick={() => dispatchAction({ type: 'PASS_CHARACTER', playerId: turn.playerId })}
          className="btn"
          style={{ flex: char.placed ? undefined : 1 }}
        >
          패스
        </button>
      </div>
    </div>
  );
}
