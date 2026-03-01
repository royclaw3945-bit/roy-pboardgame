// DA Ability action: USE_DA_ABILITY (v4: DA always enabled)

import type { GameState, GameAction, ValidationError } from '../types';
import { err } from '../types';
import { registerHandler } from './registry';
import { getPlayer } from '../state/selectors';
import { updatePlayer, adjustFame, addLog } from '../state/helpers';
import { getMagicianDef } from '../data/magicians';

type UseDaAbility = Extract<GameAction, { type: 'USE_DA_ABILITY' }>;

registerHandler<UseDaAbility>('USE_DA_ABILITY', {
  validate(state, action) {
    const errors: ValidationError[] = [];
    const player = state.players.find(p => p.id === action.playerId);
    if (!player) { errors.push(err('playerId', '플레이어 없음')); return errors; }

    if (player.usedAbilityThisTurn) {
      errors.push(err('ability', '이번 턴에 이미 능력을 사용했습니다'));
    }

    const magician = getMagicianDef(player.magicianId);
    if (magician.daAbility !== action.abilityId) {
      errors.push(err('abilityId', '해당 마술사의 능력이 아닙니다'));
    }

    return errors;
  },
  apply(state, action) {
    const player = getPlayer(state, action.playerId);
    const magician = getMagicianDef(player.magicianId);

    let s = updatePlayer(state, action.playerId, () => ({
      usedAbilityThisTurn: true,
    }));

    // MECHANIKER: 견습생 1명 AP+1 (params.apprenticeIdx)
    if (action.abilityId === 'MECHANIKER_DA' && action.params) {
      const idx = action.params.apprenticeIdx as number;
      if (idx !== undefined) {
        const p = getPlayer(s, action.playerId);
        const char = p.characters[idx];
        if (char && char.type === 'APPRENTICE' && char.location !== 'THEATER') {
          s = {
            ...s,
            players: s.players.map(pl =>
              pl.id === action.playerId ? {
                ...pl,
                characters: pl.characters.map((c, i) =>
                  i === idx ? { ...c, ap: c.ap + 1 } : c,
                ),
              } : pl,
            ),
          };
        }
      }
    }

    // GENTLEMAN: 마법사 배치 시 트릭 수만큼 명성
    if (action.abilityId === 'GENTLEMAN_DA') {
      const p = getPlayer(s, action.playerId);
      s = adjustFame(s, action.playerId, p.tricks.length);
    }

    // Other abilities: RED_LOTUS, YORUBA, OPTICO, PRIESTESS, ELECTRA, ESCAPIST
    // → stub (예언/특수카드 시스템 필요)

    s = addLog(s, `${player.name}이(가) DA 능력 사용: ${magician.daAbilityDesc}`);
    return s;
  },
});
