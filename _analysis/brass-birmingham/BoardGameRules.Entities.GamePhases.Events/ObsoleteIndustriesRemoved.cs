using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.GamePhases.Events;

public class ObsoleteIndustriesRemoved : BaseEvent
{
	public PlayerColor PlayerColor { get; }

	public ObsoleteIndustriesRemoved(PlayerColor playerColor)
	{
		PlayerColor = playerColor;
	}
}
