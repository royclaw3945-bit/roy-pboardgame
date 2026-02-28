// Trickerion Engine - Core (init, round flow, utilities)
import * as DATA from '../gameData.js';
export function createCoreMethods(engine) {
  return {
    initGame(playerConfigs) {
      const numPlayers = playerConfigs.length;
      const perfDeck = engine.createPerformanceDeck();
      const cardsByDay = engine.dealInitialCards(perfDeck, numPlayers);
      engine.state = {
        round: 1, phase: 'SETUP', numPlayers,
        players: playerConfigs.map((config, i) => engine.createPlayer(config, i, numPlayers)),
        initiativeOrder: playerConfigs.map((_, i) => i),
        currentPlayerIdx: 0, currentCharacterIdx: 0,
        turnQueue: [], currentTurnIdx: 0,
        downtownDice: { residence: [], inn: [], bank: [] },
        marketRow: { stock: ['WOOD', 'METAL', 'GLASS', 'FABRIC'], orders: [], quickOrder: null },
        theater: { performanceCardsByDay: cardsByDay, performanceDeck: perfDeck,
          characterSlots: engine.createTheaterSlots() },
        darkAlley: { specialDecks: {
          DOWNTOWN: engine.shuffleArray(engine.createSpecialDeck('DOWNTOWN')),
          MARKET_ROW: engine.shuffleArray(engine.createSpecialDeck('MARKET_ROW')),
          WORKSHOP: engine.shuffleArray(engine.createSpecialDeck('WORKSHOP')),
          THEATER: engine.shuffleArray(engine.createSpecialDeck('THEATER'))
        }},
        trickDecks: { MECHANICAL: [...DATA.TRICKS.MECHANICAL], OPTICAL: [...DATA.TRICKS.OPTICAL],
          ESCAPE: [...DATA.TRICKS.ESCAPE], SPIRITUAL: [...DATA.TRICKS.SPIRITUAL] },
        locationSlots: engine.createLocationSlots(numPlayers),
        pendingActions: [], log: [], gameOver: false, winner: null
      };
      engine.state.players.forEach((p, i) => { p.coins += DATA.STARTING_SETUP.extraCoinsByPosition[i] || 0; });
      engine.state.players.forEach(p => {
        p.tricks.forEach(trick => { if (engine.canPrepareTrick(p, trick)) engine.prepareTrickFree(p, trick); });
      });
      engine.addLog('게임이 시작되었습니다!');
      engine.startRound();
      return engine.state;
    },
    createPlayer(config, index, numPlayers) {
      const magician = DATA.MAGICIANS.find(m => m.id === config.magicianId);
      const startingTrick = engine.getStartingTrick(magician.favoriteCategory, config.startingTrickId);
      const startingComponents = config.startingComponents || ['WOOD', 'WOOD'];
      const player = {
        id: index, name: config.name, magician, color: magician.color,
        fame: DATA.STARTING_SETUP.fame, coins: DATA.STARTING_SETUP.coins, shards: DATA.STARTING_SETUP.shards,
        tricks: startingTrick ? [{ ...startingTrick, symbolMarker: index, trickMarkers: 0, prepared: false }] : [],
        components: {},
        characters: [
          { type: 'MAGICIAN', ap: 3, assigned: false, location: null, placed: false },
          { type: 'APPRENTICE', ap: 1, assigned: false, location: null, placed: false }
        ],
        specialists: [],
        assignmentCards: { permanent: Object.entries(DATA.ASSIGNMENT_CARD_TYPES).flatMap(([loc, info]) => Array(info.count).fill(loc)), special: [] },
        assignedCards: [], hasAdvertised: false, trickMarkersInTheater: [],
        startingSpecialist: config.startingSpecialist || null, isHuman: config.isHuman !== false
      };
      startingComponents.forEach(comp => { player.components[comp] = (player.components[comp] || 0) + 1; });
      if (config.startingSpecialist) {
        const specType = config.startingSpecialist;
        player.specialists.push(specType);
        player.characters.push({ type: specType, ap: 2, assigned: false, location: null, placed: false });
        if (specType === 'MANAGER') {
          (config.managerComponents || ['WOOD', 'WOOD']).forEach(comp => { player.components[comp] = (player.components[comp] || 0) + 1; });
        } else if (specType === 'ENGINEER') {
          const bonusTrick = engine.getAnyLevel1Trick(config.engineerTrickId);
          if (bonusTrick) player.tricks.push({ ...bonusTrick, symbolMarker: index, trickMarkers: 0, prepared: false });
        } else if (specType === 'ASSISTANT') {
          player.characters.push({ type: 'APPRENTICE', ap: 1, assigned: false, location: null, placed: false });
        }
      }
      return player;
    },
    getStartingTrick(category, trickId) {
      const tricks = DATA.TRICKS[category].filter(t => t.level === 1);
      return trickId ? tricks.find(t => t.id === trickId) : tricks[0];
    },
    getAnyLevel1Trick(trickId) {
      const cats = Object.values(DATA.TRICKS);
      if (trickId) { for (const c of cats) { const t = c.find(x => x.id === trickId && x.level === 1); if (t) return t; } }
      for (const c of cats) { const t = c.find(x => x.level === 1); if (t) return t; }
      return null;
    },
    createPerformanceDeck() {
      const deck = [['RIVERSIDE', 'RS'], ['GRAND_MAGORIAN', 'GM'], ['MAGNUS_PANTHEON', 'MP']].flatMap(
        ([key, px]) => [0, 1].map(i => ({ ...DATA.PERFORMANCE_CARDS[key], id: `${px}${i}`, tricks: [{}, {}, {}, {}] }))
      );
      return engine.shuffleArray(deck);
    },
    dealInitialCards(perfDeck, np) {
      const days = ['FRIDAY', 'SATURDAY', 'SUNDAY'], by = { THURSDAY: null, FRIDAY: null, SATURDAY: null, SUNDAY: null };
      for (let i = 0; i < Math.max(np - 1, 1) && i < days.length && perfDeck.length; i++) by[days[i]] = perfDeck.shift();
      return by;
    },
    createTheaterSlots() {
      const s = {}; DATA.WEEKDAYS.forEach(d => { s[d] = { backstage: [null, null], performance: null }; }); return s;
    },
    createLocationSlots(numPlayers) {
      const slots = {};
      for (const [loc, slotDefs] of Object.entries(DATA.LOCATION_SLOTS)) {
        const maxRow = numPlayers <= 2 ? 1 : numPlayers <= 3 ? 2 : 3;
        slots[loc] = slotDefs.filter(s => s.row <= maxRow).map(s => ({ ...s, occupant: null }));
      }
      return slots;
    },
    createSpecialDeck(location) {
      const fx = { DOWNTOWN: ['+1 코인', '재굴림', '-1 AP'], MARKET_ROW: ['-1 코인', '무료 주문', '+1 구매'],
        WORKSHOP: ['-1 AP', '+1 마커', '무료 이동'], THEATER: ['+1 명성', '+1 셋업', '+2 코인'] };
      const list = fx[location] || fx.DOWNTOWN;
      return Array.from({ length: 12 }, (_, i) => ({
        id: `SA_${location}_${i}`, location, bonusAP: 1,
        name: `${DATA.LOCATIONS[location]?.name || location} 특수임무 ${i + 1}`,
        effect: list[Math.floor(Math.random() * list.length)]
      }));
    },
    // Round flow
    startRound() {
      engine.addLog(`\n===== 라운드 ${engine.state.round} =====`);
      engine.state.phase = 'ROLL_DICE';
      engine.rollDice();
      engine.setInitiativeOrder();
      engine.state.phase = 'ADVERTISE';
      engine.emit('phaseChange', { phase: 'ADVERTISE', round: engine.state.round });
    },
    rollDice() {
      const rd = (f) => [engine.rollDie(f), engine.rollDie(f)];
      engine.state.downtownDice = { residence: rd(DATA.DOWNTOWN_DICE.DAHLGAARD), inn: rd(DATA.DOWNTOWN_DICE.INN), bank: rd(DATA.DOWNTOWN_DICE.BANK) };
      engine.emit('diceRolled', engine.state.downtownDice);
    },
    rollDie(faces) { return faces[Math.floor(Math.random() * faces.length)]; },
    setInitiativeOrder() {
      if (engine.state.round === 1) return;
      const sorted = [...engine.state.players].sort((a, b) => a.fame - b.fame).map(p => p.id);
      engine.state.initiativeOrder = sorted;
    },
    advertise(playerId) {
      const player = engine.state.players[playerId];
      const cost = DATA.ADVERTISE_COST[engine.state.initiativeOrder.indexOf(playerId)];
      if (player.coins < cost) return { success: false, message: '코인이 부족합니다.' };
      player.coins -= cost; player.fame += DATA.ADVERTISE_FAME; player.hasAdvertised = true;
      engine.addLog(`${player.name}: ${cost}코인 광고 → +${DATA.ADVERTISE_FAME}명성`);
      engine.emit('advertised', { playerId, cost });
      return { success: true };
    },
    skipAdvertise(playerId) { engine.state.players[playerId].hasAdvertised = false; },
    finishAdvertisePhase() {
      engine.state.phase = 'ASSIGNMENT';
      engine.state.players.forEach(p => { p.characters.forEach(c => { c.assigned = false; c.location = null; c.placed = false; }); p.assignedCards = []; });
      engine.emit('phaseChange', { phase: 'ASSIGNMENT' });
    },
    // Assignment
    assignCharacter(playerId, characterIdx, location) {
      const player = engine.state.players[playerId];
      const character = player.characters[characterIdx];
      if (!character) return { success: false, message: '유효하지 않은 캐릭터입니다.' };
      if (character.assigned) return { success: false, message: '이미 배정된 캐릭터입니다.' };
      // Check if player has a matching assignment card available
      const usedIndices = player.assignedCards.map(ac => ac.cardIdx);
      const cardIdx = player.assignmentCards.permanent.findIndex(
        (c, i) => c === location && !usedIndices.includes(i)
      );
      // Also check special assignment cards
      const specialIdx = cardIdx === -1 ? player.assignmentCards.special.findIndex(
        (c, i) => !usedIndices.includes(i + player.assignmentCards.permanent.length)
      ) : -1;
      if (cardIdx === -1 && specialIdx === -1) return { success: false, message: `${DATA.LOCATIONS[location]?.name || location} 배정 카드가 부족합니다.` };
      const usedCard = cardIdx !== -1 ? cardIdx : specialIdx + player.assignmentCards.permanent.length;
      character.assigned = true;
      character.location = location;
      player.assignedCards.push({ characterIdx, location, cardIdx: usedCard });
      engine.addLog(`${player.name}: ${DATA.CHARACTER_TYPES[character.type].name}을(를) ${DATA.LOCATIONS[location]?.name || location}에 배정`);
      return { success: true };
    },
    unassignCharacter(playerId, characterIdx) {
      const player = engine.state.players[playerId];
      const character = player.characters[characterIdx];
      if (!character || !character.assigned) return { success: false, message: '배정되지 않은 캐릭터입니다.' };
      character.assigned = false;
      character.location = null;
      player.assignedCards = player.assignedCards.filter(ac => ac.characterIdx !== characterIdx);
      return { success: true };
    },
    finishAssignmentPhase() {
      engine.state.phase = 'PLACE_CHARACTERS';
      engine.buildTurnQueue();
      engine.state.currentTurnIdx = 0;
      // Place first character into slot
      const first = engine.getCurrentTurn();
      if (first) engine.placeCharacterInSlot(first.playerId, first.characterIdx);
      engine.emit('phaseChange', { phase: 'PLACE_CHARACTERS' });
    },
    // Turn Queue - alternating placement order
    buildTurnQueue() {
      const s = engine.state;
      const queue = [];
      const playerChars = s.initiativeOrder.map(pid => {
        return s.players[pid].characters
          .map((c, idx) => ({ ...c, charIdx: idx, playerId: pid }))
          .filter(c => c.assigned && !c.placed);
      });
      const maxChars = Math.max(...playerChars.map(pc => pc.length), 0);
      for (let round = 0; round < maxChars; round++) {
        for (let i = 0; i < s.initiativeOrder.length; i++) {
          if (round < playerChars[i].length) {
            const ch = playerChars[i][round];
            queue.push({ playerId: ch.playerId, characterIdx: ch.charIdx });
          }
        }
      }
      s.turnQueue = queue;
      if (queue.length > 0) s.currentPlayerIdx = s.initiativeOrder.indexOf(queue[0].playerId);
      engine.addLog(`턴 순서가 설정되었습니다. (${queue.length}턴)`);
    },
    getCurrentTurn() { const s = engine.state; return s.currentTurnIdx >= s.turnQueue.length ? null : s.turnQueue[s.currentTurnIdx]; },
    advanceTurnQueue() {
      const s = engine.state; s.currentTurnIdx++;
      if (s.currentTurnIdx >= s.turnQueue.length) return { done: true };
      const next = s.turnQueue[s.currentTurnIdx];
      s.currentPlayerIdx = s.initiativeOrder.indexOf(next.playerId);
      engine.placeCharacterInSlot(next.playerId, next.characterIdx);
      engine.emit('turnAdvanced', next);
      return { done: false, next };
    },
    passCharacter(playerId, charIdx) {
      const p = engine.state.players[playerId], c = p?.characters[charIdx];
      if (!c) return { success: false, message: '유효하지 않은 캐릭터입니다.' };
      c.placed = true; c.ap = 0;
      engine.addLog(`${p.name}: ${DATA.CHARACTER_TYPES[c.type].name} 패스`);
      engine.emit('characterPassed', { playerId, charIdx });
      return { success: true };
    },
    // End turn
    executeEndTurn() {
      engine.state.phase = 'END_TURN';
      engine.addLog('\n--- 턴 종료 ---');
      engine.state.players.forEach(player => {
        let wages = 0;
        const hasAssistant = player.specialists.includes('ASSISTANT');
        let apprenticeExempted = false;
        player.characters.forEach(char => {
          if (char.type === 'MAGICIAN') return;
          if (hasAssistant && !apprenticeExempted && char.type === 'APPRENTICE') { apprenticeExempted = true; return; }
          wages += DATA.CHARACTER_TYPES[char.type].wage;
        });
        if (player.coins >= wages) { player.coins -= wages; engine.addLog(`${player.name}: 급여 ${wages}코인 지급${hasAssistant ? ' (견습생1 면제)' : ''}`); }
        else {
          const deficit = wages - player.coins; player.coins = 0;
          engine.adjustFame(player, -(deficit * DATA.WAGES.UNPAID_PENALTY));
          engine.addLog(`${player.name}: 급여 부족! ${deficit}코인 미지급 → -${deficit * DATA.WAGES.UNPAID_PENALTY}명성`);
        }
        player.characters.forEach(char => {
          char.assigned = false; char.location = null; char.placed = false;
          char.ap = DATA.CHARACTER_TYPES[char.type].baseAP; char.slotApMod = 0; char.shardConverted = false;
        });
        player.hasAdvertised = false;
        player.assignedCards = [];
      });
      engine.state.marketRow.orders.forEach(c => { if (!engine.state.marketRow.stock.includes(c)) engine.state.marketRow.stock.push(c); });
      engine.state.marketRow.orders = []; engine.state.marketRow.quickOrder = null;
      engine.movePerformanceCards();
      engine.state.theater.characterSlots = engine.createTheaterSlots();
      engine.state.locationSlots = engine.createLocationSlots(engine.state.numPlayers);
      engine.state.round++;
      if (engine.state.round > DATA.GAME_CONFIG.BASE_ROUNDS) engine.endGame();
      else engine.startRound();
    },
    movePerformanceCards() {
      const t = engine.state.theater;
      // 목요일 카드 버림 → 마커만 반환 (추가 수익 없음)
      const removed = t.performanceCardsByDay.THURSDAY;
      if (removed) removed.tricks.forEach(slot => {
        if (!slot || slot.playerId === undefined) return;
        const owner = engine.state.players[slot.playerId]; if (!owner) return;
        const pt = owner.tricks.find(tr => tr.id === slot.trickId);
        if (pt) pt.trickMarkers = Math.min(pt.trickMarkers + 1, pt.markerSlots);
      });
      // 금→목, 토→금, 일→토 이동
      t.performanceCardsByDay.THURSDAY = t.performanceCardsByDay.FRIDAY;
      t.performanceCardsByDay.FRIDAY = t.performanceCardsByDay.SATURDAY;
      t.performanceCardsByDay.SATURDAY = t.performanceCardsByDay.SUNDAY;
      // 일요일에 덱에서 새 카드
      t.performanceCardsByDay.SUNDAY = t.performanceDeck.length > 0 ? t.performanceDeck.shift() : null;
    },
    // Utility
    adjustFame(player, delta) {
      player.fame = Math.max(0, player.fame + delta);
    },
    shuffleArray(arr) {
      const shuffled = [...arr];
      for (let i = shuffled.length - 1; i > 0; i--) {
        const j = Math.floor(Math.random() * (i + 1));
        [shuffled[i], shuffled[j]] = [shuffled[j], shuffled[i]];
      }
      return shuffled;
    },
    addLog(message) {
      engine.state.log.push({ time: Date.now(), message });
      engine.emit('log', message);
    },
    getState() { return { ...engine.state }; }
  };
}
