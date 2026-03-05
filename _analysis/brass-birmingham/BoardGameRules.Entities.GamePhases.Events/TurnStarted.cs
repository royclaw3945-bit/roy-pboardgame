using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.GamePhases.Events;

public class TurnStarted : BaseEvent
{
	public PlayerColor PlayerColor { get; }

	public TurnStarted(PlayerColor playerColor)
	{
		PlayerColor = playerColor;
	}
}
