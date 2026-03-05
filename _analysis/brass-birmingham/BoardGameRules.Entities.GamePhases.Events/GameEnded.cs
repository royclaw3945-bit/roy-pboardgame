using System.Collections.Generic;
using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.GamePhases.Events;

public class GameEnded : BaseEvent
{
	public Dictionary<PlayerColor, int> LastEraIndustriesVPs { get; }

	public Dictionary<PlayerColor, int> LastEraLinksVPs { get; }

	public Dictionary<PlayerColor, int> LastEraBonusesVPs { get; }

	public GameEnded(Dictionary<PlayerColor, int> lastEraIndustriesVPs, Dictionary<PlayerColor, int> lastEraLinksVPs, Dictionary<PlayerColor, int> lastEraBonusesVPs)
	{
		LastEraIndustriesVPs = lastEraIndustriesVPs;
		LastEraLinksVPs = lastEraLinksVPs;
		LastEraBonusesVPs = lastEraBonusesVPs;
	}
}
