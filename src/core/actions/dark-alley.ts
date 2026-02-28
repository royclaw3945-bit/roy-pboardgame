// Dark Alley actions: DRAW_SPECIAL, FORTUNE_TELLING

import type { GameState, GameAction, ValidationError } from '../types';
import { err } from '../types';
import { registerHandler } from './registry';
import { getPlayer } from '../state/selectors';
import { updatePlayer, adjustFame, addLog, pipe } from '../state/helpers';

type DrawAction = Extract<GameAction, { type: 'DRAW_SPECIAL' }>;
type FortuneAction = Extract<GameAction, { type: 'FORTUNE_TELLING' }>;

registerHandler<DrawAction>('DRAW_SPECIAL', {
  validate(state, action) {
    const errors: ValidationError[] = [];
    const player = state.players.find(p => p.id === action.playerId);
    if (!player) errors.push(err('playerId', '플레이어 없음'));
    if (state.darkAlley.specialDeck.length === 0) {
      errors.push(err('deck', '특수 카드 덱이 비었습니다'));
    }
    return errors;
  },
  apply(state, action) {
    const player = getPlayer(state, action.playerId);
    const [card, ...remaining] = state.darkAlley.specialDeck;
    const pid = action.playerId as unknown as number;
    const drawn = state.darkAlley.drawnCards[pid] ?? [];
    let s: GameState = {
      ...state,
      darkAlley: {
        ...state.darkAlley,
        specialDeck: remaining,
        drawnCards: { ...state.darkAlley.drawnCards, [pid]: [...drawn, card] },
      },
    };
    s = addLog(s, `${player.name}이(가) 특수 카드 획득: ${card.nameKo}`);
    return s;
  },
});

registerHandler<FortuneAction>('FORTUNE_TELLING', {
  validate(state, action) {
    const errors: ValidationError[] = [];
    const player = state.players.find(p => p.id === action.playerId);
    if (!player) errors.push(err('playerId', '플레이어 없음'));
    return errors;
  },
  apply(state, action) {
    const player = getPlayer(state, action.playerId);
    // Fortune telling: gain 1 shard
    let s = updatePlayer(state, action.playerId, p => ({
      shards: p.shards + 1,
    }));
    s = addLog(s, `${player.name}이(가) 점술로 샤드 획득`);
    return s;
  },
});
