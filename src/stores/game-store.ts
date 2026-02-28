// Game store — Zustand + temporal undo

import { create } from 'zustand';
import { temporal } from 'zundo';
import type { GameState, GameAction, PlayerConfig, ActionResult } from '../core/types';
import { createGame } from '../core/state/init';
import { dispatch as coreDispatch, type DispatchResult } from '../core/dispatch';
import { executeSetup } from '../core/phases/setup';
import { checkAdvertiseComplete } from '../core/phases/advertise';
import { advanceToPlacement } from '../core/phases/assignment';
import { advanceTurn } from '../core/phases/placement';
import { advanceToEndTurn } from '../core/phases/performance';
import { executeEndTurn } from '../core/phases/end-turn';
import { applyFinalScoring } from '../core/scoring';

interface GameStore {
  state: GameState | null;
  lastResult: ActionResult | null;

  // Actions
  newGame: (configs: readonly PlayerConfig[], options?: { useDarkAlley?: boolean; seed?: number }) => void;
  dispatch: (action: GameAction) => DispatchResult | null;
  runSetup: () => void;
  finishAdvertise: () => void;
  finishAssignment: () => void;
  nextTurn: () => void;
  finishPerformance: () => void;
  finishRound: () => void;
  applyScoring: () => void;
  loadState: (state: GameState) => void;
}

export const useGameStore = create<GameStore>()(
  temporal(
    (set, get) => ({
      state: null,
      lastResult: null,

      newGame: (configs, options) => {
        const state = createGame(configs, options);
        set({ state, lastResult: null });
      },

      dispatch: (action) => {
        const { state } = get();
        if (!state) return null;
        const result = coreDispatch(state, action);
        set({ state: result.state, lastResult: result.result });
        return result;
      },

      runSetup: () => {
        const { state } = get();
        if (!state || state.phase !== 'SETUP') return;
        set({ state: executeSetup(state) });
      },

      finishAdvertise: () => {
        const { state } = get();
        if (!state || state.phase !== 'ADVERTISE') return;
        set({ state: checkAdvertiseComplete(state) });
      },

      finishAssignment: () => {
        const { state } = get();
        if (!state || state.phase !== 'ASSIGNMENT_REVEAL') return;
        set({ state: advanceToPlacement(state) });
      },

      nextTurn: () => {
        const { state } = get();
        if (!state || state.phase !== 'PLACEMENT') return;
        set({ state: advanceTurn(state) });
      },

      finishPerformance: () => {
        const { state } = get();
        if (!state || state.phase !== 'PERFORMANCE') return;
        set({ state: advanceToEndTurn(state) });
      },

      finishRound: () => {
        const { state } = get();
        if (!state || state.phase !== 'END_TURN') return;
        set({ state: executeEndTurn(state) });
      },

      applyScoring: () => {
        const { state } = get();
        if (!state || !state.gameOver) return;
        set({ state: applyFinalScoring(state) });
      },

      loadState: (loaded) => {
        set({ state: loaded, lastResult: null });
      },
    }),
    { limit: 50 },
  ),
);
