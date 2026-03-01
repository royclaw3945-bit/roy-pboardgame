// Performance actions (v4): CHOOSE_PERF_CARD, CHOOSE_LINK_REWARD, ADVANCE_PERFORMANCE
// v4: 요일보정 적용, 마커 supply로 반환, 퍼포머 링크보너스 고정 1F/link

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
import { SPECIALIST_THEATER_BONUS, WEEKDAY_MOD, PERFORMER_LINK_BONUS } from '../data/constants';

type ChooseCardAction = Extract<GameAction, { type: 'CHOOSE_PERF_CARD' }>;
type LinkAction = Extract<GameAction, { type: 'CHOOSE_LINK_REWARD' }>;
type AdvancePerfAction = Extract<GameAction, { type: 'ADVANCE_PERFORMANCE' }>;

function isPerformer(state: GameState, playerId: PlayerId, card: PerfCardState): boolean {
  if (!card.weekday) return false;
  return state.theater.weekdayPerformers[card.weekday] === playerId;
}

/** Recover markers to supply (NOT to markersOnTrick) — v4 fix */
function recoverMarkers(
  state: GameState,
  playerId: PlayerId,
  card: PerfCardState,
): GameState {
  const markersOnCard = getPlayerMarkersOnCard(card, playerId);
  if (markersOnCard.length === 0) return state;

  // Clear markers from card (markers go to supply automatically:
  // supply = MARKERS_PER_SYMBOL - markersOnTrick - onBoard)
  const newSlots = card.slotMarkers.map(m =>
    m && m.playerId === playerId ? null : m,
  );
  const newPerfCards = state.theater.perfCards.map(c =>
    c.cardId === card.cardId ? { ...c, slotMarkers: newSlots } : c,
  );

  return { ...state, theater: { ...state.theater, perfCards: newPerfCards } };
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

    // Determine weekday modifier
    const weekday = card.weekday;
    const weekdayMod = weekday ? WEEKDAY_MOD[weekday] : { fameMod: 0, coinMod: 0 };

    let s = state;

    // 트릭 수익 (퍼포머 + 게스트 모두) with 요일보정
    for (const { marker } of markers) {
      const trickSlot = player.tricks.find(t => t.symbolIndex === marker.symbolIndex);
      if (!trickSlot) continue;
      const trick = getTrickDef(trickSlot.trickId);

      // v4: 요일보정 적용 (최소 0)
      const adjustedFame = Math.max(0, trick.yields.fame + weekdayMod.fameMod);
      const adjustedCoins = Math.max(0, trick.yields.coins + weekdayMod.coinMod);

      s = adjustFame(s, action.playerId, adjustedFame);
      s = adjustCoins(s, action.playerId, adjustedCoins);
      if (trick.yields.shards > 0) {
        s = updatePlayer(s, action.playerId, p => ({
          shards: p.shards + trick.yields.shards,
        }));
      }
    }

    if (performing) {
      // 퍼포머 보너스: 카드 보너스
      s = adjustFame(s, action.playerId, def.performerBonus.fame);
      s = adjustCoins(s, action.playerId, def.performerBonus.coins);
      if (def.performerBonus.shards > 0) {
        s = updatePlayer(s, action.playerId, p => ({
          shards: p.shards + def.performerBonus.shards,
        }));
      }

      // v4: 퍼포머 링크보너스 = 고정 1F/link
      const links = countPlayerLinksOnCard(card, action.playerId);
      if (links > 0) {
        s = adjustFame(s, action.playerId, links * PERFORMER_LINK_BONUS);
      }

      // v4: 극장에 배치된 전문가만 보너스 (극장 캐릭터 체크)
      const updatedPlayer = getPlayer(s, action.playerId);
      const theaterSpecs = updatedPlayer.characters.filter(
        c => c.location === 'THEATER' && c.placed &&
          (c.type === 'ENGINEER' || c.type === 'MANAGER' || c.type === 'ASSISTANT'),
      );
      for (const char of theaterSpecs) {
        const specType = char.type as 'ENGINEER' | 'MANAGER' | 'ASSISTANT';
        const bonus = SPECIALIST_THEATER_BONUS[specType];
        if (bonus.fame > 0) s = adjustFame(s, action.playerId, bonus.fame);
        if (bonus.coins > 0) s = adjustCoins(s, action.playerId, bonus.coins);
        if (bonus.shards > 0) {
          s = updatePlayer(s, action.playerId, p => ({
            shards: p.shards + bonus.shards,
          }));
        }
      }
    }

    // v4: 마커 회수 → supply로 (markersOnTrick 변경 안 함)
    s = recoverMarkers(s, action.playerId, card);

    // 다른 플레이어의 마커도 공연됨 (게스트)
    for (const otherPlayer of s.players) {
      if (otherPlayer.id === action.playerId) continue;
      const otherMarkers = getPlayerMarkersOnCard(card, otherPlayer.id);
      if (otherMarkers.length === 0) continue;

      // 게스트 트릭 수익 + 요일보정
      for (const { marker: m } of otherMarkers) {
        const ts = otherPlayer.tricks.find(t => t.symbolIndex === m.symbolIndex);
        if (!ts) continue;
        const trick = getTrickDef(ts.trickId);
        const adjFame = Math.max(0, trick.yields.fame + weekdayMod.fameMod);
        const adjCoins = Math.max(0, trick.yields.coins + weekdayMod.coinMod);
        s = adjustFame(s, otherPlayer.id, adjFame);
        s = adjustCoins(s, otherPlayer.id, adjCoins);
        if (trick.yields.shards > 0) {
          s = updatePlayer(s, otherPlayer.id, p => ({
            shards: p.shards + trick.yields.shards,
          }));
        }
      }

      // 게스트 마커도 회수
      s = recoverMarkers(s, otherPlayer.id, card);
    }

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
    // v4: This handler is now mostly unused since link rewards are given in SETUP_TRICK
    // Kept for backward compatibility
    return addLog(state, `${getPlayer(state, action.playerId).name}: 링크 보상 선택`);
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
