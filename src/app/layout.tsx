import type { Metadata } from 'next';
import './globals.css';

export const metadata: Metadata = {
  title: 'Trickerion',
  description: 'Trickerion Board Game Digital Edition',
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="ko">
      <body>{children}</body>
    </html>
  );
}
