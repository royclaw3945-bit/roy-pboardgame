using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.GamePhases.Events;

public class LinksRemoved : BaseEvent
{
	public PlayerColor PlayerColor { get; }

	public LinksRemoved(PlayerColor playerColor)
	{
		PlayerColor = playerColor;
	}
}
