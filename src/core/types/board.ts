// Board runtime state — Theater, Market, Downtown, DarkAlley (v3)

import type {
  PlayerId, TrickId, CardId, SlotPosition, SymbolIndex,
  Weekday, ComponentType, Location, DiceGroup, TrickCategory,
  SpecialCardDefId, ProphecyTokenId,
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

/** 극장 요일 슬롯: 상단(백스테이지) + 하단(퍼포먼스, 마법사 전용) */
export interface TheaterWeekdaySlot {
  /** 백스테이지 슬롯 (전문가/견습생 배치, Set Up/Reschedule 가능) */
  readonly backstage: readonly {
    readonly occupant: { readonly playerId: PlayerId; readonly charIdx: number } | null;
    readonly apMod: number;
  }[];
  /** 퍼포먼스 슬롯 (마법사 전용, Perform 시 3AP 소비) */
  readonly performSlot: {
    readonly occupant: { readonly playerId: PlayerId; readonly charIdx: number } | null;
  };
}

export interface TheaterState {
  readonly perfCards: readonly PerfCardState[];
  readonly perfDeck: readonly CardId[];
  readonly perfDiscard: readonly CardId[];
  readonly weekdayPerformers: Readonly<Record<Weekday, PlayerId | null>>;
  /** 요일별 극장 슬롯 (기존 locationSlots.THEATER 대체) */
  readonly weekdaySlots: Readonly<Record<Weekday, TheaterWeekdaySlot>>;
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

// -- Market (v5: buyArea/orderArea/quickOrderSlot 분리) --
export interface MarketState {
  /** Buy 영역: 여기 있는 컴포넌트만 BUY 가능 */
  readonly buyArea: readonly ComponentType[];
  /** Order 영역: 다음 턴 Orders Arrive 때 buyArea로 이동 */
  readonly orderArea: readonly ComponentType[];
  /** Quick Order 슬롯: 즉시 buyArea에 포함, End Turn에 공급 반환 */
  readonly quickOrderSlot: ComponentType | null;
}

// -- Special Card Definition (40장, 장소별 10장) --
export type SpecialCardEffectType =
  | 'EXTRA_AP' | 'DISCOUNT' | 'BONUS_FAME' | 'BONUS_COINS'
  | 'FREE_COMPONENT' | 'EXTRA_LEARN' | 'CUSTOM';

export interface SpecialCardDef {
  readonly id: SpecialCardDefId;
  readonly location: Location;
  readonly name: string;
  readonly nameKo: string;
  readonly enhancedAction: string; // 강화 대상 액션 타입 or 'ANY'
  readonly effectType: SpecialCardEffectType;
  readonly effectValue: number;
  readonly effectDesc: string;
}

// -- Dark Alley (v5: 장소별 덱 + 2장 선택 UI) --
export interface DarkAlleyState {
  /** 4개 장소별 특수배치카드 덱 (DOWNTOWN에는 없음, DA 자체도 없음) */
  readonly specialDecks: Readonly<Partial<Record<Location, readonly SpecialCardDefId[]>>>;
  /** 현재 2장 뽑기 중인 카드 (CHOOSE_CARD 대기) */
  readonly drawnChoices: readonly SpecialCardDefId[] | null;
  readonly drawnFromLocation: Location | null;
}

// -- Prophecy (v5: 신규) --
export interface ProphecyState {
  /** 현재 턴 적용 중인 예언 (null = 1턴 또는 없음) */
  readonly active: ProphecyTokenId | null;
  /** Pending 슬롯 (왼→오, 최대 3개) */
  readonly pending: readonly ProphecyTokenId[];
  /** 남은 토큰 덱 */
  readonly deck: readonly ProphecyTokenId[];
  /** 버려진 토큰 */
  readonly discard: readonly ProphecyTokenId[];
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
