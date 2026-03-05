using BoardGameRules.Entities.EventSourcing.Events;

namespace BoardGameRules.Entities.GamePhases.Events;

public class CanalEraEndStarted : BaseEvent
{
	public bool IsIntroductoryGame { get; set; }

	public CanalEraEndStarted(bool isIntroductoryGame)
	{
		IsIntroductoryGame = isIntroductoryGame;
	}
}
