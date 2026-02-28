'use client';

import { useUIStore } from '@/stores/ui-store';
import { SaveLoadModal } from './SaveLoadModal';

export function ModalRouter() {
  const modal = useUIStore((s) => s.modal);

  if (!modal) return null;

  switch (modal) {
    case 'SAVE_LOAD':
      return <SaveLoadModal />;
    case 'TRICK_LEARN':
    case 'HIRE':
    case 'MARKET':
    case 'LINK_REWARD':
    case 'PERF_RESULT':
    case 'SETUP_TRICK':
      return null; // TODO: implement in next iteration
    default:
      return null;
  }
}
