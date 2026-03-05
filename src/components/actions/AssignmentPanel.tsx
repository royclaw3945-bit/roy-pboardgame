'use client';

import { useGameStore } from '@/stores/game-store';
import { LOCATION_META, CHARACTER_META } from '@/core/data/constants';
import { getMagicianDef } from '@/core/data/magicians';
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
      <div className="turn-indicator" style={{ background: `linear-gradient(135deg, ${player.color}, ${player.color}aa)` }}>
        <img src={getMagicianDef(player.magicianId).img} alt="" style={{ width: 24, height: 24, borderRadius: '50%', objectFit: 'cover', border: '2px solid rgba(255,255,255,0.3)' }}
          onError={(e) => { (e.target as HTMLImageElement).style.display = 'none'; }} />
        {player.name} {isSubmitted ? '- 제출 완료' : '- 배치카드 선택'}
      </div>

      {!isSubmitted && (
        <div className="assignment-grid" style={{ marginTop: 8 }}>
          {LOCATIONS.map(loc => {
            const cardsForLoc = player.assignmentCards.filter(c => c.location === loc);
            if (cardsForLoc.length === 0) return null;

            const hasPlacement = cardsForLoc.some(c => usedCardIds.has(c.id));
            return (
              <div
                key={loc}
                style={{
                  borderRadius: 'var(--radius)',
                  padding: '10px',
                  backgroundImage: `url(${LOCATION_META[loc].img})`,
                  backgroundSize: 'cover',
                  backgroundPosition: 'center',
                  backgroundBlendMode: 'overlay',
                  backgroundColor: 'rgba(10,10,26,0.82)',
                  border: `2px solid ${hasPlacement ? 'var(--green)' : 'var(--border)'}`,
                }}
              >
                <div style={{
                  display: 'flex', alignItems: 'center', gap: 6, marginBottom: 6,
                  padding: '4px 8px',
                  background: 'rgba(0,0,0,0.6)',
                  borderRadius: 'var(--radius)',
                  borderBottom: '2px solid var(--gold-primary)',
                }}>
                  <GameIcon type={loc} size="sm" color="var(--gold-primary)" />
                  <span style={{
                    fontFamily: 'var(--font-heading)', fontWeight: 700,
                    fontSize: '0.9rem', color: 'var(--gold-primary)',
                  }}>
                    {LOCATION_META[loc].name}
                  </span>
                  {cardsForLoc.length > 1 && (
                    <span style={{
                      fontSize: '0.72rem', color: '#fff',
                      background: 'var(--purple-light)', borderRadius: '4px',
                      padding: '0 6px', fontWeight: 700,
                    }}>
                      ×{cardsForLoc.length}
                    </span>
                  )}
                </div>

                {/* Render each card separately */}
                <div style={{ display: 'flex', flexDirection: 'column', gap: 6 }}>
                  {cardsForLoc.map((card, cardLocalIdx) => {
                    const placement = player.currentPlacements.find(p => p.cardId === card.id);
                    const isUsed = usedCardIds.has(card.id);

                    if (placement) {
                      // Card already assigned — show assigned characters + remove button
                      return (
                        <div key={card.id} style={{
                          display: 'flex', alignItems: 'center', justifyContent: 'space-between',
                          padding: '4px 8px', background: 'rgba(34, 197, 94, 0.1)',
                          borderRadius: 'var(--radius)', border: '1px solid var(--green)',
                        }}>
                          <div style={{ fontSize: '0.78rem', color: 'var(--green)', display: 'flex', alignItems: 'center', gap: 4 }}>
                            {cardsForLoc.length > 1 && (
                              <span style={{ fontSize: '0.65rem', color: 'var(--text-dim)' }}>#{cardLocalIdx + 1}</span>
                            )}
                            {placement.characterIndices.map(i =>
                              CHARACTER_META[player.characters[i]?.type]?.name ?? '?',
                            ).join(', ')}
                          </div>
                          <button
                            onClick={() => dispatchAction({
                              type: 'REMOVE_ASSIGNMENT_CARD',
                              playerId: player.id,
                              cardId: card.id,
                            })}
                            className="btn btn-sm"
                            style={{ padding: '2px 6px', fontSize: '0.65rem', opacity: 0.7 }}
                          >
                            취소
                          </button>
                        </div>
                      );
                    }

                    // Card available — show character buttons
                    const availableChars: { char: { type: string }; idx: number }[] = player.characters
                      .map((char: any, idx: number) => ({ char, idx }))
                      .filter(({ idx }: { idx: number }) => !player.currentPlacements.some(
                        (p: any) => p.characterIndices.includes(idx),
                      ));

                    if (availableChars.length === 0) return null;

                    return (
                      <div key={card.id} style={{
                        padding: '6px 8px',
                        background: 'rgba(0,0,0,0.3)',
                        borderRadius: 'var(--radius)',
                        border: '1px dashed var(--border)',
                      }}>
                        <div style={{
                          fontSize: '0.72rem', color: 'var(--cyan-light)', marginBottom: 4,
                          fontWeight: 700,
                        }}>
                          {cardsForLoc.length > 1
                            ? `${LOCATION_META[loc].name} #${cardLocalIdx + 1} — 캐릭터 배정:`
                            : `${LOCATION_META[loc].name} — 캐릭터 배정:`}
                        </div>
                        <div style={{ display: 'flex', flexWrap: 'wrap', gap: 4 }}>
                          {availableChars.map(({ char, idx }) => (
                            <button
                              key={idx}
                              onClick={() => dispatchAction({
                                type: 'PLACE_ASSIGNMENT_CARD',
                                playerId: player.id,
                                cardId: card.id,
                                characterIndices: [idx],
                              })}
                              className="action-btn"
                              style={{ width: 'auto', padding: '4px 8px', fontSize: '0.75rem' }}
                            >
                              <GameIcon type={char.type} size="xs" color="var(--text)" />
                              {CHARACTER_META[char.type as keyof typeof CHARACTER_META].name}
                            </button>
                          ))}
                        </div>
                      </div>
                    );
                  })}
                </div>
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
