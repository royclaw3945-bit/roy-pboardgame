// Trickerion: Legends of Illusion - Game Data (Official Rulebook)
import { TRICKS, getAllTricks, getTricksByFilter, findTrickById } from './data/tricks.js';
export { TRICKS, getAllTricks, getTricksByFilter, findTrickById };

export const TRICK_CATEGORIES = {
  MECHANICAL: { name: '기계', icon: '⚙️', color: '#e67e22', suit: 'HEART' },
  OPTICAL: { name: '광학', icon: '🔮', color: '#9b59b6', suit: 'DIAMOND' },
  ESCAPE: { name: '탈출', icon: '🔗', color: '#27ae60', suit: 'SPADE' },
  SPIRITUAL: { name: '영혼', icon: '👻', color: '#3498db', suit: 'CLUB' }
};

// 12 component types: 4 basic(1c) + 4 advanced(2c) + 4 superior(3c)
export const COMPONENT_TYPES = {
  WOOD: { name: '나무', tier: 'basic', cost: 1, icon: '🪵' },
  METAL: { name: '금속', tier: 'basic', cost: 1, icon: '⛓️' },
  GLASS: { name: '유리', tier: 'basic', cost: 1, icon: '🪟' },
  FABRIC: { name: '천', tier: 'basic', cost: 1, icon: '🧵' },
  ROPE: { name: '밧줄', tier: 'advanced', cost: 2, icon: '🪢' },
  OIL: { name: '석유', tier: 'advanced', cost: 2, icon: '🛢️' },
  SAW: { name: '톱', tier: 'advanced', cost: 2, icon: '🪚' },
  ANIMAL: { name: '동물', tier: 'advanced', cost: 2, icon: '🐇' },
  LOCK: { name: '자물쇠', tier: 'superior', cost: 3, icon: '🔒' },
  MIRROR: { name: '거울', tier: 'superior', cost: 3, icon: '🪞' },
  DISGUISE: { name: '변장', tier: 'superior', cost: 3, icon: '🎭' },
  GEAR: { name: '톱니', tier: 'superior', cost: 3, icon: '⚙️' }
};

// Component pool sizes (total available in game)
export const COMPONENT_POOL = {
  basic: 10,    // 10 each of WOOD, METAL, GLASS, FABRIC
  advanced: 8,  // 8 each of ROPE, OIL, SAW, ANIMAL
  superior: 6   // 6 each of LOCK, MIRROR, DISGUISE, GEAR
};

export const CHARACTER_TYPES = {
  MAGICIAN: { name: '마법사', baseAP: 3, wage: 0, icon: '🎩' },
  ENGINEER: { name: '기술자', baseAP: 2, wage: 2, icon: '🔧', specialistType: true },
  MANAGER: { name: '매니저', baseAP: 2, wage: 2, icon: '💼', specialistType: true },
  ASSISTANT: { name: '어시스턴트', baseAP: 2, wage: 2, icon: '🤝', specialistType: true },
  APPRENTICE: { name: '견습생', baseAP: 1, wage: 1, icon: '📚' }
};

export const LOCATIONS = {
  DOWNTOWN: {
    name: '다운타운', icon: '🏛️', img: 'src/img/loc_downtown.jpg',
    sublocs: ['DAHLGAARD', 'INN', 'BANK'],
    actions: {
      LEARN_TRICK: { name: '트릭 배우기', cost: 3, subloc: 'DAHLGAARD' },
      TAKE_COINS: { name: '코인 받기', cost: 3, subloc: 'BANK' },
      HIRE_CHARACTER: { name: '캐릭터 고용', cost: 3, subloc: 'INN' },
      REROLL_DIE: { name: '주사위 재굴림', cost: 1, subloc: 'ANY' },
      CHOOSE_DIE: { name: '주사위 결과 선택', cost: 2, subloc: 'ANY' }
    }
  },
  MARKET_ROW: {
    name: '시장', icon: '🏪', img: 'src/img/loc_market.jpg',
    actions: {
      BUY: { name: '구매', cost: 1 },
      ORDER: { name: '주문', cost: 1 },
      QUICK_ORDER: { name: '긴급 주문', cost: 2 },
      BARGAIN: { name: '흥정', cost: 1 }
    }
  },
  WORKSHOP: {
    name: '작업장', icon: '🔨', personal: true, img: 'src/img/loc_workshop.jpg',
    actions: {
      PREPARE: { name: '트릭 준비', cost: 'variable' },
      MOVE_TRICK: { name: '트릭 이동', cost: 1, requires: 'ENGINEER' },
      MOVE_COMPONENT: { name: '컴포넌트 이동', cost: 1, requires: 'MANAGER' },
      MOVE_APPRENTICE: { name: '견습생 이동', cost: 1, requires: 'ASSISTANT' }
    }
  },
  THEATER: {
    name: '극장', icon: '🎭', img: 'src/img/loc_theater.jpg',
    actions: {
      SETUP_TRICK: { name: '트릭 셋업', cost: 1 },
      RESCHEDULE: { name: '일정 변경', cost: 1 },
      PERFORM: { name: '공연', cost: 0, magicianOnly: true }
    }
  },
  DARK_ALLEY: {
    name: '어둠의 골목', icon: '🌙', expansion: true, img: 'src/img/loc_darkalley.jpg',
    actions: {
      DRAW_SPECIAL: { name: '특수 카드 획득', cost: 1 },
      DRAW_MORE: { name: '추가 뽑기', cost: 2 },
      FORTUNE_TELLING: { name: '점술', cost: 1 }
    }
  }
};

// Slot AP modifiers per location (pyramid structure: 1-2-3 rows)
// 4-player: all slots open. 3-player: row 3 locked. 2-player: row 2&3 locked.
export const LOCATION_SLOTS = {
  DOWNTOWN: [
    { row: 1, apMod: +1 },
    { row: 2, apMod: 0 }, { row: 2, apMod: 0 },
    { row: 3, apMod: -1 }, { row: 3, apMod: -1 }, { row: 3, apMod: -1 }
  ],
  MARKET_ROW: [
    { row: 1, apMod: +1 },
    { row: 2, apMod: 0 }, { row: 2, apMod: 0 },
    { row: 3, apMod: -1 }, { row: 3, apMod: -1 }, { row: 3, apMod: -1 }
  ],
  THEATER: [
    { row: 1, apMod: +1 },
    { row: 2, apMod: 0 }, { row: 2, apMod: 0 },
    { row: 3, apMod: -1 }, { row: 3, apMod: -1 }, { row: 3, apMod: -1 }
  ],
  DARK_ALLEY: [
    { row: 1, apMod: +1 },
    { row: 2, apMod: 0 }, { row: 2, apMod: 0 },
    { row: 3, apMod: -1 }, { row: 3, apMod: -1 }, { row: 3, apMod: -1 }
  ]
};

export const MAGICIANS = [
  { id: 'MECHANIKER', name: 'The Mechaniker', nameKo: '더 메카니커',
    fullName: 'Gerhard von Liebenstein', favoriteCategory: 'MECHANICAL', color: '#e67e22', img: 'src/img/mechaniker_portrait.jpg' },
  { id: 'OPTICIAN', name: 'The Great Optico', nameKo: '더 그레이트 옵티코',
    fullName: 'Leo Sanderberg', favoriteCategory: 'OPTICAL', color: '#9b59b6', img: 'src/img/optico_portrait.jpg' },
  { id: 'ESCAPIST', name: 'Master of Chains', nameKo: '사슬의 대가',
    fullName: 'Frederic White', favoriteCategory: 'ESCAPE', color: '#27ae60', img: 'src/img/chains_portrait.jpg' },
  { id: 'SPIRITUALIST', name: 'Priestess of Mysticism', nameKo: '신비의 여사제',
    fullName: 'Amabel Fae', favoriteCategory: 'SPIRITUAL', color: '#3498db', img: 'src/img/priestess_portrait.jpg' }
];

export const PERFORMANCE_CARDS = {
  RIVERSIDE: {
    type: 'RIVERSIDE', name: 'Riverside Theater', nameKo: '리버사이드 극장',
    slots: 4, performerBonus: { fame: 1, coins: 1, shards: 0 },
    linkCircles: [
      { hasShardSymbol: false }, { hasShardSymbol: true },
      { hasShardSymbol: true }, { hasShardSymbol: false }
    ]
  },
  GRAND_MAGORIAN: {
    type: 'GRAND_MAGORIAN', name: 'Grand Magorian', nameKo: '그랜드 마고리안',
    slots: 4, performerBonus: { fame: 2, coins: 2, shards: 0 },
    linkCircles: [
      { hasShardSymbol: true }, { hasShardSymbol: false },
      { hasShardSymbol: false }, { hasShardSymbol: true }
    ]
  },
  MAGNUS_PANTHEON: {
    type: 'MAGNUS_PANTHEON', name: 'Magnus Pantheon', nameKo: '마그누스 판테온',
    slots: 4, performerBonus: { fame: 3, coins: 3, shards: 1 },
    linkCircles: [
      { hasShardSymbol: true }, { hasShardSymbol: true },
      { hasShardSymbol: true }, { hasShardSymbol: true }
    ]
  }
};

export const WEEKDAY_MODIFIERS = {
  THURSDAY: { name: '목요일', fameMod: -1, coinMod: -1 },
  FRIDAY: { name: '금요일', fameMod: 0, coinMod: 0 },
  SATURDAY: { name: '토요일', fameMod: 0, coinMod: 0 },
  SUNDAY: { name: '일요일', fameMod: 1, coinMod: 1 }
};
export const WEEKDAYS = ['THURSDAY', 'FRIDAY', 'SATURDAY', 'SUNDAY'];

export const DOWNTOWN_DICE = {
  DAHLGAARD: ['MECHANICAL', 'OPTICAL', 'ESCAPE', 'SPIRITUAL', 'ANY', 'X'],
  INN: ['ENGINEER', 'MANAGER', 'ASSISTANT', 'APPRENTICE', 'APPRENTICE', 'X'],
  BANK: [1, 2, 3, 4, 5, 'X']
};

export const ADVERTISE_COST = [1, 2, 3, 4];
export const ADVERTISE_FAME = 2;

// --- CORRECTED STARTING VALUES (Official Rulebook) ---
export const STARTING_SETUP = {
  fame: 10,                        // was 5
  coins: 10,                       // base coins (position bonus added separately)
  shards: 1,
  extraCoinsByPosition: [0, 2, 4, 6],  // 1st:10, 2nd:12, 3rd:14, 4th:16
  maxTricks: 4,                    // engineer bonus: +1
  maxComponentsPerType: 3,         // max 3 of each component type
  maxTrickMarkers: 6,              // unused marker pool (was 4)
  assignmentCards: 10              // theater x3, workshop x2, market x2, downtown x2, darkAlley x1
};

export const ASSIGNMENT_CARD_TYPES = {
  THEATER: { name: '극장', count: 3 },
  WORKSHOP: { name: '작업장', count: 2 },
  MARKET_ROW: { name: '시장', count: 2 },
  DOWNTOWN: { name: '다운타운', count: 2 },
  DARK_ALLEY: { name: '어둠의 골목', count: 1 }
};

export const WAGES = {
  APPRENTICE: 1,
  SPECIALIST: 2,  // ENGINEER, MANAGER, ASSISTANT
  MAGICIAN: 0,
  UNPAID_PENALTY: 2  // 미지불 1코인당 2명성 차감
};

export const GAME_CONFIG = {
  BASE_ROUNDS: 5,
  DA_ROUNDS: 7,
  HIRE_LIMITS: { ENGINEER: 1, MANAGER: 1, ASSISTANT: 1, APPRENTICE: Infinity }
};

export const SPECIALIST_THEATER_BONUS = {
  ENGINEER: { fame: 0, coins: 0, shards: 1 },
  MANAGER: { fame: 0, coins: 2, shards: 0 },
  ASSISTANT: { fame: 1, coins: 0, shards: 0 }
};

// End game scoring (base game)
export const END_GAME_SCORING = {
  BASE: {
    SHARD_TO_FAME: 1,       // 1 shard = 1 fame
    COINS_PER_FAME: 3,      // 3 coins = 1 fame
    APPRENTICE_FAME: 2,     // 1 apprentice = 2 fame
    SPECIALIST_FAME: 3      // 1 specialist = 3 fame
  },
  DARK_ALLEY: {
    SHARD_RATIO: { per: 3, fame: 2, max: 20 },
    COIN_RATIO: { per: 3, fame: 1, max: 20 },
    SPECIAL_CARD_FAME: 2,   // per unused card, max 20
    L3_BONUS_MAX: 20
  }
};
