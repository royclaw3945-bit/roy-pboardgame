using BoardGameRules.Entities.EventSourcing.Events;

namespace BoardGameRules.Entities.Actions.Events;

public class CoalSupplied : BaseEvent
{
	public string SourceUniqueId { get; }

	public int Amount { get; }

	public CoalSupplied(string sourceUniqueId, int amount)
	{
		SourceUniqueId = sourceUniqueId;
		Amount = amount;
	}
}
