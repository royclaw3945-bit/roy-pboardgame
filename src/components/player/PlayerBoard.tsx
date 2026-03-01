'use client';

import { useGameStore } from '@/stores/game-store';
import { useUIStore } from '@/stores/ui-store';
import { getTrickDef } from '@/core/data/tricks';
import { getMagicianDef } from '@/core/data/magicians';
import { CHARACTER_META, COMPONENT_META, LOCATION_META, TRICK_CATEGORY_META } from '@/core/data/constants';
import { ResourceBadge } from '../shared/ResourceBadge';
import { GameIcon } from '../shared/GameIcon';
import type { ComponentType } from '@/core/types';

const ALL_COMPS: ComponentType[] = [
  'WOOD', 'METAL', 'GLASS', 'FABRIC',
  'ROPE', 'OIL', 'SAW', 'ANIMAL',
  'LOCK', 'MIRROR', 'DISGUISE', 'GEAR',
];

export function PlayerBoard() {
  const state = useGameStore((s) => s.state);
  const selectedTab = useUIStore((s) => s.selectedPlayerTab);

  if (!state) return null;
  const player = state.players[selectedTab];
  if (!player) return null;

  const mag = getMagicianDef(player.magicianId);

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 12 }}>
      {/* Header with magician art */}
      <div style={{
        position: 'relative',
        borderRadius: 'var(--radius-lg)',
        overflow: 'hidden',
        height: 100,
      }}>
        {/* Full art background */}
        <img
          src={mag.img.replace('_portrait', '')}
          alt={mag.nameKo}
          style={{
            position: 'absolute', inset: 0,
            width: '100%', height: '100%',
            objectFit: 'cover',
            filter: 'brightness(0.4)',
          }}
          onError={(e) => {
            (e.target as HTMLImageElement).style.display = 'none';
          }}
        />
        <div style={{
          position: 'relative', zIndex: 1,
          padding: '10px 14px',
          display: 'flex', flexDirection: 'column', justifyContent: 'flex-end',
          height: '100%',
          background: 'linear-gradient(transparent, rgba(10,10,26,0.85))',
        }}>
          <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
            <img
              src={mag.img}
              alt={mag.nameKo}
              className="magician-portrait"
              style={{ width: 40, height: 40 }}
              onError={(e) => {
                (e.target as HTMLImageElement).style.display = 'none';
              }}
            />
            <div>
              <div style={{
                fontFamily: 'var(--font-heading)', fontWeight: 700,
                fontSize: '1rem', color: player.color,
              }}>
                {player.name}
              </div>
              <div style={{ fontSize: '0.72rem', color: 'var(--text-dim)' }}>
                {mag.nameKo}
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Resources */}
      <div className="player-stats">
        <div className="stat-box fame">
          <div className="value">{player.fame}</div>
          <div className="label">
            <GameIcon type="fame" size="xs" /> 명성
          </div>
        </div>
        <div className="stat-box coins">
          <div className="value">{player.coins}</div>
          <div className="label">
            <GameIcon type="coins" size="xs" /> 코인
          </div>
        </div>
        <div className="stat-box shards">
          <div className="value">{player.shards}</div>
          <div className="label">
            <GameIcon type="shards" size="xs" /> 샤드
          </div>
        </div>
        <div className="stat-box">
          <div className="value" style={{ color: 'var(--purple-light)' }}>
            {player.tricks.length}/4
          </div>
          <div className="label">트릭</div>
        </div>
      </div>

      {/* Characters */}
      <div className="panel-section">
        <div className="panel-section-title">
          <GameIcon type="MAGICIAN" size="xs" color="var(--text-dim)" /> 캐릭터
        </div>
        <div className="character-list">
          {player.characters.map((c, i) => {
            const charMeta = CHARACTER_META[c.type];
            return (
              <div
                key={i}
                className={`character-item ${c.assigned ? 'assigned' : ''}`}
              >
                <div style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
                  <GameIcon type={c.type} size="sm" color={player.color} />
                  <span style={{ fontWeight: 600 }}>{charMeta.name}</span>
                  {c.location && (
                    <span style={{ fontSize: '0.72rem', color: 'var(--text-dim)' }}>
                      @{LOCATION_META[c.location].name}
                    </span>
                  )}
                </div>
                {c.ap > 0 && (
                  <span className="char-ap">AP {c.ap}</span>
                )}
              </div>
            );
          })}
        </div>
      </div>

      {/* Tricks */}
      <div className="panel-section">
        <div className="panel-section-title">트릭 ({player.tricks.length}/4)</div>
        {player.tricks.length === 0 ? (
          <p style={{ fontSize: '0.8rem', color: 'var(--text-dim)', padding: '6px 0' }}>
            보유 트릭 없음
          </p>
        ) : (
          <div className="trick-list">
            {player.tricks.map((slot, i) => {
              const trick = getTrickDef(slot.trickId);
              const catMeta = TRICK_CATEGORY_META[trick.category];
              return (
                <div key={i} className={`trick-item ${trick.category.toLowerCase()}`}>
                  <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                    <span className="trick-name">{trick.nameKo}</span>
                    <span className="trick-level">Lv.{trick.level}</span>
                  </div>
                  <div className="trick-markers">
                    {Array.from({ length: trick.markerSlots }, (_, j) => (
                      <div
                        key={j}
                        className={`trick-marker ${j < slot.markersOnTrick ? 'filled' : ''}`}
                        style={{ borderColor: catMeta.color }}
                      />
                    ))}
                  </div>
                  <div className="trick-yields">
                    {slot.prepared ? (
                      <span style={{ color: 'var(--green)' }}>준비 완료</span>
                    ) : (
                      <span style={{ color: 'var(--red)' }}>미준비</span>
                    )}
                  </div>
                </div>
              );
            })}
          </div>
        )}
      </div>

      {/* Components */}
      <div className="panel-section">
        <div className="panel-section-title">컴포넌트</div>
        <div className="component-grid">
          {ALL_COMPS.map((comp) => {
            const count = player.components[comp];
            if (count === 0) return null;
            return (
              <div key={comp} className="component-item">
                <GameIcon type={comp} size="xs" color="var(--cyan-light)" />
                <span style={{ fontSize: '0.75rem' }}>{COMPONENT_META[comp].name}</span>
                <span className="count">{count}</span>
              </div>
            );
          })}
        </div>
      </div>
    </div>
  );
}
