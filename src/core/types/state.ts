// GameState — top-level immutable game state (v3)

import type { Phase, Location, TrickCategory, TrickId, PlayerId, Weekday } from './base';
import type { PlayerState } from './player';
import type {
  TheaterState, DowntownDice, MarketState,
  DarkAlleyState, ProphecyState, LocationSlot, TurnQueueEntry, LogEntry,
} from './board';

export interface GameConfig {
  readonly numPlayers: number;
  readonly seed: number;
}

// -- Assignment Phase sub-state --
export interface AssignmentPhaseState {
  readonly playersSubmitted: readonly PlayerId[];
  readonly currentAssigner: PlayerId | null;
}

// -- Performance Phase sub-state --
export interface PerformancePhaseState {
  readonly currentWeekday: Weekday;
  readonly awaitingCardChoice: PlayerId | null;
  readonly awaitingLinkChoices: readonly PlayerId[];
}

export interface GameState {
  readonly config: GameConfig;
  readonly round: number;
  readonly maxRounds: number;
  readonly phase: Phase;
  readonly players: readonly PlayerState[];
  readonly initiativeOrder: readonly number[];
  readonly currentPlayerIdx: number;
  readonly turnQueue: readonly TurnQueueEntry[];
  readonly currentTurnIdx: number;

  readonly theater: TheaterState;
  readonly downtownDice: DowntownDice;
  readonly market: MarketState;
  readonly darkAlley: DarkAlleyState;
  readonly prophecy: ProphecyState;
  /** 이번 턴 광고한 플레이어 (End Turn에 포스터 반환) */
  readonly advertisedPlayers: readonly PlayerId[];

  readonly trickDecks: Readonly<Record<TrickCategory, readonly TrickId[]>>;
  readonly locationSlots: Readonly<Record<Location, readonly LocationSlot[]>>;

  readonly assignmentPhase: AssignmentPhaseState | null;
  readonly performancePhase: PerformancePhaseState | null;

  readonly log: readonly LogEntry[];
  readonly rngCounter: number;
  readonly gameOver: boolean;
  readonly winner: number | null;
}
