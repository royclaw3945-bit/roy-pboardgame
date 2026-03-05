// End-game scoring — final score calculation (v4: DA only)

import type { GameState, PlayerState, PlayerId, ComponentType } from './types';
import { getTrickDef } from './data/tricks';
import { adjustFame, addLog } from './state/helpers';
import { countTotalMarkersOnTricks, countSpecialCards, getEffectiveComponentCount } from './state/selectors';
import { END_SCORING } from './data/constants';

interface ScoreBreakdown {
  readonly playerId: PlayerId;
  readonly name: string;
  readonly baseFame: number;
  readonly shardBonus: number;
  readonly coinBonus: number;
  readonly specialCardBonus: number;
  readonly trickEndBonus: number;
  readonly totalFame: number;
}

const BASIC_COMPS: readonly ComponentType[] = ['WOOD', 'METAL', 'GLASS', 'FABRIC'];
const ADVANCED_COMPS: readonly ComponentType[] = ['ROPE', 'OIL', 'SAW', 'ANIMAL'];
const SUPERIOR_COMPS: readonly ComponentType[] = ['LOCK', 'MIRROR', 'DISGUISE', 'GEAR'];

/** Cap bonus to FAME_CAP (20) per category */
function cap(value: number): number {
  return Math.min(value, END_SCORING.FAME_CAP);
}

function calcTrickEndBonus(player: PlayerState, _state: GameState): number {
  let bonus = 0;
  for (const slot of player.tricks) {
    const trick = getTrickDef(slot.trickId);
    if (!trick.endBonus) continue;
    const eb = trick.endBonus;
    let catBonus = 0;
    switch (eb.type) {
      case 'TRICK_MARKERS_ON_TRICK':
        // v4: sum of markersOnTrick across all trick cards
        catBonus = countTotalMarkersOnTricks(player) * (eb.famePerUnit ?? 0);
        break;
      case 'PER_SHARD':
        catBonus = player.shards * (eb.famePerUnit ?? 0);
        break;
      case 'PER_L1_TRICK':
        catBonus = player.tricks.filter(t => getTrickDef(t.trickId).level === 1).length * (eb.famePerUnit ?? 0);
        break;
      case 'PER_L2_TRICK':
        catBonus = player.tricks.filter(t => getTrickDef(t.trickId).level === 2).length * (eb.famePerUnit ?? 0);
        break;
      case 'PER_L3_TRICK':
        catBonus = player.tricks.filter(t => getTrickDef(t.trickId).level === 3).length * (eb.famePerUnit ?? 0);
        break;
      case 'PER_APPRENTICE':
        catBonus = player.characters.filter(c => c.type === 'APPRENTICE').length * (eb.famePerUnit ?? 0);
        break;
      case 'PER_3_COINS':
        catBonus = Math.floor(player.coins / 3) * (eb.famePerUnit ?? 0);
        break;
      case 'PER_BASIC_COMP':
        catBonus = BASIC_COMPS.reduce((sum, c) => sum + getEffectiveComponentCount(player, c), 0) * (eb.famePerUnit ?? 0);
        break;
      case 'PER_ADVANCED_COMP':
        catBonus = ADVANCED_COMPS.reduce((sum, c) => sum + getEffectiveComponentCount(player, c), 0) * (eb.famePerUnit ?? 0);
        break;
      case 'PER_SUPERIOR_COMP':
        catBonus = SUPERIOR_COMPS.reduce((sum, c) => sum + getEffectiveComponentCount(player, c), 0) * (eb.famePerUnit ?? 0);
        break;
      case 'ALL_SPECIALISTS': {
        const types = new Set(player.specialists);
        if (types.has('ENGINEER') && types.has('MANAGER') && types.has('ASSISTANT')) {
          catBonus = eb.fame ?? 0;
        }
        break;
      }
      case 'FOUR_TRICKS':
        if (player.tricks.length >= 4) catBonus = eb.fame ?? 0;
        break;
      case 'HAS_ENGINEER':
        if (player.specialists.includes('ENGINEER')) catBonus = eb.fame ?? 0;
        break;
      case 'HAS_MANAGER':
        if (player.specialists.includes('MANAGER')) catBonus = eb.fame ?? 0;
        break;
      case 'HAS_ASSISTANT':
        if (player.specialists.includes('ASSISTANT')) catBonus = eb.fame ?? 0;
        break;
      case 'PER_SPECIAL_CARD':
        // v4: Hellhound — fame per unused special card
        catBonus = countSpecialCards(player) * (eb.famePerUnit ?? 0);
        break;
    }
    // v4: cap each end-bonus category at 20 fame
    bonus += cap(catBonus);
  }
  return bonus;
}

export function calculateScores(state: GameState): readonly ScoreBreakdown[] {
  return state.players.map(player => {
    const shardBonus = cap(player.shards * END_SCORING.SHARD_TO_FAME);
    const coinBonus = cap(Math.floor(player.coins / END_SCORING.COINS_PER_FAME));
    // v4: special card bonus (2F per unused special card)
    const specialCardBonus = cap(countSpecialCards(player) * END_SCORING.SPECIAL_CARD_FAME);
    const trickEndBonus = calcTrickEndBonus(player, state);

    return {
      playerId: player.id,
      name: player.name,
      baseFame: player.fame,
      shardBonus,
      coinBonus,
      specialCardBonus,
      trickEndBonus,
      totalFame: player.fame + shardBonus + coinBonus + specialCardBonus + trickEndBonus,
    };
  });
}

export function applyFinalScoring(state: GameState): GameState {
  const scores = calculateScores(state);
  let s = state;

  for (const score of scores) {
    const delta = score.totalFame - score.baseFame;
    if (delta > 0) {
      s = adjustFame(s, score.playerId, delta);
    }
    s = addLog(s, `${score.name} 최종점수: ${score.totalFame} (기본: ${score.baseFame}, 샤드: +${score.shardBonus}, 코인: +${score.coinBonus}, 특수카드: +${score.specialCardBonus}, 트릭: +${score.trickEndBonus})`);
  }

  // Determine winner
  const sorted = [...scores].sort((a, b) => b.totalFame - a.totalFame);
  const winnerIdx = state.players.findIndex(p => p.id === sorted[0].playerId);
  s = { ...s, winner: winnerIdx };

  return s;
}
