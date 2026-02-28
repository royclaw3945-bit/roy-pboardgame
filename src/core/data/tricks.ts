// 48 Trick definitions — 4 categories × 3 levels × 4 tricks
// Archetype data only — runtime state is TrickSlot in player.ts

import type { TrickId, TrickCategory, TrickLevel, ComponentType, EndBonusType } from '../types';

export interface Yields {
  readonly fame: number;
  readonly coins: number;
  readonly shards: number;
}

export interface EndBonus {
  readonly type: EndBonusType;
  readonly fame?: number;
  readonly famePerUnit?: number;
  readonly desc: string;
}

export interface TrickDef {
  readonly id: TrickId;
  readonly name: string;
  readonly nameKo: string;
  readonly category: TrickCategory;
  readonly level: TrickLevel;
  readonly fameThreshold: number;
  readonly components: Readonly<Partial<Record<ComponentType, number>>>;
  readonly prepareCost: number;
  readonly markerSlots: number;
  readonly yields: Yields;
  readonly endBonus: EndBonus | null;
}

function t(id: string, name: string, nameKo: string, category: TrickCategory,
  level: TrickLevel, fameThreshold: number,
  components: Partial<Record<ComponentType, number>>,
  prepareCost: number, markerSlots: number,
  yields: Yields, endBonus: EndBonus | null = null,
): TrickDef {
  return {
    id: id as TrickId, name, nameKo, category, level, fameThreshold,
    components, prepareCost, markerSlots, yields, endBonus,
  };
}

const ALL_TRICKS: readonly TrickDef[] = [
  // === MECHANICAL ===
  // Level 1
  t('M1A','Linking Rings','링킹 링스','MECHANICAL',1,1,{METAL:2},1,2,{fame:0,coins:2,shards:0}),
  t('M1B','Living Piano','살아있는 피아노','MECHANICAL',1,1,{WOOD:1,GLASS:1,ANIMAL:1},1,3,{fame:1,coins:1,shards:0}),
  t('M1C','Chinese Sticks','차이니즈 스틱','MECHANICAL',1,1,{WOOD:2,ROPE:2},1,2,{fame:1,coins:2,shards:0}),
  t('M1D','Levitation','공중부양','MECHANICAL',1,1,{GLASS:3,ROPE:1,OIL:2},1,2,{fame:2,coins:3,shards:0}),
  // Level 2
  t('M2A','Mechanical Hornet','기계 호넷','MECHANICAL',2,16,{METAL:3,OIL:1,GEAR:1},2,2,{fame:4,coins:3,shards:0}),
  t('M2B','Sawing the Assistant','어시스턴트 절단','MECHANICAL',2,16,{WOOD:3,SAW:3},1,3,{fame:1,coins:5,shards:0}),
  t('M2C','Vanishing Bird Cage','사라지는 새장','MECHANICAL',2,16,{METAL:2,GEAR:1,ANIMAL:2},1,2,{fame:2,coins:3,shards:1}),
  t('M2D','Bullet Catch','총알 잡기','MECHANICAL',2,16,{METAL:2,ROPE:2,OIL:2},2,3,{fame:3,coins:4,shards:0}),
  // Level 3
  t('M3A','Aztec Lady','아즈텍 레이디','MECHANICAL',3,36,{GLASS:2,SAW:1,LOCK:1,GEAR:2},2,3,{fame:5,coins:5,shards:0},
    {type:'TRICK_MARKERS_ON_PERF',famePerUnit:2,desc:'퍼포먼스 카드 위 트릭마커 1개당 2명성'}),
  t('M3B','Horror Saws','공포의 톱','MECHANICAL',3,36,{WOOD:3,SAW:2,GEAR:2},2,2,{fame:4,coins:8,shards:0},
    {type:'PER_SHARD',famePerUnit:1,desc:'보유 샤드 1개당 1명성'}),
  t('M3C','Automaton','오토마톤','MECHANICAL',3,36,{METAL:3,OIL:3,GEAR:3},1,1,{fame:7,coins:7,shards:0},
    {type:'PER_L2_TRICK',famePerUnit:4,desc:'보유 레벨2 트릭 1개당 4명성'}),
  t('M3D','Hellhound','헬하운드','MECHANICAL',3,36,{FABRIC:3,OIL:1,LOCK:2,ANIMAL:2},2,2,{fame:6,coins:5,shards:1}),

  // === OPTICAL ===
  t('O1A','Enchanted Butterflies','마법 나비','OPTICAL',1,1,{FABRIC:2},1,2,{fame:2,coins:0,shards:0}),
  t('O1B','Rabbit from Top Hat','모자 속 토끼','OPTICAL',1,1,{METAL:1,FABRIC:1,ANIMAL:1},1,1,{fame:3,coins:1,shards:0}),
  t('O1C','Pub-In-A-Bottle','병 속 선술집','OPTICAL',1,1,{GLASS:3,SAW:1,ROPE:1},1,2,{fame:2,coins:2,shards:0}),
  t('O1D','Card Manipulation','카드 마술','OPTICAL',1,1,{WOOD:3,FABRIC:3},1,2,{fame:1,coins:1,shards:1}),
  t('O2A','Self Decapitation','자기 참수','OPTICAL',2,16,{METAL:3,SAW:1,DISGUISE:1},1,3,{fame:3,coins:2,shards:0}),
  t('O2B','Paper Shred','종이 파쇄','OPTICAL',2,16,{FABRIC:3,SAW:2,MIRROR:1},2,2,{fame:4,coins:2,shards:1}),
  t('O2C','Shattered Mirror','깨진 거울','OPTICAL',2,16,{WOOD:3,GLASS:3,MIRROR:2},2,2,{fame:5,coins:3,shards:0}),
  t('O2D','Fishing in the Air','공중 낚시','OPTICAL',2,16,{WOOD:3,ROPE:2,ANIMAL:3},2,3,{fame:3,coins:4,shards:1}),
  t('O3A','Mutilation','절단술','OPTICAL',3,36,{GLASS:2,FABRIC:3,SAW:2,DISGUISE:1},2,2,{fame:6,coins:5,shards:0},
    {type:'ALL_SPECIALISTS',fame:12,desc:'3종 전문가 모두 보유 시 12명성'}),
  t('O3B','Stairs of Water','물의 계단','OPTICAL',3,36,{GLASS:3,OIL:2,GEAR:1,DISGUISE:1},3,3,{fame:5,coins:4,shards:2},
    {type:'PER_ADVANCED_COMP',famePerUnit:2,desc:'고급 컴포넌트 종류당 2명성'}),
  t('O3C','Beast Within','내면의 야수','OPTICAL',3,36,{METAL:3,ANIMAL:3,MIRROR:1,DISGUISE:1},2,2,{fame:7,coins:3,shards:1},
    {type:'FOUR_TRICKS',fame:10,desc:'트릭 4개 보유 시 10명성'}),
  t('O3D','Vanishing Elephant','사라지는 코끼리','OPTICAL',3,36,{GLASS:3,LOCK:2,ANIMAL:2,MIRROR:1},2,2,{fame:9,coins:4,shards:0},
    {type:'HAS_MANAGER',fame:7,desc:'매니저 보유 시 7명성'}),

  // === ESCAPE ===
  t('E1A','Barricaded Barrels','봉쇄된 통','ESCAPE',1,1,{WOOD:2},1,2,{fame:1,coins:1,shards:0}),
  t('E1B','Stocks Escape','족쇄 탈출','ESCAPE',1,1,{WOOD:2,METAL:2},1,2,{fame:0,coins:1,shards:1}),
  t('E1C','Burning Mummy','불타는 미라','ESCAPE',1,1,{FABRIC:3,OIL:1},1,1,{fame:2,coins:2,shards:0}),
  t('E1D','Water Tank Escape','수조 탈출','ESCAPE',1,1,{GLASS:2,METAL:2,ROPE:1},1,2,{fame:1,coins:2,shards:1}),
  t('E2A','Prison Break','감옥 탈출','ESCAPE',2,16,{METAL:3,DISGUISE:2},1,2,{fame:3,coins:3,shards:0}),
  t('E2B','Zig Zag Lady','지그재그 레이디','ESCAPE',2,16,{WOOD:3,FABRIC:1,OIL:1},1,3,{fame:2,coins:3,shards:0}),
  t('E2C','Walled','벽 속에','ESCAPE',2,16,{WOOD:3,METAL:3,LOCK:1},1,2,{fame:3,coins:2,shards:1}),
  t('E2D','Wolf Cage','늑대 우리','ESCAPE',2,16,{METAL:2,OIL:1,ANIMAL:2},1,1,{fame:3,coins:3,shards:1}),
  t('E3A','Buried Alive','생매장','ESCAPE',3,36,{WOOD:3,LOCK:3},1,2,{fame:4,coins:4,shards:0},
    {type:'HAS_ENGINEER',fame:7,desc:'기술자 보유 시 7명성'}),
  t('E3B',"Assistant's Revenge",'어시스턴트의 복수','ESCAPE',3,36,{GLASS:3,SAW:2,MIRROR:2},1,1,{fame:6,coins:6,shards:0},
    {type:'HAS_ASSISTANT',fame:7,desc:'어시스턴트 보유 시 7명성'}),
  t('E3C','Iron Maiden','아이언 메이든','ESCAPE',3,36,{METAL:3,SAW:3,LOCK:2},2,2,{fame:5,coins:5,shards:1},
    {type:'PER_L1_TRICK',famePerUnit:4,desc:'보유 레벨1 트릭 1개당 4명성'}),
  t('E3D','Transported Man','순간이동','ESCAPE',3,36,{FABRIC:3,OIL:2,DISGUISE:2},2,2,{fame:5,coins:3,shards:2},
    {type:'PER_L3_TRICK',famePerUnit:4,desc:'보유 레벨3 트릭 1개당 4명성'}),

  // === SPIRITUAL ===
  t('S1A','Mind Reading','독심술','SPIRITUAL',1,1,{GLASS:2},1,3,{fame:0,coins:0,shards:1}),
  t('S1B','Breath of Life','생명의 숨결','SPIRITUAL',1,1,{METAL:1,FABRIC:2,SAW:1},1,1,{fame:2,coins:3,shards:0}),
  t('S1C','Spirit Hand','영혼의 손','SPIRITUAL',1,1,{FABRIC:3,ROPE:1,ANIMAL:1},1,2,{fame:2,coins:1,shards:1}),
  t('S1D','Window to the Otherworld','이세계의 창','SPIRITUAL',1,1,{METAL:2,OIL:1,MIRROR:1},1,2,{fame:3,coins:2,shards:0}),
  t('S2A','Floating Table','떠다니는 테이블','SPIRITUAL',2,16,{WOOD:3,FABRIC:3,ROPE:3,OIL:1},2,2,{fame:5,coins:2,shards:1}),
  t('S2B','Future Sight','미래 투시','SPIRITUAL',2,16,{METAL:2,FABRIC:2,MIRROR:1},1,1,{fame:4,coins:1,shards:0}),
  t('S2C',"Pepper's Ghost",'페퍼스 고스트','SPIRITUAL',2,16,{SAW:2,MIRROR:2,DISGUISE:2},2,2,{fame:4,coins:5,shards:0}),
  t('S2D','Ghost Trap','유령 덫','SPIRITUAL',2,16,{WOOD:3,GLASS:3,FABRIC:3,ANIMAL:3},2,3,{fame:3,coins:3,shards:1}),
  t('S3A',"Balsamo's Skull",'발사모의 해골','SPIRITUAL',3,36,{METAL:3,ROPE:2,LOCK:1},2,3,{fame:4,coins:3,shards:2},
    {type:'PER_SUPERIOR_COMP',famePerUnit:3,desc:'최상급 컴포넌트 종류당 3명성'}),
  t('S3B','Seance','강령회','SPIRITUAL',3,36,{WOOD:3,OIL:2,MIRROR:2},2,2,{fame:7,coins:5,shards:1},
    {type:'PER_APPRENTICE',famePerUnit:3,desc:'견습생 1명당 3명성'}),
  t('S3C','Skeleton Dance','해골 춤','SPIRITUAL',3,36,{GLASS:3,ROPE:2,GEAR:2},3,3,{fame:6,coins:4,shards:0},
    {type:'PER_BASIC_COMP',famePerUnit:1,desc:'기본 컴포넌트 종류당 1명성'}),
  t('S3D','Metamorphosis','변신','SPIRITUAL',3,36,{GLASS:3,FABRIC:3,ROPE:3,DISGUISE:3},3,3,{fame:10,coins:4,shards:0},
    {type:'PER_3_COINS',famePerUnit:1,desc:'남은 코인 3개당 1명성'}),
];

// -- ReadonlyMap for O(1) lookup by TrickId --
export const TRICKS: ReadonlyMap<TrickId, TrickDef> = new Map(
  ALL_TRICKS.map(trick => [trick.id, trick]),
);

export function getTricksByCategory(category: TrickCategory): readonly TrickDef[] {
  return ALL_TRICKS.filter(t => t.category === category);
}

export function getTricksByLevel(level: TrickLevel): readonly TrickDef[] {
  return ALL_TRICKS.filter(t => t.level === level);
}

export function getTrickDef(id: TrickId): TrickDef {
  const def = TRICKS.get(id);
  if (!def) throw new Error(`Unknown trick: ${id}`);
  return def;
}

export function getAllTrickIds(): readonly TrickId[] {
  return ALL_TRICKS.map(t => t.id);
}

export function getCategoryTrickIds(category: TrickCategory): readonly TrickId[] {
  return ALL_TRICKS.filter(t => t.category === category).map(t => t.id);
}
