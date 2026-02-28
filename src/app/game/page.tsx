'use client';

import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { useGameStore } from '@/stores/game-store';
import { GameLayout } from '@/components/layout/GameLayout';

export default function GamePage() {
  const router = useRouter();
  const state = useGameStore((s) => s.state);
  const runSetup = useGameStore((s) => s.runSetup);

  useEffect(() => {
    if (!state) {
      router.push('/setup');
    }
  }, [state, router]);

  useEffect(() => {
    if (state?.phase === 'SETUP') {
      runSetup();
    }
  }, [state?.phase, runSetup]);

  if (!state) {
    return (
      <main className="flex min-h-screen items-center justify-center">
        <p className="text-[var(--text-secondary)]">게임이 시작되지 않았습니다...</p>
      </main>
    );
  }

  return <GameLayout />;
}
