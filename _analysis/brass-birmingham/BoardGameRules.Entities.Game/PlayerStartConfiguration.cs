using System;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.Game;

public class PlayerStartConfiguration : ICloneable
{
	public PlayerColor Color { get; }

	public int SittingOrder { get; }

	public PlayerStartConfiguration(PlayerColor color, int sittingOrder)
	{
		Color = color;
		SittingOrder = sittingOrder;
	}

	public override bool Equals(object obj)
	{
		if (obj is PlayerStartConfiguration playerStartConfiguration)
		{
			return Equals(playerStartConfiguration);
		}
		return false;
	}

	public bool Equals(PlayerStartConfiguration playerStartConfiguration)
	{
		if (playerStartConfiguration == null)
		{
			return false;
		}
		if (Color == playerStartConfiguration.Color)
		{
			return SittingOrder == playerStartConfiguration.SittingOrder;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return Color.GetHashCode() ^ SittingOrder.GetHashCode();
	}

	public object Clone()
	{
		return new PlayerStartConfiguration(Color, SittingOrder);
	}
}
