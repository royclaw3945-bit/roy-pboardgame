using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.NumericalResources.Events;

public class IntroductoryGameAdditionalIncomeVictoryPointsChanged : BaseEvent
{
	public PlayerColor Player { get; }

	public int Amount { get; }

	public IntroductoryGameAdditionalIncomeVictoryPointsChanged(PlayerColor player, int amount)
	{
		Player = player;
		Amount = amount;
	}
}
