using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.Actions.Events;

public class IndustryDeveloped : BaseEvent
{
	public PlayerColor Player { get; }

	public BuildingType Type { get; }

	public IndustryDeveloped(PlayerColor player, BuildingType type)
	{
		Player = player;
		Type = type;
	}
}
