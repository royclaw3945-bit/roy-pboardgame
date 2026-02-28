'use client';

import { useUIStore } from '@/stores/ui-store';

interface Props {
  children: React.ReactNode;
  title: string;
}

export function ModalOverlay({ children, title }: Props) {
  const closeModal = useUIStore((s) => s.closeModal);

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/60">
      <div className="w-full max-w-lg rounded-lg bg-[var(--bg-card)] p-4 shadow-xl">
        <div className="mb-3 flex items-center justify-between">
          <h2 className="text-lg font-bold text-[var(--gold)]">{title}</h2>
          <button
            onClick={closeModal}
            className="rounded px-2 py-1 text-sm text-[var(--text-secondary)] hover:text-white"
          >
            X
          </button>
        </div>
        {children}
      </div>
    </div>
  );
}
