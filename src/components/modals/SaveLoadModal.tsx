'use client';

import { useState } from 'react';
import { useGameStore } from '@/stores/game-store';
import { ModalOverlay } from './ModalOverlay';

export function SaveLoadModal() {
  const state = useGameStore((s) => s.state);
  const loadState = useGameStore((s) => s.loadState);
  const [saveData, setSaveData] = useState('');

  function handleSave() {
    if (!state) return;
    const json = JSON.stringify(state, null, 2);
    navigator.clipboard.writeText(json).catch(() => {});
    setSaveData(json);
  }

  function handleLoad() {
    if (!saveData.trim()) return;
    try {
      const parsed = JSON.parse(saveData);
      loadState(parsed);
    } catch {
      alert('잘못된 세이브 데이터');
    }
  }

  return (
    <ModalOverlay title="저장 / 불러오기">
      <div className="flex flex-col gap-3">
        <button
          onClick={handleSave}
          className="rounded bg-[var(--green)] px-4 py-2 text-sm font-bold hover:bg-green-700"
        >
          현재 상태 저장 (클립보드 복사)
        </button>

        <textarea
          value={saveData}
          onChange={(e) => setSaveData(e.target.value)}
          placeholder="세이브 데이터를 붙여넣기..."
          className="h-40 rounded bg-[var(--bg-dark)] p-2 text-xs text-[var(--text-secondary)]"
        />

        <button
          onClick={handleLoad}
          disabled={!saveData.trim()}
          className="rounded bg-[var(--purple)] px-4 py-2 text-sm font-bold
                     hover:bg-purple-700 disabled:opacity-50"
        >
          불러오기
        </button>
      </div>
    </ModalOverlay>
  );
}
