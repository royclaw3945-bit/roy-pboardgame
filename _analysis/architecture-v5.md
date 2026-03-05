# Trickerion v5 Architecture — 전면 재설계

> 작성일: 2026-03-05
> 목적: 룰북 기반 완전한 시스템 설계. 땜질 없이 전체 시스템을 한번에 설계.

---

## 0. 설계 원칙

1. **모든 시스템을 먼저 타입으로 정의** — 구현 전에 타입이 완성돼야 함
2. **시스템 간 의존 관계를 명시** — A가 B를 필요로 하면 B부터 구현
3. **stub/placeholder 금지** — 구현하지 않을 시스템은 타입도 만들지 않음
4. **확장 가능한 카드 효과 패턴** — 특수카드 40장, 예언 27개를 레지스트리로 처리

---

## 1. 시스템 의존 관계 그래프

```
[Assignment Card System] ←── [Special Card System] ←── [Dark Alley Actions]
        ↓                            ↑
[Placement Phase]              [End Turn: 카드 회수]
        ↓
[Action Phase] ──→ [Market System] ──→ [End Turn: Orders Arrive]
      │    │
      │    └──→ [Theater/Performance] ──→ [Magician Abilities]
      │
      └──→ [Dark Alley Actions] ──→ [Prophecy System] ──→ [End Turn: 예언 이동]
                                          ↑
                                   [Magician: Priestess]

[Scoring] ←── [Special Card count] + [Lv.3 End Bonus] + [Category Cap]
```

**구현 순서 (의존성 역순)**:
1. 타입 전면 재정의
2. Market 재설계
3. Special Card 시스템 + Assignment Card 통합
4. Prophecy 시스템
5. Magician Abilities 완성
6. End Turn 완성
7. Scoring 수정

---

## 2. 타입 재설계

### 2.1 Assignment Card (영구 + 특수 통합)

```typescript
// 기존: AssignmentCardState (영구만)
// 새로: 영구/특수를 구분하는 통합 타입

interface AssignmentCard {
  readonly id: CardId;
  readonly location: Location;
  readonly kind: 'PERMANENT' | 'SPECIAL';
  // 특수카드 전용
  readonly specialCardDefId?: SpecialCardDefId; // 40장 중 어떤 카드인지
}

// 플레이어 손패 (영구 6장 + 획득한 특수카드)
// PlayerState.assignmentCards: readonly AssignmentCard[]

// 현재 턴 배치 (영구든 특수든 동일하게 사용)
interface AssignmentCardPlacement {
  readonly cardId: CardId;
  readonly characterIndices: readonly number[];
  // 특수카드 보너스: 사용 여부
  readonly usedSpecialBonus: boolean;
  readonly chosenBonusType: 'PRINTED' | 'EXTRA_AP' | 'NONE';
}
```

**핵심 변경**: `assignmentCards`가 영구+특수 모두 포함. 배정 페이즈에서 동일하게 사용.

### 2.2 Special Card Definition (40장)

```typescript
type SpecialCardDefId = string; // e.g. 'SC_DOWNTOWN_01'

interface SpecialCardDef {
  readonly id: SpecialCardDefId;
  readonly location: Location; // DOWNTOWN | MARKET_ROW | WORKSHOP | THEATER
  readonly nameKo: string;
  readonly name: string;
  readonly enhancedAction: ActionType | 'ANY'; // 강화 대상 액션
  readonly effect: SpecialCardEffect; // 효과 정의
}

// 효과 타입 — 레지스트리 패턴으로 처리
type SpecialCardEffect =
  | { type: 'EXTRA_AP'; amount: number }
  | { type: 'DISCOUNT'; amount: number }
  | { type: 'BONUS_FAME'; amount: number }
  | { type: 'BONUS_COINS'; amount: number }
  | { type: 'FREE_COMPONENT'; tier: 'BASIC' | 'ADVANCED' | 'SUPERIOR' }
  | { type: 'CUSTOM'; handler: string }; // 복잡한 효과는 핸들러 이름으로
```

### 2.3 Dark Alley State (재설계)

```typescript
interface DarkAlleyState {
  // 4개 장소별 덱 (10장/장소)
  readonly specialDecks: Readonly<Record<Location, readonly SpecialCardDefId[]>>;
  // 현재 2장 뽑기 중인 상태 (선택 UI용)
  readonly drawnChoices: readonly SpecialCardDefId[] | null;
  readonly drawnFromLocation: Location | null;
}
```

**기존 `drawnCards` 제거** — 뽑은 카드는 `PlayerState.assignmentCards`에 직접 추가.

### 2.4 Market State (재설계)

```typescript
interface MarketState {
  // Buy 영역: 여기 있는 컴포넌트만 BUY 가능
  readonly buyArea: readonly ComponentType[];
  // Order 영역: 다음 턴 도착
  readonly orderArea: readonly ComponentType[];
  // Quick Order 슬롯: 즉시 buyArea에 포함, End Turn에 제거
  readonly quickOrderSlot: ComponentType | null;
}

// 초기 재고: WOOD, METAL, GLASS, FABRIC (기본 4종 1개씩)
```

**액션 재정의**:
- `BUY(1AP)`: buyArea(+quickOrderSlot)에서 구매. Bargain과 조합 가능.
- `BARGAIN(1AP)`: BUY와 함께만 사용. 총 가격 1코인 할인 (최소 1).
  - **구현**: BUY 액션에 `bargainCount: number` 필드 추가
- `ORDER(1AP)`: orderArea에 배치. 같은 타입 중복 불가.
- `QUICK_ORDER(2AP)`: quickOrderSlot에 배치. 즉시 BUY 가능.

### 2.5 Prophecy System (신규)

```typescript
type ProphecyTokenId = string; // 'PROPH_01' ~ 'PROPH_27'

interface ProphecyDef {
  readonly id: ProphecyTokenId;
  readonly nameKo: string;
  readonly effect: ProphecyEffect;
}

type ProphecyEffect =
  | { type: 'ALL_PLAYERS'; effect: string } // 모든 플레이어 영향
  | { type: 'ACTIVE_PLAYER'; effect: string }
  | { type: 'CUSTOM'; handler: string };

interface ProphecyState {
  readonly active: ProphecyTokenId | null; // 현재 턴 적용
  readonly pending: readonly ProphecyTokenId[]; // 3슬롯 (왼→오)
  readonly deck: readonly ProphecyTokenId[]; // 남은 토큰
  readonly discard: readonly ProphecyTokenId[];
}
```

### 2.6 PlayerState 변경사항

```typescript
interface PlayerState {
  // ... 기존 필드 유지 ...

  // 변경: assignmentCards가 영구+특수 통합
  readonly assignmentCards: readonly AssignmentCard[];

  // 변경: currentPlacements에 특수카드 보너스 정보 포함
  readonly currentPlacements: readonly AssignmentCardPlacement[];

  // 추가: 이번 캐릭터가 Draw First Card를 사용했는지
  readonly usedDrawFirstCard: boolean;
}
```

### 2.7 GameState 변경사항

```typescript
interface GameState {
  // ... 기존 필드 유지 ...

  // 변경
  readonly market: MarketState; // buyArea/orderArea/quickOrderSlot
  readonly darkAlley: DarkAlleyState; // specialDecks + drawnChoices

  // 추가
  readonly prophecy: ProphecyState;

  // 변경: advertisedPlayers 추가 (포스터 관리)
  readonly advertisedPlayers: readonly PlayerId[];
}
```

---

## 3. 액션 재설계

### 3.1 Market 액션

| 액션 | AP | 변경사항 |
|------|------|---------|
| BUY | 1 | `bargainDiscount` 파라미터 추가. buyArea+quickOrderSlot에서만 |
| ~~BARGAIN~~ | - | **삭제**. BUY에 통합 |
| ORDER | 1 | orderArea에 배치. 같은 타입 중복 불가 |
| QUICK_ORDER | 2 | quickOrderSlot에 배치 (1개만) |

```typescript
// BUY 액션 재정의
interface BuyAction {
  type: 'BUY';
  playerId: PlayerId;
  componentType: ComponentType;
  quantity: number; // 같은 타입 최대 3개
  bargainAP: number; // 0~N, 각 1AP 추가 소모하여 1코인씩 할인
}

// ORDER 액션
interface OrderAction {
  type: 'ORDER';
  playerId: PlayerId;
  componentType: ComponentType; // 공급에서 orderArea로
}

// QUICK_ORDER 액션
interface QuickOrderAction {
  type: 'QUICK_ORDER';
  playerId: PlayerId;
  componentType: ComponentType; // 공급에서 quickOrderSlot으로
}
```

### 3.2 Dark Alley 액션

| 액션 | AP | 변경사항 |
|------|------|---------|
| DRAW_FIRST_CARD | 1 | 2장 뽑기 → 1장 선택. 캐릭터당 1회 |
| DRAW_FURTHER_CARD | 2 | DRAW_FIRST 이후 추가 뽑기 |
| FORTUNE_TELLING | 1 | 예언 이동 (Pending → 시계방향) |

```typescript
// 2단계 액션: DRAW_CARDS → CHOOSE_CARD
interface DrawCardsAction {
  type: 'DRAW_CARDS';
  playerId: PlayerId;
  deckLocation: Location; // 어느 장소 덱에서 뽑을지
}
// → GameState.darkAlley.drawnChoices에 2장 세팅

interface ChooseCardAction {
  type: 'CHOOSE_CARD';
  playerId: PlayerId;
  chosenCardDefId: SpecialCardDefId; // 선택한 카드
}
// → PlayerState.assignmentCards에 추가, 나머지 덱 밑으로

interface FortuneTellingAction {
  type: 'FORTUNE_TELLING';
  playerId: PlayerId;
}
// → prophecy.pending 시계방향 이동
```

### 3.3 Theater 액션

| 액션 | AP | 변경사항 |
|------|------|---------|
| SETUP_TRICK | 1 | 샤드 심볼 보너스 추가 |
| RESCHEDULE | 1 | 변경 없음 |
| CHOOSE_WEEKDAY | 3 | **삭제**. Perform으로 대체 |
| PERFORM | 3(all) | 마법사를 퍼포먼스 슬롯에 배치 |

**주의**: 룰북에서 극장 배치는 "요일 선택" + "슬롯 배치"가 아니라:
- 백스테이지 슬롯: Set Up + Reschedule 가능
- 퍼포먼스 슬롯: 마법사만, AP 전부 소모, Performance Phase에서 실행

→ 현재 CHOOSE_WEEKDAY(3AP)는 실제로는 마법사를 퍼포먼스 슬롯에 배치하는 것.
→ 기존 구현 유지하되 이름을 명확히.

### 3.4 Workshop 추가 액션

| 액션 | AP | 상태 |
|------|------|------|
| PREPARE | 동적 | 기존 유지 |
| MOVE_TRICK | 1 | 신규 (기술자 보드) |
| MOVE_COMPONENT | 1 | 신규 (매니저 보드) |
| MOVE_APPRENTICE | 1 | 신규 (어시스턴트 보드) |

---

## 4. End Turn 재설계

8단계를 순서대로 실행:

```typescript
function executeEndTurn(state: GameState): GameState {
  let s = state;
  s = payWages(s);           // 8.1 임금
  s = returnCharacters(s);    // 8.2 캐릭터 귀환 + 신규 전문가 합류
  s = ordersArrive(s);        // 8.3 주문 도착 (NEW)
  s = rotatePerformanceCards(s); // 8.4 퍼포먼스 카드 이동
  s = returnPosters(s);       // 8.5 포스터 반환 (NEW)
  s = advanceTurnCounter(s);  // 8.6 턴 카운터
  s = moveProphecies(s);      // 8.7 예언 이동 (NEW)
  s = discardSpecialCards(s); // 8.8 특수카드 회수 (NEW)
  s = resetForNextTurn(s);    // 주사위 marked 리셋, currentPlacements 초기화 등
  return s;
}
```

### 8.3 Orders Arrive (신규)
```
orderArea의 컴포넌트 → buyArea로 이동 (기존 buyArea 교체)
quickOrderSlot → 공급으로 반환 (null로)
```

### 8.7 Prophecy Movement (신규)
```
active → discard
pending[0] → active
pending[1] → pending[0]
pending[2] → pending[1]
deck에서 1장 → pending[2]
```

### 8.8 Special Card Discard (신규)
```
currentPlacements에서 kind='SPECIAL'인 카드:
  - 사용됨 → 해당 location의 specialDecks 맨 밑에
  - Idle(미사용) → 소유자 assignmentCards에 유지
```

---

## 5. Scoring 수정

```typescript
function calculateScores(state: GameState): ScoreBreakdown[] {
  return state.players.map(player => {
    const shardBonus = cap(player.shards * 1);
    const coinBonus = cap(Math.floor(player.coins / 3));

    // 미사용 특수카드 수 (손에 남아있는 SPECIAL 카드)
    const specialCount = player.assignmentCards.filter(c => c.kind === 'SPECIAL').length;
    const specialCardBonus = cap(specialCount * 2);

    // Lv.3 종료 보너스 (각 트릭별 개별 cap)
    const trickEndBonus = calcTrickEndBonus(player, state);

    return { ..., totalFame: player.fame + shardBonus + coinBonus + specialCardBonus + trickEndBonus };
  });
}
```

**cap은 항목별로 적용**: 샤드/코인/특수카드/각 Lv.3 트릭 보너스 각각 최대 20명성.

---

## 6. Magician Abilities 설계

이벤트 훅 패턴: 각 능력이 특정 게임 이벤트에 반응

```typescript
interface MagicianAbility {
  readonly id: string;
  readonly trigger: AbilityTrigger;
  readonly validate: (state: GameState, playerId: PlayerId) => boolean;
  readonly apply: (state: GameState, playerId: PlayerId, params?: any) => GameState;
}

type AbilityTrigger =
  | 'ON_PLACEMENT'      // 캐릭터 배치 시 (Mechaniker, Gentleman)
  | 'ON_TURN'           // 턴 중 수동 발동 (Optico, Yoruba)
  | 'PRE_PERFORMANCE'   // 공연 직전 (Escapist, Yoruba)
  | 'ON_SETUP_TRICK'    // Set Up Trick 시 (Electra)
  | 'ON_PERFORM'        // 공연 실행 시 (Red Lotus)
  | 'DARK_ALLEY_ACTION' // DA 액션으로 발동 (Priestess)
  ;
```

| 마술사 | 트리거 | 구현 |
|--------|--------|------|
| Mechaniker | ON_PLACEMENT | 견습생 AP+1 (극장 외) |
| Priestess | DARK_ALLEY_ACTION | Active 예언 교체 (1 DA AP) |
| Escapist | PRE_PERFORMANCE | 무료 Reschedule + 링크보너스 |
| Optico | ON_TURN | 상대 특수카드 효과 복사 |
| Red Lotus | ON_PERFORM | 상대 트릭 수익 도용 |
| Yoruba | PRE_PERFORMANCE | 샤드로 상대 카드 선택 |
| Electra | ON_SETUP_TRICK | 같은 트릭 마커 2개 스택 |
| Gentleman | ON_PLACEMENT | 트릭 수만큼 명성 (DT/MR/DA) |

---

## 7. 구현 계획 (Phase별)

### Phase 1: 타입 재정의 (파일 4개)
- `src/core/types/player.ts` — AssignmentCard 통합, usedDrawFirstCard
- `src/core/types/board.ts` — MarketState, DarkAlleyState, ProphecyState 재정의
- `src/core/types/action.ts` — BUY 통합, DRAW_CARDS/CHOOSE_CARD, Workshop Move 액션
- `src/core/types/base.ts` — SpecialCardDefId, ProphecyTokenId 추가

### Phase 2: 데이터 정의 (파일 3개)
- `src/core/data/special-cards.ts` — 40장 정의 (카드 텍스트 필요)
- `src/core/data/prophecies.ts` — 27개 정의 (효과 텍스트 필요)
- `src/core/data/constants.ts` — MARKET_INITIAL_STOCK 등 추가

### Phase 3: 핵심 시스템 구현 (파일 5개)
- `src/core/actions/market.ts` — BUY/ORDER/QUICK_ORDER 재구현
- `src/core/actions/dark-alley.ts` — DRAW_CARDS/CHOOSE_CARD/FORTUNE_TELLING
- `src/core/actions/workshop.ts` — MOVE_TRICK/MOVE_COMPONENT/MOVE_APPRENTICE 추가
- `src/core/state/init.ts` — Market/DarkAlley/Prophecy 초기화
- `src/core/phases/end-turn.ts` — 8단계 완전 구현

### Phase 4: 마술사 + 점수 (파일 3개)
- `src/core/actions/da-abilities.ts` — 8명 전원 구현
- `src/core/scoring.ts` — 항목별 cap, countSpecialCards 실구현
- `src/core/phases/performance.ts` — 마술사 능력 훅 연동

### Phase 5: UI 연동 (파일 4개)
- `AssignmentPanel.tsx` — 특수카드 통합 표시
- `PlacementPanel.tsx` — 새 액션들 UI
- `PlayerBoard.tsx` — 특수카드 손패 표시
- `DarkAlleyUI` — 2장 선택 UI

---

## 8. 데이터 의존성 (외부 입력 필요)

| 데이터 | 수량 | 소스 | 상태 |
|--------|------|------|------|
| 특수 배치카드 효과 | 40장 | 물리 카드 텍스트 | 미확보 |
| 예언 토큰 효과 | 27개 | 마술사 워크북 | 미확보 |
| 퍼포먼스 카드 링크 서클 | 28장 | 물리 카드 이미지 | 미확보 |

→ 이 데이터 없이도 **시스템 구조는 완성 가능**. 효과 핸들러는 레지스트리 패턴이므로 데이터만 채우면 됨.

---

## 9. 마이그레이션 전략

기존 코드를 점진적으로 교체:
1. 새 타입 정의 (기존과 병행)
2. 새 액션 핸들러 작성 (기존 것 교체)
3. 기존 테스트를 새 구조에 맞게 수정
4. UI 컴포넌트 업데이트
5. 기존 타입/핸들러 삭제

**원칙**: 각 Phase 완료 후 `npx next build` + `npx vitest run` 통과 필수.
