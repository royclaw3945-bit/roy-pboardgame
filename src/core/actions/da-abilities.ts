// DA Ability action: USE_DA_ABILITY (v3: 8종 마술사 고유능력)

import type { GameState, GameAction, ValidationError } from '../types';
import { err } from '../types';
import { registerHandler } from './registry';
import { getPlayer } from '../state/selectors';
import { updatePlayer, addLog } from '../state/helpers';
import { getMagicianDef } from '../data/magicians';

type UseDaAbility = Extract<GameAction, { type: 'USE_DA_ABILITY' }>;

registerHandler<UseDaAbility>('USE_DA_ABILITY', {
  validate(state, action) {
    const errors: ValidationError[] = [];
    const player = state.players.find(p => p.id === action.playerId);
    if (!player) { errors.push(err('playerId', '플레이어 없음')); return errors; }

    if (!state.config.useDarkAlley) {
      errors.push(err('config', 'Dark Alley 확장이 비활성화'));
      return errors;
    }

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

    // 능력별 효과는 각 액션 핸들러에서 params를 통해 처리
    // 여기서는 능력 사용 플래그만 설정하고 로그 기록
    s = addLog(s, `${player.name}이(가) DA 능력 사용: ${magician.daAbilityDesc}`);
    return s;
  },
});
