using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.Actions.Events;

public class LinkPlaced : BaseEvent
{
	public PlayerColor Player { get; }

	public string LinkId { get; }

	public LinkPlaced(PlayerColor player, string linkId)
	{
		Player = player;
		LinkId = linkId;
	}
}
