'use client';

import {
  Star, Coins, Diamond, Zap,
  Wand2, GraduationCap, Wrench, Briefcase, Users,
  TreePine, Hammer, GlassWater, Shirt,
  Cable, Droplets, Scissors, Dog,
  Lock, Glasses, Drama, Cog,
  Theater, Store, Factory, Building2, Moon,
} from 'lucide-react';
import type { ComponentType as CT } from '@/core/types';

type IconSize = 'xs' | 'sm' | 'md' | 'lg';

const SIZE_MAP: Record<IconSize, number> = {
  xs: 12, sm: 16, md: 20, lg: 28,
};

interface Props {
  type: string;
  size?: IconSize;
  className?: string;
  color?: string;
}

const RESOURCE_ICONS: Record<string, typeof Star> = {
  fame: Star,
  coins: Coins,
  shards: Diamond,
  ap: Zap,
};

const CHARACTER_ICONS: Record<string, typeof Wand2> = {
  MAGICIAN: Wand2,
  APPRENTICE: GraduationCap,
  ENGINEER: Wrench,
  MANAGER: Briefcase,
  ASSISTANT: Users,
};

const COMPONENT_ICONS: Record<CT, typeof TreePine> = {
  WOOD: TreePine,
  METAL: Hammer,
  GLASS: GlassWater,
  FABRIC: Shirt,
  ROPE: Cable,
  OIL: Droplets,
  SAW: Scissors,
  ANIMAL: Dog,
  LOCK: Lock,
  MIRROR: Glasses,
  DISGUISE: Drama,
  GEAR: Cog,
};

const LOCATION_ICONS: Record<string, typeof Theater> = {
  THEATER: Theater,
  MARKET_ROW: Store,
  WORKSHOP: Factory,
  DOWNTOWN: Building2,
  DARK_ALLEY: Moon,
};

const RESOURCE_COLORS: Record<string, string> = {
  fame: 'var(--gold-primary)',
  coins: 'var(--gold-primary)',
  shards: 'var(--cyan-light)',
  ap: 'var(--purple-light)',
};

export function GameIcon({ type, size = 'sm', className = '', color }: Props) {
  const px = SIZE_MAP[size];
  const Icon =
    RESOURCE_ICONS[type] ??
    CHARACTER_ICONS[type] ??
    COMPONENT_ICONS[type as CT] ??
    LOCATION_ICONS[type];

  if (!Icon) return null;

  const finalColor = color ?? RESOURCE_COLORS[type] ?? 'currentColor';

  return <Icon size={px} color={finalColor} className={className} />;
}

export { RESOURCE_COLORS, SIZE_MAP };
