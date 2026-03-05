'use client';

import { useEffect } from 'react';

export default function GameError({
  error,
  reset,
}: {
  error: Error & { digest?: string };
  reset: () => void;
}) {
  useEffect(() => {
    console.error('[GameError]', error);
  }, [error]);

  return (
    <main style={{
      display: 'flex', flexDirection: 'column', alignItems: 'center',
      justifyContent: 'center', minHeight: '100vh', gap: 16,
      background: '#0a0a14', color: '#e0e0e0', fontFamily: 'sans-serif',
    }}>
      <h2 style={{ fontSize: '1.5rem', color: '#ff6b6b' }}>게임 오류 발생</h2>
      <p style={{ fontSize: '0.85rem', color: '#999', maxWidth: 400, textAlign: 'center' }}>
        {error.message || '알 수 없는 오류가 발생했습니다.'}
      </p>
      <div style={{ display: 'flex', gap: 12 }}>
        <button
          onClick={reset}
          style={{
            padding: '10px 24px', background: '#2a6dd4', color: '#fff',
            border: 'none', borderRadius: 8, cursor: 'pointer', fontSize: '0.9rem',
          }}
        >
          다시 시도
        </button>
        <button
          onClick={() => window.location.href = '/setup'}
          style={{
            padding: '10px 24px', background: '#333', color: '#ccc',
            border: '1px solid #555', borderRadius: 8, cursor: 'pointer', fontSize: '0.9rem',
          }}
        >
          셋업으로 돌아가기
        </button>
      </div>
    </main>
  );
}
