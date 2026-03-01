'use client';

import { TopBar } from './TopBar';
import { BoardCenter } from '../board/BoardCenter';
import { PlayerBoard } from '../player/PlayerBoard';
import { ActionPanel } from '../actions/ActionPanel';
import { GameLog } from '../actions/GameLog';
import { ModalRouter } from '../modals/ModalRouter';

export function GameLayout() {
  return (
    <div
      className="game-layout-wrapper"
      style={{
        minHeight: '100vh',
        backgroundImage: 'url(/img/bg_magoria.jpg)',
        backgroundSize: 'cover',
        backgroundPosition: 'center',
        backgroundAttachment: 'fixed',
      }}
    >
      {/* Dark overlay */}
      <div
        style={{
          position: 'fixed',
          inset: 0,
          background: 'rgba(10, 10, 26, 0.82)',
          pointerEvents: 'none',
          zIndex: 0,
        }}
      />

      <div style={{ position: 'relative', zIndex: 1, display: 'flex', flexDirection: 'column', minHeight: '100vh' }}>
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
      </div>

      <ModalRouter />
    </div>
  );
}
