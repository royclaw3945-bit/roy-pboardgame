'use client';

import { useGameStore } from '@/stores/game-store';
import { LOCATION_META, CHARACTER_META } from '@/core/data/constants';
import type { Location } from '@/core/types';

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
        {state.players.map(p => (
          <div key={p.id} className="mb-1 text-xs">
            <span className="font-bold" style={{ color: p.color }}>{p.name}</span>
            {p.currentPlacements.map(pl => {
              const card = p.assignmentCards.find(c => c.id === pl.cardId);
              return (
                <span key={pl.cardId} className="ml-2 text-[var(--text-secondary)]">
                  {card ? LOCATION_META[card.location].name : '?'}
                  ({pl.characterIndices.map(i => CHARACTER_META[p.characters[i]?.type]?.name ?? '?').join(', ')})
                </span>
              );
            })}
          </div>
        ))}
        <button
          onClick={finishAssignment}
          className="mt-2 rounded bg-[var(--green)] px-4 py-2 text-sm font-bold hover:bg-green-700"
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

  // 위치별로 카드 그룹핑 (중복 위치는 첫 번째 미사용 카드만 표시)
  const usedCardIds = new Set(player.currentPlacements.map(p => p.cardId));

  return (
    <div>
      <h3 className="mb-2 text-sm font-bold">배정 페이즈 (비밀 배치)</h3>

      <p className="mb-1 text-xs">
        <span className="font-bold" style={{ color: player.color }}>{player.name}</span>
        {isSubmitted ? ' - 제출 완료' : ' - 배치카드 선택'}
      </p>

      {!isSubmitted && (
        <div className="mb-3">
          {/* 위치별로 표시 */}
          {LOCATIONS.filter(loc =>
            loc !== 'DARK_ALLEY' || state.config.useDarkAlley,
          ).map(loc => {
            const cardsForLoc = player.assignmentCards.filter(c => c.location === loc);
            if (cardsForLoc.length === 0) return null;

            // 이 위치에 배치된 카드 찾기
            const placedCard = cardsForLoc.find(c => usedCardIds.has(c.id));
            const placement = placedCard
              ? player.currentPlacements.find(p => p.cardId === placedCard.id)
              : null;

            // 미사용 카드 중 첫 번째
            const availableCard = cardsForLoc.find(c => !usedCardIds.has(c.id));

            return (
              <div key={loc} className="mb-1 flex items-center gap-2 text-xs">
                <span className={`w-16 ${placement ? 'text-green-400' : 'text-[var(--text-secondary)]'}`}>
                  {LOCATION_META[loc].name}
                </span>
                {placement ? (
                  <span className="text-green-400">
                    → {placement.characterIndices.map(i =>
                      CHARACTER_META[player.characters[i]?.type]?.name ?? '?',
                    ).join(', ')}
                  </span>
                ) : availableCard ? (
                  <div className="flex gap-1">
                    {player.characters.map((char, charIdx) => {
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
                            cardId: availableCard.id,
                            characterIndices: [charIdx],
                          })}
                          className="rounded bg-[var(--bg-dark)] px-2 py-0.5 text-[10px] hover:bg-[var(--bg-panel)]"
                        >
                          {CHARACTER_META[char.type].name}
                        </button>
                      );
                    })}
                  </div>
                ) : (
                  <span className="text-[var(--text-secondary)] text-[10px]">카드 없음</span>
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
