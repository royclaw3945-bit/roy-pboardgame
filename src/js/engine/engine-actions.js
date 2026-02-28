import * as DATA from '../gameData.js';
export function createActionMethods(engine) {
  return {
    getAvailableActions(playerId, characterIdx) {
      const player = engine.state.players[playerId];
      const character = player.characters[characterIdx];
      if (!character || !character.location) return [];
      const loc = character.location, ap = engine.getCharacterAP(player, character), actions = [];
      switch (loc) {
        case 'DOWNTOWN':
          if (ap >= 3) {
            actions.push({ type: 'LEARN_TRICK', cost: 3, name: '트릭 배우기' });
            actions.push({ type: 'TAKE_COINS', cost: 3, name: '코인 받기' });
            actions.push({ type: 'HIRE_CHARACTER', cost: 3, name: '캐릭터 고용' });
          }
          if (ap >= 1) actions.push({ type: 'REROLL_DIE', cost: 1, name: '주사위 재굴림' });
          if (ap >= 2) actions.push({ type: 'CHOOSE_DIE', cost: 2, name: '주사위 결과 선택' });
          break;
        case 'MARKET_ROW':
          if (ap >= 1) {
            actions.push({ type: 'BUY', cost: 1, name: '컴포넌트 구매' });
            actions.push({ type: 'BARGAIN', cost: 1, name: '흥정 구매 (-1코인)' });
            actions.push({ type: 'ORDER', cost: 1, name: '컴포넌트 주문' });
          }
          if (ap >= 2) actions.push({ type: 'QUICK_ORDER', cost: 2, name: '긴급 주문' });
          break;
        case 'WORKSHOP':
          player.tricks.forEach((trick, idx) => {
            if (!trick.prepared && ap >= trick.prepareCost)
              actions.push({ type: 'PREPARE', cost: trick.prepareCost, name: `${trick.name} 준비`, trickIdx: idx });
          });
          if (ap >= 1 && player.specialists.includes('ENGINEER'))
            actions.push({ type: 'MOVE_TRICK', cost: 1, name: '트릭 마커 이동' });
          if (ap >= 1 && player.specialists.includes('MANAGER'))
            actions.push({ type: 'MOVE_COMPONENT', cost: 1, name: '컴포넌트 이동' });
          if (ap >= 1 && player.specialists.includes('ASSISTANT'))
            actions.push({ type: 'MOVE_APPRENTICE', cost: 1, name: '견습생 이동' });
          break;
        case 'THEATER':
          if (ap >= 1) {
            const cards = engine.getPerformanceCards();
            const hasOpenSlot = cards.some(c => c.tricks.some(t => !t || t.playerId === undefined));
            const hasPreparedTrick = player.tricks.some(t => t.prepared && t.trickMarkers > 0);
            if (hasOpenSlot && hasPreparedTrick) actions.push({ type: 'SETUP_TRICK', cost: 1, name: '트릭 셋업' });
            actions.push({ type: 'RESCHEDULE', cost: 1, name: '일정 변경' });
          }
          break;
        case 'DARK_ALLEY':
          if (ap >= 1) actions.push({ type: 'DRAW_SPECIAL', cost: 1, name: '특수 임무 획득' });
          if (ap >= 2) actions.push({ type: 'DRAW_MORE', cost: 2, name: '추가 특수 임무' });
          if (ap >= 1) actions.push({ type: 'FORTUNE_TELLING', cost: 1, name: '점술' });
          break;
      }
      return actions;
    },
    getCharacterAP(player, character) {
      const base = character.ap;
      const slotMod = character.slotApMod || 0;
      return base + slotMod;
    },
    placeCharacterInSlot(playerId, charIdx) {
      const player = engine.state.players[playerId];
      const character = player.characters[charIdx];
      if (!character || !character.location || character.location === 'WORKSHOP') {
        character.slotApMod = 0; return;
      }
      const locSlots = engine.state.locationSlots[character.location];
      if (!locSlots) { character.slotApMod = 0; return; }
      const bestSlot = locSlots.filter(s => !s.occupant).sort((a, b) => b.apMod - a.apMod)[0];
      if (bestSlot) {
        bestSlot.occupant = { playerId, charIdx };
        character.slotApMod = bestSlot.apMod;
      } else { character.slotApMod = 0; }
    },
    convertShardToAP(playerId, charIdx) {
      const player = engine.state.players[playerId];
      const character = player.characters[charIdx];
      if (!character || character.location === 'THEATER') return { success: false, message: '극장에서는 샤드→AP 변환 불가' };
      if (player.shards <= 0) return { success: false, message: '샤드가 부족합니다.' };
      if (character.shardConverted) return { success: false, message: '이번 배치에서 이미 변환했습니다.' };
      player.shards--;
      character.ap++;
      character.shardConverted = true;
      engine.addLog(`${player.name}: 1 샤드 → +1 AP (${DATA.CHARACTER_TYPES[character.type].name})`);
      return { success: true };
    },
    executeAction(playerId, action) {
      const player = engine.state.players[playerId];
      const h = {
        LEARN_TRICK: () => engine.actionLearnTrick(player, action),
        TAKE_COINS: () => engine.actionTakeCoins(player, action),
        HIRE_CHARACTER: () => engine.actionHireCharacter(player, action),
        REROLL_DIE: () => engine.actionRerollDie(player, action),
        CHOOSE_DIE: () => engine.actionChooseDie(player, action),
        BUY: () => engine.actionBuy(player, action),
        BARGAIN: () => engine.actionBuy(player, { ...action, discount: 1 }),
        ORDER: () => engine.actionOrder(player, action),
        QUICK_ORDER: () => engine.actionQuickOrder(player, action),
        PREPARE: () => engine.actionPrepare(player, action),
        MOVE_TRICK: () => engine.actionMoveTrick(player, action),
        MOVE_COMPONENT: () => engine.actionMoveComponent(player, action),
        MOVE_APPRENTICE: () => engine.actionMoveApprentice(player, action),
        SETUP_TRICK: () => engine.actionSetupTrick(player, action),
        RESCHEDULE: () => engine.actionReschedule(player, action),
        DRAW_SPECIAL: () => engine.actionDrawSpecial(player, action),
        DRAW_MORE: () => engine.actionDrawMore(player, action),
        FORTUNE_TELLING: () => engine.actionFortuneTelling(player),
      };
      const handler = h[action.type];
      return handler ? handler() : { success: false, message: '알 수 없는 액션입니다.' };
    },
    getAllowedTrickCategories(player) {
      const allowed = new Set([player.magician.favoriteCategory]);
      for (const d of engine.state.downtownDice.residence) {
        if (d === 'ANY') return new Set(['MECHANICAL', 'OPTICAL', 'ESCAPE', 'SPIRITUAL']);
        if (d !== 'X' && DATA.TRICK_CATEGORIES[d]) allowed.add(d);
      }
      return allowed;
    },
    actionLearnTrick(player, action) {
      const { category, trickId } = action;
      if (!category || !trickId) return { success: false, message: '트릭을 선택해주세요.' };
      if (player.tricks.length >= DATA.STARTING_SETUP.maxTricks) return { success: false, message: '트릭 슬롯이 가득 찼습니다. (최대 4개)' };
      if (!engine.getAllowedTrickCategories(player).has(category)) return { success: false, message: '현재 주사위 결과로 배울 수 없는 카테고리입니다.' };
      const deck = engine.state.trickDecks[category], ti = deck.findIndex(t => t.id === trickId);
      if (ti === -1) return { success: false, message: '해당 트릭을 찾을 수 없습니다.' };
      const trick = deck[ti];
      if (player.fame < trick.fameThreshold) {
        const diff = trick.fameThreshold - player.fame;
        if (player.coins < diff) return { success: false, message: `명성 부족 (${diff} 코인 필요)` };
        player.coins -= diff;
      }
      deck.splice(ti, 1);
      player.tricks.push({ ...trick, symbolMarker: player.id, trickMarkers: 0, prepared: false });
      engine.addLog(`${player.name}: "${trick.name}" 트릭 학습`);
      engine.emit('trickLearned', { playerId: player.id, trick });
      return { success: true, trick };
    },
    actionTakeCoins(player, action) {
      const bd = engine.state.downtownDice.bank, di = action.dieIndex || 0;
      if (di >= bd.length || bd[di] === 'X') return { success: false, message: '유효한 주사위가 없습니다.' };
      player.coins += bd[di]; const amt = bd[di]; bd[di] = 'X';
      engine.addLog(`${player.name}: ${amt} 코인 획득`);
      return { success: true, amount: amt };
    },
    actionHireCharacter(player, action) {
      const innDice = engine.state.downtownDice.inn, dieIdx = action.dieIndex || 0;
      if (dieIdx >= innDice.length || innDice[dieIdx] === 'X') return { success: false, message: '유효한 주사위가 없습니다.' };
      const ct = innDice[dieIdx];
      if (['ENGINEER', 'MANAGER', 'ASSISTANT'].includes(ct)) {
        if (player.specialists.includes(ct)) return { success: false, message: '이미 보유한 전문가입니다.' };
        player.specialists.push(ct);
      }
      player.characters.push({ type: ct, ap: DATA.CHARACTER_TYPES[ct].baseAP, assigned: false, location: null, placed: false });
      innDice[dieIdx] = 'X';
      engine.addLog(`${player.name}: ${DATA.CHARACTER_TYPES[ct].name} 고용`);
      return { success: true, character: ct };
    },
    actionRerollDie(player, action) {
      const dice = engine.state.downtownDice[action.diceGroup];
      const idx = action.dieIndex || 0;
      if (!dice || idx >= dice.length || dice[idx] === 'X') return { success: false, message: '사용된 주사위는 재굴림할 수 없습니다.' };
      const faces = { residence: DATA.DOWNTOWN_DICE.DAHLGAARD, inn: DATA.DOWNTOWN_DICE.INN, bank: DATA.DOWNTOWN_DICE.BANK }[action.diceGroup];
      dice[idx] = engine.rollDie(faces);
      engine.addLog(`${player.name}: ${action.diceGroup} 주사위 재굴림`); return { success: true };
    },
    actionChooseDie(player, action) {
      if (!action.diceGroup || action.dieIndex === undefined) return { success: false, message: '주사위 정보를 지정해주세요.' };
      const dice = engine.state.downtownDice[action.diceGroup];
      if (!dice || action.dieIndex >= dice.length || dice[action.dieIndex] === 'X') return { success: false, message: '사용된 주사위는 선택할 수 없습니다.' };
      const validFaces = { residence: DATA.DOWNTOWN_DICE.DAHLGAARD, inn: DATA.DOWNTOWN_DICE.INN, bank: DATA.DOWNTOWN_DICE.BANK }[action.diceGroup];
      if (validFaces && !validFaces.includes(action.value)) return { success: false, message: '유효하지 않은 주사위 값입니다.' };
      dice[action.dieIndex] = action.value;
      engine.addLog(`${player.name}: ${action.diceGroup} 주사위 → ${action.value}`); return { success: true };
    },
    actionBuy(player, action) {
      const { componentType, quantity = 1, discount = 0 } = action, mr = engine.state.marketRow;
      if (!mr.stock.includes(componentType) && mr.quickOrder !== componentType) return { success: false, message: '시장에 없는 컴포넌트입니다.' };
      const comp = DATA.COMPONENT_TYPES[componentType];
      if (!comp) return { success: false, message: '잘못된 컴포넌트입니다.' };
      const cost = Math.max(0, comp.cost * quantity - discount), cur = player.components[componentType] || 0;
      if (player.coins < cost) return { success: false, message: '코인이 부족합니다.' };
      if (cur + quantity > DATA.STARTING_SETUP.maxComponentsPerType) return { success: false, message: '보관 한도 초과 (최대 3개)' };
      player.coins -= cost; player.components[componentType] = cur + quantity;
      engine.addLog(`${player.name}: ${comp.name} ${quantity}개 ${cost}코인 ${discount > 0 ? '흥정' : '구매'}`);
      return { success: true };
    },
    actionOrder(player, action) {
      const { componentType } = action;
      if (engine.state.marketRow.orders.includes(componentType)) return { success: false, message: '이미 주문된 컴포넌트입니다.' };
      if (engine.state.marketRow.orders.length >= 4) return { success: false, message: '주문 슬롯이 가득 찼습니다.' };
      engine.state.marketRow.orders.push(componentType);
      engine.addLog(`${player.name}이(가) ${DATA.COMPONENT_TYPES[componentType].name}을(를) 주문했습니다.`);
      return { success: true };
    },
    actionQuickOrder(p, a) { engine.state.marketRow.quickOrder = a.componentType; engine.addLog(`${p.name}: ${DATA.COMPONENT_TYPES[a.componentType].name} 긴급 주문`); return { success: true }; },
    actionMoveTrick(p, a) { return engine.actionReschedule(p, a); },
    actionMoveComponent(p) { engine.addLog(`${p.name}: 매니저 - 컴포넌트 이동`); return { success: true }; },
    actionMoveApprentice(p, a) {
      const c = p.characters[a.apprenticeIdx]; if (!c || c.type !== 'APPRENTICE') return { success: false, message: '견습생이 아닙니다.' };
      c.location = a.newLocation || 'WORKSHOP'; engine.addLog(`${p.name}: 견습생 이동 → ${DATA.LOCATIONS[c.location]?.name || c.location}`); return { success: true };
    },
    actionPrepare(player, action) {
      const trick = player.tricks[action.trickIdx];
      if (!trick) return { success: false, message: '트릭을 찾을 수 없습니다.' };
      if (trick.prepared) return { success: false, message: '이미 준비된 트릭입니다.' };
      if (!engine.canPrepareTrick(player, trick)) return { success: false, message: '컴포넌트 부족' };
      trick.prepared = true; trick.trickMarkers = trick.markerSlots;
      const engInWorkshop = player.characters.some(c => c.type === 'ENGINEER' && c.location === 'WORKSHOP');
      if (engInWorkshop) { trick.trickMarkers++; engine.addLog(`  기술자 보너스: +1 트릭 마커`); }
      engine.addLog(`${player.name}: "${trick.name}" 준비 완료 (마커 ${trick.trickMarkers}개)`);
      return { success: true };
    },
    canPrepareTrick(player, trick) {
      return Object.entries(trick.components).every(([comp, count]) => (player.components[comp] || 0) >= count);
    },
    prepareTrickFree(player, trick) { trick.prepared = true; trick.trickMarkers = trick.markerSlots; },
    actionSetupTrick(player, action) {
      const { trickIdx, perfCardIdx, slotIdx } = action;
      const trick = player.tricks[trickIdx];
      if (!trick) return { success: false, message: '트릭을 찾을 수 없습니다.' };
      if (trick.trickMarkers <= 0) return { success: false, message: '사용 가능한 트릭 마커가 없습니다.' };
      const cards = engine.getPerformanceCards();
      const perfCard = cards[perfCardIdx];
      if (!perfCard) return { success: false, message: '퍼포먼스 카드를 찾을 수 없습니다.' };
      if (perfCard.tricks[slotIdx] && perfCard.tricks[slotIdx].playerId !== undefined) return { success: false, message: '이미 점유된 슬롯입니다.' };
      perfCard.tricks[slotIdx] = { playerId: player.id, trickId: trick.id, trickName: trick.name, category: trick.category, symbolMarker: trick.symbolMarker };
      trick.trickMarkers--;
      const links = engine.checkLinks(perfCard, slotIdx);
      const linkBonus = links > 0 ? trick.level : 0;
      engine.addLog(`${player.name}이(가) "${trick.name}"을(를) ${perfCard.name}에 셋업했습니다.`);
      if (links > 0) engine.addLog(`${player.name}: ${links}개의 링크 생성!`);
      return { success: true, links, linkBonus, perfCardIdx, slotIdx };
    },
    checkLinks(perfCard, slotIdx) {
      let links = 0;
      const placed = perfCard.tricks[slotIdx];
      if (!placed || placed.playerId === undefined) return 0;
      engine.getAdjacentSlots(slotIdx).forEach(adjIdx => {
        const adj = perfCard.tricks[adjIdx];
        if (adj && adj.playerId !== undefined && adj.category === placed.category) links++;
      });
      return links;
    },
    getAdjacentSlots(slotIdx) {
      const adjacency = { 0: [1, 2], 1: [0, 3], 2: [0, 3], 3: [1, 2] };
      return adjacency[slotIdx] || [];
    },
    actionReschedule(player, action) {
      const { fromPerfCardIdx, fromSlotIdx, toPerfCardIdx, toSlotIdx } = action;
      const cards = engine.getPerformanceCards();
      const fromCard = cards[fromPerfCardIdx], toCard = cards[toPerfCardIdx];
      if (!fromCard || !toCard) return { success: false, message: '카드를 찾을 수 없습니다.' };
      const marker = fromCard.tricks[fromSlotIdx];
      if (!marker || marker.playerId !== player.id) return { success: false, message: '자신의 트릭 마커만 이동할 수 있습니다.' };
      if (toCard.tricks[toSlotIdx] && toCard.tricks[toSlotIdx].playerId !== undefined) return { success: false, message: '목표 슬롯이 이미 점유되어 있습니다.' };
      toCard.tricks[toSlotIdx] = marker;
      fromCard.tricks[fromSlotIdx] = {};
      engine.addLog(`${player.name}이(가) 트릭 마커를 재배치했습니다.`);
      return { success: true };
    },
    actionDrawSpecial(player, action) {
      const dn = action.deck || 'DOWNTOWN', dk = engine.state.darkAlley.specialDecks[dn];
      if (!dk || dk.length < 2) return { success: false, message: '카드가 부족합니다.' };
      const drawn = [dk.shift(), dk.shift()], ci = action.chosenIdx || 0, chosen = drawn[ci];
      dk.push(drawn[1 - ci]); player.assignmentCards.special.push(chosen);
      engine.addLog(`${player.name}: "${chosen.name}" 특수 임무 획득`);
      return { success: true, cards: drawn, chosen };
    },
    actionDrawMore(player, action) { return engine.actionDrawSpecial(player, action); },
    actionFortuneTelling(player) { player.shards++; engine.addLog(`${player.name}: 점술 → +1 샤드`); return { success: true }; },
    findTrickById(trickId) {
      for (const cat of Object.values(DATA.TRICKS)) { const t = cat.find(t => t.id === trickId); if (t) return t; }
      return null;
    },
    getPerformanceCards() {
      const cards = [];
      for (const day of DATA.WEEKDAYS) {
        const card = engine.state.theater.performanceCardsByDay[day];
        if (card) cards.push({ ...card, day, _ref: engine.state.theater.performanceCardsByDay, _day: day });
      }
      return cards;
    },
    applyLinkReward(playerId, choice, amount, perfCardIdx, slotIdx) {
      const player = engine.state.players[playerId];
      if (choice === 'fame') player.fame += amount;
      else player.coins += amount;
      const cards = engine.getPerformanceCards();
      const card = cards[perfCardIdx];
      if (card?.linkCircles?.[slotIdx]?.hasShardSymbol) {
        player.shards++;
        engine.getAdjacentSlots(slotIdx).forEach(adjIdx => {
          const adj = card.tricks[adjIdx];
          if (adj?.playerId !== undefined && adj.playerId !== playerId) engine.state.players[adj.playerId].shards++;
        });
      }
      engine.addLog(`${player.name}: 링크 보상 +${amount} ${choice === 'fame' ? '명성' : '코인'}`);
    }
  };
}
