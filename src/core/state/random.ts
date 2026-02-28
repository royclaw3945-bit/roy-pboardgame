// Deterministic PRNG — Mulberry32 algorithm
// All randomness goes through state.rngCounter, never Date.now()

import type { GameState } from '../types';

function mulberry32(seed: number): number {
  let t = (seed + 0x6D2B79F5) | 0;
  t = Math.imul(t ^ (t >>> 15), t | 1);
  t ^= t + Math.imul(t ^ (t >>> 7), t | 61);
  return ((t ^ (t >>> 14)) >>> 0) / 4294967296;
}

export interface RngResult<T> {
  readonly value: T;
  readonly nextCounter: number;
}

/** Get a float [0, 1) and advance the counter */
export function nextFloat(counter: number, seed: number): RngResult<number> {
  const value = mulberry32(seed + counter);
  return { value, nextCounter: counter + 1 };
}

/** Get an integer [min, max] inclusive */
export function nextInt(counter: number, seed: number, min: number, max: number): RngResult<number> {
  const { value, nextCounter } = nextFloat(counter, seed);
  return { value: min + Math.floor(value * (max - min + 1)), nextCounter };
}

/** Pick a random element from array */
export function pick<T>(counter: number, seed: number, arr: readonly T[]): RngResult<T> {
  const { value, nextCounter } = nextInt(counter, seed, 0, arr.length - 1);
  return { value: arr[value], nextCounter };
}

/** Fisher-Yates shuffle (returns new array) */
export function shuffle<T>(counter: number, seed: number, arr: readonly T[]): RngResult<readonly T[]> {
  const result = [...arr];
  let c = counter;
  for (let i = result.length - 1; i > 0; i--) {
    const { value: j, nextCounter } = nextInt(c, seed, 0, i);
    c = nextCounter;
    [result[i], result[j]] = [result[j], result[i]];
  }
  return { value: result, nextCounter: c };
}

/** Roll a die (pick from faces array) */
export function rollDie<T>(counter: number, seed: number, faces: readonly T[]): RngResult<T> {
  return pick(counter, seed, faces);
}

// -- State-aware wrappers --

export function rngFloat(state: GameState): { value: number; state: GameState } {
  const { value, nextCounter } = nextFloat(state.rngCounter, state.config.seed);
  return { value, state: { ...state, rngCounter: nextCounter } };
}

export function rngInt(state: GameState, min: number, max: number): { value: number; state: GameState } {
  const { value, nextCounter } = nextInt(state.rngCounter, state.config.seed, min, max);
  return { value, state: { ...state, rngCounter: nextCounter } };
}

export function rngPick<T>(state: GameState, arr: readonly T[]): { value: T; state: GameState } {
  const { value, nextCounter } = pick(state.rngCounter, state.config.seed, arr);
  return { value, state: { ...state, rngCounter: nextCounter } };
}

export function rngShuffle<T>(state: GameState, arr: readonly T[]): { value: readonly T[]; state: GameState } {
  const { value, nextCounter } = shuffle(state.rngCounter, state.config.seed, arr);
  return { value, state: { ...state, rngCounter: nextCounter } };
}

export function rngRollDie<T>(state: GameState, faces: readonly T[]): { value: T; state: GameState } {
  const { value, nextCounter } = rollDie(state.rngCounter, state.config.seed, faces);
  return { value, state: { ...state, rngCounter: nextCounter } };
}
