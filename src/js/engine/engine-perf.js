// Trickerion Engine - Performance Phase & End Game
import * as DATA from '../gameData.js';

export function createPerfMethods(engine) {
  return {
    executePerformancePhase() {
      engine.state.phase = 'PERFORMANCE';
      engine.addLog('\n--- 퍼포먼스 페이즈 ---');
      DATA.WEEKDAYS.forEach(day => {
        const slots = engine.state.theater.characterSlots[day];
        if (slots.performance === null || slots.performance === undefined) return;
        const performerId = slots.performance;
        const performer = engine.state.players[performerId];
        if (!performer) return;
        const card = engine.state.theater.performanceCardsByDay[day];
        if (!card) { engine.addLog(`${performer.name}: ${DATA.WEEKDAY_MODIFIERS[day].name} 카드 없음`); return; }
        engine.performTricks(performer, card, day);
      });
      engine.emit('phaseChange', { phase: 'PERFORMANCE' });
    },

    performTricks(performer, perfCard, day) {
      const modifier = DATA.WEEKDAY_MODIFIERS[day];
      let totalFame = 0, totalCoins = 0, totalShards = 0;
      engine.addLog(`\n${performer.name}의 ${modifier.name} 공연 (${perfCard.name}):`);

      perfCard.tricks.forEach((slot, idx) => {
        if (!slot || slot.playerId === undefined) return;
        const trick = engine.findTrickById(slot.trickId);
        if (!trick) return;
        const owner = engine.state.players[slot.playerId];
        const fame = Math.max(0, trick.yields.fame + modifier.fameMod);
        const coins = Math.max(0, trick.yields.coins + modifier.coinMod);
        const shards = trick.yields.shards;
        owner.fame += fame;
        owner.coins += coins;
        owner.shards += shards;
        engine.addLog(`  "${trick.name}" (${owner.name}): +${fame} 명성, +${coins} 코인, +${shards} 샤드`);
        if (slot.playerId === performer.id) {
          totalFame += fame;
          totalCoins += coins;
          totalShards += shards;
        }
      });

      // Performer bonuses
      const linkCount = engine.countPerformerLinks(perfCard, performer.id);
      performer.fame += linkCount;
      totalFame += linkCount;
      if (linkCount > 0) engine.addLog(`  링크 보너스: +${linkCount} 명성`);

      performer.fame += perfCard.performerBonus.fame;
      performer.coins += perfCard.performerBonus.coins;
      performer.shards += perfCard.performerBonus.shards;
      engine.addLog(`  카드 보너스: +${perfCard.performerBonus.fame} 명성, +${perfCard.performerBonus.coins} 코인`);

      performer.characters.forEach(char => {
        if (char.location === 'THEATER' && char.type !== 'MAGICIAN') {
          const bonus = DATA.SPECIALIST_THEATER_BONUS[char.type];
          if (bonus) {
            performer.fame += bonus.fame;
            performer.coins += bonus.coins;
            performer.shards += bonus.shards;
          }
        }
      });

      // Return trick markers to owners after performance
      perfCard.tricks = perfCard.tricks.map(slot => {
        if (slot && slot.playerId !== undefined) {
          const owner = engine.state.players[slot.playerId];
          if (owner) {
            const playerTrick = owner.tricks.find(t => t.id === slot.trickId);
            if (playerTrick) {
              playerTrick.trickMarkers++;
              if (playerTrick.trickMarkers >= playerTrick.markerSlots) {
                playerTrick.trickMarkers = playerTrick.markerSlots;
              }
            }
          }
          return {};
        }
        return slot;
      });

      engine.emit('performanceResult', {
        performerId: performer.id, card: perfCard, day,
        totalFame, totalCoins, totalShards
      });
      engine.emit('performanceDone', {
        performerId: performer.id, card: perfCard, day
      });
    },

    countPerformerLinks(perfCard, performerId) {
      let links = 0;
      for (let i = 0; i < perfCard.tricks.length; i++) {
        const slot = perfCard.tricks[i];
        if (!slot || slot.playerId !== performerId) continue;
        engine.getAdjacentSlots(i).forEach(adjIdx => {
          const adj = perfCard.tricks[adjIdx];
          if (adj && adj.playerId !== undefined && adj.category === slot.category) links++;
        });
      }
      return Math.floor(links / 2);
    },

    // End Game
    endGame() {
      engine.state.gameOver = true;
      engine.state.phase = 'GAME_OVER';
      engine.addLog('\n===== 게임 종료 =====');

      const scoring = DATA.END_GAME_SCORING.BASE;
      engine.state.players.forEach(player => {
        let bonusFame = 0;
        // Shards: 1 per shard
        const shardFame = player.shards * scoring.SHARD_TO_FAME;
        bonusFame += shardFame;
        // Coins: 1 per 3 coins
        const coinFame = Math.floor(player.coins / scoring.COINS_PER_FAME);
        bonusFame += coinFame;
        // Apprentices: 2 per apprentice
        const apprentices = player.characters.filter(c => c.type === 'APPRENTICE').length;
        bonusFame += apprentices * scoring.APPRENTICE_FAME;
        // Specialists: 3 per specialist
        bonusFame += player.specialists.length * scoring.SPECIALIST_FAME;
        // L3 trick end-game bonuses
        let trickBonusFame = 0;
        player.tricks.forEach(trick => {
          if (trick.level === 3 && trick.endBonus) {
            const tb = engine.calculateEndBonus(player, trick);
            trickBonusFame += tb;
            if (tb > 0) engine.addLog(`  ${trick.name}: +${tb} 명성 (${trick.endBonus.desc})`);
          }
        });
        bonusFame += trickBonusFame;
        player.fame += bonusFame;
        engine.addLog(`${player.name}: +${bonusFame} 보너스 (샤드${shardFame} 코인${coinFame} 캐릭터${apprentices * 2 + player.specialists.length * 3}${trickBonusFame > 0 ? ` L3트릭${trickBonusFame}` : ''}) → 총 ${player.fame}`);
      });

      const initOrder = engine.state.initiativeOrder;
      const sorted = [...engine.state.players].sort((a, b) => {
        if (b.fame !== a.fame) return b.fame - a.fame;
        // Tiebreaker: earlier in initiative order wins
        return initOrder.indexOf(a.id) - initOrder.indexOf(b.id);
      });
      engine.state.winner = sorted[0];
      engine.addLog(`\n승자: ${engine.state.winner.name} (${engine.state.winner.fame} 명성)`);
      engine.emit('gameOver', { winner: engine.state.winner, rankings: sorted });
    },

    calculateEndBonus(player, trick) {
      const b = trick.endBonus;
      if (!b) return 0;
      switch (b.type) {
        case 'TRICK_MARKERS_ON_PERF': {
          let markers = 0;
          engine.getPerformanceCards().forEach(card => {
            card.tricks.forEach(s => { if (s && s.playerId === player.id) markers++; });
          });
          return markers * b.famePerUnit;
        }
        case 'PER_SHARD': return player.shards * b.famePerUnit;
        case 'PER_L1_TRICK': return player.tricks.filter(t => t.level === 1).length * b.famePerUnit;
        case 'PER_L2_TRICK': return player.tricks.filter(t => t.level === 2).length * b.famePerUnit;
        case 'PER_L3_TRICK': return player.tricks.filter(t => t.level === 3).length * b.famePerUnit;
        case 'PER_APPRENTICE': return player.characters.filter(c => c.type === 'APPRENTICE').length * b.famePerUnit;
        case 'PER_3_COINS': return Math.floor(player.coins / 3) * b.famePerUnit;
        case 'PER_BASIC_COMP': {
          const types = ['WOOD', 'METAL', 'GLASS', 'FABRIC'];
          return types.filter(t => (player.components[t] || 0) > 0).length * b.famePerUnit;
        }
        case 'PER_ADVANCED_COMP': {
          const types = ['ROPE', 'OIL', 'SAW', 'ANIMAL'];
          return types.filter(t => (player.components[t] || 0) > 0).length * b.famePerUnit;
        }
        case 'PER_SUPERIOR_COMP': {
          const types = ['LOCK', 'MIRROR', 'DISGUISE', 'GEAR'];
          return types.filter(t => (player.components[t] || 0) > 0).length * b.famePerUnit;
        }
        case 'ALL_SPECIALISTS': {
          const has = player.specialists;
          return (has.includes('ENGINEER') && has.includes('MANAGER') && has.includes('ASSISTANT')) ? b.fame : 0;
        }
        case 'FOUR_TRICKS': return player.tricks.length >= 4 ? b.fame : 0;
        case 'HAS_ENGINEER': return player.specialists.includes('ENGINEER') ? b.fame : 0;
        case 'HAS_MANAGER': return player.specialists.includes('MANAGER') ? b.fame : 0;
        case 'HAS_ASSISTANT': return player.specialists.includes('ASSISTANT') ? b.fame : 0;
        default: return 0;
      }
    }
  };
}
