using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.SellIndustry.Events;

public class IndustrySold : BaseEvent
{
	public PlayerColor PlayerColor { get; set; }

	public int MoneyToGet { get; set; }

	public string SlotUniqueId { get; set; }

	public IndustrySold(PlayerColor playerColor, int moneyToGet, string slotUniqueId)
	{
		PlayerColor = playerColor;
		MoneyToGet = moneyToGet;
		SlotUniqueId = slotUniqueId;
	}
}
