'use client';

import { type ReactNode } from 'react';

type CardVariant = 'default' | 'location' | 'performance' | 'trick' | 'player';

interface Props {
  children: ReactNode;
  variant?: CardVariant;
  category?: string;
  selected?: boolean;
  disabled?: boolean;
  onClick?: () => void;
  className?: string;
  bgImage?: string;
}

const CATEGORY_BORDERS: Record<string, string> = {
  MECHANICAL: 'var(--mechanical)',
  OPTICAL: 'var(--optical)',
  ESCAPE: 'var(--escape)',
  SPIRITUAL: 'var(--spiritual)',
};

export function CardFrame({
  children, variant = 'default', category, selected, disabled,
  onClick, className = '', bgImage,
}: Props) {
  const borderColor = category ? CATEGORY_BORDERS[category] : undefined;
  const isClickable = !!onClick && !disabled;

  const variantClass =
    variant === 'location' ? 'location-card' :
    variant === 'performance' ? 'perf-card' :
    '';

  const stateClass = [
    selected && 'selected',
    disabled && 'disabled',
    bgImage && 'has-bg',
  ].filter(Boolean).join(' ');

  return (
    <div
      className={`${variantClass} ${stateClass} ${className}`}
      onClick={isClickable ? onClick : undefined}
      style={{
        ...(bgImage && {
          backgroundImage: `url(${bgImage})`,
          backgroundSize: 'cover',
          backgroundPosition: 'center',
        }),
        ...(borderColor && { borderLeftColor: borderColor }),
        ...(isClickable && { cursor: 'pointer' }),
        ...(!variantClass && {
          background: 'var(--glass-bg)',
          backdropFilter: 'var(--glass-blur)',
          border: '1px solid var(--border)',
          borderRadius: 'var(--radius-lg)',
          padding: '12px',
          transition: 'all 0.25s ease',
          position: 'relative' as const,
          overflow: 'hidden' as const,
        }),
      }}
    >
      {children}
    </div>
  );
}
