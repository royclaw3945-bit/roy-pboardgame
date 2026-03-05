using System.Collections.Generic;
using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.GamePhases.Events;

public class RoundEnded : BaseEvent
{
	public GameEra GameEra { get; }

	public List<PlayerColor> PlayersOrder { get; }

	public RoundEnded(GameEra gameEra, List<PlayerColor> playersOrder)
	{
		GameEra = gameEra;
		PlayersOrder = playersOrder;
	}
}
