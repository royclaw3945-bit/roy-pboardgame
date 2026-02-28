'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import { useGameStore } from '@/stores/game-store';
import { MAGICIANS } from '@/core/data/magicians';
import type { MagicianId, PlayerConfig } from '@/core/types';

const MAGICIAN_LIST = [...MAGICIANS.values()];

interface PlayerSetup {
  name: string;
  magicianId: MagicianId;
  isHuman: boolean;
}

export default function SetupPage() {
  const router = useRouter();
  const newGame = useGameStore((s) => s.newGame);
  const [numPlayers, setNumPlayers] = useState(2);
  const [useDarkAlley, setUseDarkAlley] = useState(true);
  const [players, setPlayers] = useState<PlayerSetup[]>([
    { name: 'Player 1', magicianId: 'MECHANIKER', isHuman: true },
    { name: 'Player 2', magicianId: 'OPTICIAN', isHuman: false },
    { name: 'Player 3', magicianId: 'ESCAPIST', isHuman: false },
    { name: 'Player 4', magicianId: 'SPIRITUALIST', isHuman: false },
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
      }));
    const seed = Math.floor(Math.random() * 1000000);
    newGame(configs, { useDarkAlley, seed });
    router.push('/game');
  }

  return (
    <main className="flex min-h-screen flex-col items-center gap-6 p-8">
      <h1 className="text-3xl font-bold text-[var(--gold)]">게임 설정</h1>

      <div className="flex gap-4">
        <label className="text-[var(--text-secondary)]">플레이어 수:</label>
        {[2, 3, 4].map((n) => (
          <button
            key={n}
            onClick={() => setNumPlayers(n)}
            className={`rounded px-4 py-2 ${
              numPlayers === n
                ? 'bg-[var(--purple)] text-white'
                : 'bg-[var(--bg-card)]'
            }`}
          >
            {n}명
          </button>
        ))}
      </div>

      <label className="flex items-center gap-2">
        <input
          type="checkbox"
          checked={useDarkAlley}
          onChange={(e) => setUseDarkAlley(e.target.checked)}
        />
        <span>Dark Alley 확장 (7라운드)</span>
      </label>

      <div className="flex flex-col gap-4">
        {players.slice(0, numPlayers).map((p, idx) => (
          <div
            key={idx}
            className="flex items-center gap-4 rounded-lg bg-[var(--bg-card)] p-4"
          >
            <input
              type="text"
              value={p.name}
              onChange={(e) => updatePlayer(idx, { name: e.target.value })}
              className="rounded bg-[var(--bg-dark)] px-3 py-2 text-white"
            />
            <select
              value={p.magicianId}
              onChange={(e) =>
                updatePlayer(idx, { magicianId: e.target.value as MagicianId })
              }
              className="rounded bg-[var(--bg-dark)] px-3 py-2 text-white"
            >
              {MAGICIAN_LIST.map((m) => (
                <option
                  key={m.id}
                  value={m.id}
                  disabled={
                    usedMagicians.includes(m.id) && p.magicianId !== m.id
                  }
                >
                  {m.nameKo}
                </option>
              ))}
            </select>
            <label className="flex items-center gap-1">
              <input
                type="checkbox"
                checked={p.isHuman}
                onChange={(e) =>
                  updatePlayer(idx, { isHuman: e.target.checked })
                }
              />
              <span className="text-sm">사람</span>
            </label>
          </div>
        ))}
      </div>

      <button
        onClick={startGame}
        className="rounded-lg bg-[var(--green)] px-8 py-3 text-xl font-bold
                   transition-colors hover:bg-green-600"
      >
        게임 시작!
      </button>
    </main>
  );
}
