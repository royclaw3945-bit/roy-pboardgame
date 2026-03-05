// Market actions: BUY (with integrated Bargain), ORDER, QUICK_ORDER (v5)

import type { GameState, GameAction, ValidationError, ComponentType } from '../types';
import { err } from '../types';
import { registerHandler } from './registry';
import { getPlayer, getEffectiveComponentCount } from '../state/selectors';
import { updatePlayer, addLog } from '../state/helpers';
import { COMPONENT_META, STARTING } from '../data/constants';

type BuyAction = Extract<GameAction, { type: 'BUY' }>;
type OrderAction = Extract<GameAction, { type: 'ORDER' }>;
type QuickOrderAction = Extract<GameAction, { type: 'QUICK_ORDER' }>;

function checkComponentLimit(player: ReturnType<typeof getPlayer>, comp: ComponentType): ValidationError | null {
  const effective = getEffectiveComponentCount(player, comp);
  if (effective >= STARTING.maxComponentsPerType) {
    return err('component', `${comp} 보유 한도 초과 (최대 ${STARTING.maxComponentsPerType})`);
  }
  return null;
}

/** Check if component is available in buyArea (includes quickOrderSlot) */
function isInBuyArea(state: GameState, comp: ComponentType): boolean {
  if (state.market.buyArea.includes(comp)) return true;
  if (state.market.quickOrderSlot === comp) return true;
  return false;
}

registerHandler<BuyAction>('BUY', {
  validate(state, action) {
    const errors: ValidationError[] = [];
    const player = state.players.find(p => p.id === action.playerId);
    if (!player) { errors.push(err('playerId', '플레이어 없음')); return errors; }
    const meta = COMPONENT_META[action.componentType];
    // Bargain discount: each bargainAP reduces cost by 1 (minimum 1)
    const discount = action.bargainAP ?? 0;
    const cost = Math.max(meta.cost - discount, 1);
    if (player.coins < cost) errors.push(err('coins', `코인 부족 (필요: ${cost})`));
    if (!isInBuyArea(state, action.componentType)) {
      errors.push(err('buyArea', '시장 Buy 영역에 해당 컴포넌트가 없습니다'));
    }
    const limit = checkComponentLimit(player, action.componentType);
    if (limit) errors.push(limit);
    return errors;
  },
  apply(state, action) {
    const player = getPlayer(state, action.playerId);
    const meta = COMPONENT_META[action.componentType];
    const discount = action.bargainAP ?? 0;
    const cost = Math.max(meta.cost - discount, 1);

    let s = updatePlayer(state, action.playerId, p => ({
      coins: p.coins - cost,
      components: { ...p.components, [action.componentType]: p.components[action.componentType] + 1 },
    }));
    const discountText = discount > 0 ? ` (흥정 -${discount})` : '';
    s = addLog(s, `${player.name}이(가) ${COMPONENT_META[action.componentType].name} 구매 (-${cost}코인${discountText})`);
    return s;
  },
});

registerHandler<OrderAction>('ORDER', {
  validate(state, action) {
    const errors: ValidationError[] = [];
    const player = state.players.find(p => p.id === action.playerId);
    if (!player) { errors.push(err('playerId', '플레이어 없음')); return errors; }
    // Order: 같은 타입이 orderArea에 이미 있으면 불가
    if (state.market.orderArea.includes(action.componentType)) {
      errors.push(err('order', '이미 주문 중인 컴포넌트'));
    }
    return errors;
  },
  apply(state, action) {
    const player = getPlayer(state, action.playerId);
    let s: GameState = {
      ...state,
      market: {
        ...state.market,
        orderArea: [...state.market.orderArea, action.componentType],
      },
    };
    s = addLog(s, `${player.name}이(가) ${COMPONENT_META[action.componentType].name} 주문 (다음 턴 도착)`);
    return s;
  },
});

registerHandler<QuickOrderAction>('QUICK_ORDER', {
  validate(state, action) {
    const errors: ValidationError[] = [];
    const player = state.players.find(p => p.id === action.playerId);
    if (!player) { errors.push(err('playerId', '플레이어 없음')); return errors; }
    if (state.market.quickOrderSlot !== null) {
      errors.push(err('quickOrder', 'Quick Order 슬롯이 이미 사용 중'));
    }
    return errors;
  },
  apply(state, action) {
    const player = getPlayer(state, action.playerId);
    let s: GameState = {
      ...state,
      market: {
        ...state.market,
        quickOrderSlot: action.componentType,
      },
    };
    s = addLog(s, `${player.name}이(가) ${COMPONENT_META[action.componentType].name} 급송 주문 (즉시 구매 가능)`);
    return s;
  },
});
