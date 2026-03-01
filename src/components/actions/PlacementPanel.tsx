'use client';

import { useGameStore } from '@/stores/game-store';
import { LOCATION_META, CHARACTER_META, COMPONENT_META } from '@/core/data/constants';
import { getMagicianDef } from '@/core/data/magicians';
import { GameIcon } from '../shared/GameIcon';
import type { Location, PlayerId, ComponentType, TrickCategory } from '@/core/types';

export function PlacementPanel() {
  const state = useGameStore((s) => s.state);
  const dispatchAction = useGameStore((s) => s.dispatch);

  if (!state) return null;

  const turn = state.turnQueue[state.currentTurnIdx];
  if (!turn) {
    return (
      <div style={{ padding: 12, textAlign: 'center' }}>
        <p style={{ fontSize: '0.85rem', color: 'var(--text-dim)' }}>
          모든 캐릭터 배치가 완료되었습니다.
        </p>
      </div>
    );
  }

  const player = state.players.find((p) => p.id === turn.playerId);
  if (!player) return null;
  const mag = getMagicianDef(player.magicianId);

  // Determine sub-phase
  const charIdx = turn.characterIdx;
  const char = charIdx >= 0 ? player.characters[charIdx] : null;
  const subPhase = charIdx === -1 ? 'CHOOSING' : !char?.placed ? 'PLACING' : 'ACTING';

  return (
    <div>
      {/* Current turn header */}
      <div className="turn-indicator" style={{
        background: `linear-gradient(135deg, ${player.color}, ${player.color}99)`,
      }}>
        {player.name}의 턴
      </div>

      {subPhase === 'CHOOSING' && (
        <CharacterSelector
          player={player}
          mag={mag}
          onSelect={(idx) => dispatchAction({
            type: 'SELECT_CHARACTER',
            playerId: player.id,
            characterIdx: idx,
          })}
          onPass={() => dispatchAction({ type: 'PASS_CHARACTER', playerId: player.id })}
        />
      )}

      {subPhase === 'PLACING' && char && (
        <SlotSelector
          player={player}
          mag={mag}
          char={char}
          charIdx={charIdx}
          state={state}
          onPlace={(slotIndex) => dispatchAction({
            type: 'PLACE_CHARACTER',
            playerId: player.id,
            characterIdx: charIdx,
            slotIndex,
          })}
        />
      )}

      {subPhase === 'ACTING' && char && (
        <ActionPhase
          player={player}
          mag={mag}
          char={char}
          charIdx={charIdx}
          state={state}
          dispatch={dispatchAction}
        />
      )}
    </div>
  );
}

/* ─── Character Selection ─── */
function CharacterSelector({ player, mag, onSelect, onPass }: {
  player: any;
  mag: any;
  onSelect: (idx: number) => void;
  onPass: () => void;
}) {
  const assignedChars = player.characters
    .map((c: any, i: number) => ({ char: c, idx: i }))
    .filter(({ char }: any) => char.assigned && !char.placed);

  return (
    <div style={{ marginTop: 8 }}>
      <div style={{
        fontSize: '0.85rem', color: 'var(--text-dim)', marginBottom: 8,
        fontFamily: 'var(--font-heading)',
      }}>
        배치할 캐릭터를 선택하세요
      </div>

      <div style={{ display: 'flex', flexDirection: 'column', gap: 6 }}>
        {assignedChars.map(({ char, idx }: any) => (
          <button
            key={idx}
            onClick={() => onSelect(idx)}
            className="btn"
            style={{
              display: 'flex', alignItems: 'center', gap: 8, padding: '10px 12px',
              justifyContent: 'flex-start', border: `1px solid ${player.color}40`,
            }}
          >
            <GameIcon type={char.type} size="sm" color={player.color} />
            <div style={{ textAlign: 'left' }}>
              <div style={{ fontWeight: 700, fontSize: '0.9rem' }}>
                {CHARACTER_META[char.type as keyof typeof CHARACTER_META].name}
              </div>
              {char.location && (
                <div style={{ fontSize: '0.72rem', color: 'var(--text-dim)', display: 'flex', alignItems: 'center', gap: 3 }}>
                  <GameIcon type={char.location} size="xs" />
                  {LOCATION_META[char.location as Location].name}
                  <span style={{ marginLeft: 4, color: 'var(--cyan-light)' }}>
                    AP: {CHARACTER_META[char.type as keyof typeof CHARACTER_META].baseAP}
                  </span>
                </div>
              )}
            </div>
          </button>
        ))}
      </div>

      <button onClick={onPass} className="btn" style={{ marginTop: 8, width: '100%', opacity: 0.7 }}>
        이 턴 패스
      </button>
    </div>
  );
}

/* ─── Slot Selection ─── */
function SlotSelector({ player, mag, char, charIdx, state, onPlace }: {
  player: any; mag: any; char: any; charIdx: number; state: any;
  onPlace: (slotIndex: number) => void;
}) {
  const location = char.location as Location;
  const slots = location ? state.locationSlots[location] : [];

  return (
    <div style={{ marginTop: 8 }}>
      {/* Character info */}
      <div style={{
        display: 'flex', alignItems: 'center', gap: 10, marginBottom: 12,
        padding: '10px 12px', background: 'var(--bg-card)', borderRadius: 'var(--radius)',
        border: `1px solid ${player.color}30`,
      }}>
        <img
          src={mag.img} alt={mag.nameKo}
          style={{ width: 32, height: 32, borderRadius: '50%', objectFit: 'cover', border: `2px solid ${player.color}` }}
          onError={(e) => { (e.target as HTMLImageElement).style.display = 'none'; }}
        />
        <div>
          <div style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
            <GameIcon type={char.type} size="sm" color={player.color} />
            <span style={{ fontWeight: 700, fontSize: '0.9rem' }}>
              {CHARACTER_META[char.type as keyof typeof CHARACTER_META].name}
            </span>
          </div>
          {location && (
            <div style={{ fontSize: '0.78rem', color: 'var(--text-dim)', display: 'flex', alignItems: 'center', gap: 4 }}>
              <GameIcon type={location} size="xs" />
              {LOCATION_META[location].name}
            </div>
          )}
        </div>
      </div>

      {/* Slot selection */}
      <div style={{
        fontSize: '0.85rem', color: 'var(--text-dim)', marginBottom: 6,
        fontFamily: 'var(--font-heading)',
      }}>
        슬롯을 선택하세요
      </div>
      <div style={{ display: 'flex', gap: 6, flexWrap: 'wrap' }}>
        {slots.map((slot: any, i: number) => {
          const occupied = !!slot.occupant;
          return (
            <button
              key={i}
              onClick={() => !occupied && onPlace(i)}
              disabled={occupied}
              className={`worker-slot ${occupied ? 'occupied' : ''}`}
              style={{
                width: 52, height: 52, cursor: occupied ? 'not-allowed' : 'pointer',
                ...(occupied && { opacity: 0.4 }),
              }}
            >
              {occupied ? (
                <span style={{ fontSize: '0.6rem', color: 'var(--red)' }}>X</span>
              ) : (
                <div style={{ textAlign: 'center' }}>
                  <span className="ws-ap" style={{ position: 'static', fontSize: '0.7rem' }}>
                    {slot.apMod > 0 ? `+${slot.apMod}` : slot.apMod}
                  </span>
                  <div style={{ fontSize: '0.55rem', color: 'var(--text-dim)' }}>AP</div>
                </div>
              )}
            </button>
          );
        })}
      </div>
    </div>
  );
}

/* ─── Action Phase ─── */
function ActionPhase({ player, mag, char, charIdx, state, dispatch }: {
  player: any; mag: any; char: any; charIdx: number; state: any;
  dispatch: (action: any) => any;
}) {
  const location = char.location as Location;
  const ap = char.ap;

  return (
    <div style={{ marginTop: 8 }}>
      {/* Character + AP display */}
      <div style={{
        display: 'flex', alignItems: 'center', gap: 10, marginBottom: 10,
        padding: '10px 12px', background: 'var(--bg-card)', borderRadius: 'var(--radius)',
        border: `1px solid ${player.color}30`,
      }}>
        <img
          src={mag.img} alt={mag.nameKo}
          style={{ width: 32, height: 32, borderRadius: '50%', objectFit: 'cover', border: `2px solid ${player.color}` }}
          onError={(e) => { (e.target as HTMLImageElement).style.display = 'none'; }}
        />
        <div style={{ flex: 1 }}>
          <div style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
            <GameIcon type={char.type} size="sm" color={player.color} />
            <span style={{ fontWeight: 700, fontSize: '0.9rem' }}>
              {CHARACTER_META[char.type as keyof typeof CHARACTER_META].name}
            </span>
          </div>
          <div style={{ fontSize: '0.78rem', color: 'var(--text-dim)', display: 'flex', alignItems: 'center', gap: 4 }}>
            <GameIcon type={location} size="xs" />
            {LOCATION_META[location].name}
          </div>
        </div>
        <div style={{
          display: 'flex', flexDirection: 'column', alignItems: 'center',
          padding: '4px 10px', background: 'var(--bg-secondary)',
          borderRadius: 'var(--radius)', border: '1px solid var(--cyan-light)',
        }}>
          <span style={{ fontWeight: 900, fontSize: '1.1rem', color: 'var(--cyan-light)' }}>{ap}</span>
          <span style={{ fontSize: '0.6rem', color: 'var(--text-dim)' }}>AP</span>
        </div>
      </div>

      {/* Shard conversion */}
      {player.shards > 0 && !char.shardConverted && (
        <button
          onClick={() => dispatch({ type: 'CONVERT_SHARD', playerId: player.id })}
          className="btn btn-sm"
          style={{ width: '100%', marginBottom: 6, fontSize: '0.78rem' }}
        >
          샤드 변환 (+1 AP) — 보유 샤드: {player.shards}
        </button>
      )}

      {/* Location-specific actions */}
      {ap > 0 && (
        <LocationActions
          location={location}
          player={player}
          state={state}
          dispatch={dispatch}
        />
      )}

      {/* Finish actions */}
      <button
        onClick={() => dispatch({ type: 'FINISH_ACTIONS', playerId: player.id })}
        className="btn btn-primary"
        style={{ width: '100%', marginTop: 8 }}
      >
        {ap > 0 ? `턴 종료 (남은 AP: ${ap})` : '다음 턴'}
      </button>
    </div>
  );
}

/* ─── Location-Specific Actions ─── */
function LocationActions({ location, player, state, dispatch }: {
  location: Location; player: any; state: any;
  dispatch: (action: any) => any;
}) {
  const sectionStyle = {
    marginBottom: 6,
    padding: '8px 10px',
    background: 'var(--bg-secondary)',
    borderRadius: 'var(--radius)',
    border: '1px solid var(--border)',
  };
  const labelStyle = {
    fontSize: '0.75rem', fontWeight: 700 as const, color: 'var(--gold-primary)',
    marginBottom: 4, fontFamily: 'var(--font-heading)',
  };

  switch (location) {
    case 'DOWNTOWN':
      return <DowntownActions player={player} state={state} dispatch={dispatch} sectionStyle={sectionStyle} labelStyle={labelStyle} />;
    case 'MARKET_ROW':
      return <MarketActions player={player} state={state} dispatch={dispatch} sectionStyle={sectionStyle} labelStyle={labelStyle} />;
    case 'WORKSHOP':
      return <WorkshopActions player={player} state={state} dispatch={dispatch} sectionStyle={sectionStyle} labelStyle={labelStyle} />;
    case 'THEATER':
      return <TheaterActions player={player} state={state} dispatch={dispatch} sectionStyle={sectionStyle} labelStyle={labelStyle} />;
    case 'DARK_ALLEY':
      return <DarkAlleyActions player={player} state={state} dispatch={dispatch} sectionStyle={sectionStyle} labelStyle={labelStyle} />;
    default:
      return null;
  }
}

/* ─── Downtown: Learn Trick, Take Coins, Hire ─── */
function DowntownActions({ player, state, dispatch, sectionStyle, labelStyle }: any) {
  const dice = state.downtownDice;

  return (
    <div>
      {/* Bank - Take Coins */}
      <div style={sectionStyle}>
        <div style={labelStyle}>은행 (코인 획득)</div>
        <div style={{ display: 'flex', gap: 4, flexWrap: 'wrap' }}>
          {dice.BANK.map((face: any, i: number) => {
            const used = dice.marked.BANK[i];
            const isX = face === 'X';
            return (
              <button
                key={i}
                onClick={() => dispatch({ type: 'TAKE_COINS', playerId: player.id, dieIndex: i })}
                disabled={used || isX}
                className="btn btn-sm"
                style={{ minWidth: 40, opacity: used || isX ? 0.3 : 1 }}
              >
                {isX ? 'X' : `${face}코인`}
              </button>
            );
          })}
        </div>
      </div>

      {/* Inn - Hire */}
      <div style={sectionStyle}>
        <div style={labelStyle}>여관 (고용)</div>
        <div style={{ display: 'flex', gap: 4, flexWrap: 'wrap' }}>
          {dice.INN.map((face: any, i: number) => {
            const used = dice.marked.INN[i];
            const isX = face === 'X';
            return (
              <button
                key={i}
                onClick={() => dispatch({ type: 'HIRE', playerId: player.id, specialistType: face })}
                disabled={used || isX}
                className="btn btn-sm"
                style={{ minWidth: 40, opacity: used || isX ? 0.3 : 1 }}
              >
                {isX ? 'X' : CHARACTER_META[face as keyof typeof CHARACTER_META]?.name ?? face}
              </button>
            );
          })}
        </div>
      </div>

      {/* Dahlgaard - Learn Trick (simplified: just show available categories) */}
      <div style={sectionStyle}>
        <div style={labelStyle}>달가드 아카데미 (트릭 습득)</div>
        <div style={{ fontSize: '0.75rem', color: 'var(--text-dim)' }}>
          가용 카테고리: {dice.DAHLGAARD.map((face: any, i: number) => {
            if (dice.marked.DAHLGAARD[i] || face === 'X') return null;
            return <span key={i} style={{ marginRight: 6, color: 'var(--cyan-light)' }}>{face}</span>;
          })}
        </div>
        <div style={{ fontSize: '0.68rem', color: 'var(--text-dim)', marginTop: 2 }}>
          (트릭 습득은 트릭 덱에서 선택 — 추후 구현)
        </div>
      </div>
    </div>
  );
}

/* ─── Market: Buy Components ─── */
function MarketActions({ player, state, dispatch, sectionStyle, labelStyle }: any) {
  const stock = state.market.stock as ComponentType[];
  const uniqueStock = [...new Set(stock)];

  return (
    <div style={sectionStyle}>
      <div style={labelStyle}>시장 (컴포넌트 구매)</div>
      <div style={{ display: 'flex', gap: 4, flexWrap: 'wrap' }}>
        {uniqueStock.map((comp) => {
          const meta = COMPONENT_META[comp];
          const canAfford = player.coins >= meta.cost;
          return (
            <button
              key={comp}
              onClick={() => dispatch({ type: 'BUY', playerId: player.id, componentType: comp })}
              disabled={!canAfford}
              className="btn btn-sm"
              style={{ opacity: canAfford ? 1 : 0.4, display: 'flex', alignItems: 'center', gap: 3 }}
            >
              <GameIcon type={comp} size="xs" />
              {meta.name} ({meta.cost}코인)
            </button>
          );
        })}
      </div>
      {uniqueStock.length === 0 && (
        <div style={{ fontSize: '0.75rem', color: 'var(--text-dim)' }}>시장에 재고가 없습니다</div>
      )}
    </div>
  );
}

/* ─── Workshop: Prepare Trick ─── */
function WorkshopActions({ player, state, dispatch, sectionStyle, labelStyle }: any) {
  const unPrepared = player.tricks.filter((t: any) => !t.prepared);

  return (
    <div style={sectionStyle}>
      <div style={labelStyle}>작업장 (트릭 준비)</div>
      <div style={{ display: 'flex', flexDirection: 'column', gap: 4 }}>
        {unPrepared.map((t: any, i: number) => {
          const realIdx = player.tricks.indexOf(t);
          return (
            <button
              key={realIdx}
              onClick={() => dispatch({ type: 'PREPARE', playerId: player.id, trickIdx: realIdx })}
              className="btn btn-sm"
              style={{ textAlign: 'left' }}
            >
              트릭 #{realIdx + 1} 준비
            </button>
          );
        })}
      </div>
      {unPrepared.length === 0 && (
        <div style={{ fontSize: '0.75rem', color: 'var(--text-dim)' }}>준비할 트릭이 없습니다</div>
      )}
    </div>
  );
}

/* ─── Theater: Setup Trick, Choose Weekday ─── */
function TheaterActions({ player, state, dispatch, sectionStyle, labelStyle }: any) {
  const weekdays = ['THURSDAY', 'FRIDAY', 'SATURDAY', 'SUNDAY'] as const;

  return (
    <div>
      <div style={sectionStyle}>
        <div style={labelStyle}>극장 (공연 예약)</div>
        <div style={{ display: 'flex', gap: 4 }}>
          {weekdays.map((day) => {
            const performer = state.theater.weekdayPerformers[day];
            const taken = performer !== null && performer !== player.id;
            const mine = performer === player.id;
            return (
              <button
                key={day}
                onClick={() => dispatch({ type: 'CHOOSE_WEEKDAY', playerId: player.id, weekday: day })}
                disabled={taken || mine}
                className={`btn btn-sm ${mine ? 'btn-primary' : ''}`}
                style={{ flex: 1, opacity: taken ? 0.3 : 1 }}
              >
                {day.slice(0, 3)}
              </button>
            );
          })}
        </div>
      </div>

      <div style={sectionStyle}>
        <div style={labelStyle}>트릭 셋업</div>
        <div style={{ fontSize: '0.68rem', color: 'var(--text-dim)' }}>
          (퍼포먼스 카드에 트릭 마커 배치 — 추후 구현)
        </div>
      </div>
    </div>
  );
}

/* ─── Dark Alley: Draw Special, Fortune Telling ─── */
function DarkAlleyActions({ player, state, dispatch, sectionStyle, labelStyle }: any) {
  return (
    <div style={sectionStyle}>
      <div style={labelStyle}>어둠의 골목</div>
      <div style={{ display: 'flex', gap: 6 }}>
        <button
          onClick={() => dispatch({ type: 'DRAW_SPECIAL', playerId: player.id })}
          disabled={state.darkAlley.specialDeck.length === 0}
          className="btn btn-sm"
          style={{ flex: 1 }}
        >
          특수 카드 뽑기
        </button>
        <button
          onClick={() => dispatch({ type: 'FORTUNE_TELLING', playerId: player.id, category: 'MECHANICAL' })}
          className="btn btn-sm"
          style={{ flex: 1 }}
        >
          점술 (샤드 획득)
        </button>
      </div>
    </div>
  );
}
