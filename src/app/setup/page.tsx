'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import { useGameStore } from '@/stores/game-store';
import { MAGICIANS, getMagicianDef } from '@/core/data/magicians';
import { getTricksByCategory } from '@/core/data/tricks';
import { COMPONENT_META, TRICK_CATEGORY_META } from '@/core/data/constants';
import { GameIcon } from '@/components/shared/GameIcon';
import type { MagicianId, SpecialistType, ComponentType, PlayerConfig, TrickId } from '@/core/types';

const MAGICIAN_LIST = [...MAGICIANS.values()];
const SPECIALIST_OPTIONS: { id: SpecialistType; label: string; icon: string }[] = [
  { id: 'ENGINEER', label: '기술자', icon: 'ENGINEER' },
  { id: 'MANAGER', label: '매니저', icon: 'MANAGER' },
  { id: 'ASSISTANT', label: '어시스턴트', icon: 'ASSISTANT' },
];

/** Components selectable at setup (coin value 1 or 2, total must = 2) */
const BASIC_COMPONENTS: ComponentType[] = ['WOOD', 'METAL', 'GLASS', 'FABRIC'];
const ADVANCED_COMPONENTS: ComponentType[] = ['ROPE', 'OIL', 'SAW', 'ANIMAL'];

function getComponentCoinValue(comps: ComponentType[]): number {
  return comps.reduce((sum, c) => sum + COMPONENT_META[c].cost, 0);
}

const PLAYER_COLORS = ['#ef4444', '#3b82f6', '#22c55e', '#eab308'];

interface PlayerSetup {
  name: string;
  magicianId: MagicianId;
  isHuman: boolean;
  startingSpecialist: SpecialistType;
  startingTrickId: TrickId | null;
  startingComponents: ComponentType[];
  engineerBonusTrickId: TrickId | null;
  managerBonusComponents: ComponentType[];
}

export default function SetupPage() {
  const router = useRouter();
  const newGame = useGameStore((s) => s.newGame);
  const [numPlayers, setNumPlayers] = useState(2);
  const [players, setPlayers] = useState<PlayerSetup[]>([
    { name: 'Player 1', magicianId: 'MECHANIKER', isHuman: true, startingSpecialist: 'ENGINEER', startingTrickId: null, startingComponents: ['METAL', 'METAL'], engineerBonusTrickId: null, managerBonusComponents: [] },
    { name: 'Player 2', magicianId: 'OPTICIAN', isHuman: false, startingSpecialist: 'MANAGER', startingTrickId: null, startingComponents: ['FABRIC', 'FABRIC'], engineerBonusTrickId: null, managerBonusComponents: ['FABRIC', 'FABRIC'] },
    { name: 'Player 3', magicianId: 'ESCAPIST', isHuman: false, startingSpecialist: 'ASSISTANT', startingTrickId: null, startingComponents: ['WOOD', 'WOOD'], engineerBonusTrickId: null, managerBonusComponents: [] },
    { name: 'Player 4', magicianId: 'SPIRITUALIST', isHuman: false, startingSpecialist: 'ENGINEER', startingTrickId: null, startingComponents: ['METAL', 'METAL'], engineerBonusTrickId: null, managerBonusComponents: [] },
  ]);

  const usedMagicians = players.slice(0, numPlayers).map((p) => p.magicianId);

  function updatePlayer(idx: number, updates: Partial<PlayerSetup>) {
    setPlayers((prev) =>
      prev.map((p, i) => (i === idx ? { ...p, ...updates } : p)),
    );
  }

  function startGame() {
    const configs: PlayerConfig[] = players
      .slice(0, numPlayers)
      .map((p) => ({
        name: p.name,
        magicianId: p.magicianId,
        isHuman: p.isHuman,
        startingSpecialist: p.startingSpecialist,
        startingComponents: p.startingComponents.length > 0
          ? p.startingComponents
          : ['WOOD', 'WOOD'] as ComponentType[],
        ...(p.startingTrickId ? { startingTrickId: p.startingTrickId } : {}),
        ...(p.startingSpecialist === 'ENGINEER' && p.engineerBonusTrickId
          ? { engineerBonusTrickId: p.engineerBonusTrickId } : {}),
        ...(p.startingSpecialist === 'MANAGER' && p.managerBonusComponents.length > 0
          ? { managerBonusComponents: p.managerBonusComponents } : {}),
      }));
    const seed = Math.floor(Math.random() * 1000000);
    newGame(configs, { seed });
    router.push('/game');
  }

  return (
    <main style={{
      minHeight: '100vh',
      padding: 30,
      background: 'radial-gradient(ellipse at 50% 0%, rgba(124,58,237,0.06) 0%, transparent 60%), var(--bg-darkest)',
    }}>
      <div className="setup-container" style={{ maxWidth: 900, margin: '0 auto' }}>
        {/* Header */}
        <div style={{ textAlign: 'center', marginBottom: 30 }}>
          <h1 style={{
            fontFamily: 'var(--font-heading)',
            fontSize: '1.8rem',
            color: 'var(--purple-light)',
          }}>
            게임 설정
          </h1>
          <p style={{ color: 'var(--text-dim)', marginTop: 8 }}>
            마술사를 선택하고 게임을 시작하세요
          </p>
        </div>

        {/* Player count */}
        <div style={{ textAlign: 'center', marginBottom: 24, display: 'flex', gap: 8, justifyContent: 'center', alignItems: 'center' }}>
          <span style={{ color: 'var(--text-dim)', fontFamily: 'var(--font-heading)' }}>플레이어 수:</span>
          {[2, 3, 4].map((n) => (
            <button
              key={n}
              onClick={() => setNumPlayers(n)}
              className={`btn ${numPlayers === n ? 'btn-primary' : ''}`}
              style={{ minWidth: 48 }}
            >
              {n}명
            </button>
          ))}
        </div>

        {/* Player cards */}
        <div className="player-setup-grid">
          {players.slice(0, numPlayers).map((p, idx) => {
            const mag = MAGICIANS.get(p.magicianId)!;
            const color = PLAYER_COLORS[idx];
            return (
              <div
                key={idx}
                className="player-setup-card active"
                style={{
                  borderColor: color,
                  boxShadow: `0 0 12px ${color}30`,
                }}
              >
                {/* Magician portrait header */}
                <div style={{
                  position: 'relative', height: 80, margin: '-20px -20px 12px -20px',
                  overflow: 'hidden', borderRadius: 'var(--radius-lg) var(--radius-lg) 0 0',
                }}>
                  <img
                    src={mag.img.replace('_portrait', '')}
                    alt={mag.nameKo}
                    style={{
                      width: '100%', height: '100%', objectFit: 'cover',
                      filter: 'brightness(0.35)',
                    }}
                    onError={(e) => { (e.target as HTMLImageElement).style.display = 'none'; }}
                  />
                  <div style={{
                    position: 'absolute', bottom: 8, left: 12,
                    display: 'flex', alignItems: 'center', gap: 8,
                  }}>
                    <img
                      src={mag.img}
                      alt={mag.nameKo}
                      className="magician-portrait"
                      style={{ width: 44, height: 44, borderColor: color }}
                      onError={(e) => { (e.target as HTMLImageElement).style.display = 'none'; }}
                    />
                    <span style={{
                      fontFamily: 'var(--font-heading)', fontWeight: 700,
                      color, fontSize: '0.95rem',
                    }}>
                      P{idx + 1}
                    </span>
                  </div>
                </div>

                {/* Name */}
                <label style={{ fontFamily: 'var(--font-heading)', letterSpacing: 0.3, fontSize: '0.85rem', color: 'var(--text-dim)', display: 'block', marginBottom: 4 }}>
                  이름
                </label>
                <input
                  type="text"
                  value={p.name}
                  onChange={(e) => updatePlayer(idx, { name: e.target.value })}
                  style={{
                    width: '100%', padding: '8px 12px',
                    background: 'var(--bg-secondary)', border: '1px solid var(--border)',
                    borderRadius: 'var(--radius)', color: 'var(--text)',
                    fontSize: '0.95rem', fontFamily: 'var(--font-body)',
                  }}
                />

                {/* Magician selection */}
                <label style={{ fontFamily: 'var(--font-heading)', letterSpacing: 0.3, fontSize: '0.85rem', color: 'var(--text-dim)', display: 'block', marginTop: 10, marginBottom: 4 }}>
                  마술사
                </label>
                <div style={{ display: 'flex', flexWrap: 'wrap', gap: 6 }}>
                  {MAGICIAN_LIST.map((m) => {
                    const isSelected = p.magicianId === m.id;
                    const isUsed = usedMagicians.includes(m.id) && !isSelected;
                    return (
                      <button
                        key={m.id}
                        onClick={() => !isUsed && updatePlayer(idx, { magicianId: m.id })}
                        disabled={isUsed}
                        style={{
                          width: 44, height: 44, borderRadius: '50%', padding: 0,
                          border: `2px solid ${isSelected ? 'var(--gold-primary)' : isUsed ? 'var(--border)' : m.color}`,
                          overflow: 'hidden', cursor: isUsed ? 'not-allowed' : 'pointer',
                          opacity: isUsed ? 0.3 : 1,
                          boxShadow: isSelected ? `0 0 12px var(--gold-glow)` : 'none',
                          transition: 'all 0.2s',
                          background: 'var(--bg-card)',
                        }}
                        title={m.nameKo}
                      >
                        <img
                          src={m.img}
                          alt={m.nameKo}
                          style={{ width: '100%', height: '100%', objectFit: 'cover' }}
                          onError={(e) => {
                            const el = e.target as HTMLImageElement;
                            el.style.display = 'none';
                            el.parentElement!.textContent = m.nameKo[0];
                          }}
                        />
                      </button>
                    );
                  })}
                </div>
                <div style={{ fontSize: '0.78rem', color: mag.color, marginTop: 4, fontFamily: 'var(--font-heading)' }}>
                  {mag.nameKo}
                </div>

                {/* Specialist */}
                <label style={{ fontFamily: 'var(--font-heading)', letterSpacing: 0.3, fontSize: '0.85rem', color: 'var(--text-dim)', display: 'block', marginTop: 10, marginBottom: 4 }}>
                  시작 전문가
                </label>
                <div style={{ display: 'flex', gap: 6 }}>
                  {SPECIALIST_OPTIONS.map((s) => {
                    const isSelected = p.startingSpecialist === s.id;
                    return (
                      <button
                        key={s.id}
                        onClick={() => updatePlayer(idx, {
                          startingSpecialist: s.id,
                          engineerBonusTrickId: null,
                          managerBonusComponents: [],
                        })}
                        className={`btn btn-sm ${isSelected ? 'btn-primary' : ''}`}
                        style={{ flex: 1, display: 'flex', alignItems: 'center', justifyContent: 'center', gap: 4 }}
                      >
                        <GameIcon type={s.icon} size="xs" color={isSelected ? '#fff' : undefined} />
                        {s.label}
                      </button>
                    );
                  })}
                </div>

                {/* Specialist bonus info */}
                <SpecialistBonusSection
                  specialist={p.startingSpecialist}
                  magicianId={p.magicianId}
                  engineerBonusTrickId={p.engineerBonusTrickId}
                  managerBonusComponents={p.managerBonusComponents}
                  onEngineerTrickSelect={(id) => updatePlayer(idx, { engineerBonusTrickId: id })}
                  onManagerComponentsSelect={(comps) => updatePlayer(idx, { managerBonusComponents: comps })}
                />

                {/* Starting Trick */}
                <label style={{ fontFamily: 'var(--font-heading)', letterSpacing: 0.3, fontSize: '0.85rem', color: 'var(--text-dim)', display: 'block', marginTop: 10, marginBottom: 4 }}>
                  시작 트릭 (Lv.1, 선호 카테고리)
                </label>
                <StartingTrickSelector
                  magicianId={p.magicianId}
                  selected={p.startingTrickId}
                  onSelect={(id) => updatePlayer(idx, { startingTrickId: id })}
                />

                {/* Starting Components */}
                <label style={{ fontFamily: 'var(--font-heading)', letterSpacing: 0.3, fontSize: '0.85rem', color: 'var(--text-dim)', display: 'block', marginTop: 10, marginBottom: 4 }}>
                  시작 컴포넌트 (코인가치 합 2)
                </label>
                <StartingComponentSelector
                  selected={p.startingComponents}
                  onSelect={(comps) => updatePlayer(idx, { startingComponents: comps })}
                />

                {/* Human toggle */}
                <label style={{
                  display: 'flex', alignItems: 'center', gap: 6, marginTop: 10,
                  cursor: 'pointer', fontSize: '0.85rem',
                }}>
                  <input
                    type="checkbox"
                    checked={p.isHuman}
                    onChange={(e) => updatePlayer(idx, { isHuman: e.target.checked })}
                    style={{ accentColor: 'var(--purple)' }}
                  />
                  <span>사람</span>
                </label>
              </div>
            );
          })}
        </div>

        {/* Start button */}
        <div style={{ textAlign: 'center', marginTop: 24 }}>
          <button onClick={startGame} className="btn btn-primary btn-lg">
            <GameIcon type="MAGICIAN" size="md" color="#fff" />
            게임 시작!
          </button>
        </div>
      </div>
    </main>
  );
}

/* ─── Starting Trick Selector ─── */
function StartingTrickSelector({ magicianId, selected, onSelect }: {
  magicianId: MagicianId;
  selected: TrickId | null;
  onSelect: (id: TrickId) => void;
}) {
  const mag = getMagicianDef(magicianId);
  const lv1Tricks = getTricksByCategory(mag.favoriteCategory).filter(t => t.level === 1);
  const catMeta = TRICK_CATEGORY_META[mag.favoriteCategory];

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 4 }}>
      {lv1Tricks.map((trick) => {
        const isSelected = selected === trick.id;
        const comps = Object.entries(trick.components) as [ComponentType, number][];
        return (
          <button
            key={trick.id}
            onClick={() => onSelect(trick.id as TrickId)}
            className={`btn btn-sm ${isSelected ? 'btn-primary' : ''}`}
            style={{
              textAlign: 'left', padding: '5px 8px',
              borderLeft: `3px solid ${catMeta.color}`,
            }}
          >
            <div style={{ fontWeight: 700, fontSize: '0.78rem' }}>{trick.nameKo}</div>
            <div style={{ fontSize: '0.62rem', color: isSelected ? 'rgba(255,255,255,0.7)' : 'var(--text-dim)' }}>
              필요: {comps.map(([c, n]) => `${COMPONENT_META[c].name}${n}`).join(' ')}
              {' | '}보상: {trick.yields.fame > 0 ? `명성${trick.yields.fame} ` : ''}
              {trick.yields.coins > 0 ? `코인${trick.yields.coins} ` : ''}
              {trick.yields.shards > 0 ? `샤드${trick.yields.shards}` : ''}
            </div>
          </button>
        );
      })}
    </div>
  );
}

/* ─── Starting Component Selector ─── */
function StartingComponentSelector({ selected, onSelect }: {
  selected: ComponentType[];
  onSelect: (comps: ComponentType[]) => void;
}) {
  const totalValue = getComponentCoinValue(selected);
  const remaining = 2 - totalValue;

  function addComponent(comp: ComponentType) {
    const cost = COMPONENT_META[comp].cost;
    if (cost > remaining) return;
    onSelect([...selected, comp]);
  }

  function reset() {
    onSelect([]);
  }

  if (totalValue >= 2) {
    return (
      <div>
        <div style={{ display: 'flex', gap: 4, alignItems: 'center', flexWrap: 'wrap' }}>
          {selected.map((comp, i) => (
            <span key={i} style={{
              padding: '2px 8px', borderRadius: 'var(--radius)',
              background: 'var(--bg-secondary)', border: '1px solid var(--cyan-light)',
              fontSize: '0.75rem', display: 'flex', alignItems: 'center', gap: 3,
            }}>
              <GameIcon type={comp} size="xs" />
              {COMPONENT_META[comp].name}
            </span>
          ))}
          <button onClick={reset} className="btn btn-sm" style={{ fontSize: '0.68rem', padding: '2px 8px' }}>
            초기화
          </button>
        </div>
      </div>
    );
  }

  // Show selectable components
  const availableBasic = remaining >= 1 ? BASIC_COMPONENTS : [];
  const availableAdvanced = remaining >= 2 ? ADVANCED_COMPONENTS : [];

  return (
    <div>
      {selected.length > 0 && (
        <div style={{ display: 'flex', gap: 4, marginBottom: 4, alignItems: 'center' }}>
          {selected.map((comp, i) => (
            <span key={i} style={{
              padding: '2px 8px', borderRadius: 'var(--radius)',
              background: 'var(--bg-secondary)', border: '1px solid var(--cyan-light)',
              fontSize: '0.72rem',
            }}>
              {COMPONENT_META[comp].name}
            </span>
          ))}
          <span style={{ fontSize: '0.68rem', color: 'var(--text-dim)' }}>
            (남은 가치: {remaining})
          </span>
          <button onClick={reset} className="btn btn-sm" style={{ fontSize: '0.62rem', padding: '1px 6px' }}>
            초기화
          </button>
        </div>
      )}
      <div style={{ display: 'flex', gap: 3, flexWrap: 'wrap' }}>
        {availableBasic.map((comp) => (
          <button
            key={comp}
            onClick={() => addComponent(comp)}
            className="btn btn-sm"
            style={{ fontSize: '0.72rem', display: 'flex', alignItems: 'center', gap: 3 }}
          >
            <GameIcon type={comp} size="xs" />
            {COMPONENT_META[comp].name} (1)
          </button>
        ))}
        {availableAdvanced.map((comp) => (
          <button
            key={comp}
            onClick={() => addComponent(comp)}
            className="btn btn-sm"
            style={{ fontSize: '0.72rem', display: 'flex', alignItems: 'center', gap: 3 }}
          >
            <GameIcon type={comp} size="xs" />
            {COMPONENT_META[comp].name} (2)
          </button>
        ))}
      </div>
    </div>
  );
}

/* ─── Specialist Bonus Section ─── */
const ALL_CATEGORIES: ('MECHANICAL' | 'OPTICAL' | 'ESCAPE' | 'SPIRITUAL')[] =
  ['MECHANICAL', 'OPTICAL', 'ESCAPE', 'SPIRITUAL'];

function SpecialistBonusSection({ specialist, magicianId, engineerBonusTrickId, managerBonusComponents, onEngineerTrickSelect, onManagerComponentsSelect }: {
  specialist: SpecialistType;
  magicianId: MagicianId;
  engineerBonusTrickId: TrickId | null;
  managerBonusComponents: ComponentType[];
  onEngineerTrickSelect: (id: TrickId) => void;
  onManagerComponentsSelect: (comps: ComponentType[]) => void;
}) {
  const [bonusCat, setBonusCat] = useState<typeof ALL_CATEGORIES[number] | null>(null);
  const bonusLabelStyle = {
    fontFamily: 'var(--font-heading)', letterSpacing: 0.3,
    fontSize: '0.8rem', color: 'var(--gold-primary)', display: 'block' as const,
    marginTop: 8, marginBottom: 4,
  };

  if (specialist === 'ASSISTANT') {
    return (
      <div style={bonusLabelStyle}>
        보너스: 추가 견습생 1명 (자동)
      </div>
    );
  }

  if (specialist === 'ENGINEER') {
    const mag = getMagicianDef(magicianId);
    const allLv1 = ALL_CATEGORIES.flatMap(cat =>
      getTricksByCategory(cat).filter(t => t.level === 1),
    );

    return (
      <div>
        <div style={bonusLabelStyle}>보너스: 추가 Lv.1 트릭 (아무 카테고리)</div>
        {/* Category filter */}
        <div style={{ display: 'flex', gap: 3, marginBottom: 4 }}>
          {ALL_CATEGORIES.map(cat => {
            const meta = TRICK_CATEGORY_META[cat];
            return (
              <button
                key={cat}
                onClick={() => setBonusCat(bonusCat === cat ? null : cat)}
                className={`btn btn-sm ${bonusCat === cat ? 'btn-primary' : ''}`}
                style={{ fontSize: '0.65rem', borderColor: meta.color, color: bonusCat === cat ? '#fff' : meta.color }}
              >
                {meta.name}
              </button>
            );
          })}
        </div>
        {/* Trick list for selected category */}
        {bonusCat && (
          <div style={{ display: 'flex', flexDirection: 'column', gap: 3, maxHeight: 140, overflowY: 'auto' }}>
            {getTricksByCategory(bonusCat).filter(t => t.level === 1).map(trick => {
              const isSelected = engineerBonusTrickId === trick.id;
              const comps = Object.entries(trick.components) as [ComponentType, number][];
              const catMeta = TRICK_CATEGORY_META[trick.category];
              return (
                <button
                  key={trick.id}
                  onClick={() => onEngineerTrickSelect(trick.id as TrickId)}
                  className={`btn btn-sm ${isSelected ? 'btn-primary' : ''}`}
                  style={{ textAlign: 'left', padding: '4px 8px', borderLeft: `3px solid ${catMeta.color}` }}
                >
                  <div style={{ fontWeight: 700, fontSize: '0.72rem' }}>{trick.nameKo}</div>
                  <div style={{ fontSize: '0.6rem', color: isSelected ? 'rgba(255,255,255,0.7)' : 'var(--text-dim)' }}>
                    필요: {comps.map(([c, n]) => `${COMPONENT_META[c].name}${n}`).join(' ')}
                  </div>
                </button>
              );
            })}
          </div>
        )}
        {!bonusCat && engineerBonusTrickId && (() => {
          const t = allLv1.find(t => t.id === engineerBonusTrickId);
          return t ? (
            <div style={{ fontSize: '0.72rem', color: 'var(--cyan-light)' }}>
              선택: {t.nameKo} ({TRICK_CATEGORY_META[t.category].name})
            </div>
          ) : null;
        })()}
      </div>
    );
  }

  if (specialist === 'MANAGER') {
    return (
      <div>
        <div style={bonusLabelStyle}>보너스: 추가 컴포넌트 (코인가치 합 2)</div>
        <StartingComponentSelector
          selected={managerBonusComponents}
          onSelect={onManagerComponentsSelect}
        />
      </div>
    );
  }

  return null;
}
