'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import { useGameStore } from '@/stores/game-store';
import { MAGICIANS } from '@/core/data/magicians';
import { GameIcon } from '@/components/shared/GameIcon';
import type { MagicianId, SpecialistType, ComponentType, PlayerConfig } from '@/core/types';

const MAGICIAN_LIST = [...MAGICIANS.values()];
const SPECIALIST_OPTIONS: { id: SpecialistType; label: string; icon: string }[] = [
  { id: 'ENGINEER', label: '기술자', icon: 'ENGINEER' },
  { id: 'MANAGER', label: '매니저', icon: 'MANAGER' },
  { id: 'ASSISTANT', label: '어시스턴트', icon: 'ASSISTANT' },
];

const DEFAULT_COMPONENTS: Record<SpecialistType, readonly ComponentType[]> = {
  ENGINEER: ['METAL', 'METAL'],
  MANAGER: ['FABRIC', 'FABRIC'],
  ASSISTANT: ['WOOD', 'WOOD'],
};

const PLAYER_COLORS = ['#ef4444', '#3b82f6', '#22c55e', '#eab308'];

interface PlayerSetup {
  name: string;
  magicianId: MagicianId;
  isHuman: boolean;
  startingSpecialist: SpecialistType;
}

export default function SetupPage() {
  const router = useRouter();
  const newGame = useGameStore((s) => s.newGame);
  const [numPlayers, setNumPlayers] = useState(2);
  const [players, setPlayers] = useState<PlayerSetup[]>([
    { name: 'Player 1', magicianId: 'MECHANIKER', isHuman: true, startingSpecialist: 'ENGINEER' },
    { name: 'Player 2', magicianId: 'OPTICIAN', isHuman: false, startingSpecialist: 'MANAGER' },
    { name: 'Player 3', magicianId: 'ESCAPIST', isHuman: false, startingSpecialist: 'ASSISTANT' },
    { name: 'Player 4', magicianId: 'SPIRITUALIST', isHuman: false, startingSpecialist: 'ENGINEER' },
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
        startingComponents: DEFAULT_COMPONENTS[p.startingSpecialist],
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
                        onClick={() => updatePlayer(idx, { startingSpecialist: s.id })}
                        className={`btn btn-sm ${isSelected ? 'btn-primary' : ''}`}
                        style={{ flex: 1, display: 'flex', alignItems: 'center', justifyContent: 'center', gap: 4 }}
                      >
                        <GameIcon type={s.icon} size="xs" color={isSelected ? '#fff' : undefined} />
                        {s.label}
                      </button>
                    );
                  })}
                </div>

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
