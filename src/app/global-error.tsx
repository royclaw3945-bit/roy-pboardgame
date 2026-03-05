'use client';

export default function GlobalError({
  error,
  reset,
}: {
  error: Error & { digest?: string };
  reset: () => void;
}) {
  return (
    <html lang="ko">
      <body style={{
        display: 'flex', flexDirection: 'column', alignItems: 'center',
        justifyContent: 'center', minHeight: '100vh', gap: 16,
        background: '#0a0a14', color: '#e0e0e0', fontFamily: 'sans-serif', margin: 0,
      }}>
        <h2 style={{ fontSize: '1.5rem', color: '#ff6b6b' }}>오류 발생</h2>
        <p style={{ fontSize: '0.85rem', color: '#999' }}>
          {error.message || '알 수 없는 오류'}
        </p>
        <button
          onClick={reset}
          style={{
            padding: '10px 24px', background: '#2a6dd4', color: '#fff',
            border: 'none', borderRadius: 8, cursor: 'pointer', fontSize: '0.9rem',
          }}
        >
          다시 시도
        </button>
      </body>
    </html>
  );
}
