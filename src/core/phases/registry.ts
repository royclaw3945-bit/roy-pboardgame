// Phase configuration registry (v3)

import type { Phase } from '../types';

export interface PhaseConfig {
  readonly name: string;
  readonly nameKo: string;
  readonly autoAdvance: boolean;
}

export const PHASE_CONFIG: Readonly<Record<Phase, PhaseConfig>> = {
  SETUP:            { name: 'Setup',            nameKo: '설정',       autoAdvance: true },
  ADVERTISE:        { name: 'Advertise',        nameKo: '광고',       autoAdvance: false },
  ASSIGNMENT:       { name: 'Assignment',       nameKo: '배정',       autoAdvance: false },
  ASSIGNMENT_REVEAL:{ name: 'Assignment Reveal', nameKo: '배정공개',  autoAdvance: true },
  PLACEMENT:        { name: 'Placement',        nameKo: '배치',       autoAdvance: false },
  PERFORMANCE:      { name: 'Performance',      nameKo: '공연',       autoAdvance: false },
  END_TURN:         { name: 'End Turn',         nameKo: '턴종료',     autoAdvance: true },
  GAME_OVER:        { name: 'Game Over',        nameKo: '게임종료',   autoAdvance: false },
};
