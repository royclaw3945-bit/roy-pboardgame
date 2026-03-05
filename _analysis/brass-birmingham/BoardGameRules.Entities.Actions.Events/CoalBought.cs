using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.Actions.Events;

public class CoalBought : BaseEvent
{
	public int Amount { get; }

	public int Price { get; }

	public PlayerColor PlayerColor { get; }

	public CoalBought(int amount, int price, PlayerColor playerColor)
	{
		Amount = amount;
		Price = price;
		PlayerColor = playerColor;
	}
}
