'use client';

import { motion, AnimatePresence } from 'framer-motion';
import { GameIcon, RESOURCE_COLORS } from './GameIcon';

interface Props {
  type: 'fame' | 'coins' | 'shards' | 'ap';
  value: number;
  size?: 'sm' | 'md';
  showLabel?: boolean;
}

const LABELS: Record<string, string> = {
  fame: '명성',
  coins: '코인',
  shards: '샤드',
  ap: 'AP',
};

export function ResourceBadge({ type, value, size = 'sm', showLabel = false }: Props) {
  const color = RESOURCE_COLORS[type];
  const isMd = size === 'md';

  return (
    <div
      className={`inline-flex items-center gap-1 ${isMd ? 'px-2 py-1' : 'px-1.5 py-0.5'}`}
      style={{ color }}
    >
      <GameIcon type={type} size={isMd ? 'md' : 'xs'} />
      <AnimatePresence mode="popLayout">
        <motion.span
          key={value}
          initial={{ y: -8, opacity: 0 }}
          animate={{ y: 0, opacity: 1 }}
          exit={{ y: 8, opacity: 0 }}
          transition={{ duration: 0.2 }}
          className={`font-bold font-[var(--font-heading)] ${isMd ? 'text-base' : 'text-xs'}`}
        >
          {value}
        </motion.span>
      </AnimatePresence>
      {showLabel && (
        <span className={`opacity-70 ${isMd ? 'text-xs' : 'text-[10px]'}`}>
          {LABELS[type]}
        </span>
      )}
    </div>
  );
}
