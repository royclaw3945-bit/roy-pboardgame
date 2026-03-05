using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.NumericalResources.Events;

public class VictoryPointsChanged : BaseEvent
{
	public PlayerColor Player { get; }

	public int Amount { get; }

	public VictoryPointsChanged(PlayerColor player, int amount)
	{
		Player = player;
		Amount = amount;
	}
}
