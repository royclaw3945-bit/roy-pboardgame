// UI store — modal state, selection, view preferences

import { create } from 'zustand';
import type { Location } from '@/core/types';

export type ModalType =
  | 'TRICK_LEARN'
  | 'HIRE'
  | 'MARKET'
  | 'LINK_REWARD'
  | 'PERF_RESULT'
  | 'SAVE_LOAD'
  | 'SETUP_TRICK'
  | null;

export interface AnchorRect {
  top: number;
  left: number;
  width: number;
  height: number;
}

interface UIStore {
  modal: ModalType;
  modalData: Record<string, unknown> | null;
  selectedPlayerTab: number;
  showLog: boolean;

  // Location popover (PLACEMENT phase)
  selectedLocation: Location | null;
  popoverLocation: Location | null;
  popoverAnchorRect: AnchorRect | null;

  openModal: (type: NonNullable<ModalType>, data?: Record<string, unknown>) => void;
  closeModal: () => void;
  setPlayerTab: (idx: number) => void;
  toggleLog: () => void;

  selectLocation: (loc: Location | null) => void;
  openLocationPopover: (loc: Location, rect: DOMRect) => void;
  closeLocationPopover: () => void;
}

export const useUIStore = create<UIStore>()((set) => ({
  modal: null,
  modalData: null,
  selectedPlayerTab: 0,
  showLog: false,

  selectedLocation: null,
  popoverLocation: null,
  popoverAnchorRect: null,

  openModal: (type, data) => set({ modal: type, modalData: data ?? null }),
  closeModal: () => set({ modal: null, modalData: null }),
  setPlayerTab: (idx) => set({ selectedPlayerTab: idx }),
  toggleLog: () => set((s) => ({ showLog: !s.showLog })),

  selectLocation: (loc) => set({ selectedLocation: loc }),
  openLocationPopover: (loc, rect) => set({
    popoverLocation: loc,
    popoverAnchorRect: { top: rect.top, left: rect.left, width: rect.width, height: rect.height },
  }),
  closeLocationPopover: () => set({
    popoverLocation: null,
    popoverAnchorRect: null,
  }),
}));
