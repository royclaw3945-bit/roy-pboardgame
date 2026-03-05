using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.NumericalResources.Events;

public class FlippedIndustriesVictoryPointsChanged : BaseEvent
{
	public PlayerColor Player { get; }

	public int Amount { get; }

	public FlippedIndustriesVictoryPointsChanged(PlayerColor player, int amount)
	{
		Player = player;
		Amount = amount;
	}
}
