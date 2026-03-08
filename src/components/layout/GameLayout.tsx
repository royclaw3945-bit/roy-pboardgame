'use client';

import { TopBar } from './TopBar';
import { BoardCenter } from '../board/BoardCenter';
import { PlayerBoard } from '../player/PlayerBoard';
import { ActionPanel } from '../actions/ActionPanel';
import { GameLog } from '../actions/GameLog';
import { ModalRouter } from '../modals/ModalRouter';
import { LocationPopover } from '../board/LocationPopover';

export function GameLayout() {
  return (
    <div
      className="game-layout-wrapper"
      style={{
        minHeight: '100vh',
        background: 'var(--bg-darkest)',
      }}
    >
      <TopBar />
      <div className="game-layout">
        <aside className="left-panel">
          <PlayerBoard />
        </aside>
        <main className="center-panel">
          <BoardCenter />
        </main>
        <aside className="right-panel">
          <div className="action-panel">
            <ActionPanel />
          </div>
          <div className="log-panel">
            <GameLog />
          </div>
        </aside>
      </div>

      <ModalRouter />
      <LocationPopover />
    </div>
  );
}
