// Trickerion UI - Modal System & Notifications
import * as DATA from '../gameData.js';
import { SVG } from './constants.js';

export function createModalMethods(ui) {
  return {
    showModal(title, content, actions = []) {
      const overlay = document.getElementById('modal-overlay');
      const modal = document.getElementById('modal');
      modal.innerHTML = `<h2>${title}</h2><div class="modal-body">${content}</div>
        <div class="modal-actions"><button class="btn" onclick="window.gameUI.closeModal()">${SVG.close} 취소</button>
        ${actions.map(a => `<button class="btn btn-primary btn-steam" onclick="${a.onclick}">${a.label}</button>`).join('')}</div>`;
      overlay.classList.add('active');
      ui.animateModalOpen();
    },

    closeModal() {
      const overlay = document.getElementById('modal-overlay');
      ui.animateModalClose(() => { overlay.classList.remove('active'); });
      ui.pendingSetupTrick = null;
    },

    showTrickSelectionModal(player, action) {
      const s = ui.engine.state, fav = player.magician.favoriteCategory, allowed = ui.engine.getAllowedTrickCategories(player);
      const dd = s.downtownDice.residence.map(d => d === 'X' ? '✗' : d === 'ANY' ? '?' : DATA.TRICK_CATEGORIES[d]?.icon || d).join(' ');
      const allTricks = Object.entries(s.trickDecks).flatMap(([cat, ts]) => allowed.has(cat) ? ts : []);
      if (!allTricks.length) { ui.notify('배울 수 있는 트릭이 없습니다.', 'error'); return; }
      const cn = [...allowed].map(c => `${DATA.TRICK_CATEGORIES[c].icon}${DATA.TRICK_CATEGORIES[c].name}`).join(' ');
      const grid = allTricks.map(t => {
        const cat = DATA.TRICK_CATEGORIES[t.category], comps = Object.entries(t.components).map(([c, n]) => `${DATA.COMPONENT_TYPES[c]?.icon || c}×${n}`).join(' ');
        const isFav = t.category === fav, fd = Math.max(0, t.fameThreshold - player.fame), ok = fd <= player.coins;
        const th = fd > 0 ? (ok ? `<span style="color:var(--orange)">−${fd}${SVG.coin}</span>` : `<span style="color:#e74c3c">🔒</span>`) : '';
        return `<div class="trick-select-item${isFav ? ' favorite' : ''}" style="${ok ? '' : 'opacity:0.35;pointer-events:none'}" ${ok ? `onclick="window.gameUI.confirmLearnTrick(${player.id},'${t.category}','${t.id}')"` : ''}>
          <div class="name">${cat.icon} ${t.name}${isFav ? ' ★' : ''}</div><div class="category" style="color:${cat.color}">Lv.${t.level} ${cat.name} | ${SVG.fame}${t.fameThreshold} ${th}</div>
          <div class="components">필요: ${comps}</div><div class="yields">수익: ${SVG.fame}${t.yields.fame} ${SVG.coin}${t.yields.coins} ${SVG.shard}${t.yields.shards}</div>
          ${t.endBonus?.desc ? `<div style="font-size:0.7rem;color:var(--cyan)">종료: ${t.endBonus.desc}</div>` : ''}</div>`;
      }).join('');
      ui.showModal(`${SVG.trick} 트릭 배우기`, `<p style="margin-bottom:8px">트릭 선택 (${SVG.fame}${player.fame} ${SVG.coin}${player.coins})</p><p style="font-size:0.78rem;color:var(--text-dim);margin-bottom:12px">주사위: ${dd} | 특기: ${DATA.TRICK_CATEGORIES[fav].icon} | 가능: ${cn}</p><div class="trick-select-grid">${grid}</div>`);
    },

    confirmLearnTrick(playerId, category, trickId) {
      ui.engine.saveState();
      const result = ui.engine.executeAction(playerId, { type: 'LEARN_TRICK', category, trickId });
      if (result.success) {
        const player = ui.engine.state.players[playerId];
        ui.consumeAP(player, ui._activeCharIdx, 3);
        ui.notify(`"${result.trick.name}" 트릭을 배웠습니다!`, 'success');
        ui.closeModal();
        ui.checkAutoAdvanceTurn(player, ui._activeCharIdx);
      } else { ui.notify(result.message, 'error'); }
      ui.renderAll();
    },

    showDieSelectionModal(player, group, action) {
      const dice = ui.engine.state.downtownDice[group];
      const getDieDisplay = (val) => {
        if (val === 'X') return '&#10006; (사용됨)';
        if (typeof val === 'number') return `${val} ${SVG.coin}`;
        if (DATA.CHARACTER_TYPES[val]) return `${DATA.CHARACTER_TYPES[val].icon} ${DATA.CHARACTER_TYPES[val].name}`;
        return String(val);
      };
      const content = `<p style="margin-bottom:12px">주사위를 선택하세요:</p>
        <div style="display:flex;gap:12px;justify-content:center">
          ${dice.map((d, i) => `<div class="die ${d === 'X' ? 'used' : ''}" style="width:80px;height:80px;font-size:1.2rem"
            onclick="window.gameUI.confirmDieSelect(${player.id}, '${group}', ${i}, '${action.type}')">${getDieDisplay(d)}</div>`).join('')}
        </div>`;
      ui.showModal(action.type === 'TAKE_COINS' ? `${SVG.coin} 코인 받기` : `${SVG.magician} 캐릭터 고용`, content);
    },

    confirmDieSelect(playerId, group, dieIdx, actionType) {
      const result = ui.engine.executeAction(playerId, { type: actionType, dieIndex: dieIdx, diceGroup: group });
      if (result.success) {
        const player = ui.engine.state.players[playerId];
        ui.consumeAP(player, ui._activeCharIdx, 3);
        ui.notify('완료!', 'success');
        ui.closeModal();
        ui.checkAutoAdvanceTurn(player, ui._activeCharIdx);
      } else { ui.notify(result.message, 'error'); }
      ui.renderAll();
    },

    showBuyModal(player, action) {
      const market = ui.engine.state.marketRow;
      const discount = action.discount || 0;
      ui._pendingBuyDiscount = discount;
      const allAvailable = [...new Set([...market.stock, ...(market.quickOrder ? [market.quickOrder] : [])])];
      const label = discount > 0 ? '흥정 구매 (-1코인)' : '컴포넌트 구매';
      const content = `<p style="margin-bottom:12px">${label} (보유 코인: ${player.coins}${SVG.coin})</p>
        <div class="component-select">
          ${allAvailable.map(comp => {
            const c = DATA.COMPONENT_TYPES[comp];
            if (!c) return '';
            const isQuick = comp === market.quickOrder;
            const price = Math.max(0, c.cost + (isQuick ? 1 : 0) - discount);
            return `<div class="comp-option" onclick="window.gameUI.confirmBuy(${player.id}, '${comp}')">
              <span>${c.icon}</span><span>${c.name}</span>
              <span class="price" style="color:var(--brass);font-weight:700">${price}${SVG.coin}</span>
              ${discount > 0 ? '<span style="font-size:0.7rem;color:var(--cyan)">(흥정)</span>' : ''}
              ${isQuick ? '<span style="font-size:0.7rem;color:var(--orange)">(긴급)</span>' : ''}
            </div>`;
          }).join('')}
        </div>`;
      ui.showModal(`${SVG.market} ${label}`, content);
    },

    confirmBuy(playerId, componentType) {
      const discount = ui._pendingBuyDiscount || 0;
      const actionType = discount > 0 ? 'BARGAIN' : 'BUY';
      const result = ui.engine.executeAction(playerId, { type: actionType, componentType, quantity: 1 });
      if (result.success) {
        const player = ui.engine.state.players[playerId];
        ui.consumeAP(player, ui._activeCharIdx, 1);
        ui.notify(`${DATA.COMPONENT_TYPES[componentType].name} ${discount > 0 ? '흥정' : '구매'} 완료!`, 'success');
        ui.closeModal();
        ui.checkAutoAdvanceTurn(player, ui._activeCharIdx);
      } else { ui.notify(result.message, 'error'); }
      ui._pendingBuyDiscount = 0;
      ui.renderAll();
    },

    showOrderModal(player, action) {
      const content = `<p style="margin-bottom:12px">주문할 컴포넌트를 선택하세요:</p>
        <div class="component-select">
          ${Object.entries(DATA.COMPONENT_TYPES).map(([key, c]) =>
            `<div class="comp-option" onclick="window.gameUI.confirmOrder(${player.id}, '${key}')">
              <span>${c.icon}</span><span>${c.name}</span><span style="font-size:0.7rem;color:var(--text-dim)">${c.tier}</span>
            </div>`
          ).join('')}
        </div>`;
      ui.showModal(`${SVG.component} 컴포넌트 주문`, content);
    },

    confirmOrder(playerId, componentType) {
      const result = ui.engine.executeAction(playerId, { type: 'ORDER', componentType });
      if (result.success) {
        const player = ui.engine.state.players[playerId];
        ui.consumeAP(player, ui._activeCharIdx, 1);
        ui.notify(`${DATA.COMPONENT_TYPES[componentType].name} 주문 완료! 다음 턴에 도착`, 'success');
        ui.closeModal();
        ui.checkAutoAdvanceTurn(player, ui._activeCharIdx);
      } else { ui.notify(result.message, 'error'); }
      ui.renderAll();
    },

    showSetupTrickModal(player, action) {
      const preparedTricks = player.tricks.filter(t => t.prepared && t.trickMarkers > 0);
      if (preparedTricks.length === 0) {
        ui.notify('셋업 가능한 트릭이 없습니다.', 'error');
        return;
      }
      const content = `<p style="margin-bottom:12px">셋업할 트릭을 선택하세요:</p>
        <div class="trick-select-grid">
          ${preparedTricks.map(t => {
            const realIdx = player.tricks.indexOf(t);
            return `<div class="trick-select-item" onclick="window.gameUI.startSetupTrick(${player.id}, ${realIdx})">
              <div class="name">${DATA.TRICK_CATEGORIES[t.category].icon} ${t.name}</div>
              <div class="category">마커: ${t.trickMarkers}개 남음</div>
            </div>`;
          }).join('')}
        </div>
        <p style="font-size:0.8rem;color:var(--text-dim);margin-top:8px">트릭 선택 후 퍼포먼스 카드의 빈 슬롯을 클릭하세요.</p>`;
      ui.showModal(`${SVG.theater} 트릭 셋업`, content);
    },

    startSetupTrick(playerId, trickIdx) {
      // Close modal via DOM directly (closeModal clears pendingSetupTrick)
      document.getElementById('modal-overlay').classList.remove('active');
      ui.pendingSetupTrick = { playerId, trickIdx };
      ui.notify('퍼포먼스 카드의 빈 슬롯을 클릭하세요', 'success');
    },

    completeSetupTrick(cardIdx, slotIdx) {
      if (!ui.pendingSetupTrick) return;
      const { playerId, trickIdx } = ui.pendingSetupTrick;
      const result = ui.engine.executeAction(playerId, { type: 'SETUP_TRICK', trickIdx, perfCardIdx: cardIdx, slotIdx });
      if (result.success) {
        const player = ui.engine.state.players[playerId];
        ui.consumeAP(player, ui._activeCharIdx, 1);
        ui.notify('트릭 셋업 완료!', 'success');
        if (result.links > 0) {
          ui.pendingSetupTrick = null;
          ui.showLinkRewardModal(playerId, result.links, result.linkBonus, result.perfCardIdx, result.slotIdx);
          return;
        }
        ui.checkAutoAdvanceTurn(player, ui._activeCharIdx);
      } else { ui.notify(result.message, 'error'); }
      ui.pendingSetupTrick = null;
      ui.renderAll();
    },
    showLinkRewardModal(playerId, links, linkBonus, cardIdx, slotIdx) {
      const total = linkBonus * links;
      const content = `<p style="margin-bottom:12px">링크 ${links}개 생성! 보상을 선택하세요 (${total}포인트):</p>
        <div style="display:flex;gap:12px;justify-content:center">
          <button class="btn btn-primary btn-steam" onclick="window.gameUI.confirmLinkReward(${playerId},'fame',${total},${cardIdx},${slotIdx})">${SVG.fame} +${total} 명성</button>
          <button class="btn btn-primary btn-steam" onclick="window.gameUI.confirmLinkReward(${playerId},'coins',${total},${cardIdx},${slotIdx})">${SVG.coin} +${total} 코인</button>
        </div>`;
      ui.showModal(`${SVG.theater} 링크 보상 선택`, content);
    },
    confirmLinkReward(playerId, choice, amount, cardIdx, slotIdx) {
      ui.engine.applyLinkReward(playerId, choice, amount, cardIdx, slotIdx);
      ui.closeModal();
      const player = ui.engine.state.players[playerId];
      ui.notify(`링크 보상: +${amount} ${choice === 'fame' ? '명성' : '코인'}`, 'success');
      ui.checkAutoAdvanceTurn(player, ui._activeCharIdx);
      ui.renderAll();
    },

    showRerollModal(player, action) {
      const content = `<p style="margin-bottom:12px">재굴림할 주사위 그룹을 선택하세요:</p>
        <div style="display:flex;gap:12px;justify-content:center;flex-wrap:wrap">
          <button class="btn" onclick="window.gameUI.confirmReroll(${player.id}, 'residence')">${SVG.downtown} 달가드 저택</button>
          <button class="btn" onclick="window.gameUI.confirmReroll(${player.id}, 'inn')">${SVG.magician} 여관</button>
          <button class="btn" onclick="window.gameUI.confirmReroll(${player.id}, 'bank')">${SVG.coin} 은행</button>
        </div>`;
      ui.showModal(`${SVG.dice} 주사위 재굴림 (1 AP)`, content);
    },

    confirmReroll(playerId, group) {
      const result = ui.engine.executeAction(playerId, { type: 'REROLL_DIE', diceGroup: group, dieIndex: 0 });
      if (result.success) {
        const player = ui.engine.state.players[playerId];
        ui.consumeAP(player, ui._activeCharIdx, 1);
        ui.notify('주사위 재굴림 완료!', 'success');
        ui.closeModal();
        ui.checkAutoAdvanceTurn(player, ui._activeCharIdx);
      } else { ui.notify(result.message, 'error'); }
      ui.renderAll();
    },

    showChooseDieModal(player, action) {
      const id = player.id, opts = [['residence',0,"'ANY'",'저택→?'],['inn',0,"'ENGINEER'",'여관→기술자'],['inn',0,"'MANAGER'",'여관→매니저'],['inn',0,"'ASSISTANT'",'여관→어시'],['bank',0,5,`은행→5${SVG.coin}`]];
      const btns = opts.map(([g,i,v,l]) => `<button class="btn" onclick="window.gameUI.confirmChooseDie(${id},'${g}',${i},${v})">${l}</button>`).join('');
      ui.showModal(`${SVG.dice} 주사위 결과 선택 (2 AP)`, `<p style="margin-bottom:12px">원하는 결과를 선택하세요:</p><div style="display:flex;gap:8px;flex-wrap:wrap;justify-content:center">${btns}</div>`);
    },
    confirmChooseDie(playerId, group, dieIdx, value) {
      const result = ui.engine.executeAction(playerId, { type: 'CHOOSE_DIE', diceGroup: group, dieIndex: dieIdx, value });
      if (result.success) {
        const player = ui.engine.state.players[playerId];
        ui.consumeAP(player, ui._activeCharIdx, 2);
        ui.notify('주사위 결과 변경!', 'success');
        ui.closeModal();
        ui.checkAutoAdvanceTurn(player, ui._activeCharIdx);
      } else { ui.notify(result.message, 'error'); }
      ui.renderAll();
    },

    showDrawSpecialModal(player, action) {
      const decks = Object.entries(ui.engine.state.darkAlley.specialDecks).filter(([, d]) => d.length >= 2);
      if (decks.length === 0) { ui.notify('카드가 부족합니다.', 'error'); return; }
      const [dn, dk] = decks[Math.floor(Math.random() * decks.length)];
      ui._pendingDraw = { cards: [dk[0], dk[1]], deck: dn, cost: action.type === 'DRAW_MORE' ? 2 : 1 };
      const content = `<p style="margin-bottom:12px">2장 중 1장을 선택하세요:</p><div class="trick-select-grid">${[0, 1].map(i => {
        const c = ui._pendingDraw.cards[i];
        return `<div class="trick-select-item" onclick="window.gameUI.confirmDrawSpecial(${player.id}, ${i})"><div class="name">🌙 ${c.name}</div><div class="category">${c.effect}</div><div style="font-size:0.7rem;color:var(--cyan)">+${c.bonusAP} AP 보너스</div></div>`;
      }).join('')}</div>`;
      ui.showModal('🌙 특수 임무 획득', content);
    },
    confirmDrawSpecial(playerId, chosenIdx) {
      if (!ui._pendingDraw) return;
      const { deck, cost } = ui._pendingDraw;
      const result = ui.engine.executeAction(playerId, { type: 'DRAW_SPECIAL', deck, chosenIdx });
      if (result.success) { const p = ui.engine.state.players[playerId]; ui.consumeAP(p, ui._activeCharIdx, cost); ui.notify(`"${result.chosen.name}" 획득!`, 'success'); ui.closeModal(); ui.checkAutoAdvanceTurn(p, ui._activeCharIdx); }
      else ui.notify(result.message, 'error');
      ui._pendingDraw = null; ui.renderAll();
    },

    showPerformanceResults(results) {
      const s = ui.engine.state, logs = s.log.slice(-30).map(l => l.message).filter(m => /공연|링크|보너스|명성/.test(m));
      const lh = logs.length ? '<div class="perf-results-list">' + logs.map(e => `<div class="perf-result-entry">${e}</div>`).join('') + '</div>' : '<p style="color:var(--text-dim);text-align:center;padding:20px">공연 없음</p>';
      const rk = [...s.players].sort((a, b) => b.fame - a.fame).map((p, i) => `<div style="display:flex;justify-content:space-between;padding:6px 12px;background:var(--bg-card);border-radius:var(--radius);margin-bottom:4px;border-left:3px solid ${p.color}"><span>${i + 1}위 <strong style="color:${p.color}">${p.name}</strong></span><span>${SVG.fame}${p.fame} ${SVG.coin}${p.coins}</span></div>`).join('');
      ui.showModal(`${SVG.theater} 퍼포먼스 결과`, `<div class="performance-results">${lh}<h3 style="margin-top:12px;margin-bottom:6px">순위</h3>${rk}<div style="text-align:center;margin-top:12px"><button class="btn btn-primary btn-steam" onclick="window.gameUI.advanceToEndTurn()">다음 ${SVG.arrowRight}</button></div></div>`);
    },
    advanceToEndTurn() {
      ui.closeModal(); ui.engine.executeEndTurn(); ui.renderAll();
      if (!ui.engine.state.gameOver) ui.notify(`라운드 ${ui.engine.state.round} 시작!`, 'success');
    },
    notify(message, type = 'info') {
      const n = document.createElement('div'); n.className = `notification ${type}`; n.textContent = message; document.body.appendChild(n);
      if (ui.gsapAvailable()) gsap.fromTo(n, { x: 80, opacity: 0 }, { x: 0, opacity: 1, duration: 0.4, ease: 'back.out(1.5)' });
      setTimeout(() => { if (ui.gsapAvailable()) gsap.to(n, { x: 80, opacity: 0, duration: 0.3, ease: 'power2.in', onComplete: () => n.remove() }); else n.remove(); }, 3000);
    },
    appendLog(message) {
      const el = document.getElementById('log-content'); if (!el) return;
      const e = document.createElement('div'); e.className = `log-entry ${message.includes('===') || message.includes('---') ? 'highlight' : ''}`; e.textContent = message; el.appendChild(e); el.scrollTop = el.scrollHeight;
    }
  };
}
