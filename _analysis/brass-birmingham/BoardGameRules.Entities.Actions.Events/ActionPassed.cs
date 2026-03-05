using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.Actions.Events;

public class ActionPassed : BaseEvent
{
	public PlayerColor Player { get; }

	public BaseCard CardToDiscard { get; }

	public ActionPassed(BaseCard cardToDiscard, PlayerColor player)
	{
		CardToDiscard = cardToDiscard;
		Player = player;
	}
}
