// Dark Alley actions: DRAW_CARDS, CHOOSE_CARD, FORTUNE_TELLING (v5)

import type { GameState, GameAction, ValidationError } from '../types';
import { err } from '../types';
import { registerHandler } from './registry';
import { getPlayer } from '../state/selectors';
import { updatePlayer, addLog } from '../state/helpers';

type DrawCardsAction = Extract<GameAction, { type: 'DRAW_CARDS' }>;
type ChooseCardAction = Extract<GameAction, { type: 'CHOOSE_CARD' }>;
type FortuneAction = Extract<GameAction, { type: 'FORTUNE_TELLING' }>;

/** DRAW_CARDS — 특수배치카드 덱에서 2장 뽑기 (선택 대기) */
registerHandler<DrawCardsAction>('DRAW_CARDS', {
  validate(state, action) {
    const errors: ValidationError[] = [];
    const player = state.players.find(p => p.id === action.playerId);
    if (!player) { errors.push(err('playerId', '플레이어 없음')); return errors; }
    const deck = state.darkAlley.specialDecks[action.deckLocation];
    if (!deck || deck.length < 2) {
      errors.push(err('deck', '해당 장소의 특수카드 덱에 카드가 부족합니다'));
    }
    if (state.darkAlley.drawnChoices !== null) {
      errors.push(err('drawn', '이미 카드를 뽑은 상태입니다 (선택 필요)'));
    }
    // 캐릭터당 Draw First Card 1회 제한 체크
    const turn = state.turnQueue[state.currentTurnIdx];
    if (turn && turn.characterIdx >= 0) {
      const char = player.characters[turn.characterIdx];
      if (char?.usedDrawFirstCard) {
        errors.push(err('drawLimit', '이 캐릭터는 이미 Draw First Card를 사용했습니다'));
      }
    }
    return errors;
  },
  apply(state, action) {
    const deck = [...(state.darkAlley.specialDecks[action.deckLocation] ?? [])];
    const drawn = deck.splice(0, 2);
    return {
      ...state,
      darkAlley: {
        ...state.darkAlley,
        specialDecks: {
          ...state.darkAlley.specialDecks,
          [action.deckLocation]: deck,
        },
        drawnChoices: drawn,
        drawnFromLocation: action.deckLocation,
      },
    };
  },
});

/** CHOOSE_CARD — 2장 중 1장 선택, 나머지 덱 밑으로 */
registerHandler<ChooseCardAction>('CHOOSE_CARD', {
  validate(state, action) {
    const errors: ValidationError[] = [];
    const player = state.players.find(p => p.id === action.playerId);
    if (!player) { errors.push(err('playerId', '플레이어 없음')); return errors; }
    if (!state.darkAlley.drawnChoices) {
      errors.push(err('drawn', '뽑은 카드가 없습니다'));
      return errors;
    }
    if (!state.darkAlley.drawnChoices.includes(action.chosenCardDefId)) {
      errors.push(err('choice', '뽑은 카드 중에 없는 카드입니다'));
    }
    return errors;
  },
  apply(state, action) {
    const player = getPlayer(state, action.playerId);
    const choices = state.darkAlley.drawnChoices!;
    const loc = state.darkAlley.drawnFromLocation!;
    const rejected = choices.filter(id => id !== action.chosenCardDefId);

    // 나머지 카드를 덱 맨 밑으로
    const deck = [...(state.darkAlley.specialDecks[loc] ?? []), ...rejected];

    // 특수카드를 플레이어 assignmentCards에 추가
    const newCardId = `${player.id}_SC_${Date.now()}` as any;
    let s: GameState = {
      ...state,
      darkAlley: {
        ...state.darkAlley,
        specialDecks: { ...state.darkAlley.specialDecks, [loc]: deck },
        drawnChoices: null,
        drawnFromLocation: null,
      },
    };
    s = updatePlayer(s, action.playerId, p => ({
      assignmentCards: [...p.assignmentCards, {
        id: newCardId,
        location: loc,
        kind: 'SPECIAL' as const,
        specialCardDefId: action.chosenCardDefId,
      }],
    }));
    s = addLog(s, `${player.name}이(가) 특수 배치카드 획득 (${loc})`);
    return s;
  },
});

/** FORTUNE_TELLING — Pending 예언을 시계방향으로 1슬롯 이동 */
registerHandler<FortuneAction>('FORTUNE_TELLING', {
  validate(state, action) {
    const errors: ValidationError[] = [];
    const player = state.players.find(p => p.id === action.playerId);
    if (!player) errors.push(err('playerId', '플레이어 없음'));
    if (state.prophecy.pending.length === 0) {
      errors.push(err('prophecy', 'Pending 예언이 없습니다'));
    }
    return errors;
  },
  apply(state, action) {
    const player = getPlayer(state, action.playerId);
    // 시계방향 이동: 각 Pending 예언이 1슬롯씩 왼쪽으로
    // (Active 쪽으로 가까워짐 = 더 빨리 발동)
    // 실제로는 pending 배열 순서가 곧 순서이므로, 이 액션은
    // pending[마지막] → pending[0] 앞으로 이동하는 효과
    // 룰북: "시계방향 1칸" = 발동이 1턴 빨라짐
    const pending = [...state.prophecy.pending];
    if (pending.length > 1) {
      const last = pending.pop()!;
      pending.unshift(last);
    }
    let s: GameState = {
      ...state,
      prophecy: { ...state.prophecy, pending },
    };
    s = addLog(s, `${player.name}이(가) 점술 수행 (예언 이동)`);
    return s;
  },
});
