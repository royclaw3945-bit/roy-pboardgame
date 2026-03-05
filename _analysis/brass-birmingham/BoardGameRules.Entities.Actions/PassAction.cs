using BoardGameRules.Entities.Actions.Events;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.EventSourcing.Aggregates;
using BoardGameRules.Entities.NumericalResources;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.Actions;

public class PassAction : BaseAction
{
	public PassAction(IAggregateRoot aggregateRoot, PlayersTrack playersTrack, BoardGameRules.Entities.Board.Board board, GeneralAction generalAction)
		: base(aggregateRoot, playersTrack, board, generalAction)
	{
		aggregateRoot.RegisterApplyEvent<ActionPassed>(ApplyEvent);
	}

	public void Pass(BaseCard card, Player player)
	{
		DiscardCard(card, player);
		base.AggregateRoot.ApplyEvent(new ActionPassed(card, player.Color));
	}

	private void ApplyEvent(ActionPassed industryBuilt)
	{
	}
}
