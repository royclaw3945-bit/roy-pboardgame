'use client';

import { TopBar } from './TopBar';
import { BoardCenter } from '../board/BoardCenter';
import { PlayerBoard } from '../player/PlayerBoard';
import { ActionPanel } from '../actions/ActionPanel';
import { GameLog } from '../actions/GameLog';
import { ModalRouter } from '../modals/ModalRouter';

export function GameLayout() {
  return (
    <div className="flex min-h-screen flex-col gap-2 p-2">
      <TopBar />
      <div className="flex flex-1 gap-2">
        <div className="flex-1 flex flex-col gap-2">
          <BoardCenter />
          <ActionPanel />
        </div>
        <div className="w-80 flex flex-col gap-2">
          <PlayerBoard />
          <GameLog />
        </div>
      </div>
      <ModalRouter />
    </div>
  );
}
