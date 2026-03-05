using System.Collections.Generic;
using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.GamePhases.Events;

public class CanalEraEnded : BaseEvent
{
	public Dictionary<PlayerColor, int> EndOfEraLinksVPs { get; }

	public Dictionary<PlayerColor, int> EndOfEraIndustriesVPs { get; }

	public Dictionary<PlayerColor, int> CurrentEraBonusesVPs { get; }

	public CanalEraEnded(Dictionary<PlayerColor, int> endOfEraLinksVPs, Dictionary<PlayerColor, int> endOfEraIndustriesVPs, Dictionary<PlayerColor, int> currentEraBonusesVPs)
	{
		EndOfEraLinksVPs = endOfEraLinksVPs;
		EndOfEraIndustriesVPs = endOfEraIndustriesVPs;
		CurrentEraBonusesVPs = currentEraBonusesVPs;
	}
}
