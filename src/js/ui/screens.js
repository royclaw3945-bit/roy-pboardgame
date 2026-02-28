// Trickerion UI - Screen Management (Title, Setup, Game Over)
import * as DATA from '../gameData.js';
import { SVG, CHAR_SVG } from './constants.js';

function basicCompOptions() {
  return Object.entries(DATA.COMPONENT_TYPES).filter(([_, c]) => c.tier === 'basic')
    .map(([k, c]) => `<option value="${k}">${c.icon} ${c.name}</option>`).join('');
}

export function createScreenMethods(ui) {
  return {
    showScreen(screenId) {
      document.querySelectorAll('.screen').forEach(s => s.style.display = 'none');
      const el = document.getElementById(screenId);
      if (el) {
        el.style.display = screenId === 'title-screen' ? 'flex' : 'block';
        ui.animateScreenTransition(screenId);
      }
    },

    showTitle() {
      ui.showScreen('title-screen');
      ui.setupTitleParticles();
      setTimeout(() => ui.animateTitleEntrance(), 100);
    },

    showSetup() { ui.showScreen('setup-screen'); ui.renderSetup(); },

    renderSetup() {
      const container = document.getElementById('setup-players');
      const n = parseInt(document.getElementById('num-players')?.value || '2');
      const colors = ['var(--player1)', 'var(--player2)', 'var(--player3)', 'var(--player4)'];
      let html = '';
      for (let i = 0; i < n; i++) {
        html += `<div class="player-setup-card" style="border-top: 3px solid ${colors[i]}">
          <h3>플레이어 ${i + 1}</h3>
          <label>이름</label><input type="text" id="pname-${i}" value="플레이어 ${i + 1}" />
          <label>마법사</label>
          <select id="pmagician-${i}">${DATA.MAGICIANS.map(m =>
            `<option value="${m.id}">${m.name} (${DATA.TRICK_CATEGORIES[m.favoriteCategory].name})</option>`).join('')}
          </select>
          <div class="magician-preview" id="preview-${i}"></div>
          <label>시작 전문가</label>
          <select id="pspecialist-${i}">
            <option value="ENGINEER">🔧 기술자 - 추가 L1 트릭</option>
            <option value="MANAGER">💼 매니저 - 추가 컴포넌트</option>
            <option value="ASSISTANT">🤝 어시스턴트 - 추가 견습생</option>
          </select>
          <label>시작 트릭 (선호 카테고리 L1)</label>
          <select id="ptrick-${i}"></select>
          <label>시작 컴포넌트 (기본 2개)</label>
          <div style="display:flex;gap:8px">
            <select id="pcomp1-${i}">${basicCompOptions()}</select>
            <select id="pcomp2-${i}">${basicCompOptions()}</select>
          </div>
          <div id="spec-extra-${i}"></div>
        </div>`;
      }
      container.innerHTML = html;
      for (let i = 0; i < n; i++) {
        const mSel = document.getElementById(`pmagician-${i}`);
        mSel?.addEventListener('change', () => {
          ui.updateMagicianPreview(i); ui.updateTrickOptions(i);
          ui.updateSpecialistExtra(i); ui.syncMagicians(n);
        });
        document.getElementById(`pspecialist-${i}`)?.addEventListener('change', () => ui.updateSpecialistExtra(i));
        if (mSel && i < DATA.MAGICIANS.length) mSel.value = DATA.MAGICIANS[i].id;
        ui.updateMagicianPreview(i);
        ui.updateTrickOptions(i);
        ui.updateSpecialistExtra(i);
        const comp2 = document.getElementById(`pcomp2-${i}`);
        if (comp2) comp2.value = 'METAL';
      }
      ui.syncMagicians(n);
      if (ui.gsapAvailable()) gsap.fromTo('.player-setup-card',
        { y: 20, opacity: 0 }, { y: 0, opacity: 1, duration: 0.4, ease: 'power2.out', stagger: 0.1 });
    },

    updateTrickOptions(idx) {
      const magId = document.getElementById(`pmagician-${idx}`)?.value;
      const mag = DATA.MAGICIANS.find(m => m.id === magId);
      if (!mag) return;
      const tricks = DATA.TRICKS[mag.favoriteCategory].filter(t => t.level === 1);
      const sel = document.getElementById(`ptrick-${idx}`);
      if (sel) sel.innerHTML = tricks.map(t =>
        `<option value="${t.id}">${t.nameKo} - 명성${t.yields.fame} 코인${t.yields.coins} 샤드${t.yields.shards}</option>`).join('');
    },

    syncMagicians(n) {
      const selected = [];
      for (let i = 0; i < n; i++) selected.push(document.getElementById(`pmagician-${i}`)?.value);
      for (let i = 0; i < n; i++) {
        const sel = document.getElementById(`pmagician-${i}`);
        if (!sel) continue;
        Array.from(sel.options).forEach(opt => {
          opt.disabled = selected.includes(opt.value) && opt.value !== sel.value;
        });
      }
    },

    updateSpecialistExtra(idx) {
      const spec = document.getElementById(`pspecialist-${idx}`)?.value;
      const container = document.getElementById(`spec-extra-${idx}`);
      if (!container) return;
      if (spec === 'ENGINEER') {
        const magId = document.getElementById(`pmagician-${idx}`)?.value;
        const mag = DATA.MAGICIANS.find(m => m.id === magId);
        const otherCats = Object.keys(DATA.TRICKS).filter(c => !mag || c !== mag.favoriteCategory);
        const tricks = otherCats.flatMap(c => DATA.TRICKS[c].filter(t => t.level === 1));
        container.innerHTML = `<label>기술자 보너스 트릭 (타 카테고리 L1)</label>
          <select id="pengtrick-${idx}">${tricks.map(t =>
            `<option value="${t.id}">${DATA.TRICK_CATEGORIES[t.category].icon} ${t.nameKo}</option>`).join('')}</select>`;
      } else if (spec === 'MANAGER') {
        container.innerHTML = `<label>매니저 보너스 컴포넌트 (기본 2개)</label>
          <div style="display:flex;gap:8px"><select id="pmgcomp1-${idx}">${basicCompOptions()}</select>
            <select id="pmgcomp2-${idx}">${basicCompOptions()}</select></div>`;
      } else {
        container.innerHTML = '<p style="font-size:0.85rem;color:var(--text-dim);margin-top:8px">견습생 1명이 추가됩니다.</p>';
      }
    },

    updateMagicianPreview(playerIdx) {
      const sel = document.getElementById(`pmagician-${playerIdx}`);
      const preview = document.getElementById(`preview-${playerIdx}`);
      if (!sel || !preview) return;
      const magician = DATA.MAGICIANS.find(m => m.id === sel.value);
      if (!magician) return;
      const catInfo = DATA.TRICK_CATEGORIES[magician.favoriteCategory];
      const img = magician.img ? `<img src="${magician.img}" class="magician-portrait" alt="${magician.name}">` : `<span class="icon">${catInfo.icon}</span>`;
      preview.innerHTML = `${img}
        <div><div class="name" style="color:${magician.color}">${magician.fullName}</div>
        <div class="ability">${catInfo.name} 전문</div></div>`;
    },

    startGame() {
      const n = parseInt(document.getElementById('num-players')?.value || '2');
      const configs = [];
      for (let i = 0; i < n; i++) {
        const name = document.getElementById(`pname-${i}`)?.value || `플레이어 ${i + 1}`;
        const magicianId = document.getElementById(`pmagician-${i}`)?.value || DATA.MAGICIANS[i].id;
        const specialist = document.getElementById(`pspecialist-${i}`)?.value || 'ENGINEER';
        const startingTrickId = document.getElementById(`ptrick-${i}`)?.value || null;
        const comp1 = document.getElementById(`pcomp1-${i}`)?.value || 'WOOD';
        const comp2 = document.getElementById(`pcomp2-${i}`)?.value || 'METAL';
        let engineerTrickId = null, managerComponents = null;
        if (specialist === 'ENGINEER') engineerTrickId = document.getElementById(`pengtrick-${i}`)?.value || null;
        if (specialist === 'MANAGER') managerComponents = [
          document.getElementById(`pmgcomp1-${i}`)?.value || 'WOOD',
          document.getElementById(`pmgcomp2-${i}`)?.value || 'WOOD'];
        configs.push({ name, magicianId, startingSpecialist: specialist, startingTrickId,
          startingComponents: [comp1, comp2], engineerTrickId, managerComponents, isHuman: true });
      }
      ui.engine.initGame(configs);
      ui.currentViewPlayer = 0;
      ui.showScreen('game-screen');
      ui.switchToGameParticles();
      ui.renderAll();
    },

    showGameOver(data) {
      const { winner, rankings } = data;
      ui.triggerCelebrationParticles();
      const overlay = document.createElement('div');
      overlay.className = 'game-over-overlay';
      overlay.innerHTML = `
        <div class="game-over-card">
          <div class="crown">${SVG.crown}</div>
          <h1>게임 종료!</h1>
          <div class="winner-name" style="color:${winner.color}">${winner.name}</div>
          <p style="margin-bottom:20px">최종 명성: ${SVG.fame} ${winner.fame}</p>
          <div class="rankings">
            ${rankings.map((p, i) => `<div class="ranking-row">
              <span class="rank">${i + 1}위</span>
              <span class="name" style="color:${p.color}">${p.name} (${p.magician.name})</span>
              <span class="score">${SVG.fame}${p.fame} ${SVG.coin}${p.coins} ${SVG.shard}${p.shards}</span>
            </div>`).join('')}
          </div>
          <button class="btn btn-primary btn-lg btn-steam" onclick="location.reload()">
            ${SVG.play} 새 게임
          </button>
        </div>`;
      document.body.appendChild(overlay);
      if (ui.gsapAvailable()) {
        const tl = gsap.timeline();
        tl.fromTo(overlay, { opacity: 0 }, { opacity: 1, duration: 0.5 });
        tl.fromTo('.game-over-card', { scale: 0.7, y: 40, opacity: 0 },
          { scale: 1, y: 0, opacity: 1, duration: 0.6, ease: 'back.out(1.5)' }, '-=0.2');
        tl.fromTo('.game-over-card .crown', { scale: 0, rotation: -45 },
          { scale: 1, rotation: 0, duration: 0.5, ease: 'back.out(2)' }, '-=0.3');
        tl.fromTo('.ranking-row', { x: -30, opacity: 0 },
          { x: 0, opacity: 1, duration: 0.3, stagger: 0.1, ease: 'power2.out' }, '-=0.2');
      }
    },

    getPhaseName(phase) {
      const names = {
        'SETUP': '셋업', 'ROLL_DICE': '주사위 굴림', 'ADVERTISE': '광고',
        'ASSIGNMENT': '배정', 'PLACE_CHARACTERS': '캐릭터 배치',
        'PERFORMANCE': '퍼포먼스', 'END_TURN': '턴 종료', 'GAME_OVER': '게임 종료'
      };
      return names[phase] || phase;
    },

    getPhaseDescription(phase) {
      const descs = {
        'ADVERTISE': '코인을 지불하여 2 명성을 얻을 수 있습니다.',
        'ASSIGNMENT': '캐릭터를 각 장소에 배정하세요.',
        'PLACE_CHARACTERS': '이니셔티브 순서대로 캐릭터를 교대로 배치하고 액션을 수행하세요.',
        'PERFORMANCE': '극장에서 공연을 진행합니다.',
        'END_TURN': '급여 지급 및 턴 정리 중...',
        'GAME_OVER': '게임이 종료되었습니다!'
      };
      return descs[phase] || '';
    },

    showHandoverOverlay(player) {
      const overlay = document.createElement('div');
      overlay.className = 'handover-overlay';
      overlay.id = 'handover-overlay';
      overlay.innerHTML = `<div class="handover-card">
        <h2>🎩 기기를 넘겨주세요</h2>
        <div class="player-name" style="color:${player.color}">${player.name}</div>
        <p class="hint">의 배정 차례입니다.<br>다른 플레이어는 보지 마세요!</p>
        <button class="btn btn-primary btn-lg btn-steam" onclick="window.gameUI.confirmHandover()">
          ${SVG.play} 배정 시작
        </button>
      </div>`;
      document.body.appendChild(overlay);
      if (ui.gsapAvailable()) gsap.fromTo(overlay, { opacity: 0 }, { opacity: 1, duration: 0.3 });
    },

    confirmHandover() {
      const overlay = document.getElementById('handover-overlay');
      if (overlay) overlay.remove();
      const s = ui.engine.state;
      ui.currentViewPlayer = s.initiativeOrder[s.currentPlayerIdx];
      ui.renderAll();
    },

    showAssignmentReveal() {
      const s = ui.engine.state;
      const items = s.players.map(p => {
        const assigned = p.characters.filter(c => c.assigned).map(c =>
          `${DATA.CHARACTER_TYPES[c.type].icon} ${DATA.CHARACTER_TYPES[c.type].name} → ${DATA.LOCATIONS[c.location]?.icon || ''} ${DATA.LOCATIONS[c.location]?.name || '유휴'}`
        );
        const idle = p.characters.filter(c => !c.assigned).length;
        return `<div class="reveal-item" style="border-left-color:${p.color}">
          <div><strong style="color:${p.color}">${p.name}</strong>
            <div style="font-size:0.8rem;color:var(--text-dim)">${assigned.join(' / ') || '전부 유휴'}${idle > 0 ? ` (유휴 ${idle}명)` : ''}</div>
          </div></div>`;
      }).join('');
      const overlay = document.createElement('div');
      overlay.className = 'handover-overlay';
      overlay.id = 'reveal-overlay';
      overlay.innerHTML = `<div class="handover-card" style="max-width:500px">
        <h2>📋 배정 공개</h2>
        <p class="hint">모든 플레이어의 배정이 공개됩니다</p>
        <div class="reveal-list">${items}</div>
        <button class="btn btn-primary btn-lg btn-steam" onclick="window.gameUI.confirmReveal()">
          ${SVG.play} 캐릭터 배치 시작
        </button>
      </div>`;
      document.body.appendChild(overlay);
      if (ui.gsapAvailable()) {
        gsap.fromTo(overlay, { opacity: 0 }, { opacity: 1, duration: 0.3 });
        gsap.fromTo('.reveal-item', { x: -20, opacity: 0 },
          { x: 0, opacity: 1, duration: 0.3, stagger: 0.1, ease: 'power2.out', delay: 0.2 });
      }
    },

    confirmReveal() {
      const overlay = document.getElementById('reveal-overlay');
      if (overlay) overlay.remove();
      ui.engine.finishAssignmentPhase();
      ui.renderAll();
    }
  };
}
