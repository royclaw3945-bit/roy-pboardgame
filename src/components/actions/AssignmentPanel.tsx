'use client';

import { useGameStore } from '@/stores/game-store';
import { LOCATION_META } from '@/core/data/constants';
import type { PlayerId, Location } from '@/core/types';

const LOCATIONS: Location[] = ['DOWNTOWN', 'MARKET_ROW', 'WORKSHOP', 'THEATER', 'DARK_ALLEY'];

export function AssignmentPanel() {
  const state = useGameStore((s) => s.state);
  const dispatchAction = useGameStore((s) => s.dispatch);
  const finishAssignment = useGameStore((s) => s.finishAssignment);

  if (!state) return null;

  // v3: ASSIGNMENT_REVEAL → advance to PLACEMENT
  if (state.phase === 'ASSIGNMENT_REVEAL') {
    return (
      <div>
        <h3 className="mb-2 text-sm font-bold">배정 공개 완료</h3>
        <p className="mb-2 text-xs text-[var(--text-secondary)]">
          모든 플레이어의 배정이 공개되었습니다.
        </p>
        <button
          onClick={finishAssignment}
          className="rounded bg-[var(--green)] px-4 py-2 text-sm font-bold hover:bg-green-700"
        >
          배치 페이즈로 →
        </button>
      </div>
    );
  }

  // v3: ASSIGNMENT phase — simultaneous blind placement
  const submitted = state.assignmentPhase?.playersSubmitted ?? [];
  const currentAssigner = state.assignmentPhase?.currentAssigner;
  const player = state.players.find(p => p.id === currentAssigner) ?? state.players[0];
  const isSubmitted = submitted.includes(player.id);
  const allSubmitted = submitted.length >= state.players.length;

  return (
    <div>
      <h3 className="mb-2 text-sm font-bold">배정 페이즈 (비밀 배치)</h3>

      <p className="mb-1 text-xs">
        <span className="font-bold" style={{ color: player.color }}>{player.name}</span>
        {isSubmitted ? ' - 제출 완료' : ' - 배치카드 선택'}
      </p>

      {!isSubmitted && (
        <div className="mb-3">
          {/* Show available assignment cards */}
          {player.assignmentCards
            .filter(card =>
              card.location !== 'DARK_ALLEY' || state.config.useDarkAlley,
            )
            .map((card) => {
              const placed = player.currentPlacements.find(p => p.cardId === card.id);
              return (
                <div key={card.id} className="mb-1 flex items-center gap-2 text-xs">
                  <span className={placed ? 'text-green-400' : 'text-[var(--text-secondary)]'}>
                    {LOCATION_META[card.location].name}
                  </span>
                  {placed ? (
                    <span className="text-green-400">
                      (캐릭터: {placed.characterIndices.map(i => player.characters[i]?.type).join(', ')})
                    </span>
                  ) : (
                    <div className="flex gap-1">
                      {player.characters.map((char, charIdx) => {
                        if (char.assigned) return null;
                        const alreadyPlaced = player.currentPlacements.some(
                          p => p.characterIndices.includes(charIdx),
                        );
                        if (alreadyPlaced) return null;
                        return (
                          <button
                            key={charIdx}
                            onClick={() => dispatchAction({
                              type: 'PLACE_ASSIGNMENT_CARD',
                              playerId: player.id,
                              cardId: card.id,
                              characterIndices: [charIdx],
                            })}
                            className="rounded bg-[var(--bg-dark)] px-2 py-0.5 text-[10px] hover:bg-[var(--bg-panel)]"
                          >
                            {char.type}
                          </button>
                        );
                      })}
                    </div>
                  )}
                </div>
              );
            })}

          {player.currentPlacements.length > 0 && (
            <button
              onClick={() => dispatchAction({
                type: 'SUBMIT_ASSIGNMENT',
                playerId: player.id,
              })}
              className="mt-2 rounded bg-[var(--blue)] px-3 py-1 text-xs font-bold hover:bg-blue-700"
            >
              배정 확정
            </button>
          )}
        </div>
      )}

      {allSubmitted && (
        <button
          onClick={() => dispatchAction({ type: 'REVEAL_ASSIGNMENTS' })}
          className="rounded bg-[var(--green)] px-4 py-2 text-sm font-bold hover:bg-green-700"
        >
          배정 공개
        </button>
      )}
    </div>
  );
}
