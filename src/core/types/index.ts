// Re-export all core types (v3)

export type {
  Phase, Location, Weekday, TrickCategory, TrickLevel,
  ComponentType, ComponentTier, CharacterType, SpecialistType,
  MagicianId, Venue, SlotPosition, DiceGroup,
  TrickId, PlayerId, CardId, AssignmentCardId,
  SymbolIndex, SymbolShape, LinkRewardChoice, DaAbilityId,
  EndBonusType,
} from './base';

export type {
  CharacterState, TrickSlot, SymbolMarkerState,
  AssignmentCardState, AssignmentCardPlacement,
  EngineerBoard, ManagerBoard, AssistantBoard, SpecialistBoard,
  PlayerState, PlayerConfig,
} from './player';

export type {
  TrickMarker, PerfCardState, TheaterState,
  DahlgaardFace, InnFace, BankFace, DowntownDice,
  MarketState, SpecialCard, DarkAlleyState,
  LocationSlotDef, LocationSlot, TurnQueueEntry, LogEntry,
} from './board';

export type { GameAction } from './action';

export type {
  ValidationError, ActionSuccess, ActionFailure, ActionResult,
} from './result';

export { success, failure, err } from './result';

export type {
  GameConfig, AssignmentPhaseState, PerformancePhaseState, GameState,
} from './state';
