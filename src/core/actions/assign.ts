// Assignment phase actions (v3: 동시 비밀배치)
// PLACE_ASSIGNMENT_CARD, REMOVE_ASSIGNMENT_CARD, SUBMIT_ASSIGNMENT, REVEAL_ASSIGNMENTS

import type { GameState, GameAction, ValidationError, PlayerId, AssignmentCardPlacement } from '../types';
import { err } from '../types';
import { registerHandler } from './registry';
import { getPlayer } from '../state/selectors';
import { updatePlayer, updateCharacter, addLog } from '../state/helpers';

type PlaceCardAction = Extract<GameAction, { type: 'PLACE_ASSIGNMENT_CARD' }>;
type RemoveCardAction = Extract<GameAction, { type: 'REMOVE_ASSIGNMENT_CARD' }>;
type SubmitAction = Extract<GameAction, { type: 'SUBMIT_ASSIGNMENT' }>;
type RevealAction = Extract<GameAction, { type: 'REVEAL_ASSIGNMENTS' }>;

registerHandler<PlaceCardAction>('PLACE_ASSIGNMENT_CARD', {
  validate(state, action) {
    const errors: ValidationError[] = [];
    if (state.phase !== 'ASSIGNMENT') errors.push(err('phase', '배정 페이즈가 아닙니다'));
    const player = state.players.find(p => p.id === action.playerId);
    if (!player) { errors.push(err('playerId', '플레이어 없음')); return errors; }

    // 이미 제출한 플레이어는 변경 불가
    if (state.assignmentPhase?.playersSubmitted.includes(action.playerId)) {
      errors.push(err('submitted', '이미 배정을 확정했습니다'));
      return errors;
    }

    // 카드 소유 확인
    const card = player.assignmentCards.find(c => c.id === action.cardId);
    if (!card) { errors.push(err('cardId', '소유하지 않은 배정 카드')); return errors; }

    // 이미 배치된 카드인지 확인
    if (player.currentPlacements.some(p => p.cardId === action.cardId)) {
      errors.push(err('cardId', '이미 배치된 카드'));
    }

    // 캐릭터 인덱스 유효성
    for (const idx of action.characterIndices) {
      const char = player.characters[idx];
      if (!char) { errors.push(err('charIdx', `캐릭터 인덱스 ${idx} 오류`)); continue; }
      // 다른 카드에 이미 배정된 캐릭터인지 확인
      const alreadyPlaced = player.currentPlacements.some(
        p => p.characterIndices.includes(idx),
      );
      if (alreadyPlaced) errors.push(err('charIdx', `캐릭터 ${idx}는 이미 다른 카드에 배정됨`));
    }

    if (action.characterIndices.length === 0) {
      errors.push(err('characters', '최소 1명의 캐릭터를 배정해야 합니다'));
    }

    return errors;
  },
  apply(state, action) {
    const placement: AssignmentCardPlacement = {
      cardId: action.cardId,
      characterIndices: action.characterIndices,
    };
    return updatePlayer(state, action.playerId, p => ({
      currentPlacements: [...p.currentPlacements, placement],
    }));
  },
});

registerHandler<RemoveCardAction>('REMOVE_ASSIGNMENT_CARD', {
  validate(state, action) {
    const errors: ValidationError[] = [];
    if (state.phase !== 'ASSIGNMENT') errors.push(err('phase', '배정 페이즈가 아닙니다'));
    const player = state.players.find(p => p.id === action.playerId);
    if (!player) { errors.push(err('playerId', '플레이어 없음')); return errors; }

    if (state.assignmentPhase?.playersSubmitted.includes(action.playerId)) {
      errors.push(err('submitted', '이미 배정을 확정했습니다'));
      return errors;
    }

    const idx = player.currentPlacements.findIndex(p => p.cardId === action.cardId);
    if (idx === -1) errors.push(err('cardId', '배치되지 않은 카드'));

    return errors;
  },
  apply(state, action) {
    return updatePlayer(state, action.playerId, p => ({
      currentPlacements: p.currentPlacements.filter(pl => pl.cardId !== action.cardId),
    }));
  },
});

registerHandler<SubmitAction>('SUBMIT_ASSIGNMENT', {
  validate(state, action) {
    const errors: ValidationError[] = [];
    if (state.phase !== 'ASSIGNMENT') errors.push(err('phase', '배정 페이즈가 아닙니다'));
    const player = state.players.find(p => p.id === action.playerId);
    if (!player) { errors.push(err('playerId', '플레이어 없음')); return errors; }

    if (state.assignmentPhase?.playersSubmitted.includes(action.playerId)) {
      errors.push(err('submitted', '이미 배정을 확정했습니다'));
    }

    // 최소 1명은 배정해야 함
    const totalChars = player.currentPlacements.reduce(
      (sum, p) => sum + p.characterIndices.length, 0,
    );
    if (totalChars === 0) {
      errors.push(err('assignment', '최소 1명은 배정해야 합니다'));
    }

    return errors;
  },
  apply(state, action) {
    const player = getPlayer(state, action.playerId);
    const newSubmitted = [
      ...(state.assignmentPhase?.playersSubmitted ?? []),
      action.playerId,
    ];

    // 다음 미제출 플레이어로 전환
    const nextAssigner = state.players.find(
      p => !newSubmitted.includes(p.id),
    )?.id ?? null;

    let s: GameState = {
      ...state,
      assignmentPhase: {
        ...state.assignmentPhase!,
        playersSubmitted: newSubmitted,
        currentAssigner: nextAssigner,
      },
    };
    s = addLog(s, `${player.name}이(가) 배정 확정`);
    return s;
  },
});

registerHandler<RevealAction>('REVEAL_ASSIGNMENTS', {
  validate(state) {
    const errors: ValidationError[] = [];
    if (state.phase !== 'ASSIGNMENT') errors.push(err('phase', '배정 페이즈가 아닙니다'));
    // 모든 플레이어가 제출했는지 확인
    const submitted = state.assignmentPhase?.playersSubmitted ?? [];
    if (submitted.length < state.players.length) {
      errors.push(err('submitted', '모든 플레이어가 배정을 확정하지 않았습니다'));
    }
    return errors;
  },
  apply(state) {
    // 모든 플레이어의 currentPlacements를 실제 character state에 반영
    let s = state;
    for (const player of s.players) {
      for (const placement of player.currentPlacements) {
        const card = player.assignmentCards.find(c => c.id === placement.cardId);
        if (!card) continue;
        for (const charIdx of placement.characterIndices) {
          s = updateCharacter(s, player.id, charIdx, () => ({
            assigned: true,
            location: card.location,
          }));
        }
      }
    }

    s = { ...s, phase: 'ASSIGNMENT_REVEAL' };
    s = addLog(s, '모든 플레이어의 배정이 공개되었습니다');
    return s;
  },
});
