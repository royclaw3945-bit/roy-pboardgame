using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Actions.Events;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Common;
using BoardGameRules.Entities.EventSourcing.Aggregates;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.Markets;
using BoardGameRules.Entities.NumericalResources;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.Consumables;

public class ConsumingCoal
{
	private IAggregateRoot AggregateRoot { get; }

	protected BoardGameRules.Entities.Board.Board Board { get; }

	protected PlayersTrack PlayersTrack { get; }

	public ConsumingCoal(IAggregateRoot aggregateRoot, BoardGameRules.Entities.Board.Board board, PlayersTrack playersTrack)
	{
		AggregateRoot = aggregateRoot;
		Board = board;
		PlayersTrack = playersTrack;
		AggregateRoot.RegisterApplyEvent<CoalSupplied>(ApplyEvent);
		AggregateRoot.RegisterApplyEvent<CoalBought>(ApplyEvent);
	}

	public void SupplyCoal(ICoalSource coalSource, int amount, int neededCoal, Player player, List<Region> regions)
	{
		if (neededCoal <= 0)
		{
			throw new InvalidOperationException("No more coal needed.");
		}
		if (neededCoal < amount)
		{
			throw new InvalidOperationException("You want to supply more coal than needed.");
		}
		if (player == null)
		{
			throw new InvalidOperationException("Player is not set up.");
		}
		if (coalSource == null)
		{
			throw new InvalidOperationException("Given source is null.");
		}
		if (!(coalSource is CoalMarket) && coalSource.AvailableCoal < amount)
		{
			throw new InvalidOperationException("Given source doesn't have enough coal.");
		}
		if (amount == 0)
		{
			throw new InvalidOperationException("Amount is equal 0.");
		}
		if (!regions.Any((Region r) => r != null && CanCoalBeSuppliedToRegionFromSource(coalSource, r)))
		{
			throw new InvalidOperationException("Coal can't be supplied from given source.");
		}
		int num = coalSource.CoalPrice(amount);
		if (num > 0)
		{
			if (player.Money < num)
			{
				throw new InvalidOperationException("Player doesn't have enough money to buy given amount of coal.");
			}
			AggregateRoot.ApplyEvent(new CoalBought(amount, Board.CoalMarket.GetBuyingPrice(amount), player.Color));
		}
		AggregateRoot.ApplyEvent(new CoalSupplied(coalSource.UniqueId, amount));
		if (!coalSource.HasCoal)
		{
			BaseIndustry baseIndustry = (BaseIndustry)coalSource;
			RegionName name = Board.GetRegionContainingGivenIndustry(baseIndustry).Name;
			AggregateRoot.ApplyEvent(new IndustryFlipped(baseIndustry.UniqueId, name));
			PlayersTrack.GetPlayerByColor(baseIndustry.Color).ChangeIncomeMarkerPositionBy(baseIndustry.IncomeIncrease, baseIndustry.UniqueId);
		}
	}

	public bool CanCoalBeSuppliedToRegionFromSource(ICoalSource source, Region region)
	{
		return CanCoalBeSuppliedToRegionFromSource(source, region, Board);
	}

	public static bool CanCoalBeSuppliedToRegionFromSource(ICoalSource source, Region region, BoardGameRules.Entities.Board.Board board, bool ignoreCoalOnBoard = false)
	{
		if (region == null)
		{
			return false;
		}
		if (source is CoalMarket)
		{
			if (!ignoreCoalOnBoard && IsThereACoalOnBoardThatCanBeUsedToSupplyToGivenRegion(board, region))
			{
				return false;
			}
			if (IsRegionCoalMarket(board, region))
			{
				return true;
			}
			return board.GetBorderRegions((BorderRegion r) => true).Any((BorderRegion r) => board.GetRouteLength(r, region) > 0);
		}
		Region regionContainingGivenIndustry = board.GetRegionContainingGivenIndustry(source as BaseIndustry);
		if (regionContainingGivenIndustry == region)
		{
			return true;
		}
		List<Region> shortestRouteTo = board.GetShortestRouteTo(regionContainingGivenIndustry, region);
		if (shortestRouteTo != null)
		{
			return TheSourceIsTheClosestOne(board, region, source, shortestRouteTo);
		}
		return false;
	}

	private void ApplyEvent(CoalSupplied coalSupplied)
	{
		Board.GetCoalSourceById(coalSupplied.SourceUniqueId).ConsumeCoal(coalSupplied.Amount);
	}

	private void ApplyEvent(CoalBought coalBought)
	{
		PlayersTrack.GetPlayerByColor(coalBought.PlayerColor).Money -= coalBought.Price;
		PlayersTrack.SpendMoneyInCurrentRound[coalBought.PlayerColor] += coalBought.Price;
	}

	private static bool IsThereACoalOnBoardThatCanBeUsedToSupplyToGivenRegion(BoardGameRules.Entities.Board.Board board, Region region)
	{
		foreach (Region r in board.Regions)
		{
			if (r.BuildingSlots.Any((BuildingSlot s) => s.BuildedIndustry != null && s.BuildedIndustry is CoalMine { HasCoal: not false } && board.DoesRouteExists(r, region)))
			{
				return true;
			}
		}
		return false;
	}

	private static bool TheSourceIsTheClosestOne(BoardGameRules.Entities.Board.Board board, Region region, ICoalSource source, List<Region> route)
	{
		foreach (Region region2 in board.Regions)
		{
			if (region2.BuildingSlots.Any((BuildingSlot s) => s.BuildedIndustry != null && s.BuildedIndustry is CoalMine { HasCoal: not false } coalMine && coalMine != source))
			{
				int routeLength = board.GetRouteLength(region2, region);
				if (board.DoesRouteExists(region2, region) && routeLength < route.Count)
				{
					return false;
				}
			}
		}
		return true;
	}

	private static bool IsRegionCoalMarket(BoardGameRules.Entities.Board.Board board, Region region)
	{
		return board.GetBorderRegions((BorderRegion br) => br.Name == region.Name).Any();
	}
}
