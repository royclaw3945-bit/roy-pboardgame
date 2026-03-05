using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.Actions.Events;

public class IndustryBuilt : BaseEvent
{
	public PlayerColor Player { get; }

	public BuildingType Type { get; }

	public string SlotUniqueId { get; }

	public IndustryBuilt(PlayerColor player, BuildingType type, string slotUniqueId)
	{
		Player = player;
		Type = type;
		SlotUniqueId = slotUniqueId;
	}
}
