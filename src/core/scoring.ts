// End-game scoring — final score calculation

import type { GameState, PlayerState, PlayerId, ComponentType } from './types';
import { getTrickDef } from './data/tricks';
import { adjustFame, addLog } from './state/helpers';
import { countTotalMarkersOnBoard } from './state/selectors';
import { END_SCORING, COMPONENT_META } from './data/constants';

interface ScoreBreakdown {
  readonly playerId: PlayerId;
  readonly name: string;
  readonly baseFame: number;
  readonly shardBonus: number;
  readonly coinBonus: number;
  readonly characterBonus: number;
  readonly trickEndBonus: number;
  readonly totalFame: number;
}

const BASIC_COMPS: readonly ComponentType[] = ['WOOD', 'METAL', 'GLASS', 'FABRIC'];
const ADVANCED_COMPS: readonly ComponentType[] = ['ROPE', 'OIL', 'SAW', 'ANIMAL'];
const SUPERIOR_COMPS: readonly ComponentType[] = ['LOCK', 'MIRROR', 'DISGUISE', 'GEAR'];

function calcTrickEndBonus(player: PlayerState, state: GameState): number {
  let bonus = 0;
  for (const slot of player.tricks) {
    const trick = getTrickDef(slot.trickId);
    if (!trick.endBonus) continue;
    const eb = trick.endBonus;
    switch (eb.type) {
      case 'TRICK_MARKERS_ON_PERF':
        bonus += countTotalMarkersOnBoard(state, player.id) * (eb.famePerUnit ?? 0);
        break;
      case 'PER_SHARD':
        bonus += player.shards * (eb.famePerUnit ?? 0);
        break;
      case 'PER_L1_TRICK':
        bonus += player.tricks.filter(t => getTrickDef(t.trickId).level === 1).length * (eb.famePerUnit ?? 0);
        break;
      case 'PER_L2_TRICK':
        bonus += player.tricks.filter(t => getTrickDef(t.trickId).level === 2).length * (eb.famePerUnit ?? 0);
        break;
      case 'PER_L3_TRICK':
        bonus += player.tricks.filter(t => getTrickDef(t.trickId).level === 3).length * (eb.famePerUnit ?? 0);
        break;
      case 'PER_APPRENTICE':
        bonus += player.characters.filter(c => c.type === 'APPRENTICE').length * (eb.famePerUnit ?? 0);
        break;
      case 'PER_3_COINS':
        bonus += Math.floor(player.coins / 3) * (eb.famePerUnit ?? 0);
        break;
      case 'PER_BASIC_COMP':
        bonus += BASIC_COMPS.filter(c => player.components[c] > 0).length * (eb.famePerUnit ?? 0);
        break;
      case 'PER_ADVANCED_COMP':
        bonus += ADVANCED_COMPS.filter(c => player.components[c] > 0).length * (eb.famePerUnit ?? 0);
        break;
      case 'PER_SUPERIOR_COMP':
        bonus += SUPERIOR_COMPS.filter(c => player.components[c] > 0).length * (eb.famePerUnit ?? 0);
        break;
      case 'ALL_SPECIALISTS': {
        const types = new Set(player.specialists);
        if (types.has('ENGINEER') && types.has('MANAGER') && types.has('ASSISTANT')) {
          bonus += eb.fame ?? 0;
        }
        break;
      }
      case 'FOUR_TRICKS':
        if (player.tricks.length >= 4) bonus += eb.fame ?? 0;
        break;
      case 'HAS_ENGINEER':
        if (player.specialists.includes('ENGINEER')) bonus += eb.fame ?? 0;
        break;
      case 'HAS_MANAGER':
        if (player.specialists.includes('MANAGER')) bonus += eb.fame ?? 0;
        break;
      case 'HAS_ASSISTANT':
        if (player.specialists.includes('ASSISTANT')) bonus += eb.fame ?? 0;
        break;
    }
  }
  return bonus;
}

export function calculateScores(state: GameState): readonly ScoreBreakdown[] {
  return state.players.map(player => {
    const shardBonus = player.shards * END_SCORING.SHARD_TO_FAME;
    const coinBonus = Math.floor(player.coins / END_SCORING.COINS_PER_FAME);
    const apprenticeCount = player.characters.filter(c => c.type === 'APPRENTICE').length;
    const specialistCount = player.specialists.length;
    const characterBonus =
      apprenticeCount * END_SCORING.APPRENTICE_FAME +
      specialistCount * END_SCORING.SPECIALIST_FAME;
    const trickEndBonus = calcTrickEndBonus(player, state);

    return {
      playerId: player.id,
      name: player.name,
      baseFame: player.fame,
      shardBonus,
      coinBonus,
      characterBonus,
      trickEndBonus,
      totalFame: player.fame + shardBonus + coinBonus + characterBonus + trickEndBonus,
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
    s = addLog(s, `${score.name} 최종점수: ${score.totalFame} (기본: ${score.baseFame}, 샤드: +${score.shardBonus}, 코인: +${score.coinBonus}, 캐릭터: +${score.characterBonus}, 트릭: +${score.trickEndBonus})`);
  }

  // Determine winner
  const sorted = [...scores].sort((a, b) => b.totalFame - a.totalFame);
  const winnerIdx = state.players.findIndex(p => p.id === sorted[0].playerId);
  s = { ...s, winner: winnerIdx };

  return s;
}
