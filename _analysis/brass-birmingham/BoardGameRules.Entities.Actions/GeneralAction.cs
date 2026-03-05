using System;
using BoardGameRules.Entities.Actions.Events;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.Consumables;
using BoardGameRules.Entities.EventSourcing.Aggregates;
using BoardGameRules.Entities.NumericalResources;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.Actions;

public class GeneralAction
{
	private readonly Action DecreaseRemainingActionsCountAction;

	private IAggregateRoot AggregateRoot { get; }

	private PlayersTrack PlayersTrack { get; }

	private BoardGameRules.Entities.Board.Board Board { get; }

	public ConsumingCoal ConsumingCoal { get; }

	public ConsumingIron ConsumingIron { get; }

	public ConsumingBeer ConsumingBeer { get; }

	public GeneralAction(IAggregateRoot aggregateRoot, PlayersTrack playersTrack, BoardGameRules.Entities.Board.Board board, Action decreaseRemainingActionsCountAction, ConsumingCoal consumingCoal, ConsumingIron consumingIron, ConsumingBeer consumingBeer)
	{
		AggregateRoot = aggregateRoot;
		PlayersTrack = playersTrack;
		Board = board;
		DecreaseRemainingActionsCountAction = decreaseRemainingActionsCountAction;
		ConsumingCoal = consumingCoal;
		ConsumingIron = consumingIron;
		ConsumingBeer = consumingBeer;
		AggregateRoot.RegisterApplyEvent<CardDiscarded>(ApplyEvent);
		AggregateRoot.RegisterApplyEvent<ActionCountDecreased>(ApplyEvent);
		AggregateRoot.RegisterApplyEvent<IndustryFlipped>(ApplyEvent);
	}

	public void DiscardCard(BaseCard card, PlayerColor playerColor)
	{
		AggregateRoot.ApplyEvent(new CardDiscarded(card, playerColor));
	}

	public void DecreaseRemainingActionsCount()
	{
		AggregateRoot.ApplyEvent(new ActionCountDecreased());
	}

	private void ApplyEvent(CardDiscarded cardDiscarded)
	{
		Player playerByColor = PlayersTrack.GetPlayerByColor(cardDiscarded.Player);
		if (cardDiscarded.CardToDiscard.GetType() == typeof(LocationCard) || cardDiscarded.CardToDiscard.GetType() == typeof(IndustryCard))
		{
			playerByColor.DiscardPile.Add(cardDiscarded.CardToDiscard);
		}
		else if (cardDiscarded.CardToDiscard.GetType() == typeof(WildIndustryCard))
		{
			Board.WildIndustryCardDeck.Add(cardDiscarded.CardToDiscard);
		}
		else if (cardDiscarded.CardToDiscard.GetType() == typeof(WildLocationCard))
		{
			Board.WildLocationCardDeck.Add(cardDiscarded.CardToDiscard);
		}
		playerByColor.Hand.Remove(cardDiscarded.CardToDiscard);
	}

	private void ApplyEvent(ActionCountDecreased actionCountDecreased)
	{
		DecreaseRemainingActionsCountAction();
	}

	private void ApplyEvent(IndustryFlipped industryFlipped)
	{
		Board.GetIndustryByUniqueId(industryFlipped.IndustryUniqueId).IsFlipped = true;
	}
}
