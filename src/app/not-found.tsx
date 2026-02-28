import Link from 'next/link';

export default function NotFound() {
  return (
    <main className="flex min-h-screen flex-col items-center justify-center gap-4">
      <h1 className="text-4xl font-bold text-[var(--gold)]">404</h1>
      <p className="text-[var(--text-secondary)]">페이지를 찾을 수 없습니다</p>
      <Link href="/" className="text-[var(--purple)] hover:underline">
        홈으로 돌아가기
      </Link>
    </main>
  );
}
