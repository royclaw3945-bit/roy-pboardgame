// Theater actions: SETUP_TRICK, RESCHEDULE, CHOOSE_WEEKDAY (v3)
// v3: symbolIndex 기반 마커, 마법사만 Perform, 요일 제한

import type {
  GameState, GameAction, ValidationError,
  CardId, PerfCardState, Weekday,
} from '../types';
import { err } from '../types';
import { registerHandler } from './registry';
import { getPlayer } from '../state/selectors';
import { updatePlayer, addLog } from '../state/helpers';
import { getPerfCardDef } from '../data/perf-cards';
import { MARKERS_PER_SYMBOL } from '../data/constants';

type SetupAction = Extract<GameAction, { type: 'SETUP_TRICK' }>;
type RescheduleAction = Extract<GameAction, { type: 'RESCHEDULE' }>;
type ChooseWeekdayAction = Extract<GameAction, { type: 'CHOOSE_WEEKDAY' }>;

function findCard(state: GameState, cardId: CardId): PerfCardState | undefined {
  return state.theater.perfCards.find(c => c.cardId === cardId);
}

registerHandler<SetupAction>('SETUP_TRICK', {
  validate(state, action) {
    const errors: ValidationError[] = [];
    const player = state.players.find(p => p.id === action.playerId);
    if (!player) { errors.push(err('playerId', '플레이어 없음')); return errors; }

    // v3: 마법사는 SETUP_TRICK 불가 (Perform만 가능)
    const turn = state.turnQueue[state.currentTurnIdx];
    if (turn && turn.playerId === action.playerId) {
      const char = player.characters[turn.characterIdx];
      if (char?.type === 'MAGICIAN') {
        errors.push(err('character', '마법사는 트릭 셋업 불가 (Perform만 가능)'));
      }
    }

    const trickSlot = player.tricks[action.trickIdx];
    if (!trickSlot) { errors.push(err('trickIdx', '트릭 인덱스 오류')); return errors; }
    if (!trickSlot.prepared) errors.push(err('prepared', '준비되지 않은 트릭'));
    if (trickSlot.markersOnTrick <= 0) {
      errors.push(err('markers', '배치할 마커가 없습니다'));
    }

    const card = findCard(state, action.cardId);
    if (!card) { errors.push(err('cardId', '퍼포먼스 카드를 찾을 수 없음')); return errors; }

    const def = getPerfCardDef(action.cardId);
    if (!def.activeSlots.includes(action.slotPosition)) {
      errors.push(err('slot', '비활성 슬롯'));
    }
    if (card.slotMarkers[action.slotPosition] !== null) {
      errors.push(err('slot', '이미 점유된 슬롯'));
    }

    // v3: 같은 심볼 + 같은 카드 불가
    const symbolIdx = trickSlot.symbolIndex;
    for (const marker of card.slotMarkers) {
      if (marker && marker.playerId === action.playerId && marker.symbolIndex === symbolIdx) {
        errors.push(err('symbol', '같은 심볼 마커를 같은 카드에 중복 배치 불가'));
        break;
      }
    }

    return errors;
  },
  apply(state, action) {
    const player = getPlayer(state, action.playerId);
    const trickSlot = player.tricks[action.trickIdx];

    // v3: symbolIndex 기반 마커 배치
    const newPerfCards = state.theater.perfCards.map(c => {
      if (c.cardId !== action.cardId) return c;
      const newSlots = [...c.slotMarkers];
      newSlots[action.slotPosition] = {
        playerId: action.playerId,
        symbolIndex: trickSlot.symbolIndex,
      };
      return { ...c, slotMarkers: newSlots };
    });

    // markersOnTrick 감소
    const newTricks = player.tricks.map((t, i) =>
      i === action.trickIdx ? { ...t, markersOnTrick: t.markersOnTrick - 1 } : t,
    );

    let s: GameState = {
      ...state,
      theater: { ...state.theater, perfCards: newPerfCards },
      players: state.players.map(p =>
        p.id === action.playerId ? { ...p, tricks: newTricks } : p,
      ),
    };
    s = addLog(s, `${player.name}이(가) 트릭 마커를 카드 ${action.cardId} 슬롯 ${action.slotPosition}에 배치`);
    return s;
  },
});

registerHandler<RescheduleAction>('RESCHEDULE', {
  validate(state, action) {
    const errors: ValidationError[] = [];
    const fromCard = findCard(state, action.fromCardId);
    if (!fromCard) { errors.push(err('fromCard', '소스 카드 없음')); return errors; }
    const marker = fromCard.slotMarkers[action.fromSlot];
    if (!marker) errors.push(err('fromSlot', '해당 슬롯에 마커 없음'));
    else if (marker.playerId !== action.playerId) errors.push(err('owner', '자기 마커만 이동 가능'));

    const toCard = findCard(state, action.toCardId);
    if (!toCard) { errors.push(err('toCard', '대상 카드 없음')); return errors; }
    const toDef = getPerfCardDef(action.toCardId);
    if (!toDef.activeSlots.includes(action.toSlot)) errors.push(err('toSlot', '비활성 슬롯'));
    if (toCard.slotMarkers[action.toSlot] !== null) errors.push(err('toSlot', '이미 점유된 슬롯'));

    // v3: 같은 심볼 + 같은 카드 불가
    if (marker && toCard) {
      for (const m of toCard.slotMarkers) {
        if (m && m.playerId === action.playerId && m.symbolIndex === marker.symbolIndex) {
          errors.push(err('symbol', '같은 심볼 마커를 같은 카드에 중복 배치 불가'));
          break;
        }
      }
    }
    return errors;
  },
  apply(state, action) {
    const player = getPlayer(state, action.playerId);
    const fromCard = findCard(state, action.fromCardId)!;
    const marker = fromCard.slotMarkers[action.fromSlot]!;

    const newPerfCards = state.theater.perfCards.map(c => {
      if (c.cardId === action.fromCardId) {
        const newSlots = [...c.slotMarkers];
        newSlots[action.fromSlot] = null;
        return { ...c, slotMarkers: newSlots };
      }
      if (c.cardId === action.toCardId) {
        const newSlots = [...c.slotMarkers];
        newSlots[action.toSlot] = marker;
        return { ...c, slotMarkers: newSlots };
      }
      return c;
    });

    let s: GameState = { ...state, theater: { ...state.theater, perfCards: newPerfCards } };
    s = addLog(s, `${player.name}이(가) 마커 이동: ${action.fromCardId}[${action.fromSlot}] → ${action.toCardId}[${action.toSlot}]`);
    return s;
  },
});

registerHandler<ChooseWeekdayAction>('CHOOSE_WEEKDAY', {
  validate(state, action) {
    const errors: ValidationError[] = [];
    const player = state.players.find(p => p.id === action.playerId);
    if (!player) { errors.push(err('playerId', '플레이어 없음')); return errors; }

    const validDays: readonly Weekday[] = ['THURSDAY', 'FRIDAY', 'SATURDAY', 'SUNDAY'];
    if (!validDays.includes(action.weekday)) {
      errors.push(err('weekday', '잘못된 요일'));
      return errors;
    }

    // v3: 상대가 이미 점유한 요일 불가
    const currentPerformer = state.theater.weekdayPerformers[action.weekday];
    if (currentPerformer !== null && currentPerformer !== action.playerId) {
      errors.push(err('weekday', '상대 플레이어가 이미 해당 요일을 점유'));
    }

    // v3: 같은 플레이어의 극장 캐릭터는 모두 같은 요일이어야 함
    if (player.chosenWeekday !== null && player.chosenWeekday !== action.weekday) {
      errors.push(err('weekday', '이미 다른 요일을 선택함'));
    }

    return errors;
  },
  apply(state, action) {
    const player = getPlayer(state, action.playerId);

    // v3: weekdayPerformers에 기록
    const newPerformers = {
      ...state.theater.weekdayPerformers,
      [action.weekday]: action.playerId,
    };

    let s: GameState = {
      ...state,
      theater: { ...state.theater, weekdayPerformers: newPerformers },
    };
    s = updatePlayer(s, action.playerId, () => ({ chosenWeekday: action.weekday }));
    s = addLog(s, `${player.name}이(가) ${action.weekday}에 공연 예약`);
    return s;
  },
});
