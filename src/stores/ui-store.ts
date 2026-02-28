// UI store — modal state, selection, view preferences

import { create } from 'zustand';

export type ModalType =
  | 'TRICK_LEARN'
  | 'HIRE'
  | 'MARKET'
  | 'LINK_REWARD'
  | 'PERF_RESULT'
  | 'SAVE_LOAD'
  | 'SETUP_TRICK'
  | null;

interface UIStore {
  modal: ModalType;
  modalData: Record<string, unknown> | null;
  selectedPlayerTab: number;
  showLog: boolean;

  openModal: (type: NonNullable<ModalType>, data?: Record<string, unknown>) => void;
  closeModal: () => void;
  setPlayerTab: (idx: number) => void;
  toggleLog: () => void;
}

export const useUIStore = create<UIStore>()((set) => ({
  modal: null,
  modalData: null,
  selectedPlayerTab: 0,
  showLog: false,

  openModal: (type, data) => set({ modal: type, modalData: data ?? null }),
  closeModal: () => set({ modal: null, modalData: null }),
  setPlayerTab: (idx) => set({ selectedPlayerTab: idx }),
  toggleLog: () => set((s) => ({ showLog: !s.showLog })),
}));
