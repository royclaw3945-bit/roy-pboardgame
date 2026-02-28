// Trickerion - Trick Cards Data (Official Rulebook)
// 48 tricks: 4 categories x 3 levels x 4 tricks
// Components use { TYPE: quantity } format (non-consumption: check presence only)

export const TRICKS = {
  MECHANICAL: [
    // Level 1 (Fame >= 1)
    { id: 'M1A', name: 'Linking Rings', nameKo: '링킹 링스', category: 'MECHANICAL', level: 1,
      fameThreshold: 1, components: { METAL: 2 }, prepareCost: 1, markerSlots: 2,
      yields: { fame: 0, coins: 2, shards: 0 } },
    { id: 'M1B', name: 'Living Piano', nameKo: '살아있는 피아노', category: 'MECHANICAL', level: 1,
      fameThreshold: 1, components: { WOOD: 1, GLASS: 1, ANIMAL: 1 }, prepareCost: 1, markerSlots: 3,
      yields: { fame: 1, coins: 1, shards: 0 } },
    { id: 'M1C', name: 'Chinese Sticks', nameKo: '차이니즈 스틱', category: 'MECHANICAL', level: 1,
      fameThreshold: 1, components: { WOOD: 2, ROPE: 2 }, prepareCost: 1, markerSlots: 2,
      yields: { fame: 1, coins: 2, shards: 0 } },
    { id: 'M1D', name: 'Levitation', nameKo: '공중부양', category: 'MECHANICAL', level: 1,
      fameThreshold: 1, components: { GLASS: 3, ROPE: 1, OIL: 2 }, prepareCost: 1, markerSlots: 2,
      yields: { fame: 2, coins: 3, shards: 0 } },
    // Level 2 (Fame >= 16)
    { id: 'M2A', name: 'Mechanical Hornet', nameKo: '기계 호넷', category: 'MECHANICAL', level: 2,
      fameThreshold: 16, components: { METAL: 3, OIL: 1, GEAR: 1 }, prepareCost: 2, markerSlots: 2,
      yields: { fame: 4, coins: 3, shards: 0 } },
    { id: 'M2B', name: 'Sawing the Assistant', nameKo: '어시스턴트 절단', category: 'MECHANICAL', level: 2,
      fameThreshold: 16, components: { WOOD: 3, SAW: 3 }, prepareCost: 1, markerSlots: 3,
      yields: { fame: 1, coins: 5, shards: 0 } },
    { id: 'M2C', name: 'Vanishing Bird Cage', nameKo: '사라지는 새장', category: 'MECHANICAL', level: 2,
      fameThreshold: 16, components: { METAL: 2, GEAR: 1, ANIMAL: 2 }, prepareCost: 1, markerSlots: 2,
      yields: { fame: 2, coins: 3, shards: 1 } },
    { id: 'M2D', name: 'Bullet Catch', nameKo: '총알 잡기', category: 'MECHANICAL', level: 2,
      fameThreshold: 16, components: { METAL: 2, ROPE: 2, OIL: 2 }, prepareCost: 2, markerSlots: 3,
      yields: { fame: 3, coins: 4, shards: 0 } },
    // Level 3 (Fame >= 36)
    { id: 'M3A', name: 'Aztec Lady', nameKo: '아즈텍 레이디', category: 'MECHANICAL', level: 3,
      fameThreshold: 36, components: { GLASS: 2, SAW: 1, LOCK: 1, GEAR: 2 }, prepareCost: 2, markerSlots: 3,
      yields: { fame: 5, coins: 5, shards: 0 },
      endBonus: { type: 'TRICK_MARKERS_ON_PERF', famePerUnit: 2, desc: '퍼포먼스 카드 위 트릭마커 1개당 2명성' } },
    { id: 'M3B', name: 'Horror Saws', nameKo: '공포의 톱', category: 'MECHANICAL', level: 3,
      fameThreshold: 36, components: { WOOD: 3, SAW: 2, GEAR: 2 }, prepareCost: 2, markerSlots: 2,
      yields: { fame: 4, coins: 8, shards: 0 },
      endBonus: { type: 'PER_SHARD', famePerUnit: 1, desc: '보유 샤드 1개당 1명성' } },
    { id: 'M3C', name: 'Automaton', nameKo: '오토마톤', category: 'MECHANICAL', level: 3,
      fameThreshold: 36, components: { METAL: 3, OIL: 3, GEAR: 3 }, prepareCost: 1, markerSlots: 1,
      yields: { fame: 7, coins: 7, shards: 0 },
      endBonus: { type: 'PER_L2_TRICK', famePerUnit: 4, desc: '보유 레벨2 트릭 1개당 4명성' } },
    { id: 'M3D', name: 'Hellhound', nameKo: '헬하운드', category: 'MECHANICAL', level: 3,
      fameThreshold: 36, components: { FABRIC: 3, OIL: 1, LOCK: 2, ANIMAL: 2 }, prepareCost: 2, markerSlots: 2,
      yields: { fame: 6, coins: 5, shards: 1 },
      endBonus: null },
  ],
  OPTICAL: [
    // Level 1
    { id: 'O1A', name: 'Enchanted Butterflies', nameKo: '마법 나비', category: 'OPTICAL', level: 1,
      fameThreshold: 1, components: { FABRIC: 2 }, prepareCost: 1, markerSlots: 2,
      yields: { fame: 2, coins: 0, shards: 0 } },
    { id: 'O1B', name: 'Rabbit from Top Hat', nameKo: '모자 속 토끼', category: 'OPTICAL', level: 1,
      fameThreshold: 1, components: { METAL: 1, FABRIC: 1, ANIMAL: 1 }, prepareCost: 1, markerSlots: 1,
      yields: { fame: 3, coins: 1, shards: 0 } },
    { id: 'O1C', name: 'Pub-In-A-Bottle', nameKo: '병 속 선술집', category: 'OPTICAL', level: 1,
      fameThreshold: 1, components: { GLASS: 3, SAW: 1, ROPE: 1 }, prepareCost: 1, markerSlots: 2,
      yields: { fame: 2, coins: 2, shards: 0 } },
    { id: 'O1D', name: 'Card Manipulation', nameKo: '카드 마술', category: 'OPTICAL', level: 1,
      fameThreshold: 1, components: { WOOD: 3, FABRIC: 3 }, prepareCost: 1, markerSlots: 2,
      yields: { fame: 1, coins: 1, shards: 1 } },
    // Level 2
    { id: 'O2A', name: 'Self Decapitation', nameKo: '자기 참수', category: 'OPTICAL', level: 2,
      fameThreshold: 16, components: { METAL: 3, SAW: 1, DISGUISE: 1 }, prepareCost: 1, markerSlots: 3,
      yields: { fame: 3, coins: 2, shards: 0 } },
    { id: 'O2B', name: 'Paper Shred', nameKo: '종이 파쇄', category: 'OPTICAL', level: 2,
      fameThreshold: 16, components: { FABRIC: 3, SAW: 2, MIRROR: 1 }, prepareCost: 2, markerSlots: 2,
      yields: { fame: 4, coins: 2, shards: 1 } },
    { id: 'O2C', name: 'Shattered Mirror', nameKo: '깨진 거울', category: 'OPTICAL', level: 2,
      fameThreshold: 16, components: { WOOD: 3, GLASS: 3, MIRROR: 2 }, prepareCost: 2, markerSlots: 2,
      yields: { fame: 5, coins: 3, shards: 0 } },
    { id: 'O2D', name: 'Fishing in the Air', nameKo: '공중 낚시', category: 'OPTICAL', level: 2,
      fameThreshold: 16, components: { WOOD: 3, ROPE: 2, ANIMAL: 3 }, prepareCost: 2, markerSlots: 3,
      yields: { fame: 3, coins: 4, shards: 1 } },
    // Level 3
    { id: 'O3A', name: 'Mutilation', nameKo: '절단술', category: 'OPTICAL', level: 3,
      fameThreshold: 36, components: { GLASS: 2, FABRIC: 3, SAW: 2, DISGUISE: 1 }, prepareCost: 2, markerSlots: 2,
      yields: { fame: 6, coins: 5, shards: 0 },
      endBonus: { type: 'ALL_SPECIALISTS', fame: 12, desc: '3종 전문가 모두 보유 시 12명성' } },
    { id: 'O3B', name: 'Stairs of Water', nameKo: '물의 계단', category: 'OPTICAL', level: 3,
      fameThreshold: 36, components: { GLASS: 3, OIL: 2, GEAR: 1, DISGUISE: 1 }, prepareCost: 3, markerSlots: 3,
      yields: { fame: 5, coins: 4, shards: 2 },
      endBonus: { type: 'PER_ADVANCED_COMP', famePerUnit: 2, desc: '고급 컴포넌트 종류당 2명성' } },
    { id: 'O3C', name: 'Beast Within', nameKo: '내면의 야수', category: 'OPTICAL', level: 3,
      fameThreshold: 36, components: { METAL: 3, ANIMAL: 3, MIRROR: 1, DISGUISE: 1 }, prepareCost: 2, markerSlots: 2,
      yields: { fame: 7, coins: 3, shards: 1 },
      endBonus: { type: 'FOUR_TRICKS', fame: 10, desc: '트릭 4개 보유 시 10명성' } },
    { id: 'O3D', name: 'Vanishing Elephant', nameKo: '사라지는 코끼리', category: 'OPTICAL', level: 3,
      fameThreshold: 36, components: { GLASS: 3, LOCK: 2, ANIMAL: 2, MIRROR: 1 }, prepareCost: 2, markerSlots: 2,
      yields: { fame: 9, coins: 4, shards: 0 },
      endBonus: { type: 'HAS_MANAGER', fame: 7, desc: '매니저 보유 시 7명성' } },
  ],
  ESCAPE: [
    // Level 1
    { id: 'E1A', name: 'Barricaded Barrels', nameKo: '봉쇄된 통', category: 'ESCAPE', level: 1,
      fameThreshold: 1, components: { WOOD: 2 }, prepareCost: 1, markerSlots: 2,
      yields: { fame: 1, coins: 1, shards: 0 } },
    { id: 'E1B', name: 'Stocks Escape', nameKo: '족쇄 탈출', category: 'ESCAPE', level: 1,
      fameThreshold: 1, components: { WOOD: 2, METAL: 2 }, prepareCost: 1, markerSlots: 2,
      yields: { fame: 0, coins: 1, shards: 1 } },
    { id: 'E1C', name: 'Burning Mummy', nameKo: '불타는 미라', category: 'ESCAPE', level: 1,
      fameThreshold: 1, components: { FABRIC: 3, OIL: 1 }, prepareCost: 1, markerSlots: 1,
      yields: { fame: 2, coins: 2, shards: 0 } },
    { id: 'E1D', name: 'Water Tank Escape', nameKo: '수조 탈출', category: 'ESCAPE', level: 1,
      fameThreshold: 1, components: { GLASS: 2, METAL: 2, ROPE: 1 }, prepareCost: 1, markerSlots: 2,
      yields: { fame: 1, coins: 2, shards: 1 } },
    // Level 2
    { id: 'E2A', name: 'Prison Break', nameKo: '감옥 탈출', category: 'ESCAPE', level: 2,
      fameThreshold: 16, components: { METAL: 3, DISGUISE: 2 }, prepareCost: 1, markerSlots: 2,
      yields: { fame: 3, coins: 3, shards: 0 } },
    { id: 'E2B', name: 'Zig Zag Lady', nameKo: '지그재그 레이디', category: 'ESCAPE', level: 2,
      fameThreshold: 16, components: { WOOD: 3, FABRIC: 1, OIL: 1 }, prepareCost: 1, markerSlots: 3,
      yields: { fame: 2, coins: 3, shards: 0 } },
    { id: 'E2C', name: 'Walled', nameKo: '벽 속에', category: 'ESCAPE', level: 2,
      fameThreshold: 16, components: { WOOD: 3, METAL: 3, LOCK: 1 }, prepareCost: 1, markerSlots: 2,
      yields: { fame: 3, coins: 2, shards: 1 } },
    { id: 'E2D', name: 'Wolf Cage', nameKo: '늑대 우리', category: 'ESCAPE', level: 2,
      fameThreshold: 16, components: { METAL: 2, OIL: 1, ANIMAL: 2 }, prepareCost: 1, markerSlots: 1,
      yields: { fame: 3, coins: 3, shards: 1 } },
    // Level 3
    { id: 'E3A', name: 'Buried Alive', nameKo: '생매장', category: 'ESCAPE', level: 3,
      fameThreshold: 36, components: { WOOD: 3, LOCK: 3 }, prepareCost: 1, markerSlots: 2,
      yields: { fame: 4, coins: 4, shards: 0 },
      endBonus: { type: 'HAS_ENGINEER', fame: 7, desc: '기술자 보유 시 7명성' } },
    { id: 'E3B', name: "Assistant's Revenge", nameKo: '어시스턴트의 복수', category: 'ESCAPE', level: 3,
      fameThreshold: 36, components: { GLASS: 3, SAW: 2, MIRROR: 2 }, prepareCost: 1, markerSlots: 1,
      yields: { fame: 6, coins: 6, shards: 0 },
      endBonus: { type: 'HAS_ASSISTANT', fame: 7, desc: '어시스턴트 보유 시 7명성' } },
    { id: 'E3C', name: 'Iron Maiden', nameKo: '아이언 메이든', category: 'ESCAPE', level: 3,
      fameThreshold: 36, components: { METAL: 3, SAW: 3, LOCK: 2 }, prepareCost: 2, markerSlots: 2,
      yields: { fame: 5, coins: 5, shards: 1 },
      endBonus: { type: 'PER_L1_TRICK', famePerUnit: 4, desc: '보유 레벨1 트릭 1개당 4명성' } },
    { id: 'E3D', name: 'Transported Man', nameKo: '순간이동', category: 'ESCAPE', level: 3,
      fameThreshold: 36, components: { FABRIC: 3, OIL: 2, DISGUISE: 2 }, prepareCost: 2, markerSlots: 2,
      yields: { fame: 5, coins: 3, shards: 2 },
      endBonus: { type: 'PER_L3_TRICK', famePerUnit: 4, desc: '보유 레벨3 트릭 1개당 4명성' } },
  ],
  SPIRITUAL: [
    // Level 1
    { id: 'S1A', name: 'Mind Reading', nameKo: '독심술', category: 'SPIRITUAL', level: 1,
      fameThreshold: 1, components: { GLASS: 2 }, prepareCost: 1, markerSlots: 3,
      yields: { fame: 0, coins: 0, shards: 1 } },
    { id: 'S1B', name: 'Breath of Life', nameKo: '생명의 숨결', category: 'SPIRITUAL', level: 1,
      fameThreshold: 1, components: { METAL: 1, FABRIC: 2, SAW: 1 }, prepareCost: 1, markerSlots: 1,
      yields: { fame: 2, coins: 3, shards: 0 } },
    { id: 'S1C', name: 'Spirit Hand', nameKo: '영혼의 손', category: 'SPIRITUAL', level: 1,
      fameThreshold: 1, components: { FABRIC: 3, ROPE: 1, ANIMAL: 1 }, prepareCost: 1, markerSlots: 2,
      yields: { fame: 2, coins: 1, shards: 1 } },
    { id: 'S1D', name: 'Window to the Otherworld', nameKo: '이세계의 창', category: 'SPIRITUAL', level: 1,
      fameThreshold: 1, components: { METAL: 2, OIL: 1, MIRROR: 1 }, prepareCost: 1, markerSlots: 2,
      yields: { fame: 3, coins: 2, shards: 0 } },
    // Level 2
    { id: 'S2A', name: 'Floating Table', nameKo: '떠다니는 테이블', category: 'SPIRITUAL', level: 2,
      fameThreshold: 16, components: { WOOD: 3, FABRIC: 3, ROPE: 3, OIL: 1 }, prepareCost: 2, markerSlots: 2,
      yields: { fame: 5, coins: 2, shards: 1 } },
    { id: 'S2B', name: 'Future Sight', nameKo: '미래 투시', category: 'SPIRITUAL', level: 2,
      fameThreshold: 16, components: { METAL: 2, FABRIC: 2, MIRROR: 1 }, prepareCost: 1, markerSlots: 1,
      yields: { fame: 4, coins: 1, shards: 0 } },
    { id: 'S2C', name: "Pepper's Ghost", nameKo: '페퍼스 고스트', category: 'SPIRITUAL', level: 2,
      fameThreshold: 16, components: { SAW: 2, MIRROR: 2, DISGUISE: 2 }, prepareCost: 2, markerSlots: 2,
      yields: { fame: 4, coins: 5, shards: 0 } },
    { id: 'S2D', name: 'Ghost Trap', nameKo: '유령 덫', category: 'SPIRITUAL', level: 2,
      fameThreshold: 16, components: { WOOD: 3, GLASS: 3, FABRIC: 3, ANIMAL: 3 }, prepareCost: 2, markerSlots: 3,
      yields: { fame: 3, coins: 3, shards: 1 } },
    // Level 3
    { id: 'S3A', name: "Balsamo's Skull", nameKo: '발사모의 해골', category: 'SPIRITUAL', level: 3,
      fameThreshold: 36, components: { METAL: 3, ROPE: 2, LOCK: 1 }, prepareCost: 2, markerSlots: 3,
      yields: { fame: 4, coins: 3, shards: 2 },
      endBonus: { type: 'PER_SUPERIOR_COMP', famePerUnit: 3, desc: '최상급 컴포넌트 종류당 3명성' } },
    { id: 'S3B', name: 'Seance', nameKo: '강령회', category: 'SPIRITUAL', level: 3,
      fameThreshold: 36, components: { WOOD: 3, OIL: 2, MIRROR: 2 }, prepareCost: 2, markerSlots: 2,
      yields: { fame: 7, coins: 5, shards: 1 },
      endBonus: { type: 'PER_APPRENTICE', famePerUnit: 3, desc: '견습생 1명당 3명성' } },
    { id: 'S3C', name: 'Skeleton Dance', nameKo: '해골 춤', category: 'SPIRITUAL', level: 3,
      fameThreshold: 36, components: { GLASS: 3, ROPE: 2, GEAR: 2 }, prepareCost: 3, markerSlots: 3,
      yields: { fame: 6, coins: 4, shards: 0 },
      endBonus: { type: 'PER_BASIC_COMP', famePerUnit: 1, desc: '기본 컴포넌트 종류당 1명성' } },
    { id: 'S3D', name: 'Metamorphosis', nameKo: '변신', category: 'SPIRITUAL', level: 3,
      fameThreshold: 36, components: { GLASS: 3, FABRIC: 3, ROPE: 3, DISGUISE: 3 }, prepareCost: 3, markerSlots: 3,
      yields: { fame: 10, coins: 4, shards: 0 },
      endBonus: { type: 'PER_3_COINS', famePerUnit: 1, desc: '남은 코인 3개당 1명성' } },
  ]
};

// Helper: get all tricks as flat array
export function getAllTricks() {
  return Object.values(TRICKS).flat();
}

// Helper: get tricks by category and level
export function getTricksByFilter(category, level) {
  let tricks = category ? TRICKS[category] : getAllTricks();
  if (level) tricks = tricks.filter(t => t.level === level);
  return tricks;
}

// Helper: find trick by ID
export function findTrickById(id) {
  return getAllTricks().find(t => t.id === id);
}
