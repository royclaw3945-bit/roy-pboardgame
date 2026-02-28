// Character definitions — static data for each character type

import type { CharacterType } from '../types';

export interface CharacterDef {
  readonly type: CharacterType;
  readonly name: string;
  readonly baseAP: number;
  readonly wage: number;
}

export const CHARACTER_DEFS: Readonly<Record<CharacterType, CharacterDef>> = {
  MAGICIAN:   { type: 'MAGICIAN',   name: '마법사',     baseAP: 3, wage: 0 },
  ENGINEER:   { type: 'ENGINEER',   name: '기술자',     baseAP: 2, wage: 2 },
  MANAGER:    { type: 'MANAGER',    name: '매니저',     baseAP: 2, wage: 2 },
  ASSISTANT:  { type: 'ASSISTANT',  name: '어시스턴트', baseAP: 2, wage: 2 },
  APPRENTICE: { type: 'APPRENTICE', name: '견습생',     baseAP: 1, wage: 1 },
};
