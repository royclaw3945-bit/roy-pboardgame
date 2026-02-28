// Market actions: BUY, BARGAIN, ORDER, QUICK_ORDER

import type { GameState, GameAction, ValidationError } from '../types';
import { err } from '../types';
import { registerHandler } from './registry';
import { getPlayer } from '../state/selectors';
import { updatePlayer, addLog, pipe } from '../state/helpers';
import { COMPONENT_META, STARTING } from '../data/constants';

type BuyAction = Extract<GameAction, { type: 'BUY' }>;
type BargainAction = Extract<GameAction, { type: 'BARGAIN' }>;
type OrderAction = Extract<GameAction, { type: 'ORDER' }>;
type QuickOrderAction = Extract<GameAction, { type: 'QUICK_ORDER' }>;

function checkComponentLimit(player: ReturnType<typeof getPlayer>, comp: string): ValidationError | null {
  const current = player.components[comp as keyof typeof player.components] ?? 0;
  if (current >= STARTING.maxComponentsPerType) {
    return err('component', `${comp} 보유 한도 초과 (최대 ${STARTING.maxComponentsPerType})`);
  }
  return null;
}

registerHandler<BuyAction>('BUY', {
  validate(state, action) {
    const errors: ValidationError[] = [];
    const player = state.players.find(p => p.id === action.playerId);
    if (!player) { errors.push(err('playerId', '플레이어 없음')); return errors; }
    const meta = COMPONENT_META[action.componentType];
    if (player.coins < meta.cost) errors.push(err('coins', '코인 부족'));
    if (!state.market.stock.includes(action.componentType)) {
      errors.push(err('stock', '시장에 해당 컴포넌트가 없습니다'));
    }
    const limit = checkComponentLimit(player, action.componentType);
    if (limit) errors.push(limit);
    return errors;
  },
  apply(state, action) {
    const player = getPlayer(state, action.playerId);
    const cost = COMPONENT_META[action.componentType].cost;
    const stockIdx = state.market.stock.indexOf(action.componentType);
    const newStock = [...state.market.stock];
    newStock.splice(stockIdx, 1);

    let s: GameState = { ...state, market: { ...state.market, stock: newStock } };
    s = updatePlayer(s, action.playerId, p => ({
      coins: p.coins - cost,
      components: { ...p.components, [action.componentType]: p.components[action.componentType] + 1 },
    }));
    s = addLog(s, `${player.name}이(가) ${action.componentType} 구매 (-${cost}코인)`);
    return s;
  },
});

registerHandler<BargainAction>('BARGAIN', {
  validate(state, action) {
    const errors: ValidationError[] = [];
    const player = state.players.find(p => p.id === action.playerId);
    if (!player) { errors.push(err('playerId', '플레이어 없음')); return errors; }
    // Bargain = buy at reduced cost (1 coin regardless of tier)
    if (player.coins < 1) errors.push(err('coins', '코인 부족'));
    if (!state.market.stock.includes(action.componentType)) {
      errors.push(err('stock', '시장에 없음'));
    }
    const limit = checkComponentLimit(player, action.componentType);
    if (limit) errors.push(limit);
    return errors;
  },
  apply(state, action) {
    const player = getPlayer(state, action.playerId);
    const stockIdx = state.market.stock.indexOf(action.componentType);
    const newStock = [...state.market.stock];
    newStock.splice(stockIdx, 1);

    let s: GameState = { ...state, market: { ...state.market, stock: newStock } };
    s = updatePlayer(s, action.playerId, p => ({
      coins: p.coins - 1,
      components: { ...p.components, [action.componentType]: p.components[action.componentType] + 1 },
    }));
    s = addLog(s, `${player.name}이(가) ${action.componentType} 흥정 구매 (-1코인)`);
    return s;
  },
});

registerHandler<OrderAction>('ORDER', {
  validate(state, action) {
    const errors: ValidationError[] = [];
    const player = state.players.find(p => p.id === action.playerId);
    if (!player) { errors.push(err('playerId', '플레이어 없음')); return errors; }
    const meta = COMPONENT_META[action.componentType];
    if (player.coins < meta.cost) errors.push(err('coins', '코인 부족'));
    const limit = checkComponentLimit(player, action.componentType);
    if (limit) errors.push(limit);
    return errors;
  },
  apply(state, action) {
    const player = getPlayer(state, action.playerId);
    const cost = COMPONENT_META[action.componentType].cost;
    let s = updatePlayer(state, action.playerId, p => ({
      coins: p.coins - cost,
      components: { ...p.components, [action.componentType]: p.components[action.componentType] + 1 },
    }));
    s = addLog(s, `${player.name}이(가) ${action.componentType} 주문 (-${cost}코인)`);
    return s;
  },
});

registerHandler<QuickOrderAction>('QUICK_ORDER', {
  validate(state, action) {
    const errors: ValidationError[] = [];
    const player = state.players.find(p => p.id === action.playerId);
    if (!player) { errors.push(err('playerId', '플레이어 없음')); return errors; }
    const meta = COMPONENT_META[action.componentType];
    // Quick order costs 1 extra
    if (player.coins < meta.cost + 1) errors.push(err('coins', '코인 부족 (급송 추가비용 포함)'));
    const limit = checkComponentLimit(player, action.componentType);
    if (limit) errors.push(limit);
    return errors;
  },
  apply(state, action) {
    const player = getPlayer(state, action.playerId);
    const cost = COMPONENT_META[action.componentType].cost + 1;
    let s = updatePlayer(state, action.playerId, p => ({
      coins: p.coins - cost,
      components: { ...p.components, [action.componentType]: p.components[action.componentType] + 1 },
    }));
    s = addLog(s, `${player.name}이(가) ${action.componentType} 급송 주문 (-${cost}코인)`);
    return s;
  },
});
