// Trickerion UI - Game Rendering (top bar, player board, main board)
import * as DATA from '../gameData.js';
import { SVG, CHAR_SVG, LOC_SVG } from './constants.js';

export function createRenderMethods(ui) {
  return {
    renderAll() {
      if (!ui.engine.state) return;
      ui.renderTopBar();
      ui.renderPlayerBoard();
      ui.renderMainBoard();
      ui.renderActionPanel();
    },

    renderTopBar() {
      const s = ui.engine.state;
      document.getElementById('round-badge').textContent = s.gameOver
        ? '게임 종료' : `라운드 ${s.round} / ${DATA.GAME_CONFIG.BASE_ROUNDS}`;
      document.getElementById('phase-badge').textContent = ui.getPhaseName(s.phase);
      const tabsEl = document.getElementById('player-tabs');
      const undoBtn = ui.engine.canUndo()
        ? `<div class="player-tab" onclick="window.gameUI.doUndo()" style="cursor:pointer;border-color:var(--cyan);color:var(--cyan);font-size:0.75rem" title="되돌리기 (Ctrl+Z)">↩ 되돌리기</div>`
        : '';
      const saveBtn = ui.renderSaveButton ? ui.renderSaveButton() : '';
      tabsEl.innerHTML = s.players.map((p, i) => `
        <div class="player-tab ${i === ui.currentViewPlayer ? 'active' : ''}"
             onclick="window.gameUI.switchPlayerView(${i})"
             style="border-color: ${i === ui.currentViewPlayer ? p.color : 'transparent'}">
          ${p.magician.name.substring(0, 6)}
          <span class="fame" style="color:${p.color}">${SVG.fame}${p.fame}</span>
        </div>`).join('') + undoBtn + saveBtn;
    },

    switchPlayerView(idx) {
      if (ui.engine.state?.phase === 'ASSIGNMENT') { ui.notify('배정 중에는 다른 플레이어를 볼 수 없습니다', 'error'); return; }
      ui.currentViewPlayer = idx;
      ui.renderAll();
    },

    renderPlayerBoard() {
      const p = ui.engine.state.players[ui.currentViewPlayer];
      if (!p) return;
      const panel = document.getElementById('player-board');
      let nameHeader = document.getElementById('player-name-header');
      if (!nameHeader) {
        nameHeader = document.createElement('div');
        nameHeader.id = 'player-name-header';
        nameHeader.className = 'panel-section';
        panel.insertBefore(nameHeader, panel.firstChild);
      }
      const portrait = p.magician.img ? `<img src="${p.magician.img}" class="magician-portrait" alt="${p.magician.name}">` : `<span style="font-size:1.5rem">${DATA.TRICK_CATEGORIES[p.magician.favoriteCategory].icon}</span>`;
      nameHeader.innerHTML = `
        <div style="display:flex;align-items:center;gap:10px;margin-bottom:4px">
          ${portrait}
          <div>
            <div style="font-weight:700;color:${p.color};font-size:1.05rem;font-family:var(--font-heading)">${p.name}</div>
            <div style="font-size:0.72rem;color:var(--text-dim)">${p.magician.fullName}</div>
          </div>
        </div>`;

      const prevFame = document.getElementById('stat-fame').textContent;
      document.getElementById('stat-fame').textContent = p.fame;
      document.getElementById('stat-coins').textContent = p.coins;
      document.getElementById('stat-shards').textContent = p.shards;
      document.getElementById('stat-tricks-count').textContent = p.tricks.length;
      if (prevFame !== String(p.fame)) ui.animateScoreChange('stat-fame');

      document.getElementById('character-list').innerHTML = p.characters.map((c, i) => {
        const info = DATA.CHARACTER_TYPES[c.type];
        const charIcon = CHAR_SVG[c.type] || `<span class="char-icon">${info.icon}</span>`;
        const status = c.placed ? '완료' : c.assigned ? `-> ${DATA.LOCATIONS[c.location]?.name || c.location}` : '대기';
        return `<div class="character-item ${c.assigned ? 'assigned' : ''} ${c.placed ? 'placed' : ''}">
          <span>${charIcon} ${info.name}</span>
          <span style="font-size:0.72rem;color:var(--text-dim)">${status}</span>
          <span class="char-ap">${c.ap} AP</span>
        </div>`;
      }).join('');

      document.getElementById('trick-list').innerHTML = p.tricks.map(t => {
        const catClass = t.category.toLowerCase();
        const markers = Array(t.markerSlots).fill(0).map((_, mi) =>
          `<div class="trick-marker ${mi < t.trickMarkers ? 'filled' : ''}"></div>`
        ).join('');
        return `<div class="trick-item ${catClass}">
          <div class="trick-name">${DATA.TRICK_CATEGORIES[t.category].icon} ${t.name}</div>
          <div class="trick-level">Lv.${t.level} | ${t.prepared ? '준비됨' : '미준비'}</div>
          <div class="trick-markers">${markers}</div>
          <div class="trick-yields">수익: ${SVG.fame}${t.yields.fame} ${SVG.coin}${t.yields.coins} ${SVG.shard}${t.yields.shards}</div>
        </div>`;
      }).join('') || '<div style="color:var(--text-dim);font-size:0.85rem">트릭 없음</div>';

      const comps = Object.entries(p.components).filter(([_, v]) => v > 0);
      document.getElementById('component-grid').innerHTML = comps.map(([key, count]) => {
        const comp = DATA.COMPONENT_TYPES[key];
        return `<div class="component-item"><span>${comp.icon}</span><span>${comp.name}</span><span class="count">x${count}</span></div>`;
      }).join('') || '<div style="color:var(--text-dim);font-size:0.85rem">컴포넌트 없음</div>';
    },

    renderMainBoard() {
      const s = ui.engine.state;
      const center = document.getElementById('main-board');
      let html = `<div class="phase-banner"><h2>${ui.getPhaseName(s.phase)}</h2><p>${ui.getPhaseDescription(s.phase)}</p></div>`;

      // Turn indicator for alternating turns
      if (s.phase === 'PLACE_CHARACTERS' && s.turnQueue && s.turnQueue.length > 0) {
        const currentTurn = ui.engine.getCurrentTurn();
        if (currentTurn) {
          const tp = s.players[currentTurn.playerId];
          const tc = tp.characters[currentTurn.characterIdx];
          const tInfo = DATA.CHARACTER_TYPES[tc.type];
          html += `<div class="turn-indicator" style="background:linear-gradient(135deg, ${tp.color}33, ${tp.color}22);border:1px solid ${tp.color};border-radius:var(--radius);padding:8px 16px;margin-bottom:12px;text-align:center;font-weight:700">
            현재 턴: <span style="color:${tp.color}">${tp.name}</span> - ${tInfo.name} (${s.currentTurnIdx + 1}/${s.turnQueue.length})
          </div>`;
        }
      }

      html += ui.renderDiceSection();
      html += '<div class="locations-grid">';
      for (const [key, loc] of Object.entries(DATA.LOCATIONS)) {
        if (key === 'WORKSHOP') continue;
        const locIcon = LOC_SVG[key] || `<span class="location-icon">${loc.icon}</span>`;
        const bgStyle = loc.img ? `background-image:url('${loc.img}')` : '';
        html += `<div class="location-card has-bg ${ui.selectedLocation === key ? 'selected' : ''}" onclick="window.gameUI.selectLocation('${key}')" style="${bgStyle}">
          <div class="location-header">${locIcon}<span class="location-name">${loc.name}</span></div>
          ${ui.renderLocationContent(key, loc)}
        </div>`;
      }
      const wsLoc = DATA.LOCATIONS.WORKSHOP;
      const wsBg = wsLoc.img ? `background-image:url('${wsLoc.img}')` : '';
      html += `<div class="location-card has-bg ${ui.selectedLocation === 'WORKSHOP' ? 'selected' : ''}" onclick="window.gameUI.selectLocation('WORKSHOP')" style="${wsBg}">
        <div class="location-header">${LOC_SVG.WORKSHOP}<span class="location-name">${wsLoc.name} (개인)</span></div>
        ${ui.renderWorkshopContent()}
      </div></div>`;
      html += ui.renderFameTrack();
      html += ui.renderTheaterSection();
      center.innerHTML = html;
      ui.animateRoundBanner();
    },

    renderDiceSection() {
      const dice = ui.engine.state.downtownDice;
      const getDieDisplay = (val) => {
        if (val === 'X') return '&#10006;';
        if (val === 'ANY') return '?';
        if (typeof val === 'number') return `${val}`;
        if (DATA.TRICK_CATEGORIES[val]) return DATA.TRICK_CATEGORIES[val].icon;
        if (DATA.CHARACTER_TYPES[val]) return DATA.CHARACTER_TYPES[val].icon;
        return val;
      };
      return `<div class="dice-section">
        ${['residence', 'inn', 'bank'].map(group => {
          const labels = { residence: '달가드 저택', inn: '여관', bank: '은행' };
          return `<div class="dice-group"><span class="dice-group-label">${labels[group]}</span><div class="dice-row">
            ${dice[group].map((d, i) => `<div class="die ${d === 'X' ? 'used' : ''}">${getDieDisplay(d)}</div>`).join('')}
          </div></div>`;
        }).join('')}
      </div>`;
    },

    renderLocationContent(key, loc) {
      const slots = ui.renderLocationSlots(key);
      if (key === 'MARKET_ROW') return ui.renderMarketContent() + slots;
      if (key === 'DARK_ALLEY') return `<div style="font-size:0.8rem;color:var(--text-dim)">특수 임무 카드를 획득하세요</div>` + slots;
      return `<div style="font-size:0.8rem;color:var(--text-dim)">${Object.values(loc.actions).map(a => a.name).join(' | ')}</div>` + slots;
    },

    renderLocationSlots(key) {
      const locSlots = ui.engine.state.locationSlots[key];
      if (!locSlots) return '';
      const allSlots = DATA.LOCATION_SLOTS[key] || [];
      const maxRow = ui.engine.state.numPlayers <= 2 ? 1 : ui.engine.state.numPlayers <= 3 ? 2 : 3;
      let availIdx = 0;
      return `<div class="worker-slots">${allSlots.map(s => {
        const apLabel = s.apMod > 0 ? `+${s.apMod}` : `${s.apMod}`;
        if (s.row > maxRow) return `<div class="worker-slot locked" title="잠김 (${s.row}행)"><span class="ws-ap">${apLabel}</span>🔒</div>`;
        const slot = locSlots[availIdx++];
        const occ = slot?.occupant;
        if (occ) {
          const p = ui.engine.state.players[occ.playerId];
          const c = p.characters[occ.charIdx];
          return `<div class="worker-slot occupied" style="border-color:${p.color}" title="${p.name} ${DATA.CHARACTER_TYPES[c.type].name}">
            <span class="ws-ap">${apLabel}</span><span style="color:${p.color}">${DATA.CHARACTER_TYPES[c.type].icon}</span></div>`;
        }
        return `<div class="worker-slot"><span class="ws-ap">${apLabel}</span></div>`;
      }).join('')}</div>`;
    },

    renderMarketContent() {
      const market = ui.engine.state.marketRow;
      return `<div class="market-display">
        ${market.stock.map(comp => {
          const c = DATA.COMPONENT_TYPES[comp];
          return `<div class="market-item"><span>${c.icon} ${c.name}</span><span class="price">${c.cost}${SVG.coin}</span></div>`;
        }).join('')}
        ${market.quickOrder ? `<div class="market-item" style="border-color:var(--orange)"><span>${DATA.COMPONENT_TYPES[market.quickOrder].icon} 긴급</span></div>` : ''}
      </div>
      ${market.orders.length > 0 ? `<div style="font-size:0.72rem;color:var(--text-dim);margin-top:4px">주문중: ${market.orders.map(o => DATA.COMPONENT_TYPES[o].icon).join(' ')}</div>` : ''}`;
    },

    renderWorkshopContent() {
      const p = ui.engine.state.players[ui.currentViewPlayer];
      return `<div style="font-size:0.8rem;color:var(--text-dim)">트릭 ${p.tricks.length}/${DATA.STARTING_SETUP.maxTricks} | 컴포넌트 ${Object.values(p.components).reduce((a, b) => a + b, 0)}개</div>`;
    },

    renderTheaterSection() {
      const s = ui.engine.state;
      const cards = ui.engine.getPerformanceCards();
      let html = `<div class="theater-section"><h3>${SVG.theater} 극장</h3><div class="theater-weekdays">`;
      DATA.WEEKDAYS.forEach((day, di) => {
        const mod = DATA.WEEKDAY_MODIFIERS[day], slots = s.theater.characterSlots[day];
        const card = s.theater.performanceCardsByDay[day];
        const modText = mod.fameMod !== 0 ? ` (${mod.fameMod > 0 ? '+' : ''}${mod.fameMod}${SVG.fame})` : '';
        const ci = cards.findIndex(c => c.day === day);
        html += `<div class="weekday-col"><h4>${mod.name}${modText}</h4>`;
        if (card) {
          html += `<div class="perf-card ${ui.selectedPerfCard === ci ? 'selected' : ''}" onclick="window.gameUI.selectPerfCard(${ci})">
            <div class="perf-card-title">${card.name}</div>
            <div class="perf-card-bonus">${SVG.fame}${card.performerBonus.fame} ${SVG.coin}${card.performerBonus.coins}</div>
            <div class="perf-card-grid">${card.tricks.map((slot, si) => {
              const occ = slot && slot.playerId !== undefined;
              const lc = card.linkCircles[si];
              return `<div class="trick-slot ${occ ? 'occupied' : ''}" onclick="event.stopPropagation();window.gameUI.selectTrickSlot(${ci},${si})" style="${occ ? `border-color:${s.players[slot.playerId]?.color}` : ''}">
                ${occ ? `<span style="color:${s.players[slot.playerId]?.color}">${DATA.TRICK_CATEGORIES[slot.category]?.icon || '?'}</span>` : '<span class="trick-slot-info">빈</span>'}${lc?.hasShardSymbol ? `<span class="shard-indicator">${SVG.shard}</span>` : ''}</div>`;
            }).join('')}</div></div>`;
        } else { html += `<div class="perf-card empty" style="opacity:0.4;text-align:center;padding:16px">카드 없음</div>`; }
        html += `<div class="weekday-slot" onclick="window.gameUI.assignToTheaterDay('${day}','backstage',0)">${slots.backstage[0] !== null ? `${SVG.theater} P${slots.backstage[0] + 1}` : '백스테이지'}</div>
          <div class="weekday-slot" onclick="window.gameUI.assignToTheaterDay('${day}','backstage',1)">${slots.backstage[1] !== null ? `${SVG.theater} P${slots.backstage[1] + 1}` : '백스테이지'}</div>
          <div class="weekday-slot performance-slot" onclick="window.gameUI.assignToTheaterDay('${day}','performance')">${slots.performance !== null ? `${SVG.magician} P${slots.performance + 1}` : '공연 슬롯'}</div>
        </div>`;
      });
      html += '</div></div>';
      return html;
    },

    renderFameTrack() {
      const s = ui.engine.state;
      const maxFame = Math.max(...s.players.map(p => p.fame), 10);
      return `<div class="fame-track"><h4>${SVG.fame} 명성 트랙</h4>
        ${s.players.map(p => {
          const pct = maxFame > 0 ? Math.min((p.fame / maxFame) * 100, 100) : 0;
          return `<div class="fame-row">
            <span class="fame-name" style="color:${p.color}">${p.name}</span>
            <div class="fame-bar-bg"><div class="fame-bar" style="width:${pct}%;background:${p.color}"></div></div>
            <span class="fame-val">${p.fame}</span>
          </div>`;
        }).join('')}
      </div>`;
    },

    renderDice() { ui.renderAll(); }
  };
}
