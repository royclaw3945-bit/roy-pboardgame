'use client';

import Link from 'next/link';

export default function HomePage() {
  return (
    <main
      style={{
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        justifyContent: 'center',
        minHeight: '100vh',
        textAlign: 'center',
        padding: 20,
        position: 'relative',
        overflow: 'hidden',
        backgroundImage: 'url(/img/bg_magoria.jpg)',
        backgroundSize: 'cover',
        backgroundPosition: 'center',
      }}
    >
      {/* Dark overlay */}
      <div style={{
        position: 'absolute', inset: 0,
        background: 'radial-gradient(ellipse 70% 50% at 30% 40%, rgba(124,58,237,0.12) 0%, transparent 60%), radial-gradient(ellipse 60% 70% at 75% 65%, rgba(6,182,212,0.08) 0%, transparent 60%), rgba(10,10,26,0.85)',
      }} />

      {/* Magic circles */}
      <div style={{
        position: 'absolute',
        width: 500, height: 500, top: '50%', left: '50%',
        transform: 'translate(-50%, -50%)',
        border: '2px solid rgba(124,58,237,0.1)',
        borderRadius: '50%',
        boxShadow: '0 0 40px rgba(124,58,237,0.05), inset 0 0 40px rgba(124,58,237,0.03)',
        animation: 'gearSpin 30s linear infinite',
      }} />
      <div style={{
        position: 'absolute',
        width: 350, height: 350, top: '50%', left: '50%',
        transform: 'translate(-50%, -50%)',
        border: '1px dashed rgba(6,182,212,0.12)',
        borderRadius: '50%',
        animation: 'gearSpin 20s linear infinite reverse',
      }} />

      <div style={{ position: 'relative', zIndex: 2 }}>
        {/* Title */}
        <h1 style={{
          fontFamily: 'var(--font-title)',
          fontSize: '3.5rem',
          fontWeight: 900,
          letterSpacing: 6,
          marginBottom: 4,
          background: 'linear-gradient(135deg, var(--purple-light) 0%, var(--cyan-light) 50%, var(--purple-light) 100%)',
          backgroundSize: '200% 100%',
          WebkitBackgroundClip: 'text',
          WebkitTextFillColor: 'transparent',
          backgroundClip: 'text',
          animation: 'shimmer 8s ease-in-out infinite',
          filter: 'drop-shadow(0 2px 4px rgba(0,0,0,0.5))',
        }}>
          TRICKERION
        </h1>

        {/* Ornament line */}
        <div style={{
          width: 200, height: 2, margin: '8px auto 16px',
          background: 'linear-gradient(90deg, transparent, var(--purple-light), var(--cyan-light), transparent)',
          position: 'relative',
        }}>
          <div style={{
            position: 'absolute', top: -3, left: '50%', transform: 'translateX(-50%)',
            width: 8, height: 8, background: 'var(--purple-light)',
            borderRadius: '50%', boxShadow: '0 0 8px var(--purple-glow)',
          }} />
        </div>

        {/* Subtitle */}
        <p style={{
          color: 'var(--cyan-light)',
          fontFamily: 'var(--font-heading)',
          fontSize: '1.15rem',
          marginBottom: 30,
          fontStyle: 'italic',
          letterSpacing: 2,
        }}>
          Legends of Illusion
        </p>

        {/* Description */}
        <p style={{
          color: 'var(--text-dim)',
          marginBottom: 30,
          maxWidth: 500,
          lineHeight: 1.8,
        }}>
          마고리아의 유명 마술사가 되어 가장 위대한 마술 공연을 펼치세요.
        </p>

        {/* Buttons */}
        <div style={{ display: 'flex', gap: 12, justifyContent: 'center' }}>
          <Link href="/setup" className="btn btn-primary btn-lg">
            새 게임 시작
          </Link>
        </div>

        {/* Footer */}
        <div style={{
          marginTop: 60, fontSize: '0.75rem', color: 'var(--text-dim)',
          opacity: 0.6, fontFamily: 'var(--font-heading)', letterSpacing: 0.5,
        }}>
          Dark Alley Expansion
        </div>
      </div>
    </main>
  );
}
