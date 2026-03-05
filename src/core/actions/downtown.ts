// Downtown actions: LEARN_TRICK, TAKE_COINS, HIRE, REROLL, SET_DIE, CHOOSE_DIE (v4)

import type { GameState, GameAction, ValidationError, SymbolIndex } from '../types';
import { err } from '../types';
import { registerHandler } from './registry';
import { getPlayer, getAllowedCategories, getNextAvailableSymbol } from '../state/selectors';
import { updatePlayer, updateSymbol, adjustFame, addLog } from '../state/helpers';
import { rngRollDie } from '../state/random';
import { getTrickDef } from '../data/tricks';
import { getMagicianDef } from '../data/magicians';
import { DICE_FACES, STARTING, HIRE_LIMITS } from '../data/constants';

type LearnAction = Extract<GameAction, { type: 'LEARN_TRICK' }>;
type TakeCoinsAction = Extract<GameAction, { type: 'TAKE_COINS' }>;
type HireAction = Extract<GameAction, { type: 'HIRE' }>;
type RerollAction = Extract<GameAction, { type: 'REROLL' }>;
type SetDieAction = Extract<GameAction, { type: 'SET_DIE' }>;
type ChooseDieAction = Extract<GameAction, { type: 'CHOOSE_DIE' }>;

registerHandler<LearnAction>('LEARN_TRICK', {
  validate(state, action) {
    const errors: ValidationError[] = [];
    const player = state.players.find(p => p.id === action.playerId);
    if (!player) { errors.push(err('playerId', '플레이어 없음')); return errors; }
    if (player.tricks.length >= STARTING.maxTricks) {
      errors.push(err('tricks', '트릭 최대 보유 한도 초과'));
    }
    const sym = player.symbols[action.symbolIndex];
    if (!sym) { errors.push(err('symbolIndex', '심볼 인덱스 오류')); return errors; }
    if (sym.assigned) errors.push(err('symbol', '이미 사용된 심볼 마커'));

    const trick = getTrickDef(action.trickId);

    // v4: 명성 부족분은 코인으로 대납 가능 (validate에서 완화)
    // 코인 대납: 부족분 1명성 = 1코인
    if (player.fame < trick.fameThreshold) {
      const deficit = trick.fameThreshold - player.fame;
      if (player.coins < deficit) {
        errors.push(err('fame', `명성 부족 (필요: ${trick.fameThreshold}, 코인 대납 불가)`));
      }
    }

    // v4: 선호 카테고리는 주사위 무관하게 학습 가능
    const magician = getMagicianDef(player.magicianId);
    if (trick.category !== magician.favoriteCategory) {
      const allowed = getAllowedCategories(state);
      if (!allowed.includes(trick.category)) {
        errors.push(err('category', '해당 카테고리의 주사위가 없습니다'));
      }
    }

    const deck = state.trickDecks[trick.category];
    if (!deck.includes(action.trickId)) {
      errors.push(err('trickId', '해당 트릭이 덱에 없습니다'));
    }
    if (player.tricks.some(t => t.trickId === action.trickId)) {
      errors.push(err('trickId', '이미 보유한 트릭'));
    }
    return errors;
  },
  apply(state, action) {
    const player = getPlayer(state, action.playerId);
    const trick = getTrickDef(action.trickId);

    // Remove from deck
    const newDeck = state.trickDecks[trick.category].filter(id => id !== action.trickId);
    let s: GameState = {
      ...state,
      trickDecks: { ...state.trickDecks, [trick.category]: newDeck },
    };

    // v4: 코인 대납 처리
    if (player.fame < trick.fameThreshold) {
      const deficit = trick.fameThreshold - player.fame;
      s = updatePlayer(s, action.playerId, p => ({
        coins: Math.max(0, p.coins - deficit),
      }));
    }

    // Mark Dahlgaard die as used (find first unmarked matching die)
    const dahlgaard = s.downtownDice.DAHLGAARD;
    const marked = [...s.downtownDice.marked.DAHLGAARD];
    for (let i = 0; i < dahlgaard.length; i++) {
      if (!marked[i]) {
        const face = dahlgaard[i];
        if (face === trick.category || face === 'ANY') {
          marked[i] = true;
          break;
        }
      }
    }
    s = {
      ...s,
      downtownDice: {
        ...s.downtownDice,
        marked: { ...s.downtownDice.marked, DAHLGAARD: marked },
      },
    };

    s = updatePlayer(s, action.playerId, p => ({
      tricks: [...p.tricks, {
        trickId: action.trickId,
        symbolIndex: action.symbolIndex,
        prepared: false,
        markersOnTrick: 0,
      }],
    }));
    s = updateSymbol(s, action.playerId, action.symbolIndex, () => ({
      assigned: true,
      trickId: action.trickId,
    }));
    s = addLog(s, `${player.name}이(가) ${trick.nameKo} 습득 (심볼 ${action.symbolIndex})`);
    return s;
  },
});

registerHandler<TakeCoinsAction>('TAKE_COINS', {
  validate(state, action) {
    const errors: ValidationError[] = [];
    const dice = state.downtownDice;
    if (action.dieIndex < 0 || action.dieIndex >= dice.BANK.length) {
      errors.push(err('dieIndex', '주사위 인덱스 오류'));
      return errors;
    }
    if (dice.marked.BANK[action.dieIndex]) {
      errors.push(err('die', '이미 사용된 주사위'));
    }
    const face = dice.BANK[action.dieIndex];
    if (face === 'X') errors.push(err('die', 'X면은 사용할 수 없습니다'));
    return errors;
  },
  apply(state, action) {
    const player = getPlayer(state, action.playerId);
    const amount = state.downtownDice.BANK[action.dieIndex] as number;
    const newMarked = [...state.downtownDice.marked.BANK];
    newMarked[action.dieIndex] = true;
    let s: GameState = {
      ...state,
      downtownDice: {
        ...state.downtownDice,
        marked: { ...state.downtownDice.marked, BANK: newMarked },
      },
    };
    s = updatePlayer(s, action.playerId, p => ({ coins: p.coins + amount }));
    s = addLog(s, `${player.name}이(가) ${amount}코인 획득`);
    return s;
  },
});

registerHandler<HireAction>('HIRE', {
  validate(state, action) {
    const errors: ValidationError[] = [];
    const player = state.players.find(p => p.id === action.playerId);
    if (!player) { errors.push(err('playerId', '플레이어 없음')); return errors; }
    const type = action.specialistType;
    if (type !== 'APPRENTICE') {
      const count = player.specialists.filter(s => s === type).length;
      const limit = HIRE_LIMITS[type];
      if (count >= limit) errors.push(err('hire', `${type} 고용 한도 초과`));
    }
    const inn = state.downtownDice.INN;
    const marked = state.downtownDice.marked.INN;
    const available = inn.some((face, i) => !marked[i] && face === type);
    if (!available) errors.push(err('dice', '해당 타입의 주사위가 없습니다'));
    return errors;
  },
  apply(state, action) {
    const player = getPlayer(state, action.playerId);
    const type = action.specialistType;
    const inn = state.downtownDice.INN;
    const newMarked = [...state.downtownDice.marked.INN];
    const dieIdx = inn.findIndex((face, i) => !newMarked[i] && face === type);
    if (dieIdx >= 0) newMarked[dieIdx] = true;

    let s: GameState = {
      ...state,
      downtownDice: {
        ...state.downtownDice,
        marked: { ...state.downtownDice.marked, INN: newMarked },
      },
    };
    s = updatePlayer(s, action.playerId, p => ({
      characters: [...p.characters, {
        type, ap: 0, assigned: false, location: null,
        placed: false, slotIndex: null, slotApMod: 0, shardConverted: false,
        usedDrawFirstCard: false,
      }],
      specialists: type !== 'APPRENTICE'
        ? [...p.specialists, type] as typeof p.specialists
        : p.specialists,
    }));
    s = addLog(s, `${player.name}이(가) ${type} 고용`);
    return s;
  },
});

registerHandler<RerollAction>('REROLL', {
  validate(state, action) {
    const errors: ValidationError[] = [];
    if (!['DAHLGAARD', 'INN', 'BANK'].includes(action.diceGroup)) {
      errors.push(err('diceGroup', '잘못된 주사위 그룹'));
    }
    const dice = state.downtownDice[action.diceGroup];
    if (action.dieIndex < 0 || action.dieIndex >= dice.length) {
      errors.push(err('dieIndex', '주사위 인덱스 오류'));
    }
    return errors;
  },
  apply(state, action) {
    const player = getPlayer(state, action.playerId);
    const group = action.diceGroup;
    const faces = DICE_FACES[group];

    // v4: 주사위 1개만 리롤
    const dice = [...state.downtownDice[group]] as any[];
    const { value, state: ns } = rngRollDie(state, faces as readonly any[]);
    dice[action.dieIndex] = value;
    let s: GameState = { ...ns, downtownDice: { ...ns.downtownDice, [group]: dice } };
    s = addLog(s, `${player.name}이(가) ${group}[${action.dieIndex}] 주사위 리롤`);
    return s;
  },
});

registerHandler<SetDieAction>('SET_DIE', {
  validate(state, action) {
    const errors: ValidationError[] = [];
    if (!['DAHLGAARD', 'INN', 'BANK'].includes(action.diceGroup)) {
      errors.push(err('diceGroup', '잘못된 주사위 그룹'));
      return errors;
    }
    const dice = state.downtownDice[action.diceGroup];
    if (action.dieIndex < 0 || action.dieIndex >= dice.length) {
      errors.push(err('dieIndex', '주사위 인덱스 오류'));
    }
    // Validate desired face is valid for this group
    const validFaces = DICE_FACES[action.diceGroup] as readonly (string | number)[];
    if (!validFaces.includes(action.desiredFace)) {
      errors.push(err('face', '해당 주사위 그룹의 유효한 면이 아닙니다'));
    }
    return errors;
  },
  apply(state, action) {
    const player = getPlayer(state, action.playerId);
    const dice = [...state.downtownDice[action.diceGroup]] as any[];
    dice[action.dieIndex] = action.desiredFace;
    let s: GameState = {
      ...state,
      downtownDice: { ...state.downtownDice, [action.diceGroup]: dice },
    };
    s = addLog(s, `${player.name}이(가) ${action.diceGroup}[${action.dieIndex}]를 ${action.desiredFace}로 설정`);
    return s;
  },
});

registerHandler<ChooseDieAction>('CHOOSE_DIE', {
  validate(state, action) {
    const errors: ValidationError[] = [];
    const group = action.diceGroup;
    if (!['DAHLGAARD', 'INN', 'BANK'].includes(group)) {
      errors.push(err('diceGroup', '잘못된 주사위 그룹'));
      return errors;
    }
    const dice = state.downtownDice[group];
    if (action.dieIndex < 0 || action.dieIndex >= dice.length) {
      errors.push(err('dieIndex', '주사위 인덱스 오류'));
      return errors;
    }
    if (state.downtownDice.marked[group][action.dieIndex]) {
      errors.push(err('die', '이미 사용된 주사위'));
    }
    if (dice[action.dieIndex] === 'X') {
      errors.push(err('die', 'X면은 선택할 수 없습니다'));
    }
    return errors;
  },
  apply(state, action) {
    const newMarked = [...state.downtownDice.marked[action.diceGroup]];
    newMarked[action.dieIndex] = true;
    return {
      ...state,
      downtownDice: {
        ...state.downtownDice,
        marked: { ...state.downtownDice.marked, [action.diceGroup]: newMarked },
      },
    };
  },
});
