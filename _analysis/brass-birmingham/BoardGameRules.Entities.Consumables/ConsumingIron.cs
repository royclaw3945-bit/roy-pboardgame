using System;
using BoardGameRules.Entities.Actions.Events;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Common;
using BoardGameRules.Entities.EventSourcing.Aggregates;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.NumericalResources;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.Consumables;

public class ConsumingIron
{
	private IAggregateRoot AggregateRoot { get; }

	protected BoardGameRules.Entities.Board.Board Board { get; }

	protected PlayersTrack PlayersTrack { get; }

	public ConsumingIron(IAggregateRoot aggregateRoot, BoardGameRules.Entities.Board.Board board, PlayersTrack playersTrack)
	{
		AggregateRoot = aggregateRoot;
		Board = board;
		PlayersTrack = playersTrack;
		AggregateRoot.RegisterApplyEvent<IronSupplied>(ApplyEvent);
		AggregateRoot.RegisterApplyEvent<IronBought>(ApplyEvent);
	}

	public void SupplyIron(IIronSource ironSource, int amount, int neededIron, Player player)
	{
		if (neededIron <= 0)
		{
			throw new InvalidOperationException("No more iron needed.");
		}
		if (neededIron < amount)
		{
			throw new InvalidOperationException("You want to supply more iron than needed.");
		}
		if (player == null)
		{
			throw new InvalidOperationException("Player is not set up.");
		}
		if (ironSource == null)
		{
			throw new InvalidOperationException("Given source is null.");
		}
		if (ironSource.AvailableIron < amount && ironSource is IronWorks)
		{
			throw new InvalidOperationException("Given source doesn't have enough iron.");
		}
		if (amount == 0)
		{
			throw new InvalidOperationException("Amount is equal 0.");
		}
		int num = ironSource.IronPrice(amount);
		if (num > 0)
		{
			if (player.Money < num)
			{
				throw new InvalidOperationException("Player doesn't have enough money to buy given amount of iron.");
			}
			AggregateRoot.ApplyEvent(new IronBought(amount, Board.IronMarket.GetBuyingPrice(amount), player.Color));
		}
		AggregateRoot.ApplyEvent(new IronSupplied(ironSource.UniqueId, amount));
		if (!ironSource.HasIron)
		{
			BaseIndustry baseIndustry = (BaseIndustry)ironSource;
			RegionName name = Board.GetRegionContainingGivenIndustry(baseIndustry).Name;
			AggregateRoot.ApplyEvent(new IndustryFlipped(baseIndustry.UniqueId, name));
			PlayersTrack.GetPlayerByColor(baseIndustry.Color).ChangeIncomeMarkerPositionBy(baseIndustry.IncomeIncrease, baseIndustry.UniqueId);
		}
	}

	private void ApplyEvent(IronSupplied ironSupplied)
	{
		Board.GetIronSourceById(ironSupplied.SourceId).ConsumeIron(ironSupplied.Amount);
	}

	private void ApplyEvent(IronBought ironBought)
	{
		PlayersTrack.GetPlayerByColor(ironBought.PlayerColor).Money -= ironBought.Price;
		PlayersTrack.SpendMoneyInCurrentRound[ironBought.PlayerColor] += ironBought.Price;
	}
}
