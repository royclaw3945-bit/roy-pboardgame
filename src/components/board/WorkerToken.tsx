'use client';

import { CHARACTER_META } from '@/core/data/constants';
import { GameIcon } from '../shared/GameIcon';
import type { CharacterType } from '@/core/types';

interface OccupiedProps {
  charType: CharacterType;
  playerColor: string;
  playerName: string;
}

interface EmptyProps {
  apMod: number;
}

type Props = { occupied: OccupiedProps } | { empty: EmptyProps };

export function WorkerToken(props: Props) {
  if ('occupied' in props) {
    const { charType, playerColor, playerName } = props.occupied;
    return (
      <div
        className="worker-token occupied"
        style={{
          '--token-color': playerColor,
          borderColor: playerColor,
          boxShadow: `inset 0 -2px 4px rgba(0,0,0,0.4), 0 0 6px ${playerColor}50`,
        } as React.CSSProperties}
        title={`${playerName} - ${CHARACTER_META[charType].name}`}
      >
        <GameIcon type={charType} size="xs" color={playerColor} />
        <span className="worker-token-name" style={{ color: playerColor }}>
          {playerName.slice(0, 2)}
        </span>
      </div>
    );
  }

  const { apMod } = props.empty;
  return (
    <div
      className="worker-token empty"
      title={`AP 보정: ${apMod >= 0 ? '+' : ''}${apMod}`}
    >
      <span className={`worker-token-ap ${apMod > 0 ? 'positive' : ''}`}>
        {apMod > 0 ? `+${apMod}` : apMod}
      </span>
    </div>
  );
}
