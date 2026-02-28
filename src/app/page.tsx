'use client';

import Link from 'next/link';

export default function HomePage() {
  return (
    <main className="flex min-h-screen flex-col items-center justify-center gap-8 p-8">
      <h1 className="text-5xl font-bold text-[var(--gold)]">
        TRICKERION
      </h1>
      <p className="text-lg text-[var(--text-secondary)]">
        Legends of Illusion
      </p>
      <Link
        href="/setup"
        className="rounded-lg bg-[var(--bg-panel)] px-8 py-4 text-xl font-semibold
                   transition-colors hover:bg-[var(--purple)]"
      >
        새 게임 시작
      </Link>
    </main>
  );
}
