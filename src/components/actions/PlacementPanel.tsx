'use client';

import { useGameStore } from '@/stores/game-store';
import {
  LOCATION_META, CHARACTER_META,
} from '@/core/data/constants';
import { getMagicianDef } from '@/core/data/magicians';
import { GameIcon } from '../shared/GameIcon';
import { CharInfoBar } from './placement-actions';
import type { Location } from '@/core/types';

/* ========== Main Component ========== */
export function PlacementPanel() {
  const state = useGameStore((s) => s.state);
  const dispatchAction = useGameStore((s) => s.dispatch);

  if (!state) return null;

  const turn = state.turnQueue[state.currentTurnIdx];
  if (!turn) {
    return (
      <div className="panel-section text-center text-dim" style={{ padding: 12 }}>
        <p>모든 캐릭터 배치가 완료되었습니다.</p>
      </div>
    );
  }

  const player = state.players.find((p) => p.id === turn.playerId);
  if (!player) return null;
  const mag = getMagicianDef(player.magicianId);

  const charIdx = turn.characterIdx;
  const char = charIdx >= 0 ? player.characters[charIdx] : null;
  const subPhase = charIdx === -1 ? 'CHOOSING' : !char?.placed ? 'PLACING' : 'ACTING';

  return (
    <div>
      <div className="turn-indicator" style={{ background: `linear-gradient(135deg, ${player.color}, ${player.color}aa)` }}>
        <img src={mag.img} alt="" style={{ width: 24, height: 24, borderRadius: '50%', objectFit: 'cover', border: '2px solid rgba(255,255,255,0.3)' }}
          onError={(e) => { (e.target as HTMLImageElement).style.display = 'none'; }} />
        {player.name}의 턴
      </div>

      {subPhase === 'CHOOSING' && (
        <CharacterSelector player={player} mag={mag}
          onSelect={(idx) => dispatchAction({ type: 'SELECT_CHARACTER', playerId: player.id, characterIdx: idx })}
          onPass={() => dispatchAction({ type: 'PASS_CHARACTER', playerId: player.id })}
        />
      )}

      {subPhase === 'PLACING' && char && (
        <PlacingGuide player={player} mag={mag} char={char} />
      )}

      {subPhase === 'ACTING' && char && (
        <ActingGuide player={player} mag={mag} char={char} dispatch={dispatchAction} />
      )}
    </div>
  );
}

/* ========== Character Selection (unchanged) ========== */
function CharacterSelector({ player, mag, onSelect, onPass }: {
  player: any; mag: any; onSelect: (idx: number) => void; onPass: () => void;
}) {
  const assignedChars = player.characters
    .map((c: any, i: number) => ({ char: c, idx: i }))
    .filter(({ char }: any) => char.assigned && !char.placed);

  return (
    <div className="action-group">
      <div className="action-group-title">배치할 캐릭터를 선택하세요</div>
      <div className="character-list">
        {assignedChars.map(({ char, idx }: any) => (
          <button key={idx} onClick={() => onSelect(idx)} className="action-btn"
            style={{ borderLeft: `3px solid ${player.color}` }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
              <GameIcon type={char.type} size="sm" color={player.color} />
              <div style={{ textAlign: 'left' }}>
                <div className="trick-name">{CHARACTER_META[char.type as keyof typeof CHARACTER_META].name}</div>
                {char.location && (
                  <span className="trick-level">
                    <GameIcon type={char.location} size="xs" />
                    {' '}{LOCATION_META[char.location as Location].name}
                    {' '}<span className="text-cyan">AP: {CHARACTER_META[char.type as keyof typeof CHARACTER_META].baseAP}</span>
                  </span>
                )}
              </div>
            </div>
          </button>
        ))}
      </div>
      <button onClick={onPass} className="btn" style={{ width: '100%', marginTop: 8, opacity: 0.7 }}>
        이 턴 패스
      </button>
    </div>
  );
}

/* ========== PLACING sub-phase: guide message ========== */
function PlacingGuide({ player, mag, char }: { player: any; mag: any; char: any }) {
  const location = char.location as Location;
  const locName = LOCATION_META[location]?.name ?? location;

  return (
    <div className="action-group">
      <CharInfoBar player={player} mag={mag} char={char} />
      <div className="placement-guide">
        <GameIcon type={location} size="md" color="var(--loc-color)" />
        <div>
          <div className="placement-guide-title">보드에서 <strong>{locName}</strong> 구역을 클릭하세요</div>
          <div className="placement-guide-sub">슬롯을 선택하여 캐릭터를 배치합니다</div>
        </div>
      </div>
    </div>
  );
}

/* ========== ACTING sub-phase: summary + guide ========== */
function ActingGuide({ player, mag, char, dispatch }: {
  player: any; mag: any; char: any; dispatch: (action: any) => any;
}) {
  const location = char.location as Location;
  const locName = LOCATION_META[location]?.name ?? location;

  return (
    <div className="action-group">
      <CharInfoBar player={player} mag={mag} char={char} ap={char.ap} />

      {char.ap > 0 ? (
        <div className="placement-guide">
          <GameIcon type={location} size="md" color="var(--loc-color)" />
          <div>
            <div className="placement-guide-title">보드에서 <strong>{locName}</strong> 구역을 클릭하세요</div>
            <div className="placement-guide-sub">액션을 선택하여 AP를 사용합니다</div>
          </div>
        </div>
      ) : (
        <div className="text-dim" style={{ fontSize: '0.82rem', padding: '8px 0' }}>
          남은 AP가 없습니다
        </div>
      )}

      {player.shards > 0 && !char.shardConverted && location !== 'THEATER' && (
        <button onClick={() => dispatch({ type: 'CONVERT_SHARD', playerId: player.id })}
          className="action-btn" style={{ marginTop: 8 }}>
          샤드 변환 (+1 AP) -- 보유 샤드: {player.shards}
          <span className="ap-cost">FREE</span>
        </button>
      )}

      <button onClick={() => dispatch({ type: 'FINISH_ACTIONS', playerId: player.id })}
        className="btn btn-primary" style={{ width: '100%', marginTop: 8 }}>
        {char.ap > 0 ? `턴 종료 (남은 AP: ${char.ap})` : '다음 턴'}
      </button>
    </div>
  );
}
