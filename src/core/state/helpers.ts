// Immutable state update helpers (v3)

import type {
  GameState, PlayerState, CharacterState, TrickSlot,
  PlayerId, LogEntry, SymbolMarkerState,
} from '../types';

/** Update a player by playerId */
export function updatePlayer(
  state: GameState,
  playerId: PlayerId,
  updater: (p: PlayerState) => Partial<PlayerState>,
): GameState {
  return {
    ...state,
    players: state.players.map(p =>
      p.id === playerId ? { ...p, ...updater(p) } : p,
    ),
  };
}

/** Update a specific character within a player */
export function updateCharacter(
  state: GameState,
  playerId: PlayerId,
  charIdx: number,
  updater: (c: CharacterState) => Partial<CharacterState>,
): GameState {
  return updatePlayer(state, playerId, p => ({
    characters: p.characters.map((c, i) =>
      i === charIdx ? { ...c, ...updater(c) } : c,
    ),
  }));
}

/** Update a specific trick slot within a player */
export function updateTrick(
  state: GameState,
  playerId: PlayerId,
  trickIdx: number,
  updater: (t: TrickSlot) => Partial<TrickSlot>,
): GameState {
  return updatePlayer(state, playerId, p => ({
    tricks: p.tricks.map((t, i) =>
      i === trickIdx ? { ...t, ...updater(t) } : t,
    ),
  }));
}

/** Update a specific symbol marker within a player */
export function updateSymbol(
  state: GameState,
  playerId: PlayerId,
  symbolIdx: number,
  updater: (s: SymbolMarkerState) => Partial<SymbolMarkerState>,
): GameState {
  return updatePlayer(state, playerId, p => ({
    symbols: p.symbols.map((s, i) =>
      i === symbolIdx ? { ...s, ...updater(s) } : s,
    ),
  }));
}

/** Adjust fame (clamped to >= 0) */
export function adjustFame(
  state: GameState,
  playerId: PlayerId,
  delta: number,
): GameState {
  return updatePlayer(state, playerId, p => ({
    fame: Math.max(0, p.fame + delta),
  }));
}

/** Adjust coins (clamped to >= 0) */
export function adjustCoins(
  state: GameState,
  playerId: PlayerId,
  delta: number,
): GameState {
  return updatePlayer(state, playerId, p => ({
    coins: Math.max(0, p.coins + delta),
  }));
}

/** Add a log entry */
export function addLog(state: GameState, message: string): GameState {
  const entry: LogEntry = { round: state.round, message };
  return { ...state, log: [...state.log, entry] };
}

/** Get current player from state */
export function currentPlayer(state: GameState): PlayerState {
  return state.players[state.currentPlayerIdx];
}

/** Get current turn queue entry */
export function currentTurn(state: GameState): { playerId: PlayerId; characterIdx: number } | null {
  if (state.currentTurnIdx < 0 || state.currentTurnIdx >= state.turnQueue.length) {
    return null;
  }
  return state.turnQueue[state.currentTurnIdx];
}

/** Pipe multiple state transforms */
export function pipe(
  state: GameState,
  ...fns: readonly ((s: GameState) => GameState)[]
): GameState {
  return fns.reduce((s, fn) => fn(s), state);
}
