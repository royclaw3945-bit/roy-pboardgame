// GameAction — discriminated union of all player/system actions (v3)

import type {
  PlayerId, TrickId, CardId, SlotPosition, AssignmentCardId,
  Location, ComponentType, TrickCategory, Weekday, SymbolIndex,
  LinkRewardChoice, DaAbilityId, SpecialCardDefId,
  SpecialistType,
} from './base';

// -- Advertise Phase --
interface AdvertiseAction {
  readonly type: 'ADVERTISE';
  readonly playerId: PlayerId;
}
interface SkipAdvertiseAction {
  readonly type: 'SKIP_ADVERTISE';
  readonly playerId: PlayerId;
}
interface FinishAdvertiseAction {
  readonly type: 'FINISH_ADVERTISE';
}

// -- Assignment Phase (v3: 동시 비밀배치) --
interface PlaceAssignmentCardAction {
  readonly type: 'PLACE_ASSIGNMENT_CARD';
  readonly playerId: PlayerId;
  readonly cardId: AssignmentCardId;
  readonly characterIndices: readonly number[];
}
interface RemoveAssignmentCardAction {
  readonly type: 'REMOVE_ASSIGNMENT_CARD';
  readonly playerId: PlayerId;
  readonly cardId: AssignmentCardId;
}
interface SubmitAssignmentAction {
  readonly type: 'SUBMIT_ASSIGNMENT';
  readonly playerId: PlayerId;
}
interface RevealAssignmentsAction {
  readonly type: 'REVEAL_ASSIGNMENTS';
}

// -- Placement Phase --
interface SelectCharacterAction {
  readonly type: 'SELECT_CHARACTER';
  readonly playerId: PlayerId;
  readonly characterIdx: number;
}
interface PlaceCharacterAction {
  readonly type: 'PLACE_CHARACTER';
  readonly playerId: PlayerId;
  readonly characterIdx: number;
  readonly slotIndex: number;
  /** 극장 전용: 배치 요일 */
  readonly weekday?: Weekday;
  /** 극장 전용: 'BACKSTAGE' | 'PERFORM' */
  readonly theaterSlotType?: 'BACKSTAGE' | 'PERFORM';
}
interface PassCharacterAction {
  readonly type: 'PASS_CHARACTER';
  readonly playerId: PlayerId;
}
interface ConvertShardAction {
  readonly type: 'CONVERT_SHARD';
  readonly playerId: PlayerId;
}
interface FinishActionsAction {
  readonly type: 'FINISH_ACTIONS';
  readonly playerId: PlayerId;
}

// -- Downtown Actions --
interface LearnTrickAction {
  readonly type: 'LEARN_TRICK';
  readonly playerId: PlayerId;
  readonly trickId: TrickId;
  readonly symbolIndex: SymbolIndex;
}
interface TakeCoinsAction {
  readonly type: 'TAKE_COINS';
  readonly playerId: PlayerId;
  readonly dieIndex: number;
}
interface HireAction {
  readonly type: 'HIRE';
  readonly playerId: PlayerId;
  readonly specialistType: SpecialistType | 'APPRENTICE';
}
interface RerollAction {
  readonly type: 'REROLL';
  readonly playerId: PlayerId;
  readonly diceGroup: 'DAHLGAARD' | 'INN' | 'BANK';
  readonly dieIndex: number;
}
interface SetDieAction {
  readonly type: 'SET_DIE';
  readonly playerId: PlayerId;
  readonly diceGroup: 'DAHLGAARD' | 'INN' | 'BANK';
  readonly dieIndex: number;
  readonly desiredFace: string | number;
}
interface ChooseDieAction {
  readonly type: 'CHOOSE_DIE';
  readonly playerId: PlayerId;
  readonly diceGroup: 'DAHLGAARD' | 'INN' | 'BANK';
  readonly dieIndex: number;
}

// -- Market Actions (v5: BARGAIN을 BUY에 통합) --
interface BuyAction {
  readonly type: 'BUY';
  readonly playerId: PlayerId;
  readonly componentType: ComponentType;
  /** Bargain으로 사용한 추가 AP (각 1AP당 총 가격 1코인 할인, 최소 1코인) */
  readonly bargainAP: number;
}
interface OrderAction {
  readonly type: 'ORDER';
  readonly playerId: PlayerId;
  readonly componentType: ComponentType;
}
interface QuickOrderAction {
  readonly type: 'QUICK_ORDER';
  readonly playerId: PlayerId;
  readonly componentType: ComponentType;
}

// -- Workshop Actions (v5: Move 액션 추가) --
interface PrepareAction {
  readonly type: 'PREPARE';
  readonly playerId: PlayerId;
  readonly trickIdx: number;
}
interface MoveTrickAction {
  readonly type: 'MOVE_TRICK';
  readonly playerId: PlayerId;
  readonly fromIdx: number;
  readonly toEngineerSlot: boolean;
}
interface MoveComponentAction {
  readonly type: 'MOVE_COMPONENT';
  readonly playerId: PlayerId;
  readonly componentType: ComponentType;
  readonly managerSlotIdx: number; // 0 | 1
}
interface MoveApprenticeAction {
  readonly type: 'MOVE_APPRENTICE';
  readonly playerId: PlayerId;
  readonly apprenticeIdx: number;
}

// -- Theater Actions --
interface SetupTrickAction {
  readonly type: 'SETUP_TRICK';
  readonly playerId: PlayerId;
  readonly trickIdx: number;
  readonly cardId: CardId;
  readonly slotPosition: SlotPosition;
  readonly linkRewardChoices?: readonly LinkRewardChoice[];
}
interface RescheduleAction {
  readonly type: 'RESCHEDULE';
  readonly playerId: PlayerId;
  readonly fromCardId: CardId;
  readonly fromSlot: SlotPosition;
  readonly toCardId: CardId;
  readonly toSlot: SlotPosition;
}
interface ChooseWeekdayAction {
  readonly type: 'CHOOSE_WEEKDAY';
  readonly playerId: PlayerId;
  readonly weekday: Weekday;
}

// -- Dark Alley Actions (v5: 2장 뽑기 + 선택 분리) --
interface DrawCardsAction {
  readonly type: 'DRAW_CARDS';
  readonly playerId: PlayerId;
  readonly deckLocation: Location; // 어느 장소 덱에서 뽑을지
}
interface ChooseCardAction {
  readonly type: 'CHOOSE_CARD';
  readonly playerId: PlayerId;
  readonly chosenCardDefId: SpecialCardDefId;
}
interface FortuneTellingAction {
  readonly type: 'FORTUNE_TELLING';
  readonly playerId: PlayerId;
}

// -- Performance Phase (v3: 링크 보상 선택 분리) --
interface ChoosePerfCardAction {
  readonly type: 'CHOOSE_PERF_CARD';
  readonly playerId: PlayerId;
  readonly cardId: CardId;
}
interface ChooseLinkRewardAction {
  readonly type: 'CHOOSE_LINK_REWARD';
  readonly playerId: PlayerId;
  readonly choice: LinkRewardChoice;
}
interface AdvancePerformanceAction {
  readonly type: 'ADVANCE_PERFORMANCE';
}

// -- DA Ability (v3: 신규) --
interface UseDaAbilityAction {
  readonly type: 'USE_DA_ABILITY';
  readonly playerId: PlayerId;
  readonly abilityId: DaAbilityId;
  readonly params?: Readonly<Record<string, unknown>>;
}

// -- Phase Transitions --
interface AdvanceTurnAction {
  readonly type: 'ADVANCE_TURN';
}
interface ExecuteEndTurnAction {
  readonly type: 'EXECUTE_END_TURN';
}

export type GameAction =
  | AdvertiseAction
  | SkipAdvertiseAction
  | FinishAdvertiseAction
  | PlaceAssignmentCardAction
  | RemoveAssignmentCardAction
  | SubmitAssignmentAction
  | RevealAssignmentsAction
  | SelectCharacterAction
  | PlaceCharacterAction
  | PassCharacterAction
  | ConvertShardAction
  | FinishActionsAction
  | LearnTrickAction
  | TakeCoinsAction
  | HireAction
  | RerollAction
  | SetDieAction
  | ChooseDieAction
  | BuyAction
  | OrderAction
  | QuickOrderAction
  | PrepareAction
  | MoveTrickAction
  | MoveComponentAction
  | MoveApprenticeAction
  | SetupTrickAction
  | RescheduleAction
  | ChooseWeekdayAction
  | DrawCardsAction
  | ChooseCardAction
  | FortuneTellingAction
  | ChoosePerfCardAction
  | ChooseLinkRewardAction
  | AdvancePerformanceAction
  | UseDaAbilityAction
  | AdvanceTurnAction
  | ExecuteEndTurnAction;
