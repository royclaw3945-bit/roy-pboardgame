'use client';

import { useGameStore } from '@/stores/game-store';
import { useUIStore } from '@/stores/ui-store';
import { getTrickDef } from '@/core/data/tricks';
import { getMagicianDef } from '@/core/data/magicians';
import {
  CHARACTER_META, COMPONENT_META, LOCATION_META,
  TRICK_CATEGORY_META, SYMBOL_INDEX_TO_SHAPE,
} from '@/core/data/constants';
import { GameIcon } from '../shared/GameIcon';
import type { ComponentType, CharacterState, Location } from '@/core/types';

const COMP_TIERS: { label: string; items: ComponentType[] }[] = [
  { label: '기본', items: ['WOOD', 'METAL', 'GLASS', 'FABRIC'] },
  { label: '고급', items: ['ROPE', 'OIL', 'SAW', 'ANIMAL'] },
  { label: '최상', items: ['LOCK', 'MIRROR', 'DISGUISE', 'GEAR'] },
];

const SHAPE_ICONS: Record<string, string> = {
  CIRCLE: '●', TRIANGLE: '▲', SQUARE: '■', STAR: '★',
};

function charStatusLabel(c: CharacterState): { text: string; color: string } {
  if (c.placed && c.ap > 0) return { text: '활동 중', color: 'var(--cyan-light)' };
  if (c.placed) return { text: '완료', color: 'var(--text-dim)' };
  if (c.assigned) return { text: '배정됨', color: 'var(--green)' };
  return { text: '대기', color: 'var(--text-dim)' };
}

export function PlayerBoard() {
  const state = useGameStore((s) => s.state);
  const selectedTab = useUIStore((s) => s.selectedPlayerTab);

  if (!state) return null;
  const player = state.players[selectedTab];
  if (!player) return null;

  const mag = getMagicianDef(player.magicianId);

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 10 }}>
      {/* ── Header ── */}
      <div className="pb-header">
        <img
          src={mag.img.replace('_portrait', '')}
          alt="" className="pb-header-bg"
          onError={(e) => { (e.target as HTMLImageElement).style.display = 'none'; }}
        />
        <div className="pb-header-overlay">
          <img
            src={mag.img} alt={mag.nameKo}
            className="magician-portrait"
            style={{ width: 44, height: 44, border: `2px solid ${player.color}` }}
            onError={(e) => { (e.target as HTMLImageElement).style.display = 'none'; }}
          />
          <div>
            <div style={{ fontFamily: 'var(--font-heading)', fontWeight: 700, fontSize: '1rem', color: player.color }}>
              {player.name}
            </div>
            <div style={{ fontSize: '0.72rem', color: 'var(--text-dim)' }}>{mag.nameKo}</div>
          </div>
        </div>
      </div>

      {/* ── Resources ── */}
      <div className="player-stats">
        <div className="stat-box fame">
          <div className="value">{player.fame}</div>
          <div className="label"><GameIcon type="fame" size="xs" /> 명성</div>
        </div>
        <div className="stat-box coins">
          <div className="value">{player.coins}</div>
          <div className="label"><GameIcon type="coins" size="xs" /> 코인</div>
        </div>
        <div className="stat-box shards">
          <div className="value">{player.shards}</div>
          <div className="label"><GameIcon type="shards" size="xs" /> 샤드</div>
        </div>
        <div className="stat-box">
          <div className="value" style={{ color: 'var(--purple-light)' }}>{player.tricks.length}/4</div>
          <div className="label">트릭</div>
        </div>
      </div>

      {/* ── Characters / Workers ── */}
      <Section title="일꾼" icon="MAGICIAN" count={player.characters.length}>
        <div className="character-list">
          {player.characters.map((c, i) => {
            const meta = CHARACTER_META[c.type];
            const status = charStatusLabel(c);
            return (
              <div key={i} className={`character-item ${c.assigned ? 'assigned' : ''}`}>
                <div style={{ display: 'flex', alignItems: 'center', gap: 6, flex: 1 }}>
                  <GameIcon type={c.type} size="sm" color={player.color} />
                  <div style={{ flex: 1 }}>
                    <div style={{ fontWeight: 600, fontSize: '0.85rem' }}>{meta.name}</div>
                    <div style={{ fontSize: '0.68rem', color: 'var(--text-dim)', display: 'flex', alignItems: 'center', gap: 4 }}>
                      {c.location ? (
                        <>
                          <GameIcon type={c.location} size="xs" />
                          <span>{LOCATION_META[c.location].name}</span>
                        </>
                      ) : (
                        <span style={{ color: status.color }}>{status.text}</span>
                      )}
                    </div>
                  </div>
                </div>
                <div style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
                  {c.ap > 0 && <span className="char-ap">AP {c.ap}</span>}
                  {c.location && !c.placed && (
                    <span style={{ fontSize: '0.65rem', padding: '1px 6px', borderRadius: 8, background: 'var(--green)', color: '#000', fontWeight: 700 }}>
                      배정
                    </span>
                  )}
                  {c.placed && c.ap <= 0 && (
                    <span style={{ fontSize: '0.65rem', color: 'var(--text-dim)' }}>완료</span>
                  )}
                </div>
              </div>
            );
          })}
        </div>
      </Section>

      {/* ── Assignment Cards ── */}
      <Section title="배정카드" count={`${player.currentPlacements.length}/${player.assignmentCards.length}`}>
        <div style={{ display: 'flex', flexWrap: 'wrap', gap: 4 }}>
          {player.assignmentCards.map((card) => {
            const used = player.currentPlacements.some(p => p.cardId === card.id);
            const placement = player.currentPlacements.find(p => p.cardId === card.id);
            const locMeta = LOCATION_META[card.location];
            return (
              <div
                key={card.id}
                style={{
                  display: 'flex', alignItems: 'center', gap: 4,
                  padding: '3px 8px', borderRadius: 'var(--radius)',
                  background: used ? 'rgba(124, 58, 237, 0.12)' : 'var(--bg-card)',
                  border: `1px solid ${used ? 'var(--purple)' : 'var(--border)'}`,
                  fontSize: '0.72rem', opacity: used ? 1 : 0.5,
                }}
              >
                <GameIcon type={card.location} size="xs" />
                <span style={{ fontWeight: used ? 700 : 400 }}>{locMeta.name}</span>
                {placement && (
                  <span style={{ color: 'var(--cyan-light)', fontSize: '0.65rem' }}>
                    ×{placement.characterIndices.length}
                  </span>
                )}
              </div>
            );
          })}
        </div>
      </Section>

      {/* ── Tricks ── */}
      <Section title="트릭" count={`${player.tricks.length}/4`}>
        {player.tricks.length === 0 ? (
          <p style={{ fontSize: '0.78rem', color: 'var(--text-dim)', padding: '4px 0' }}>보유 트릭 없음</p>
        ) : (
          <div className="trick-list">
            {player.tricks.map((slot, i) => {
              const trick = getTrickDef(slot.trickId);
              const catMeta = TRICK_CATEGORY_META[trick.category];
              const shape = SYMBOL_INDEX_TO_SHAPE[slot.symbolIndex];
              const y = trick.yields;
              const comps = Object.entries(trick.components) as [ComponentType, number][];
              return (
                <div key={i} className={`trick-item ${trick.category.toLowerCase()}`}>
                  {/* Row 1: Name + Level */}
                  <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                    <div style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
                      <span style={{ color: catMeta.color, fontSize: '0.8rem' }}>
                        {SHAPE_ICONS[shape] ?? '?'}
                      </span>
                      <span className="trick-name">{trick.nameKo}</span>
                    </div>
                    <span className="trick-level">Lv.{trick.level}</span>
                  </div>

                  {/* Row 2: Yields (performance rewards) */}
                  <div style={{
                    display: 'flex', gap: 8, marginTop: 4, fontSize: '0.7rem',
                    padding: '3px 6px', background: 'rgba(255,255,255,0.03)', borderRadius: 4,
                  }}>
                    {y.fame > 0 && (
                      <span style={{ color: 'var(--gold-primary)' }}>
                        <GameIcon type="fame" size="xs" /> {y.fame}
                      </span>
                    )}
                    {y.coins > 0 && (
                      <span style={{ color: 'var(--gold-primary)' }}>
                        <GameIcon type="coins" size="xs" /> {y.coins}
                      </span>
                    )}
                    {y.shards > 0 && (
                      <span style={{ color: 'var(--cyan-light)' }}>
                        <GameIcon type="shards" size="xs" /> {y.shards}
                      </span>
                    )}
                    <span style={{ marginLeft: 'auto', color: 'var(--text-dim)' }}>
                      필요: {comps.map(([c, n]) => `${COMPONENT_META[c].name}${n}`).join(' ')}
                    </span>
                  </div>

                  {/* Row 3: End bonus (Lv.3 only) */}
                  {trick.endBonus && (
                    <div style={{
                      fontSize: '0.65rem', color: 'var(--purple-light)', marginTop: 3,
                      fontStyle: 'italic',
                    }}>
                      종료: {trick.endBonus.desc}
                    </div>
                  )}

                  {/* Row 4: Markers + Status */}
                  <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginTop: 4 }}>
                    <div className="trick-markers">
                      {Array.from({ length: trick.markerSlots }, (_, j) => (
                        <div
                          key={j}
                          className={`trick-marker ${j < slot.markersOnTrick ? 'filled' : ''}`}
                          style={{ borderColor: catMeta.color }}
                        />
                      ))}
                    </div>
                    <span style={{
                      fontSize: '0.68rem', fontWeight: 700,
                      color: slot.prepared ? 'var(--green)' : 'var(--red)',
                    }}>
                      {slot.prepared ? '준비 완료' : '미준비'}
                    </span>
                  </div>
                </div>
              );
            })}
          </div>
        )}
      </Section>

      {/* ── Symbol Markers ── */}
      <Section title="심볼 마커" count={`${player.symbols.filter(s => s.assigned).length}/4`}>
        <div style={{ display: 'flex', gap: 6 }}>
          {player.symbols.map((sym, i) => {
            const shape = SYMBOL_INDEX_TO_SHAPE[i as 0 | 1 | 2 | 3];
            const icon = SHAPE_ICONS[shape];
            return (
              <div
                key={i}
                style={{
                  width: 36, height: 36, borderRadius: 'var(--radius)',
                  display: 'flex', alignItems: 'center', justifyContent: 'center',
                  fontSize: '1.1rem',
                  background: sym.assigned ? 'rgba(124, 58, 237, 0.15)' : 'var(--bg-card)',
                  border: `1px solid ${sym.assigned ? 'var(--purple)' : 'var(--border)'}`,
                  color: sym.assigned ? 'var(--purple-light)' : 'var(--text-dim)',
                  opacity: sym.assigned ? 1 : 0.4,
                }}
                title={sym.assigned ? `트릭에 배정됨` : '미사용'}
              >
                {icon}
              </div>
            );
          })}
        </div>
      </Section>

      {/* ── Components ── */}
      <Section title="컴포넌트">
        {COMP_TIERS.map((tier) => {
          const hasAny = tier.items.some(c => player.components[c] > 0);
          if (!hasAny) return null;
          return (
            <div key={tier.label} style={{ marginBottom: 6 }}>
              <div style={{ fontSize: '0.65rem', color: 'var(--text-dim)', marginBottom: 3, letterSpacing: 1 }}>
                {tier.label}
              </div>
              <div className="component-grid">
                {tier.items.map((comp) => {
                  const count = player.components[comp];
                  if (count === 0) return null;
                  return (
                    <div key={comp} className="component-item">
                      <GameIcon type={comp} size="xs" color="var(--cyan-light)" />
                      <span style={{ fontSize: '0.72rem' }}>{COMPONENT_META[comp].name}</span>
                      <span className="count">{count}</span>
                    </div>
                  );
                })}
              </div>
            </div>
          );
        })}
        {COMP_TIERS.every(t => t.items.every(c => player.components[c] === 0)) && (
          <p style={{ fontSize: '0.78rem', color: 'var(--text-dim)', padding: '4px 0' }}>보유 컴포넌트 없음</p>
        )}
      </Section>

      {/* ── Specialists ── */}
      {player.specialists.length > 0 && (
        <Section title="전문가">
          <div style={{ display: 'flex', gap: 6 }}>
            {player.specialists.map((s, i) => (
              <div
                key={i}
                style={{
                  display: 'flex', alignItems: 'center', gap: 4,
                  padding: '3px 8px', borderRadius: 'var(--radius)',
                  background: 'var(--bg-card)', border: '1px solid var(--border)',
                  fontSize: '0.78rem',
                }}
              >
                <GameIcon type={s} size="xs" color="var(--cyan-light)" />
                {CHARACTER_META[s].name}
              </div>
            ))}
          </div>
        </Section>
      )}
    </div>
  );
}

/* ── Reusable Section wrapper ── */
function Section({ title, icon, count, children }: {
  title: string;
  icon?: string;
  count?: string | number;
  children: React.ReactNode;
}) {
  return (
    <div className="panel-section">
      <div className="panel-section-title">
        {icon && <GameIcon type={icon} size="xs" color="var(--text-dim)" />}
        <span>{title}</span>
        {count !== undefined && (
          <span style={{ marginLeft: 'auto', fontSize: '0.72rem', color: 'var(--purple-light)', fontWeight: 700 }}>
            {count}
          </span>
        )}
      </div>
      {children}
    </div>
  );
}
