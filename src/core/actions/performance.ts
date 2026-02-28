// Performance actions (v3): CHOOSE_PERF_CARD, CHOOSE_LINK_REWARD, ADVANCE_PERFORMANCE
// v3: 퍼포머 vs 게스트 분리, 심볼마커 회수, 전문가 보너스

import type {
  GameState, GameAction, ValidationError,
  PlayerId, PerfCardState, SymbolIndex,
} from '../types';
import { err } from '../types';
import { registerHandler } from './registry';
import { getPlayer, countPlayerLinksOnCard, getPlayerMarkersOnCard } from '../state/selectors';
import { updatePlayer, adjustFame, adjustCoins, addLog } from '../state/helpers';
import { getPerfCardDef } from '../data/perf-cards';
import { getTrickDef } from '../data/tricks';
import { LINK_REWARDS, SPECIALIST_THEATER_BONUS } from '../data/constants';

type ChooseCardAction = Extract<GameAction, { type: 'CHOOSE_PERF_CARD' }>;
type LinkAction = Extract<GameAction, { type: 'CHOOSE_LINK_REWARD' }>;
type AdvancePerfAction = Extract<GameAction, { type: 'ADVANCE_PERFORMANCE' }>;

function isPerformer(state: GameState, playerId: PlayerId, card: PerfCardState): boolean {
  if (!card.weekday) return false;
  return state.theater.weekdayPerformers[card.weekday] === playerId;
}

/** Recover markers to markersOnTrick */
function recoverMarkers(
  state: GameState,
  playerId: PlayerId,
  card: PerfCardState,
): GameState {
  const player = getPlayer(state, playerId);
  const markersOnCard = getPlayerMarkersOnCard(card, playerId);
  if (markersOnCard.length === 0) return state;

  // Count markers per symbolIndex to restore
  const symbolCounts = new Map<SymbolIndex, number>();
  for (const { marker } of markersOnCard) {
    const prev = symbolCounts.get(marker.symbolIndex) ?? 0;
    symbolCounts.set(marker.symbolIndex, prev + 1);
  }

  // Restore markers to trick slots
  const newTricks = player.tricks.map(t => {
    const extra = symbolCounts.get(t.symbolIndex) ?? 0;
    return extra > 0 ? { ...t, markersOnTrick: t.markersOnTrick + extra } : t;
  });

  // Clear markers from card
  const newSlots = card.slotMarkers.map(m =>
    m && m.playerId === playerId ? null : m,
  );
  const newPerfCards = state.theater.perfCards.map(c =>
    c.cardId === card.cardId ? { ...c, slotMarkers: newSlots } : c,
  );

  return {
    ...state,
    theater: { ...state.theater, perfCards: newPerfCards },
    players: state.players.map(p =>
      p.id === playerId ? { ...p, tricks: newTricks } : p,
    ),
  };
}

registerHandler<ChooseCardAction>('CHOOSE_PERF_CARD', {
  validate(state, action) {
    const errors: ValidationError[] = [];
    if (state.phase !== 'PERFORMANCE') errors.push(err('phase', '공연 페이즈가 아닙니다'));
    const player = state.players.find(p => p.id === action.playerId);
    if (!player) { errors.push(err('playerId', '플레이어 없음')); return errors; }
    const card = state.theater.perfCards.find(c => c.cardId === action.cardId);
    if (!card) { errors.push(err('cardId', '카드를 찾을 수 없음')); return errors; }
    const markers = getPlayerMarkersOnCard(card, action.playerId);
    if (markers.length === 0) {
      errors.push(err('markers', '이 카드에 마커가 없습니다'));
    }
    return errors;
  },
  apply(state, action) {
    const player = getPlayer(state, action.playerId);
    const card = state.theater.perfCards.find(c => c.cardId === action.cardId)!;
    const def = getPerfCardDef(action.cardId);
    const markers = getPlayerMarkersOnCard(card, action.playerId);
    const performing = isPerformer(state, action.playerId, card);

    let s = state;

    // 트릭 수익 (퍼포머 + 게스트 모두)
    for (const { marker } of markers) {
      // Find trick by symbolIndex
      const trickSlot = player.tricks.find(t => t.symbolIndex === marker.symbolIndex);
      if (!trickSlot) continue;
      const trick = getTrickDef(trickSlot.trickId);
      s = adjustFame(s, action.playerId, trick.yields.fame);
      s = adjustCoins(s, action.playerId, trick.yields.coins);
      if (trick.yields.shards > 0) {
        s = updatePlayer(s, action.playerId, p => ({
          shards: p.shards + trick.yields.shards,
        }));
      }
    }

    // 요일 보정 (퍼포머 + 게스트 모두) - card.weekday 기반
    // (요일 보정은 performance phase에서 별도 처리)

    if (performing) {
      // 퍼포머 보너스: 카드 보너스 + 링크 보너스 + 전문가 보너스
      s = adjustFame(s, action.playerId, def.performerBonus.fame);
      s = adjustCoins(s, action.playerId, def.performerBonus.coins);
      if (def.performerBonus.shards > 0) {
        s = updatePlayer(s, action.playerId, p => ({
          shards: p.shards + def.performerBonus.shards,
        }));
      }

      // 링크 보너스
      const links = countPlayerLinksOnCard(card, action.playerId);
      if (links > 0) {
        s = adjustFame(s, action.playerId, links);
      }

      // 전문가 보너스
      const updatedPlayer = getPlayer(s, action.playerId);
      for (const spec of updatedPlayer.specialists) {
        const bonus = SPECIALIST_THEATER_BONUS[spec];
        if (bonus.fame > 0) s = adjustFame(s, action.playerId, bonus.fame);
        if (bonus.coins > 0) s = adjustCoins(s, action.playerId, bonus.coins);
        if (bonus.shards > 0) {
          s = updatePlayer(s, action.playerId, p => ({
            shards: p.shards + bonus.shards,
          }));
        }
      }
    }

    // 마커 회수
    s = recoverMarkers(s, action.playerId, card);

    const role = performing ? '퍼포머' : '게스트';
    s = addLog(s, `${player.name}이(가) ${role}로 ${markers.length}개 트릭 공연 (카드: ${action.cardId})`);
    return s;
  },
});

registerHandler<LinkAction>('CHOOSE_LINK_REWARD', {
  validate(state, action) {
    const errors: ValidationError[] = [];
    const player = state.players.find(p => p.id === action.playerId);
    if (!player) errors.push(err('playerId', '플레이어 없음'));
    if (!['FAME', 'COINS'].includes(action.choice)) {
      errors.push(err('choice', '명성 또는 코인 중 선택'));
    }
    return errors;
  },
  apply(state, action) {
    const player = getPlayer(state, action.playerId);
    let amount = 0;
    for (const tier of LINK_REWARDS) {
      if (player.fame >= tier.fameThreshold) amount = tier.amount;
    }
    if (amount === 0) return state;

    let s = state;
    if (action.choice === 'FAME') {
      s = adjustFame(s, action.playerId, amount);
      s = addLog(s, `${player.name}이(가) 링크 보상: +${amount}명성`);
    } else {
      s = adjustCoins(s, action.playerId, amount);
      s = addLog(s, `${player.name}이(가) 링크 보상: +${amount}코인`);
    }
    return s;
  },
});

registerHandler<AdvancePerfAction>('ADVANCE_PERFORMANCE', {
  validate(state) {
    const errors: ValidationError[] = [];
    if (state.phase !== 'PERFORMANCE') {
      errors.push(err('phase', '공연 페이즈가 아닙니다'));
    }
    return errors;
  },
  apply(state) {
    return addLog(state, '공연 진행');
  },
});
