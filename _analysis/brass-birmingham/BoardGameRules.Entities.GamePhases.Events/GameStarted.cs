using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Entities.Game;

namespace BoardGameRules.Entities.GamePhases.Events;

public class GameStarted : BaseEvent
{
	public GameStartConfiguration GameStartConfiguration { get; }

	public GameStarted(GameStartConfiguration gameStartConfiguration)
	{
		GameStartConfiguration = gameStartConfiguration;
	}
}
