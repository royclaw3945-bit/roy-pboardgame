// Trickerion UI - User Action Handlers
import * as DATA from '../gameData.js';
import { SVG, CHAR_SVG } from './constants.js';

export function createActionHandlers(ui) {
  return {
    selectLocation(key) { ui.selectedLocation = ui.selectedLocation === key ? null : key; ui.renderAll(); },
    selectPerfCard(idx) { ui.selectedPerfCard = ui.selectedPerfCard === idx ? null : idx; ui.renderAll(); },
    selectTrickSlot(cardIdx, slotIdx) {
      ui.selectedPerfCard = cardIdx; ui.selectedSlot = slotIdx;
      if (ui.engine.state.phase === 'PLACE_CHARACTERS' && ui.pendingSetupTrick) ui.completeSetupTrick(cardIdx, slotIdx);
    },
    doUndo() {
      if (ui.engine.undo()) { ui.currentViewPlayer = ui.engine.state.currentPlayerIdx || 0; ui.notify('되돌리기 완료', 'success'); ui.renderAll(); }
      else ui.notify('되돌릴 수 없습니다', 'error');
    },
    doAdvertise() {
      ui.engine.saveState();
      const s = ui.engine.state;
      const playerId = s.initiativeOrder[s.currentPlayerIdx];
      const result = ui.engine.advertise(playerId);
      if (result.success) ui.notify('광고 완료! +2 명성', 'success');
      ui.advanceAdvertise();
    },
    doSkipAdvertise() {
      ui.engine.saveState();
      const s = ui.engine.state;
      ui.engine.skipAdvertise(s.initiativeOrder[s.currentPlayerIdx]);
      ui.advanceAdvertise();
    },
    advanceAdvertise() {
      const s = ui.engine.state;
      s.currentPlayerIdx++;
      if (s.currentPlayerIdx >= s.numPlayers) {
        s.currentPlayerIdx = 0;
        ui.engine.finishAdvertisePhase();
      }
      ui.renderAll();
    },
    doAssign(charIdx, location) {
      ui.engine.saveState();
      const s = ui.engine.state;
      const playerId = s.initiativeOrder[s.currentPlayerIdx] || 0;
      const result = ui.engine.assignCharacter(playerId, charIdx, location);
      if (result.success) ui.notify('배정 완료', 'success');
      else ui.notify(result.message, 'error');
      ui.renderAll();
    },
    doUnassign(charIdx) {
      ui.engine.saveState();
      const s = ui.engine.state;
      const playerId = s.initiativeOrder[s.currentPlayerIdx] || 0;
      const result = ui.engine.unassignCharacter(playerId, charIdx);
      if (result.success) ui.notify('배정 취소', 'success');
      ui.renderAll();
    },
    finishAssignment() {
      const s = ui.engine.state;
      s.currentPlayerIdx++;
      if (s.currentPlayerIdx >= s.numPlayers) {
        s.currentPlayerIdx = 0;
        ui.showAssignmentReveal();
      } else {
        const nextPlayerId = s.initiativeOrder[s.currentPlayerIdx];
        ui.showHandoverOverlay(s.players[nextPlayerId]);
      }
    },
    doAction(playerId, charIdx, actionData) {
      const s = ui.engine.state;
      const player = s.players[playerId];
      const action = typeof actionData === 'string' ? JSON.parse(actionData) : actionData;
      ui._activeCharIdx = charIdx;
      ui._activePlayerId = playerId;

      const modalActions = {
        LEARN_TRICK: () => ui.showTrickSelectionModal(player, action),
        TAKE_COINS: () => ui.showDieSelectionModal(player, 'bank', action),
        HIRE_CHARACTER: () => ui.showDieSelectionModal(player, 'inn', action),
        BUY: () => ui.showBuyModal(player, action),
        BARGAIN: () => ui.showBuyModal(player, { ...action, discount: 1 }),
        ORDER: () => ui.showOrderModal(player, action),
        PREPARE: () => ui.doPrepare(player, action),
        SETUP_TRICK: () => ui.showSetupTrickModal(player, action),
        DRAW_SPECIAL: () => ui.showDrawSpecialModal(player, action),
        DRAW_MORE: () => ui.showDrawSpecialModal(player, action),
        FORTUNE_TELLING: () => ui.doFortuneTelling(player, action),
        REROLL_DIE: () => ui.showRerollModal(player, action),
        CHOOSE_DIE: () => ui.showChooseDieModal(player, action),
      };
      const handler = modalActions[action.type];
      if (handler) { handler(); return; }
      ui.executeAndAdvance(player, charIdx, action);
    },

    executeAndAdvance(player, charIdx, action) {
      ui.engine.saveState();
      const result = ui.engine.executeAction(player.id, action);
      if (result.success) {
        ui.consumeAP(player, charIdx, action.cost || 0);
        ui.notify('액션 완료!', 'success');
        ui.checkAutoAdvanceTurn(player, charIdx);
      } else { ui.notify(result.message, 'error'); }
      ui.renderAll();
    },

    consumeAP(player, charIdx, cost) {
      const char = player.characters[charIdx];
      if (!char) return;
      char.ap -= cost;
      if (char.ap <= 0) { char.placed = true; char.ap = 0; }
    },
    checkAutoAdvanceTurn(player, charIdx) {
      const char = player.characters[charIdx];
      if (char && char.placed) ui.advanceTurn();
    },

    doConvertShard(playerId, charIdx) {
      ui.engine.saveState();
      const result = ui.engine.convertShardToAP(playerId, charIdx);
      if (result.success) ui.notify('샤드 → +1 AP 변환!', 'success');
      else ui.notify(result.message, 'error');
      ui.renderAll();
    },
    doPass(playerId, charIdx) {
      ui.engine.saveState();
      const result = ui.engine.passCharacter(playerId, charIdx);
      if (result.success) {
        ui.notify('패스했습니다.', 'success');
        ui.advanceTurn();
      }
      ui.renderAll();
    },

    advanceTurn() {
      const result = ui.engine.advanceTurnQueue();
      if (result.done) {
        const perfResults = [];
        ui.engine.on('performanceResult', (data) => perfResults.push(data));
        ui.engine.executePerformancePhase();
        ui.renderAll();
        ui.showPerformanceResults(perfResults);
      } else {
        ui.currentViewPlayer = result.next.playerId;
        ui.renderAll();
      }
    },

    finishPlacement() { ui.advanceTurn(); },
    doPrepare(player, action) {
      ui.executeAndAdvance(player, ui._activeCharIdx, { ...action, cost: action.cost || action.prepareCost || 1 });
    },
    doFortuneTelling(player, action) {
      ui.engine.saveState();
      const result = ui.engine.executeAction(player.id, { type: 'FORTUNE_TELLING' });
      if (result.success) { ui.consumeAP(player, ui._activeCharIdx, 1); ui.notify('점술: +1 샤드 획득!', 'success'); ui.checkAutoAdvanceTurn(player, ui._activeCharIdx); }
      else ui.notify(result.message, 'error');
      ui.renderAll();
    },

    assignToTheaterDay(day, slotType, slotIdx) {
      ui.engine.saveState();
      const s = ui.engine.state;
      if (s.phase !== 'PLACE_CHARACTERS') return;
      const ct = ui.engine.getCurrentTurn(); if (!ct) return;
      const player = s.players[ct.playerId], char = player.characters[ct.characterIdx];
      if (slotType === 'performance' && char.type !== 'MAGICIAN') { ui.notify('마법사만 공연 슬롯에 배치 가능', 'error'); return; }
      const playerDay = ui.getPlayerTheaterDay(ct.playerId, s);
      if (playerDay && playerDay !== day) { ui.notify(`이미 ${DATA.WEEKDAY_MODIFIERS[playerDay].name}에 배치됨. 같은 요일만 가능.`, 'error'); return; }
      const slots = s.theater.characterSlots[day];
      let placed = false;
      if (slotType === 'performance' && slots.performance === null) { slots.performance = ct.playerId; placed = true; }
      else if (slotType !== 'performance' && slots.backstage[slotIdx] === null) { slots.backstage[slotIdx] = ct.playerId; placed = true; }
      if (placed) {
        char.placed = true;
        ui.notify(`${DATA.WEEKDAY_MODIFIERS[day].name} ${slotType === 'performance' ? '공연' : '백스테이지'} 배치`, 'success');
        ui.engine.addLog(`${player.name}: ${DATA.CHARACTER_TYPES[char.type].name} → ${DATA.WEEKDAY_MODIFIERS[day].name} ${slotType === 'performance' ? '공연' : '백스테이지'}`);
        ui.advanceTurn();
      }
      ui.renderAll();
    },
    getPlayerTheaterDay(playerId, s) {
      for (const day of DATA.WEEKDAYS) {
        const sl = s.theater.characterSlots[day];
        if (sl.performance === playerId || sl.backstage.includes(playerId)) return day;
      }
      return null;
    },
    renderActionPanel() {
      const s = ui.engine.state;
      const panel = document.getElementById('action-content');
      let html = '';
      switch (s.phase) {
        case 'ADVERTISE': html = ui.renderAdvertiseActions(s); break;
        case 'ASSIGNMENT': html = ui.renderAssignmentActions(s); break;
        case 'PLACE_CHARACTERS': html = ui.renderPlaceActions(s); break;
        case 'PERFORMANCE':
          html = `<div style="color:var(--text-dim);text-align:center;padding:20px">${SVG.theater} 퍼포먼스 진행 중...</div>`;
          break;
        case 'GAME_OVER': html = ui.renderGameOverPanel(s); break;
        default:
          html = `<div style="color:var(--text-dim);text-align:center;padding:20px">${ui.getPhaseName(s.phase)} 진행 중...</div>`;
      }
      panel.innerHTML = html;
    },

    renderAdvertiseActions(s) {
      const player = s.players[s.initiativeOrder[s.currentPlayerIdx]];
      if (!player) return '';
      const cost = DATA.ADVERTISE_COST[s.initiativeOrder.indexOf(player.id)] || 1;
      return `<div class="action-group"><div class="action-group-title">광고 페이즈</div>
        <p style="font-size:0.85rem;margin-bottom:12px"><strong style="color:${player.color}">${player.name}</strong>의 차례</p>
        <p style="font-size:0.8rem;color:var(--text-dim);margin-bottom:12px">${cost} 코인을 지불하여 2 명성을 얻을 수 있습니다.</p>
        <div style="display:flex;gap:8px">
          <button class="btn btn-primary" onclick="window.gameUI.doAdvertise()" ${player.coins < cost ? 'disabled' : ''}>${SVG.coin} 광고하기 (${cost}${SVG.coin} -> +2${SVG.fame})</button>
          <button class="btn" onclick="window.gameUI.doSkipAdvertise()">건너뛰기</button>
        </div></div>`;
    },

    renderAssignmentActions(s) {
      const player = s.players[s.initiativeOrder[s.currentPlayerIdx]];
      if (!player) return '';
      const unassigned = player.characters.filter(c => !c.assigned);
      const assigned = player.characters.filter(c => c.assigned);
      const usedIndices = player.assignedCards.map(ac => ac.cardIdx);
      const availCards = {};
      player.assignmentCards.permanent.forEach((loc, i) => {
        if (!usedIndices.includes(i)) availCards[loc] = (availCards[loc] || 0) + 1;
      });
      return `<div class="action-group"><div class="action-group-title">배정 페이즈</div>
        <p style="font-size:0.85rem;margin-bottom:8px"><strong style="color:${player.color}">${player.name}</strong>: 캐릭터를 장소에 배정하세요</p>
        <div style="font-size:0.75rem;color:var(--text-dim);margin-bottom:8px;display:flex;gap:6px;flex-wrap:wrap">배정 카드: ${Object.entries(availCards).map(([loc, cnt]) => `<span style="background:var(--bg-card);padding:2px 6px;border-radius:4px">${DATA.LOCATIONS[loc]?.icon || ''}${cnt}</span>`).join('')}</div>
        ${assigned.map(c => {
          const realIdx = player.characters.indexOf(c);
          const info = DATA.CHARACTER_TYPES[c.type];
          return `<div style="margin-bottom:4px;padding:6px 10px;background:var(--bg-card);border-radius:var(--radius);border:1px solid ${player.color}40;display:flex;justify-content:space-between;align-items:center">
            <span>${CHAR_SVG[c.type] || info.icon} ${info.name} → ${DATA.LOCATIONS[c.location]?.name || c.location}</span>
            <button class="btn btn-sm" onclick="window.gameUI.doUnassign(${realIdx})" style="font-size:0.7rem">취소</button></div>`;
        }).join('')}
        ${unassigned.map(c => {
          const realIdx = player.characters.indexOf(c);
          const info = DATA.CHARACTER_TYPES[c.type];
          return `<div style="margin-bottom:12px;padding:10px;background:var(--bg-card);border-radius:var(--radius);border:1px solid var(--border)">
            <div style="font-weight:700;margin-bottom:6px;font-family:var(--font-heading)">${CHAR_SVG[c.type] || info.icon} ${info.name} (${c.ap} AP)</div>
            <div style="display:flex;flex-wrap:wrap;gap:4px">
              ${Object.entries(DATA.LOCATIONS).map(([key, loc]) => {
                const hasCard = (availCards[key] || 0) > 0;
                return `<button class="btn btn-sm" onclick="window.gameUI.doAssign(${realIdx}, '${key}')" ${!hasCard ? 'disabled title="배정 카드 부족"' : ''}>${loc.icon} ${loc.name}</button>`;
              }).join('')}
            </div></div>`;
        }).join('')}
        <button class="btn btn-primary btn-steam" onclick="window.gameUI.finishAssignment()" style="width:100%">${unassigned.length > 0 ? '유휴 캐릭터 포함 배정 완료' : '배정 완료'}</button>
      </div>`;
    },
    renderPlaceActions(s) {
      const currentTurn = ui.engine.getCurrentTurn();
      if (!currentTurn) {
        return `<div style="text-align:center;padding:16px;color:var(--text-dim)">모든 캐릭터가 배치되었습니다.</div>
          <button class="btn btn-primary btn-steam" onclick="window.gameUI.finishPlacement()" style="width:100%">다음 단계로</button>`;
      }
      const player = s.players[currentTurn.playerId];
      const charIdx = currentTurn.characterIdx;
      const c = player.characters[charIdx];
      const info = DATA.CHARACTER_TYPES[c.type];
      const charIcon = CHAR_SVG[c.type] || info.icon;
      const actions = ui.engine.getAvailableActions(player.id, charIdx);

      return `<div class="action-group"><div class="action-group-title">캐릭터 배치 & 액션 (교대 턴)</div>
        <p style="font-size:0.85rem;margin-bottom:8px"><strong style="color:${player.color}">${player.name}</strong>의 차례</p>
        <div style="margin-bottom:16px;padding:12px;background:var(--bg-card);border-radius:var(--radius);border:1px solid var(--border);border-left:3px solid ${player.color}">
          <div style="font-weight:700;margin-bottom:4px;font-family:var(--font-heading)">${charIcon} ${info.name} -> ${DATA.LOCATIONS[c.location]?.icon || ''} ${DATA.LOCATIONS[c.location]?.name || c.location}</div>
          <div style="font-size:0.8rem;color:var(--text-dim);margin-bottom:4px">기본 AP: ${info.baseAP}${c.slotApMod ? ` ${c.slotApMod > 0 ? '+' : ''}${c.slotApMod} (슬롯)` : ''} = 가용 ${ui.engine.getCharacterAP(player, c)} AP</div>
          <div style="display:flex;gap:4px;margin-bottom:8px">
            ${!c.shardConverted && c.location !== 'THEATER' && player.shards > 0 ? `<button class="btn btn-sm" style="border-color:var(--cyan);color:var(--cyan)" onclick="window.gameUI.doConvertShard(${player.id}, ${charIdx})">💎→+1 AP</button>` : ''}
          </div>
          <div class="location-actions">
            ${actions.map(a => `<button class="action-btn" onclick="window.gameUI.doAction(${player.id}, ${charIdx}, ${JSON.stringify(a).replace(/"/g, '&quot;')})">
              <span>${a.name}</span><span class="ap-cost">${a.cost} AP</span>
            </button>`).join('')}
            <button class="action-btn" style="border-color:var(--text-dim);color:var(--text-dim)" onclick="window.gameUI.doPass(${player.id}, ${charIdx})">
              <span>패스</span><span class="ap-cost">턴 종료</span>
            </button>
          </div>
        </div>
      </div>`;
    },

    renderGameOverPanel(s) {
      const sorted = [...s.players].sort((a, b) => b.fame - a.fame);
      return `<div class="action-group"><div class="action-group-title">${SVG.crown} 최종 결과</div>
        ${sorted.map((p, i) => `<div style="display:flex;justify-content:space-between;padding:8px 12px;background:var(--bg-card);border-radius:var(--radius);margin-bottom:4px;border-left:3px solid ${p.color}">
          <span><strong>${i + 1}위</strong> <span style="color:${p.color}">${p.name}</span></span>
          <span>${SVG.fame}${p.fame}</span></div>`).join('')}
        <button class="btn btn-primary btn-steam" onclick="location.reload()" style="width:100%;margin-top:12px">새 게임</button>
      </div>`;
    }
  };
}
