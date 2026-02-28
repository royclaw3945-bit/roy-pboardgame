// Player & Character runtime state (v3)

import type {
  PlayerId, MagicianId, CharacterType, SpecialistType,
  TrickId, ComponentType, Location, SymbolIndex, Weekday,
  AssignmentCardId,
} from './base';

// -- Character --
export interface CharacterState {
  readonly type: CharacterType;
  readonly ap: number;
  readonly assigned: boolean;
  readonly location: Location | null;
  readonly placed: boolean;
  readonly slotIndex: number | null;
  readonly slotApMod: number;
  readonly shardConverted: boolean;
}

// -- Trick Slot (v3: symbolIndex로 식별, 마커 유한 리소스) --
export interface TrickSlot {
  readonly trickId: TrickId;
  readonly symbolIndex: SymbolIndex;
  readonly prepared: boolean;
  readonly markersOnTrick: number;
}

// -- Symbol Marker (v3: 4개/플레이어, 트릭 배울 때 배정) --
export interface SymbolMarkerState {
  readonly assigned: boolean;
  readonly trickId: TrickId | null;
}

// -- Assignment Card (v3: 개인 소유) --
export interface AssignmentCardState {
  readonly id: AssignmentCardId;
  readonly location: Location;
  readonly isExpansion: boolean;
}

// -- Assignment Card Placement (현재 턴 배치) --
export interface AssignmentCardPlacement {
  readonly cardId: AssignmentCardId;
  readonly characterIndices: readonly number[];
}

// -- Specialist Extension Board --
export interface EngineerBoard {
  readonly type: 'ENGINEER';
  readonly extraTrickSlot: boolean;
}

export interface ManagerBoard {
  readonly type: 'MANAGER';
  readonly extraComponentSlot: boolean;
}

export interface AssistantBoard {
  readonly type: 'ASSISTANT';
  readonly freeApprentice: boolean;
}

export type SpecialistBoard = EngineerBoard | ManagerBoard | AssistantBoard;

// -- Player Config (game setup) --
export interface PlayerConfig {
  readonly name: string;
  readonly magicianId: MagicianId;
  readonly isHuman: boolean;
}

// -- Player State --
export interface PlayerState {
  readonly id: PlayerId;
  readonly name: string;
  readonly magicianId: MagicianId;
  readonly color: string;
  readonly fame: number;
  readonly coins: number;
  readonly shards: number;
  readonly tricks: readonly TrickSlot[];
  readonly components: Readonly<Record<ComponentType, number>>;
  readonly characters: readonly CharacterState[];
  readonly specialists: readonly SpecialistType[];
  readonly symbols: readonly SymbolMarkerState[];
  readonly specialistBoards: readonly SpecialistBoard[];
  readonly assignmentCards: readonly AssignmentCardState[];
  readonly currentPlacements: readonly AssignmentCardPlacement[];
  readonly chosenWeekday: Weekday | null;
  readonly usedAbilityThisTurn: boolean;
  readonly hasAdvertised: boolean;
  readonly isHuman: boolean;
}
