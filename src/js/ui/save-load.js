// Trickerion UI - Save/Load System
import * as DB from '../supabase.js';
import { SVG } from './constants.js';

export function createSaveLoadMethods(ui) {
  return {
    currentSaveId: null,

    async showSaveModal() {
      if (!DB.isConfigured()) {
        ui.notify('Supabase가 설정되지 않았습니다. config.js를 확인하세요.', 'error');
        return;
      }
      const defaultName = `${ui.engine.state.players.map(p => p.name).join(' vs ')} - R${ui.engine.state.round}`;
      const content = `
        <div style="margin-bottom:16px">
          <label style="display:block;margin-bottom:4px;font-size:0.85rem;color:var(--text-dim)">세이브 이름</label>
          <input type="text" id="save-name-input" value="${defaultName}"
            style="width:100%;padding:8px 12px;background:var(--bg-deep);border:1px solid var(--border);border-radius:var(--radius);color:var(--text);font-size:0.9rem"/>
        </div>
        ${ui.currentSaveId ? `<p style="font-size:0.78rem;color:var(--cyan);margin-bottom:8px">현재 세이브에 덮어쓰기도 가능합니다.</p>` : ''}`;
      const actions = [
        { label: `${SVG.play} 새로 저장`, onclick: 'window.gameUI.doSaveNew()' }
      ];
      if (ui.currentSaveId) {
        actions.unshift({ label: `${SVG.play} 덮어쓰기`, onclick: 'window.gameUI.doSaveOverwrite()' });
      }
      ui.showModal(`${SVG.component} 게임 저장`, content, actions);
    },

    async doSaveNew() {
      const name = document.getElementById('save-name-input')?.value || '무제';
      ui.closeModal();
      ui.notify('저장 중...', 'info');
      const result = await DB.saveGame(name, ui.engine.state);
      if (result.success) {
        ui.currentSaveId = result.data.id;
        ui.notify('게임이 저장되었습니다!', 'success');
      } else {
        ui.notify(`저장 실패: ${result.message}`, 'error');
      }
    },

    async doSaveOverwrite() {
      if (!ui.currentSaveId) return;
      ui.closeModal();
      ui.notify('저장 중...', 'info');
      const result = await DB.updateSave(ui.currentSaveId, ui.engine.state);
      if (result.success) {
        ui.notify('세이브가 업데이트되었습니다!', 'success');
      } else {
        ui.notify(`저장 실패: ${result.message}`, 'error');
      }
    },

    async showLoadModal() {
      if (!DB.isConfigured()) {
        ui.notify('Supabase가 설정되지 않았습니다. config.js를 확인하세요.', 'error');
        return;
      }
      ui.notify('세이브 목록 불러오는 중...', 'info');
      const result = await DB.listSaves();
      if (!result.success) {
        ui.notify(`불러오기 실패: ${result.message}`, 'error');
        return;
      }
      const saves = result.data;
      if (saves.length === 0) {
        ui.notify('저장된 게임이 없습니다.', 'info');
        return;
      }
      const formatDate = (d) => {
        const dt = new Date(d);
        return `${dt.getMonth() + 1}/${dt.getDate()} ${dt.getHours()}:${String(dt.getMinutes()).padStart(2, '0')}`;
      };
      const content = `<div class="save-list" style="max-height:400px;overflow-y:auto">
        ${saves.map(s => `
          <div class="save-item" style="display:flex;justify-content:space-between;align-items:center;padding:10px 12px;background:var(--bg-card);border-radius:var(--radius);margin-bottom:6px;border:1px solid var(--border);cursor:pointer"
            onmouseover="this.style.borderColor='var(--accent)'" onmouseout="this.style.borderColor='var(--border)'"
            onclick="window.gameUI.doLoadGame('${s.id}')">
            <div>
              <div style="font-weight:700;margin-bottom:2px">${s.name}</div>
              <div style="font-size:0.75rem;color:var(--text-dim)">${s.player_names.join(', ')} | R${s.round} ${s.phase} | ${s.num_players}P</div>
            </div>
            <div style="display:flex;align-items:center;gap:8px">
              <span style="font-size:0.72rem;color:var(--text-dim)">${formatDate(s.updated_at)}</span>
              <button class="btn btn-sm" onclick="event.stopPropagation();window.gameUI.doDeleteSave('${s.id}')" style="color:var(--danger);border-color:var(--danger);padding:2px 6px;font-size:0.7rem">삭제</button>
            </div>
          </div>`).join('')}
      </div>`;
      ui.showModal(`${SVG.component} 게임 불러오기`, content);
    },

    async doLoadGame(saveId) {
      ui.closeModal();
      ui.notify('불러오는 중...', 'info');
      const result = await DB.loadGame(saveId);
      if (!result.success) {
        ui.notify(`불러오기 실패: ${result.message}`, 'error');
        return;
      }
      ui.engine.state = result.data.game_state;
      ui.engine.stateHistory = [];
      ui.currentSaveId = saveId;
      ui.currentViewPlayer = 0;
      ui.showScreen('game-screen');
      ui.switchToGameParticles();
      ui.renderAll();
      ui.notify(`"${result.data.name}" 게임을 불러왔습니다!`, 'success');
    },

    async doDeleteSave(saveId) {
      const result = await DB.deleteSave(saveId);
      if (result.success) {
        ui.notify('세이브가 삭제되었습니다.', 'success');
        if (ui.currentSaveId === saveId) ui.currentSaveId = null;
        ui.showLoadModal();
      } else {
        ui.notify(`삭제 실패: ${result.message}`, 'error');
      }
    },

    renderSaveButton() {
      if (!DB.isConfigured()) return '';
      return `<div class="player-tab" onclick="window.gameUI.showSaveModal()" style="cursor:pointer;border-color:var(--accent-dim);color:var(--accent-dim);font-size:0.75rem" title="게임 저장">💾 저장</div>`;
    }
  };
}
