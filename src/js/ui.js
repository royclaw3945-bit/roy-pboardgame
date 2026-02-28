// ============================================================
// Trickerion: Legends of Illusion - UI Controller
// Modular Composition Architecture
// ============================================================

import { GameEngine } from './gameEngine.js';
import { createParticleMethods } from './ui/particles.js';
import { createAnimationMethods } from './ui/animations.js';
import { createScreenMethods } from './ui/screens.js';
import { createRenderMethods } from './ui/render.js';
import { createActionHandlers } from './ui/actions.js';
import { createModalMethods } from './ui/modals.js';
import { createSaveLoadMethods } from './ui/save-load.js';

export class GameUI {
  constructor() {
    this.engine = new GameEngine();
    this.currentViewPlayer = 0;
    this.selectedLocation = null;
    this.selectedAction = null;
    this.selectedTrick = null;
    this.selectedPerfCard = null;
    this.selectedSlot = null;
    this.modalStack = [];
    this.particlesInitialized = false;
    this.pendingSetupTrick = null;
    this._activeCharIdx = null;
    this._activePlayerId = null;
    this.currentSaveId = null;

    // Compose methods from sub-modules
    Object.assign(this, createParticleMethods(this));
    Object.assign(this, createAnimationMethods(this));
    Object.assign(this, createScreenMethods(this));
    Object.assign(this, createRenderMethods(this));
    Object.assign(this, createActionHandlers(this));
    Object.assign(this, createModalMethods(this));
    Object.assign(this, createSaveLoadMethods(this));

    this.setupEngineListeners();
    this.setupKeyboardShortcuts();
    this.initParticles();
  }

  setupEngineListeners() {
    this.engine.on('log', (msg) => this.appendLog(msg));
    this.engine.on('phaseChange', () => this.renderAll());
    this.engine.on('gameOver', (data) => this.showGameOver(data));
    this.engine.on('diceRolled', () => this.renderDice());
    this.engine.on('performanceDone', () => this.renderAll());
  }

  setupKeyboardShortcuts() {
    document.addEventListener('keydown', (e) => {
      if (e.ctrlKey && e.key === 'z') { e.preventDefault(); this.doUndo(); }
    });
  }
}
