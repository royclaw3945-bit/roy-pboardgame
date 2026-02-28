// Trickerion: Legends of Illusion - Game Engine
// Composes sub-modules: core, actions, performance
import { createCoreMethods } from './engine/engine-core.js';
import { createActionMethods } from './engine/engine-actions.js';
import { createPerfMethods } from './engine/engine-perf.js';

export class GameEngine {
  constructor() {
    this.state = null;
    this.listeners = [];
    this.stateHistory = [];
    this.maxHistory = 30;

    // Compose methods from sub-modules
    Object.assign(this, createCoreMethods(this));
    Object.assign(this, createActionMethods(this));
    Object.assign(this, createPerfMethods(this));
  }

  on(event, callback) {
    this.listeners.push({ event, callback });
  }

  emit(event, data) {
    this.listeners
      .filter(l => l.event === event)
      .forEach(l => l.callback(data));
  }

  saveState() {
    if (!this.state) return;
    this.stateHistory.push(JSON.parse(JSON.stringify(this.state)));
    if (this.stateHistory.length > this.maxHistory) this.stateHistory.shift();
  }

  undo() {
    if (this.stateHistory.length === 0) return false;
    this.state = this.stateHistory.pop();
    this.emit('stateRestored', this.state);
    return true;
  }

  canUndo() {
    return this.stateHistory.length > 0;
  }
}
