using BoardGameRules.Entities.EventSourcing.Events;

namespace BoardGameRules.Entities.Actions.Events;

public class BeerSupplied : BaseEvent
{
	public string BeerSourceId { get; }

	public int Amount { get; }

	public BeerSupplied(string beerSourceId, int amount)
	{
		BeerSourceId = beerSourceId;
		Amount = amount;
	}
}
