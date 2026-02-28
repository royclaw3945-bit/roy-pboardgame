// Board runtime state — Theater, Market, Downtown, DarkAlley (v3)

import type {
  PlayerId, TrickId, CardId, SlotPosition, SymbolIndex,
  Weekday, ComponentType, Location, DiceGroup, TrickCategory,
} from './base';

// -- Theater --
// v3: TrickMarker uses symbolIndex instead of trickId
export interface TrickMarker {
  readonly playerId: PlayerId;
  readonly symbolIndex: SymbolIndex;
}

export interface PerfCardState {
  readonly cardId: CardId;
  readonly weekday: Weekday | null;
  readonly slotMarkers: readonly (TrickMarker | null)[];
}

export interface TheaterState {
  readonly perfCards: readonly PerfCardState[];
  readonly perfDeck: readonly CardId[];
  readonly perfDiscard: readonly CardId[];
  readonly weekdayPerformers: Readonly<Record<Weekday, PlayerId | null>>;
}

// -- Downtown --
export type DahlgaardFace = TrickCategory | 'ANY' | 'X';
export type InnFace = 'ENGINEER' | 'MANAGER' | 'ASSISTANT' | 'APPRENTICE' | 'X';
export type BankFace = 1 | 2 | 3 | 4 | 5 | 'X';

export interface DowntownDice {
  readonly DAHLGAARD: readonly DahlgaardFace[];
  readonly INN: readonly InnFace[];
  readonly BANK: readonly BankFace[];
  readonly marked: Readonly<Record<DiceGroup, readonly boolean[]>>;
}

// -- Market --
export interface MarketState {
  readonly stock: readonly ComponentType[];
  readonly orders: readonly ComponentType[];
  readonly quickOrder: ComponentType | null;
}

// -- Dark Alley --
export interface SpecialCard {
  readonly id: string;
  readonly name: string;
  readonly nameKo: string;
  readonly effect: string;
  readonly bonusAP: number;
}

export interface DarkAlleyState {
  readonly specialDeck: readonly SpecialCard[];
  readonly drawnCards: Readonly<Record<number, readonly SpecialCard[]>>;
}

// -- Location Slots --
export interface LocationSlotDef {
  readonly row: number;
  readonly apMod: number;
}

export interface LocationSlot extends LocationSlotDef {
  readonly occupant: { readonly playerId: PlayerId; readonly charIdx: number } | null;
}

// -- Turn Queue --
export interface TurnQueueEntry {
  readonly playerId: PlayerId;
  readonly characterIdx: number;
}

// -- Log --
export interface LogEntry {
  readonly round: number;
  readonly message: string;
}
