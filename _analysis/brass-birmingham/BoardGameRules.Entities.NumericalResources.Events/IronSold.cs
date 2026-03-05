using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.NumericalResources.Events;

public class IronSold : BaseEvent
{
	public PlayerColor PlayerColor { get; }

	public string SlotUniqueId { get; }

	public int Amount { get; }

	public int MoneyReceived { get; }

	public IronSold(string slotUniqueId, int amount, int moneyReceived, PlayerColor playerColor)
	{
		SlotUniqueId = slotUniqueId;
		Amount = amount;
		MoneyReceived = moneyReceived;
		PlayerColor = playerColor;
	}
}
