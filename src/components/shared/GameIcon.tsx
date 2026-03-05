'use client';

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

const RESOURCE_COLORS: Record<string, string> = {
  fame: 'var(--gold-primary)',
  coins: 'var(--gold-primary)',
  shards: 'var(--cyan-light)',
  ap: 'var(--purple-light)',
};

// Minimal SVG paths (lucide-compatible viewBox 0 0 24 24)
const ICON_PATHS: Record<string, string> = {
  // Resources
  fame: 'M12 2l3.09 6.26L22 9.27l-5 4.87 1.18 6.88L12 17.77l-6.18 3.25L7 14.14 2 9.27l6.91-1.01L12 2z',
  coins: 'M12 2a10 10 0 1 0 0 20 10 10 0 0 0 0-20zm0 4a1 1 0 0 1 1 1v1.5a2.5 2.5 0 0 1 0 5V15a1 1 0 0 1-2 0v-1.5a2.5 2.5 0 0 1 0-5V7a1 1 0 0 1 1-1z',
  shards: 'M6 3l6 3 6-3v6l-6 12-6-12V3z',
  ap: 'M13 2L3 14h9l-1 8 10-12h-9l1-8z',
  // Characters
  MAGICIAN: 'M15 4V2M15 16v-2M8 9h2M20 9h2M17.8 11.8L19 13M17.8 6.2L19 5M3 21l5-5M12.2 6.2L11 5M12.2 11.8L11 13M15 9a2 2 0 1 0 0-4 2 2 0 0 0 0 4z',
  APPRENTICE: 'M22 10v6M2 10l10-5 10 5-10 5z M6 12v5c3 3 9 3 12 0v-5',
  ENGINEER: 'M14.7 6.3a1 1 0 0 0 0 1.4l1.6 1.6a1 1 0 0 0 1.4 0l3.77-3.77a6 6 0 0 1-7.94 7.94l-6.91 6.91a2.12 2.12 0 0 1-3-3l6.91-6.91a6 6 0 0 1 7.94-7.94l-3.76 3.76z',
  MANAGER: 'M20 7h-4V3H8v4H4l-1 13h18L20 7zM8 3h8',
  ASSISTANT: 'M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2 M9 11a4 4 0 1 0 0-8 4 4 0 0 0 0 8zM22 21v-2a4 4 0 0 0-3-3.87M16 3.13a4 4 0 0 1 0 7.75',
  // Components
  WOOD: 'M17 10.5V7a1 1 0 0 0-1-1h-2a1 1 0 0 0-1 1v3 M12 13.5V17a1 1 0 0 0 1 1h2a1 1 0 0 0 1-1v-3.5M10 3v4a2 2 0 0 1-2 2H4M20 3v4a2 2 0 0 0 2 2h0',
  METAL: 'M15 12L9 18l-4-4 6-6M19.5 8.5l-7-7-2 2 7 7zM14 4l4 4',
  GLASS: 'M15.2 22H8.8a2 2 0 0 1-2-1.79L5 3h14l-1.81 17.21A2 2 0 0 1 15.2 22zM6 12h12',
  FABRIC: 'M20.38 3.46L16 2 12 5.5 8 2l-4.38 1.46a.5.5 0 0 0-.27.54l1.17 7.5 2.2 9.5a1 1 0 0 0 .98.8h8.6a1 1 0 0 0 .98-.8l2.2-9.5 1.17-7.5a.5.5 0 0 0-.27-.54z',
  ROPE: 'M4 9a2 2 0 0 1-2-2V5h6v2a2 2 0 0 1-2 2zm0 0v6a2 2 0 0 0 2 2h2a2 2 0 0 0 2-2v-6a2 2 0 0 1 2-2h2a2 2 0 0 1 2 2v6a2 2 0 0 0 2 2h2a2 2 0 0 0 2-2V9m0 0a2 2 0 0 0 2-2V5h-6v2a2 2 0 0 0 2 2z',
  OIL: 'M12 2.69l5.66 5.66a8 8 0 1 1-11.31 0z',
  SAW: 'M14 4l6 6M4 20l6-6m4-4l-6 6M14 10l2-2M8 16l2-2',
  ANIMAL: 'M10 5.172C10 3.782 8.423 2.679 6.5 3c-2.823.47-4.113 6.006-4 7 .08.703 1.725 1.722 3.656 1 1.261-.472 1.96-1.45 2.344-2.5M14.267 5.172c0-1.39 1.577-2.493 3.5-2.172 2.823.47 4.113 6.006 4 7-.08.703-1.725 1.722-3.656 1-1.261-.472-1.855-1.45-2.239-2.5M8 14v.5M16 14v.5M11.25 16.25h1.5L12 17z',
  LOCK: 'M19 11H5a2 2 0 0 0-2 2v7a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7a2 2 0 0 0-2-2zM7 11V7a5 5 0 0 1 10 0v4',
  MIRROR: 'M6 2h12v7a6 6 0 0 1-12 0V2zM4 22h16M10 15v7M14 15v7',
  DISGUISE: 'M2 12a5 5 0 0 0 5 5 8 8 0 0 1 5 2 8 8 0 0 1 5-2 5 5 0 0 0 5-5V7H2z M6 11a3 3 0 0 1 3-3 3 3 0 0 1 3 3M12 11a3 3 0 0 1 3-3 3 3 0 0 1 3 3',
  GEAR: 'M12 20a8 8 0 1 0 0-16 8 8 0 0 0 0 16zM12 14a2 2 0 1 0 0-4 2 2 0 0 0 0 4z M12 2v2M12 20v2M4.93 4.93l1.41 1.41M17.66 17.66l1.41 1.41M2 12h2M20 12h2M6.34 17.66l-1.41 1.41M19.07 4.93l-1.41 1.41',
  // Locations
  THEATER: 'M2 10s3-3 10-3 10 3 10 3M4 10v10h16V10M12 20v-6M8 14v6M16 14v6M12 7v3',
  MARKET_ROW: 'M3 9l1.5-6h15L21 9M3 9v11a1 1 0 0 0 1 1h16a1 1 0 0 0 1-1V9M9 21V9M3 9h18',
  WORKSHOP: 'M2 20a2 2 0 0 0 2 2h16a2 2 0 0 0 2-2V8l-7 5V8l-7 5V4a2 2 0 0 0-2-2H4a2 2 0 0 0-2 2z',
  DOWNTOWN: 'M6 22V4a2 2 0 0 1 2-2h8a2 2 0 0 1 2 2v18zM6 12H4a2 2 0 0 0-2 2v6a2 2 0 0 0 2 2h2M18 9h2a2 2 0 0 1 2 2v9a2 2 0 0 1-2 2h-2M10 6h4M10 10h4M10 14h4M10 18h4',
  DARK_ALLEY: 'M12 3a6 6 0 0 0 9 9 9 9 0 1 1-9-9z',
};

export function GameIcon({ type, size = 'sm', className = '', color }: Props) {
  const px = SIZE_MAP[size];
  const pathData = ICON_PATHS[type];
  if (!pathData) return null;

  const finalColor = color ?? RESOURCE_COLORS[type] ?? 'currentColor';

  return (
    <svg
      width={px}
      height={px}
      viewBox="0 0 24 24"
      fill="none"
      stroke={finalColor}
      strokeWidth="2"
      strokeLinecap="round"
      strokeLinejoin="round"
      className={className}
      style={{ flexShrink: 0 }}
    >
      <path d={pathData} />
    </svg>
  );
}

export { RESOURCE_COLORS, SIZE_MAP };
