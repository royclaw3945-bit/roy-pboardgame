'use client';

import { useEffect, useCallback, useRef } from 'react';
import { useUIStore } from '@/stores/ui-store';
import { useGameStore } from '@/stores/game-store';
import { LOCATION_META } from '@/core/data/constants';
import { getMagicianDef } from '@/core/data/magicians';
import { GameIcon } from '../shared/GameIcon';
import {
  SlotSelector, TheaterWeekdaySelector, LocationActions, CharInfoBar,
} from '../actions/placement-actions';
import type { Location } from '@/core/types';

export function LocationPopover() {
  const popoverLocation = useUIStore((s) => s.popoverLocation);
  const anchorRect = useUIStore((s) => s.popoverAnchorRect);
  const closePopover = useUIStore((s) => s.closeLocationPopover);

  const state = useGameStore((s) => s.state);
  const dispatch = useGameStore((s) => s.dispatch);

  const popoverRef = useRef<HTMLDivElement>(null);

  // ESC key to close
  const handleKeyDown = useCallback((e: KeyboardEvent) => {
    if (e.key === 'Escape') closePopover();
  }, [closePopover]);

  useEffect(() => {
    if (popoverLocation) {
      document.addEventListener('keydown', handleKeyDown);
      return () => document.removeEventListener('keydown', handleKeyDown);
    }
  }, [popoverLocation, handleKeyDown]);

  // Close when sub-phase changes
  const turnKey = state
    ? `${state.currentTurnIdx}-${state.turnQueue[state.currentTurnIdx]?.characterIdx}`
    : '';
  useEffect(() => {
    closePopover();
  }, [turnKey]); // eslint-disable-line react-hooks/exhaustive-deps

  if (!popoverLocation || !anchorRect || !state) return null;

  const meta = LOCATION_META[popoverLocation];
  const turn = state.turnQueue[state.currentTurnIdx];
  if (!turn) return null;

  const player = state.players.find((p) => p.id === turn.playerId);
  if (!player) return null;
  const mag = getMagicianDef(player.magicianId);

  const charIdx = turn.characterIdx;
  const char = charIdx >= 0 ? player.characters[charIdx] : null;
  const subPhase = charIdx === -1 ? 'CHOOSING' : !char?.placed ? 'PLACING' : 'ACTING';

  // Compute popover position: prefer right of anchor, fall back left
  const popoverWidth = 340;
  const popoverMaxHeight = window.innerHeight * 0.7;
  let top = anchorRect.top;
  let left = anchorRect.left + anchorRect.width + 8;

  // If overflows right, place to left
  if (left + popoverWidth > window.innerWidth - 16) {
    left = anchorRect.left - popoverWidth - 8;
  }
  // If overflows left, center horizontally over anchor
  if (left < 16) {
    left = Math.max(16, anchorRect.left + anchorRect.width / 2 - popoverWidth / 2);
  }
  // Clamp vertical
  if (top + popoverMaxHeight > window.innerHeight - 16) {
    top = Math.max(16, window.innerHeight - popoverMaxHeight - 16);
  }

  // Only show popover for the matching location
  const isRelevant =
    (subPhase === 'PLACING' && char?.location === popoverLocation) ||
    (subPhase === 'ACTING' && char?.location === popoverLocation);

  if (!isRelevant) {
    return null;
  }

  return (
    <>
      {/* Backdrop */}
      <div className="popover-backdrop" onClick={closePopover} />

      {/* Popover */}
      <div
        ref={popoverRef}
        className="location-popover"
        style={{ top, left, width: popoverWidth, maxHeight: popoverMaxHeight }}
      >
        {/* Header */}
        <div className="location-popover-header" data-loc={popoverLocation}>
          <GameIcon type={popoverLocation} size="sm" color="var(--loc-color)" />
          <span className="location-popover-title">{meta.name}</span>
          <button className="location-popover-close" onClick={closePopover}>
            &times;
          </button>
        </div>

        {/* Body */}
        <div className="location-popover-body">
          {subPhase === 'PLACING' && char && (
            char.location === 'THEATER' ? (
              <TheaterWeekdaySelector
                player={player} mag={mag} char={char} charIdx={charIdx}
                state={state} dispatch={dispatch} compact
              />
            ) : (
              <SlotSelector
                player={player} mag={mag} char={char} charIdx={charIdx} state={state}
                onPlace={(slotIndex) => dispatch({
                  type: 'PLACE_CHARACTER', playerId: player.id,
                  characterIdx: charIdx, slotIndex,
                })}
                compact
              />
            )
          )}

          {subPhase === 'ACTING' && char && (
            <div>
              <CharInfoBar player={player} mag={mag} char={char} ap={char.ap} />

              {player.shards > 0 && !char.shardConverted && char.location !== 'THEATER' && (
                <button
                  onClick={() => dispatch({ type: 'CONVERT_SHARD', playerId: player.id })}
                  className="action-btn" style={{ marginBottom: 6 }}>
                  샤드 변환 (+1 AP) -- 보유 샤드: {player.shards}
                  <span className="ap-cost">FREE</span>
                </button>
              )}

              {char.ap > 0 && (
                <LocationActions
                  location={popoverLocation}
                  player={player} state={state} dispatch={dispatch}
                />
              )}

              <button
                onClick={() => dispatch({ type: 'FINISH_ACTIONS', playerId: player.id })}
                className="btn btn-primary" style={{ width: '100%', marginTop: 8 }}>
                {char.ap > 0 ? `턴 종료 (남은 AP: ${char.ap})` : '다음 턴'}
              </button>
            </div>
          )}
        </div>
      </div>
    </>
  );
}
