using System.Collections.Generic;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.Actions.Events;

public class WildCardsTaken : BaseEvent
{
	public PlayerColor Player { get; }

	public List<BaseCard> CardsToTrade { get; }

	public WildCardsTaken(PlayerColor player, List<BaseCard> cardsToTrade)
	{
		Player = player;
		CardsToTrade = cardsToTrade;
	}
}
