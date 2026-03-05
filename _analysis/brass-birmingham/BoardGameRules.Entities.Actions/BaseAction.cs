using System;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.Common;
using BoardGameRules.Entities.EventSourcing.Aggregates;
using BoardGameRules.Entities.NumericalResources;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.Actions;

public abstract class BaseAction : IGameAction
{
	protected Player _player;

	protected BaseCard _chosenCard;

	protected IAggregateRoot AggregateRoot { get; }

	protected PlayersTrack PlayersTrack { get; }

	protected BoardGameRules.Entities.Board.Board Board { get; }

	public GeneralAction GeneralAction { get; }

	protected BaseAction(IAggregateRoot aggregateRoot, PlayersTrack playersTrack, BoardGameRules.Entities.Board.Board board, GeneralAction generalAction)
	{
		AggregateRoot = aggregateRoot;
		PlayersTrack = playersTrack;
		Board = board;
		GeneralAction = generalAction;
	}

	public void DiscardCard(BaseCard card, Player player)
	{
		if (card == null)
		{
			throw new InvalidOperationException("Chosen card is null.");
		}
		if (player == null)
		{
			throw new InvalidOperationException("Given player doesn't exist in game.");
		}
		_player = player;
		_chosenCard = card;
		GeneralAction.DiscardCard(card, player.Color);
	}

	public virtual void ResetAction()
	{
		_player = null;
		_chosenCard = null;
	}

	public virtual void SupplyCoal(ICoalSource coalSource, int amount)
	{
		throw new InvalidOperationException("SupplyCoal should not be used with this action.");
	}

	public virtual void SupplyIron(IIronSource ironSource, int amount)
	{
		throw new InvalidOperationException("SupplyIron should not be used with this action.");
	}

	public virtual void SupplyBeer(IBeerSource beerSource, int amount)
	{
		throw new InvalidOperationException("SupplyBeer should not be used with this action.");
	}
}
