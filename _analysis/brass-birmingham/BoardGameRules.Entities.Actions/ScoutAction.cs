using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Actions.Events;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.EventSourcing.Aggregates;
using BoardGameRules.Entities.Game;
using BoardGameRules.Entities.NumericalResources;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.Actions;

public class ScoutAction : BaseAction
{
	public ScoutAction(IAggregateRoot aggregateRoot, PlayersTrack playersTrack, BoardGameRules.Entities.Board.Board board, GeneralAction generalAction)
		: base(aggregateRoot, playersTrack, board, generalAction)
	{
		aggregateRoot.RegisterApplyEvent<WildCardsTaken>(ApplyEvent);
	}

	public void Scout(List<BaseCard> cardsToTrade, Player player)
	{
		if (cardsToTrade.Count != 2)
		{
			throw new InvalidOperationException("Player doesn't have enough cards to perform action.");
		}
		if (player.Hand.Any((BaseCard c) => c is WildIndustryCard || c is WildLocationCard))
		{
			throw new InvalidOperationException("Player has already a wild card in his hand.");
		}
		base.AggregateRoot.ApplyEvent(new WildCardsTaken(player.Color, cardsToTrade));
	}

	private void ApplyEvent(WildCardsTaken wildCardsTaken)
	{
		Player playerByColor = base.PlayersTrack.GetPlayerByColor(wildCardsTaken.Player);
		int[] array = new int[2]
		{
			playerByColor.Hand.IndexOf(wildCardsTaken.CardsToTrade[0]),
			playerByColor.Hand.IndexOf(wildCardsTaken.CardsToTrade[1])
		};
		playerByColor.DiscardPile.Add(wildCardsTaken.CardsToTrade[0]);
		playerByColor.DiscardPile.Add(wildCardsTaken.CardsToTrade[1]);
		BaseCard item = base.Board.WildIndustryCardDeck.First();
		playerByColor.Hand.Insert(array[0], item);
		playerByColor.Hand.Remove(wildCardsTaken.CardsToTrade[0]);
		base.Board.WildIndustryCardDeck.RemoveAt(0);
		item = base.Board.WildLocationCardDeck.First();
		playerByColor.Hand.Insert(array[1], item);
		playerByColor.Hand.Remove(wildCardsTaken.CardsToTrade[1]);
		base.Board.WildLocationCardDeck.RemoveAt(0);
	}

	public List<CantUseScoutActionReason> GetScoutActionNotPossibleReasons(BoardGameRules.Entities.Board.Board board, GameProgressInformation gameProgressInformation)
	{
		List<CantUseScoutActionReason> list = new List<CantUseScoutActionReason>();
		Player currentPlayer = gameProgressInformation.CurrentPlayer;
		if (currentPlayer.Hand.Any((BaseCard c) => c is WildIndustryCard || c is WildLocationCard))
		{
			list.Add(CantUseScoutActionReason.ALREADY_WILD_CARD_IN_HAND);
		}
		if (currentPlayer.Hand.Count < 3)
		{
			list.Add(CantUseScoutActionReason.NOT_ENOUGH_CARDS_IN_HAND);
		}
		if (!board.WildIndustryCardDeck.Any() || !board.WildLocationCardDeck.Any())
		{
			list.Add(CantUseScoutActionReason.NOT_ENOUGH_WILD_CARDS);
		}
		return list;
	}
}
