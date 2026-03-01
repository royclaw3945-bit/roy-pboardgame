// Magician definitions — 8 playable magicians (v3: base 4 + DA 4)

import type { MagicianId, TrickCategory, DaAbilityId } from '../types';

export interface MagicianDef {
  readonly id: MagicianId;
  readonly name: string;
  readonly nameKo: string;
  readonly fullName: string;
  readonly favoriteCategory: TrickCategory;
  readonly color: string;
  readonly img: string;
  readonly isExpansion: boolean;
  readonly daAbility: DaAbilityId;
  readonly daAbilityDesc: string;
}

export const MAGICIANS: ReadonlyMap<MagicianId, MagicianDef> = new Map([
  // -- Base Game --
  ['MECHANIKER', {
    id: 'MECHANIKER',
    name: 'The Mechaniker',
    nameKo: '더 메카니커',
    fullName: 'Gerhard von Liebenstein',
    favoriteCategory: 'MECHANICAL',
    color: '#e67e22',
    img: '/img/mechaniker_portrait.jpg',
    isExpansion: false,
    daAbility: 'MECHANIKER_DA',
    daAbilityDesc: '턴 1회: 견습생 1명에게 극장 외 장소에서 AP +1',
  }],
  ['OPTICIAN', {
    id: 'OPTICIAN',
    name: 'The Great Optico',
    nameKo: '더 그레이트 옵티코',
    fullName: 'Leo Sanderberg',
    favoriteCategory: 'OPTICAL',
    color: '#9b59b6',
    img: '/img/optico_portrait.jpg',
    isExpansion: false,
    daAbility: 'OPTICIAN_DA',
    daAbilityDesc: '턴 1회: 자기 영구 배치카드에 상대 공개 특수 배치카드 효과 복사',
  }],
  ['ESCAPIST', {
    id: 'ESCAPIST',
    name: 'Master of Chains',
    nameKo: '사슬의 대가',
    fullName: 'Frederic White',
    favoriteCategory: 'ESCAPE',
    color: '#27ae60',
    img: '/img/chains_portrait.jpg',
    isExpansion: false,
    daAbility: 'ESCAPIST_DA',
    daAbilityDesc: 'Performance 전 무료 Reschedule 1회 (링크보너스 받음)',
  }],
  ['SPIRITUALIST', {
    id: 'SPIRITUALIST',
    name: 'Priestess of Mysticism',
    nameKo: '신비의 여사제',
    fullName: 'Amabel Fae',
    favoriteCategory: 'SPIRITUAL',
    color: '#3498db',
    img: '/img/priestess_portrait.jpg',
    isExpansion: false,
    daAbility: 'SPIRITUALIST_DA',
    daAbilityDesc: 'DA AP 1: Active 예언 교체 (Pending 예언 선택, 새 예언 뽑기)',
  }],
  // -- Dark Alley Expansion --
  ['ELECTRA', {
    id: 'ELECTRA',
    name: 'The Great Electra',
    nameKo: '더 그레이트 일렉트라',
    fullName: 'Electra Voronova',
    favoriteCategory: 'MECHANICAL',
    color: '#f39c12',
    img: '/img/electra_portrait.jpg',
    isExpansion: true,
    daAbility: 'ELECTRA_DA',
    daAbilityDesc: 'Set Up 시 같은 트릭 마커 2개를 한 슬롯에 쌓기 가능',
  }],
  ['GENTLEMAN', {
    id: 'GENTLEMAN',
    name: 'The Gentleman',
    nameKo: '더 젠틀맨',
    fullName: 'Arthur Livingstone',
    favoriteCategory: 'OPTICAL',
    color: '#8e44ad',
    img: '/img/gentleman_portrait.jpg',
    isExpansion: true,
    daAbility: 'GENTLEMAN_DA',
    daAbilityDesc: '마법사 다운타운/시장/DA 배치 시 보유 트릭 수만큼 명성',
  }],
  ['RED_LOTUS', {
    id: 'RED_LOTUS',
    name: 'The Red Lotus',
    nameKo: '더 레드 로터스',
    fullName: 'Mei Ling',
    favoriteCategory: 'ESCAPE',
    color: '#c0392b',
    img: '/img/red_lotus_portrait.jpg',
    isExpansion: true,
    daAbility: 'RED_LOTUS_DA',
    daAbilityDesc: '퍼포머일 때 상대 트릭 수익을 자기 것 대신 받기 가능',
  }],
  ['YORUBA', {
    id: 'YORUBA',
    name: 'The Yoruba',
    nameKo: '더 요루바',
    fullName: 'Olufemi Adeyemi',
    favoriteCategory: 'SPIRITUAL',
    color: '#2980b9',
    img: '/img/yoruba_portrait.jpg',
    isExpansion: true,
    daAbility: 'YORUBA_DA',
    daAbilityDesc: '턴 1회: 샤드 1개로 상대 퍼포먼스 카드 대신 선택',
  }],
]);

export function getMagicianDef(id: MagicianId): MagicianDef {
  const def = MAGICIANS.get(id);
  if (!def) throw new Error(`Unknown magician: ${id}`);
  return def;
}

export function getBaseMagicianIds(): readonly MagicianId[] {
  return [...MAGICIANS.values()]
    .filter(m => !m.isExpansion)
    .map(m => m.id);
}

export function getAllMagicianIds(): readonly MagicianId[] {
  return [...MAGICIANS.keys()];
}
