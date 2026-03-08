'use client';

import { useState } from 'react';
import { useGameStore } from '@/stores/game-store';
import {
  LOCATION_META, CHARACTER_META, COMPONENT_META,
  TRICK_CATEGORY_META, SYMBOL_INDEX_TO_SHAPE,
  WEEKDAY_MOD, WEEKDAYS,
} from '@/core/data/constants';
import { getMagicianDef } from '@/core/data/magicians';
import { getTrickDef, getTricksByCategory } from '@/core/data/tricks';
import type { TrickDef } from '@/core/data/tricks';
import { getAllowedCategories, getNextAvailableSymbol } from '@/core/state/selectors';
import { GameIcon } from '../shared/GameIcon';
import type { Location, PlayerId, ComponentType, TrickCategory, SymbolIndex, TrickId, Weekday, CardId, SlotPosition } from '@/core/types';

const SHAPE_ICONS: Record<string, string> = {
  CIRCLE: '\u25CF', TRIANGLE: '\u25B2', SQUARE: '\u25A0', STAR: '\u2605',
};

function symbolChar(idx: number): string {
  const shape = SYMBOL_INDEX_TO_SHAPE[idx as keyof typeof SYMBOL_INDEX_TO_SHAPE];
  return SHAPE_ICONS[shape] ?? '?';
}

/* ========== Character Info Bar (shared) ========== */
export function CharInfoBar({ player, mag, char, ap }: { player: any; mag: any; char: any; ap?: number }) {
  return (
    <div className="character-item" style={{ borderColor: `${player.color}30`, marginBottom: 10, padding: '10px 12px' }}>
      <div style={{ display: 'flex', alignItems: 'center', gap: 10, flex: 1 }}>
        <img src={mag.img} alt={mag.nameKo}
          style={{ width: 32, height: 32, borderRadius: '50%', objectFit: 'cover', border: `2px solid ${player.color}` }}
          onError={(e) => { (e.target as HTMLImageElement).style.display = 'none'; }}
        />
        <div>
          <div style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
            <GameIcon type={char.type} size="sm" color={player.color} />
            <span className="trick-name">{CHARACTER_META[char.type as keyof typeof CHARACTER_META].name}</span>
          </div>
          {char.location && (
            <div className="trick-level" style={{ display: 'flex', alignItems: 'center', gap: 4 }}>
              <GameIcon type={char.location} size="xs" />
              {LOCATION_META[char.location as Location].name}
            </div>
          )}
        </div>
      </div>
      {ap !== undefined && (
        <div className="char-ap">{ap} AP</div>
      )}
    </div>
  );
}

/* ========== Slot Selection ========== */
export function SlotSelector({ player, mag, char, charIdx, state, onPlace, compact }: {
  player: any; mag: any; char: any; charIdx: number; state: any;
  onPlace: (slotIndex: number) => void;
  compact?: boolean;
}) {
  const location = char.location as Location;
  const slots = location ? state.locationSlots[location] : [];

  return (
    <div className="action-group">
      {!compact && <CharInfoBar player={player} mag={mag} char={char} />}
      <div className="action-group-title">슬롯을 선택하세요</div>
      <div className="worker-slots" style={{ gap: 8 }}>
        {slots.map((slot: any, i: number) => {
          const occupied = !!slot.occupant;
          const apClass = slot.apMod > 0 ? 'positive' : 'zero';
          return (
            <button key={i} onClick={() => !occupied && onPlace(i)} disabled={occupied}
              className={`worker-slot ${occupied ? 'occupied' : ''}`}
              style={{ width: 60, height: 60, cursor: occupied ? 'not-allowed' : 'pointer' }}>
              {occupied ? (
                <span style={{ fontSize: '0.8rem', color: 'var(--red)', fontWeight: 700 }}>X</span>
              ) : (
                <div style={{ textAlign: 'center' }}>
                  <span className={`ws-ap ${apClass}`} style={{ fontSize: '1rem', fontWeight: 900 }}>
                    {slot.apMod > 0 ? `+${slot.apMod}` : slot.apMod}
                  </span>
                  <div style={{ fontSize: '0.65rem', color: 'var(--text-dim)', marginTop: 2 }}>AP</div>
                </div>
              )}
            </button>
          );
        })}
      </div>
    </div>
  );
}

/* ========== Theater Weekday + Slot Selector ========== */
export function TheaterWeekdaySelector({ player, mag, char, charIdx, state, dispatch, compact }: {
  player: any; mag: any; char: any; charIdx: number; state: any;
  dispatch: (action: any) => any;
  compact?: boolean;
}) {
  const isMagician = char.type === 'MAGICIAN';
  const chosenWeekday = player.chosenWeekday;

  const availableDays = WEEKDAYS.filter(day => {
    const performer = state.theater.weekdayPerformers[day];
    if (performer !== null && performer !== player.id) return false;
    if (chosenWeekday !== null && chosenWeekday !== day) return false;
    return true;
  });

  return (
    <div className="action-group">
      {!compact && <CharInfoBar player={player} mag={mag} char={char} />}
      <div className="action-group-title">
        극장 -- {isMagician ? '퍼포먼스 또는 백스테이지' : '백스테이지'} 슬롯 선택
      </div>

      {availableDays.length === 0 && (
        <div className="text-dim" style={{ fontSize: '0.78rem', padding: 8, color: 'var(--red)' }}>
          배치 가능한 요일이 없습니다
        </div>
      )}

      <div style={{ display: 'flex', flexDirection: 'column', gap: 8 }}>
        {availableDays.map(day => {
          const mod = WEEKDAY_MOD[day];
          const daySlot = state.theater.weekdaySlots[day];
          const baseAP = CHARACTER_META[char.type as keyof typeof CHARACTER_META].baseAP;

          return (
            <div key={day} className="weekday-col">
              <h4>{mod.name}</h4>
              <div className="weekday-mod">
                명성{mod.fameMod >= 0 ? '+' : ''}{mod.fameMod} / 코인{mod.coinMod >= 0 ? '+' : ''}{mod.coinMod}
              </div>
              <div style={{ display: 'flex', gap: 6, flexWrap: 'wrap' }}>
                {daySlot.backstage.map((bs: any, bsIdx: number) => {
                  const occupied = !!bs.occupant;
                  return (
                    <button key={`bs-${bsIdx}`}
                      onClick={() => !occupied && dispatch({
                        type: 'PLACE_CHARACTER', playerId: player.id,
                        characterIdx: charIdx, slotIndex: bsIdx,
                        weekday: day, theaterSlotType: 'BACKSTAGE',
                      })}
                      disabled={occupied}
                      className={`weekday-slot ${occupied ? 'occupied' : ''}`}
                      style={{ width: 90, minHeight: 50, cursor: occupied ? 'not-allowed' : 'pointer' }}>
                      <div style={{ textAlign: 'center' }}>
                        <div style={{ fontSize: '0.72rem', color: 'var(--text-dim)', fontWeight: 600 }}>백스테이지</div>
                        <div style={{ fontWeight: 900, fontSize: '0.9rem', color: 'var(--cyan-light)' }}>AP {baseAP + bs.apMod}</div>
                        {occupied && <div style={{ fontSize: '0.65rem', color: 'var(--red)', fontWeight: 700 }}>점유</div>}
                      </div>
                    </button>
                  );
                })}

                {isMagician && (() => {
                  const perfOccupied = !!daySlot.performSlot.occupant;
                  return (
                    <button
                      onClick={() => !perfOccupied && dispatch({
                        type: 'PLACE_CHARACTER', playerId: player.id,
                        characterIdx: charIdx, slotIndex: 0,
                        weekday: day, theaterSlotType: 'PERFORM',
                      })}
                      disabled={perfOccupied}
                      className={`weekday-slot performance-slot ${perfOccupied ? 'occupied' : ''}`}
                      style={{ width: 90, minHeight: 50, cursor: perfOccupied ? 'not-allowed' : 'pointer' }}>
                      <div style={{ textAlign: 'center' }}>
                        <div style={{ fontSize: '0.72rem', color: 'var(--loc-theater)', fontWeight: 700 }}>퍼포먼스</div>
                        <div style={{ fontWeight: 900, fontSize: '0.9rem', color: 'var(--loc-theater)' }}>AP {baseAP}</div>
                        {perfOccupied && <div style={{ fontSize: '0.65rem', color: 'var(--red)', fontWeight: 700 }}>점유</div>}
                      </div>
                    </button>
                  );
                })()}
              </div>
            </div>
          );
        })}
      </div>
    </div>
  );
}

/* ========== Location-Specific Actions ========== */
export function LocationActions({ location, player, state, dispatch }: {
  location: Location; player: any; state: any; dispatch: (action: any) => any;
}) {
  switch (location) {
    case 'DOWNTOWN':
      return <DowntownActions player={player} state={state} dispatch={dispatch} />;
    case 'MARKET_ROW':
      return <MarketActions player={player} state={state} dispatch={dispatch} />;
    case 'WORKSHOP':
      return <WorkshopActions player={player} state={state} dispatch={dispatch} />;
    case 'THEATER':
      return <TheaterActions player={player} state={state} dispatch={dispatch} />;
    case 'DARK_ALLEY':
      return <DarkAlleyActions player={player} state={state} dispatch={dispatch} />;
    default:
      return null;
  }
}

/* ========== Downtown: Bank, Inn, Learn Trick ========== */
function DowntownActions({ player, state, dispatch }: any) {
  const dice = state.downtownDice;

  return (
    <div>
      {/* Bank */}
      <div className="panel-section">
        <div className="panel-section-title">은행 (코인 획득, 3AP)</div>
        <div className="dice-section">
          <div className="dice-group">
            <div className="dice-group-label">BANK</div>
            <div className="dice-row">
              {dice.BANK.map((face: any, i: number) => {
                const used = dice.marked.BANK[i];
                const isX = face === 'X';
                return (
                  <button key={i}
                    onClick={() => dispatch({ type: 'TAKE_COINS', playerId: player.id, dieIndex: i })}
                    disabled={used || isX}
                    className={`die ${used ? 'used' : ''}`}>
                    {isX ? 'X' : face}
                  </button>
                );
              })}
            </div>
          </div>
        </div>
      </div>

      {/* Inn */}
      <div className="panel-section">
        <div className="panel-section-title">여관 (고용, 3AP)</div>
        <div className="dice-section">
          <div className="dice-group">
            <div className="dice-group-label">INN</div>
            <div className="dice-row">
              {dice.INN.map((face: any, i: number) => {
                const used = dice.marked.INN[i];
                const isX = face === 'X';
                return (
                  <button key={i}
                    onClick={() => dispatch({ type: 'HIRE', playerId: player.id, specialistType: face })}
                    disabled={used || isX}
                    className={`die ${used ? 'used' : ''}`}>
                    {isX ? 'X' : CHARACTER_META[face as keyof typeof CHARACTER_META]?.name?.[0] ?? face}
                  </button>
                );
              })}
            </div>
          </div>
        </div>
      </div>

      {/* Learn Trick */}
      <LearnTrickSection player={player} state={state} dispatch={dispatch} />
    </div>
  );
}

/* ========== Market: Buy, Order, Quick Order ========== */
function MarketActions({ player, state, dispatch }: any) {
  const buyArea = state.market.buyArea as ComponentType[];
  const orderArea = state.market.orderArea as ComponentType[];
  const quickSlot = state.market.quickOrderSlot as ComponentType | null;

  const COMP_TIERS: { label: string; types: ComponentType[] }[] = [
    { label: 'Tier 1', types: Object.keys(COMPONENT_META).filter(k => COMPONENT_META[k as ComponentType].cost <= 2) as ComponentType[] },
    { label: 'Tier 2', types: Object.keys(COMPONENT_META).filter(k => COMPONENT_META[k as ComponentType].cost > 2 && COMPONENT_META[k as ComponentType].cost <= 4) as ComponentType[] },
    { label: 'Tier 3', types: Object.keys(COMPONENT_META).filter(k => COMPONENT_META[k as ComponentType].cost > 4) as ComponentType[] },
  ];

  return (
    <div>
      {/* Buy Area */}
      <div className="panel-section">
        <div className="panel-section-title">구매 (1AP)</div>
        {buyArea.length === 0 ? (
          <div className="text-dim" style={{ fontSize: '0.75rem' }}>구매 가능한 재고 없음</div>
        ) : (
          <div className="market-display">
            {buyArea.map((comp, i) => {
              const meta = COMPONENT_META[comp];
              const canAfford = player.coins >= meta.cost;
              return (
                <button key={`${comp}-${i}`}
                  onClick={() => dispatch({ type: 'BUY', playerId: player.id, componentType: comp, bargainAP: 0 })}
                  disabled={!canAfford}
                  className="market-item">
                  <GameIcon type={comp} size="xs" />
                  {meta.name}
                  <span className="price">{meta.cost}C</span>
                </button>
              );
            })}
          </div>
        )}
      </div>

      {/* Order Area */}
      <div className="panel-section">
        <div className="panel-section-title">주문 (1AP)</div>
        {COMP_TIERS.map(tier => {
          const types = tier.types;
          if (types.length === 0) return null;
          return (
            <div key={tier.label} style={{ marginBottom: 6 }}>
              <div className="dice-group-label" style={{ marginBottom: 4 }}>{tier.label}</div>
              <div className="market-display">
                {types.map(comp => {
                  const meta = COMPONENT_META[comp];
                  const alreadyOrdered = orderArea.includes(comp);
                  return (
                    <button key={comp}
                      onClick={() => dispatch({ type: 'ORDER', playerId: player.id, componentType: comp })}
                      disabled={alreadyOrdered}
                      className={`market-item ${alreadyOrdered ? 'selected' : ''}`}>
                      <GameIcon type={comp} size="xs" />
                      {meta.name}
                    </button>
                  );
                })}
              </div>
            </div>
          );
        })}
        {orderArea.length > 0 && (
          <div className="text-dim" style={{ fontSize: '0.65rem', marginTop: 4 }}>
            주문 중: {orderArea.map((c: ComponentType) => COMPONENT_META[c].name).join(', ')}
          </div>
        )}
      </div>

      {/* Quick Order */}
      <div className="panel-section">
        <div className="panel-section-title">급송 주문 (2AP)</div>
        {quickSlot ? (
          <div className="text-dim" style={{ fontSize: '0.72rem' }}>
            이미 급송 중: {COMPONENT_META[quickSlot].name}
          </div>
        ) : (
          <div className="market-display">
            {(Object.keys(COMPONENT_META) as ComponentType[]).map(comp => {
              const meta = COMPONENT_META[comp];
              return (
                <button key={comp}
                  onClick={() => dispatch({ type: 'QUICK_ORDER', playerId: player.id, componentType: comp })}
                  className="market-item">
                  <GameIcon type={comp} size="xs" />
                  {meta.name}
                </button>
              );
            })}
          </div>
        )}
      </div>
    </div>
  );
}

/* ========== Workshop: Prepare Trick ========== */
function WorkshopActions({ player, state, dispatch }: any) {
  const unPrepared = player.tricks
    .map((t: any, i: number) => ({ slot: t, idx: i }))
    .filter(({ slot }: any) => !slot.prepared);

  return (
    <div className="panel-section">
      <div className="panel-section-title">작업장 (트릭 준비)</div>
      <div className="trick-list">
        {unPrepared.map(({ slot, idx }: any) => {
          const trick = getTrickDef(slot.trickId);
          const catMeta = TRICK_CATEGORY_META[trick.category];
          return (
            <button key={idx}
              onClick={() => dispatch({ type: 'PREPARE', playerId: player.id, trickIdx: idx })}
              className="action-btn"
              style={{ borderLeft: `3px solid ${catMeta.color}` }}>
              {trick.nameKo} (Lv.{trick.level})
              <span className="ap-cost">{trick.prepareCost} AP</span>
            </button>
          );
        })}
      </div>
      {unPrepared.length === 0 && (
        <div className="text-dim" style={{ fontSize: '0.75rem' }}>준비할 트릭이 없습니다</div>
      )}
    </div>
  );
}

/* ========== Theater: Setup Trick on Performance Cards ========== */
function TheaterActions({ player, state, dispatch }: any) {
  const [selectedTrickIdx, setSelectedTrickIdx] = useState<number | null>(null);

  const readyTricks = player.tricks
    .map((t: any, i: number) => ({ slot: t, idx: i }))
    .filter(({ slot }: any) => slot.prepared && slot.markersOnTrick > 0);

  const chosenDay = player.chosenWeekday;

  return (
    <div>
      {chosenDay && (
        <div className="panel-section" style={{ borderLeft: '3px solid var(--gold-primary)', paddingLeft: 10 }}>
          <span className="text-gold" style={{ fontWeight: 700, fontSize: '0.78rem' }}>
            공연 예약: {WEEKDAY_MOD[chosenDay as keyof typeof WEEKDAY_MOD]?.name ?? chosenDay}
          </span>
        </div>
      )}

      <div className="panel-section">
        <div className="panel-section-title">트릭 셋업 (1AP)</div>

        {readyTricks.length === 0 ? (
          <div className="text-dim" style={{ fontSize: '0.72rem' }}>배치 가능한 준비된 트릭이 없습니다</div>
        ) : (
          <div>
            <div className="dice-group-label" style={{ marginBottom: 4 }}>트릭 선택:</div>
            <div style={{ display: 'flex', gap: 4, flexWrap: 'wrap', marginBottom: 8 }}>
              {readyTricks.map(({ slot, idx }: any) => {
                const trick = getTrickDef(slot.trickId);
                const catMeta = TRICK_CATEGORY_META[trick.category];
                const selected = selectedTrickIdx === idx;
                return (
                  <button key={idx}
                    onClick={() => setSelectedTrickIdx(selected ? null : idx)}
                    className={`btn btn-sm ${selected ? 'btn-primary' : ''}`}
                    style={{ borderLeft: `3px solid ${catMeta.color}` }}>
                    {symbolChar(slot.symbolIndex)} {trick.nameKo} (마커: {slot.markersOnTrick})
                  </button>
                );
              })}
            </div>

            {selectedTrickIdx !== null && (
              <PerfCardSlotSelector player={player} state={state}
                trickIdx={selectedTrickIdx} dispatch={dispatch}
                onPlaced={() => setSelectedTrickIdx(null)}
              />
            )}
          </div>
        )}
      </div>
    </div>
  );
}

/* ========== Performance Card Slot Selector ========== */
function PerfCardSlotSelector({ player, state, trickIdx, dispatch, onPlaced }: {
  player: any; state: any; trickIdx: number;
  dispatch: (action: any) => any; onPlaced: () => void;
}) {
  const trickSlot = player.tricks[trickIdx];
  const symbolIdx = trickSlot.symbolIndex;
  const perfCards = state.theater.perfCards;

  return (
    <div>
      <div className="dice-group-label" style={{ marginBottom: 4 }}>퍼포먼스 카드의 빈 슬롯을 선택하세요:</div>
      <div style={{ display: 'flex', flexDirection: 'column', gap: 6 }}>
        {perfCards.map((card: any) => {
          const hasSameSymbol = card.slotMarkers.some(
            (m: any) => m && m.playerId === player.id && m.symbolIndex === symbolIdx,
          );
          if (hasSameSymbol) return null;

          const emptySlots = [0, 1, 2, 3, 4, 5].filter((i: number) => card.slotMarkers[i] === null);
          if (emptySlots.length === 0) return null;

          return (
            <div key={card.cardId} className="perf-card">
              <div className="perf-card-title">카드 {card.cardId}</div>
              <div className="perf-card-grid">
                {[0, 1, 2, 3, 4, 5].map((slotPos: number) => {
                  const marker = card.slotMarkers[slotPos];
                  const isEmpty = marker === null;
                  return (
                    <button key={slotPos}
                      onClick={() => {
                        if (isEmpty) {
                          dispatch({ type: 'SETUP_TRICK', playerId: player.id, trickIdx, cardId: card.cardId, slotPosition: slotPos });
                          onPlaced();
                        }
                      }}
                      disabled={!isEmpty}
                      className={`trick-slot ${isEmpty ? '' : 'occupied'}`}
                      style={marker ? { color: state.players[marker.playerId]?.color ?? '#888' } : undefined}>
                      {marker ? symbolChar(marker.symbolIndex) : (
                        <span className="trick-slot-info">{slotPos}</span>
                      )}
                    </button>
                  );
                })}
              </div>
            </div>
          );
        })}
      </div>
    </div>
  );
}

/* ========== Dark Alley ========== */
function DarkAlleyActions({ player, state, dispatch }: any) {
  const hasDrawnChoices = state.darkAlley.drawnChoices !== null;

  return (
    <div className="panel-section">
      <div className="panel-section-title">어둠의 골목</div>
      <div className="location-actions">
        {!hasDrawnChoices && (
          <button onClick={() => dispatch({ type: 'DRAW_CARDS', playerId: player.id, deckLocation: 'DOWNTOWN' })}
            className="action-btn">
            특수 배치카드 뽑기
            <span className="ap-cost">1 AP</span>
          </button>
        )}
        {hasDrawnChoices && (
          <div>
            <div className="text-gold" style={{ fontSize: '0.72rem', marginBottom: 4 }}>2장 중 1장을 선택하세요:</div>
            <div style={{ display: 'flex', gap: 4 }}>
              {(state.darkAlley.drawnChoices as string[]).map((cardId: string) => (
                <button key={cardId}
                  onClick={() => dispatch({ type: 'CHOOSE_CARD', playerId: player.id, chosenCardDefId: cardId })}
                  className="btn btn-sm" style={{ flex: 1 }}>
                  {cardId}
                </button>
              ))}
            </div>
          </div>
        )}
        <button onClick={() => dispatch({ type: 'FORTUNE_TELLING', playerId: player.id })}
          disabled={state.prophecy.pending.length === 0}
          className="action-btn">
          점술
          <span className="ap-cost">1 AP</span>
        </button>
      </div>
    </div>
  );
}

/* ========== Learn Trick (Dahlgaard Academy) ========== */
function LearnTrickSection({ player, state, dispatch }: any) {
  const [selectedCat, setSelectedCat] = useState<TrickCategory | null>(null);

  const mag = getMagicianDef(player.magicianId);
  const allowed = getAllowedCategories(state);
  const favCat = mag.favoriteCategory;
  const categories = Array.from(new Set([favCat, ...allowed])) as TrickCategory[];
  const nextSymbol = getNextAvailableSymbol(player);
  const atMaxTricks = player.tricks.length >= 4;

  if (atMaxTricks) {
    return (
      <div className="panel-section">
        <div className="panel-section-title">달가드 아카데미 (트릭 습득, 3AP)</div>
        <div className="text-dim" style={{ fontSize: '0.75rem' }}>트릭 최대 보유 (4/4)</div>
      </div>
    );
  }
  if (nextSymbol === null) {
    return (
      <div className="panel-section">
        <div className="panel-section-title">달가드 아카데미 (트릭 습득, 3AP)</div>
        <div className="text-dim" style={{ fontSize: '0.75rem' }}>사용 가능한 심볼 마커 없음</div>
      </div>
    );
  }

  const deckTricks: TrickDef[] = selectedCat
    ? (state.trickDecks[selectedCat] as TrickId[])
        .map((id: TrickId) => getTrickDef(id))
        .filter((t: TrickDef) => !player.tricks.some((s: any) => s.trickId === t.id))
    : [];

  const grouped = { 1: [] as TrickDef[], 2: [] as TrickDef[], 3: [] as TrickDef[] };
  deckTricks.forEach(t => {
    if (grouped[t.level as 1 | 2 | 3]) grouped[t.level as 1 | 2 | 3].push(t);
  });

  return (
    <div className="panel-section">
      <div className="panel-section-title">달가드 아카데미 (트릭 습득, 3AP)</div>

      <div className="category-list" style={{ marginBottom: 8, gap: 6 }}>
        {categories.map(cat => {
          const meta = TRICK_CATEGORY_META[cat];
          const catClass = `cat-${cat.toLowerCase()}` as string;
          const isFav = cat === favCat;
          const isSelected = selectedCat === cat;
          return (
            <span key={cat}
              onClick={() => setSelectedCat(isSelected ? null : cat)}
              className={catClass}
              style={{ cursor: 'pointer', opacity: isSelected ? 1 : 0.6, outline: isSelected ? '2px solid currentColor' : 'none' }}>
              {meta.name}{isFav ? '\u2605' : ''}
            </span>
          );
        })}
      </div>

      {selectedCat && deckTricks.length === 0 && (
        <div className="text-dim" style={{ fontSize: '0.72rem' }}>해당 카테고리에 습득 가능한 트릭이 없습니다</div>
      )}
      {selectedCat && deckTricks.length > 0 && (
        <div style={{ maxHeight: 280, overflowY: 'auto' }}>
          {([1, 2, 3] as const).map(level => {
            const tricks = grouped[level];
            if (tricks.length === 0) return null;
            return (
              <div key={level} style={{ marginBottom: 10 }}>
                <div className="dice-group-label" style={{ marginBottom: 6 }}>Lv.{level}</div>
                <div className="trick-select-grid" style={{ margin: 0 }}>
                  {tricks.map(trick => {
                    const deficit = Math.max(0, trick.fameThreshold - player.fame);
                    const canAfford = player.coins >= deficit;
                    const fameMet = player.fame >= trick.fameThreshold;
                    const fameColor = fameMet ? 'var(--green)' : canAfford ? 'var(--orange)' : 'var(--red)';
                    const comps = Object.entries(trick.components) as [ComponentType, number][];
                    const shape = SYMBOL_INDEX_TO_SHAPE[nextSymbol];

                    return (
                      <div key={trick.id}
                        onClick={() => canAfford && dispatch({
                          type: 'LEARN_TRICK', playerId: player.id,
                          trickId: trick.id, symbolIndex: nextSymbol,
                        })}
                        className={`trick-select-item ${!canAfford ? 'disabled' : ''}`}
                        style={{
                          opacity: canAfford ? 1 : 0.4,
                          cursor: canAfford ? 'pointer' : 'not-allowed',
                          borderLeftWidth: 3,
                          borderLeftColor: TRICK_CATEGORY_META[trick.category].color,
                        }}>
                        <div className="name">
                          {SHAPE_ICONS[shape]} {trick.nameKo}
                        </div>

                        <div style={{ fontSize: '0.75rem', marginTop: 4 }}>
                          <span style={{ color: fameColor, fontWeight: 700 }}>
                            명성 {trick.fameThreshold}{'\u2191'}
                          </span>
                          {!fameMet && canAfford && (
                            <span className="text-dim"> (부족분 {deficit}코인으로 대체)</span>
                          )}
                        </div>

                        {comps.length > 0 && (
                          <div className="components">
                            <span className="text-dim">준비에 필요한 컴포넌트: </span>
                            {comps.map(([c, n]) => {
                              const has = player.components?.[c] ?? 0;
                              const enough = has >= n;
                              return (
                                <span key={c} style={{ color: enough ? 'var(--green)' : 'var(--red)', marginRight: 6 }}>
                                  {COMPONENT_META[c].name} {has}/{n}
                                </span>
                              );
                            })}
                          </div>
                        )}

                        <div className="yields">
                          보상: {trick.yields.fame > 0 ? `명성${trick.yields.fame} ` : ''}
                          {trick.yields.coins > 0 ? `코인${trick.yields.coins} ` : ''}
                          {trick.yields.shards > 0 ? `샤드${trick.yields.shards}` : ''}
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
    </div>
  );
}
