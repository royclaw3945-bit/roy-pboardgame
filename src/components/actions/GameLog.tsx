'use client';

import { useRef, useEffect } from 'react';
import { useGameStore } from '@/stores/game-store';
import { useUIStore } from '@/stores/ui-store';

function getLogColor(message: string): string | undefined {
  if (message.includes('페이즈') || message.includes('라운드')) return 'var(--purple-light)';
  if (message.includes('명성') || message.includes('보상') || message.includes('코인')) return 'var(--gold-primary)';
  if (message.includes('패널티') || message.includes('실패') || message.includes('-')) return 'var(--red)';
  if (message.includes('배치') || message.includes('완료')) return 'var(--green)';
  return undefined;
}

export function GameLog() {
  const state = useGameStore((s) => s.state);
  const showLog = useUIStore((s) => s.showLog);
  const toggleLog = useUIStore((s) => s.toggleLog);
  const scrollRef = useRef<HTMLDivElement>(null);

  const entries = state?.log.slice(-30).reverse() ?? [];

  useEffect(() => {
    if (showLog && scrollRef.current) {
      scrollRef.current.scrollTop = 0;
    }
  }, [entries.length, showLog]);

  if (!state) return null;

  return (
    <>
      <div className="log-header" onClick={toggleLog} style={{ cursor: 'pointer' }}>
        <span>게임 로그</span>
        <span style={{ marginLeft: 'auto', fontSize: '0.7rem' }}>
          ({state.log.length}) {showLog ? '▼' : '▶'}
        </span>
      </div>
      {showLog && (
        <div className="log-content" ref={scrollRef}>
          {entries.map((entry, i) => {
            const color = getLogColor(entry.message);
            return (
              <div
                key={i}
                className={`log-entry ${color ? 'highlight' : ''}`}
                style={{ color }}
              >
                <span style={{ color: 'var(--gold-primary)', marginRight: 4 }}>
                  [R{entry.round}]
                </span>
                {entry.message}
              </div>
            );
          })}
        </div>
      )}
    </>
  );
}
