using BoardGameRules.Entities.EventSourcing.Events;

namespace BoardGameRules.Entities.Actions.Events;

public class IronSupplied : BaseEvent
{
	public string SourceId { get; }

	public int Amount { get; }

	public IronSupplied(string sourceId, int amount)
	{
		SourceId = sourceId;
		Amount = amount;
	}
}
