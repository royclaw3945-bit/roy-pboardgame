// Derived selectors from game store

import { useGameStore } from './game-store';
import type { PlayerId, Phase } from '../core/types';
import { getPlayer, getActivePerfCards, getRankings } from '../core/state/selectors';

export function useGameState() {
  return useGameStore((s) => s.state);
}

export function usePhase(): Phase | null {
  return useGameStore((s) => s.state?.phase ?? null);
}

export function useRound(): number {
  return useGameStore((s) => s.state?.round ?? 0);
}

export function usePlayers() {
  return useGameStore((s) => s.state?.players ?? []);
}

export function usePlayer(id: PlayerId) {
  return useGameStore((s) => {
    if (!s.state) return null;
    return s.state.players.find((p) => p.id === id) ?? null;
  });
}

export function useCurrentPlayer() {
  return useGameStore((s) => {
    if (!s.state) return null;
    return s.state.players[s.state.currentPlayerIdx] ?? null;
  });
}

export function useActivePerfCards() {
  return useGameStore((s) => {
    if (!s.state) return [];
    return getActivePerfCards(s.state);
  });
}

export function useLog() {
  return useGameStore((s) => s.state?.log ?? []);
}

export function useIsGameOver() {
  return useGameStore((s) => s.state?.gameOver ?? false);
}
