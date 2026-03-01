'use client';

import { useGameStore } from '@/stores/game-store';
import { LOCATION_META, CHARACTER_META } from '@/core/data/constants';
import { GameIcon } from '../shared/GameIcon';
import type { Location } from '@/core/types';

const LOCATIONS: Location[] = ['DOWNTOWN', 'MARKET_ROW', 'WORKSHOP', 'THEATER', 'DARK_ALLEY'];

export function AssignmentPanel() {
  const state = useGameStore((s) => s.state);
  const dispatchAction = useGameStore((s) => s.dispatch);
  const finishAssignment = useGameStore((s) => s.finishAssignment);

  if (!state) return null;

  // ASSIGNMENT_REVEAL
  if (state.phase === 'ASSIGNMENT_REVEAL') {
    return (
      <div>
        <p style={{ fontSize: '0.85rem', color: 'var(--text-dim)', marginBottom: 12 }}>
          모든 배정이 공개되었습니다.
        </p>

        {state.players.map(p => (
          <div
            key={p.id}
            style={{
              padding: '8px 12px', marginBottom: 6,
              background: 'var(--bg-card)', borderRadius: 'var(--radius)',
              borderLeft: `3px solid ${p.color}`,
            }}
          >
            <span style={{ fontWeight: 700, color: p.color, fontFamily: 'var(--font-heading)', fontSize: '0.85rem' }}>
              {p.name}
            </span>
            <div style={{ display: 'flex', flexWrap: 'wrap', gap: 6, marginTop: 4 }}>
              {p.currentPlacements.map(pl => {
                const card = p.assignmentCards.find(c => c.id === pl.cardId);
                return (
                  <span
                    key={pl.cardId}
                    style={{
                      fontSize: '0.75rem', padding: '2px 8px',
                      background: 'var(--bg-secondary)', borderRadius: 'var(--radius)',
                      display: 'flex', alignItems: 'center', gap: 4,
                    }}
                  >
                    <GameIcon type={card?.location ?? 'DOWNTOWN'} size="xs" color="var(--text-dim)" />
                    {card ? LOCATION_META[card.location].name : '?'}
                    <span style={{ color: 'var(--cyan-light)' }}>
                      ({pl.characterIndices.map(i =>
                        CHARACTER_META[p.characters[i]?.type]?.name ?? '?',
                      ).join(', ')})
                    </span>
                  </span>
                );
              })}
            </div>
          </div>
        ))}

        <button onClick={finishAssignment} className="btn btn-primary" style={{ marginTop: 8 }}>
          배치 페이즈로
        </button>
      </div>
    );
  }

  // ASSIGNMENT phase
  const submitted = state.assignmentPhase?.playersSubmitted ?? [];
  const currentAssigner = state.assignmentPhase?.currentAssigner;
  const player = state.players.find(p => p.id === currentAssigner) ?? state.players[0];
  const isSubmitted = submitted.includes(player.id);
  const allSubmitted = submitted.length >= state.players.length;
  const usedCardIds = new Set(player.currentPlacements.map(p => p.cardId));

  return (
    <div>
      {/* Current player */}
      <div className="turn-indicator" style={{ background: `linear-gradient(135deg, ${player.color}, ${player.color}99)` }}>
        {player.name} {isSubmitted ? '- 제출 완료' : '- 배치카드 선택'}
      </div>

      {!isSubmitted && (
        <div className="assignment-grid" style={{ marginTop: 8 }}>
          {LOCATIONS.map(loc => {
            const cardsForLoc = player.assignmentCards.filter(c => c.location === loc);
            if (cardsForLoc.length === 0) return null;

            const placedCard = cardsForLoc.find(c => usedCardIds.has(c.id));
            const placement = placedCard
              ? player.currentPlacements.find(p => p.cardId === placedCard.id)
              : null;
            const availableCard = cardsForLoc.find(c => !usedCardIds.has(c.id));

            return (
              <div
                key={loc}
                style={{
                  background: placement ? 'rgba(34, 197, 94, 0.08)' : 'var(--bg-card)',
                  border: `1px solid ${placement ? 'var(--green)' : 'var(--border)'}`,
                  borderRadius: 'var(--radius)',
                  padding: '10px',
                  backgroundImage: `url(${LOCATION_META[loc].img})`,
                  backgroundSize: 'cover',
                  backgroundPosition: 'center',
                  backgroundBlendMode: 'overlay',
                  backgroundColor: 'rgba(10,10,26,0.82)',
                }}
              >
                <div style={{
                  display: 'flex', alignItems: 'center', gap: 6, marginBottom: 6,
                }}>
                  <GameIcon type={loc} size="sm" color="var(--gold-primary)" />
                  <span style={{
                    fontFamily: 'var(--font-heading)', fontWeight: 700,
                    fontSize: '0.82rem', color: 'var(--purple-light)',
                  }}>
                    {LOCATION_META[loc].name}
                  </span>
                </div>

                {placement ? (
                  <div style={{ fontSize: '0.78rem', color: 'var(--green)' }}>
                    {placement.characterIndices.map(i =>
                      CHARACTER_META[player.characters[i]?.type]?.name ?? '?',
                    ).join(', ')}
                  </div>
                ) : availableCard ? (
                  <div style={{ display: 'flex', flexWrap: 'wrap', gap: 4 }}>
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
                          className="action-btn"
                          style={{ width: 'auto', padding: '4px 8px', fontSize: '0.75rem' }}
                        >
                          <GameIcon type={char.type} size="xs" color="var(--text)" />
                          {CHARACTER_META[char.type].name}
                        </button>
                      );
                    })}
                  </div>
                ) : (
                  <span style={{ fontSize: '0.72rem', color: 'var(--text-dim)' }}>카드 없음</span>
                )}
              </div>
            );
          })}
        </div>
      )}

      {!isSubmitted && player.currentPlacements.length > 0 && (
        <button
          onClick={() => dispatchAction({ type: 'SUBMIT_ASSIGNMENT', playerId: player.id })}
          className="btn btn-primary"
          style={{ marginTop: 8, width: '100%' }}
        >
          배정 확정
        </button>
      )}

      {allSubmitted && (
        <button
          onClick={() => dispatchAction({ type: 'REVEAL_ASSIGNMENTS' })}
          className="btn btn-primary"
          style={{ marginTop: 8, width: '100%' }}
        >
          배정 공개
        </button>
      )}
    </div>
  );
}
