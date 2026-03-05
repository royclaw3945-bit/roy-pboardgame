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
import { executePerformance } from '../core/phases/performance';
import { executeEndTurn } from '../core/phases/end-turn';
import { applyFinalScoring } from '../core/scoring';

interface GameStore {
  state: GameState | null;
  lastResult: ActionResult | null;
  lastError: string | null;

  // Actions
  newGame: (configs: readonly PlayerConfig[], options?: { seed?: number }) => void;
  dispatch: (action: GameAction) => DispatchResult | null;
  runSetup: () => void;
  finishAdvertise: () => void;
  finishAssignment: () => void;
  nextTurn: () => void;
  finishActions: () => void;
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
      lastError: null,

      newGame: (configs, options) => {
        const state = createGame(configs, options);
        set({ state, lastResult: null });
      },

      dispatch: (action) => {
        const { state } = get();
        if (!state) return null;
        const result = coreDispatch(state, action);
        if (!result.result.ok) {
          const msgs = result.result.errors?.map(e => e.message).join(', ') ?? '알 수 없는 오류';
          console.warn(`[dispatch FAIL] ${action.type}:`, msgs);
          set({ lastResult: result.result, lastError: `${action.type}: ${msgs}` });
        } else {
          set({ state: result.state, lastResult: result.result, lastError: null });
        }
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

      finishActions: () => {
        const { state } = get();
        if (!state || state.phase !== 'PLACEMENT') return;
        // Dispatch FINISH_ACTIONS for current turn player
        const turn = state.turnQueue[state.currentTurnIdx];
        if (!turn) return;
        const result = coreDispatch(state, { type: 'FINISH_ACTIONS', playerId: turn.playerId });
        set({ state: result.state, lastResult: result.result });
      },

      finishPerformance: () => {
        const { state } = get();
        if (!state || state.phase !== 'PERFORMANCE') return;
        set({ state: executePerformance(state) });
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
