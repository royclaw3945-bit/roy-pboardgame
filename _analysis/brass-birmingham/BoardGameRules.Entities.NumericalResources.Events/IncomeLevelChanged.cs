using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.NumericalResources.Events;

public class IncomeLevelChanged : BaseEvent
{
	public PlayerColor Player { get; }

	public int NumberOfLevels { get; }

	public IncomeLevelChanged(PlayerColor player, int numberOfLevels)
	{
		Player = player;
		NumberOfLevels = numberOfLevels;
	}
}
