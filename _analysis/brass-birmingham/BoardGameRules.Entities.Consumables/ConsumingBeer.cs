using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Actions.Events;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Events;
using BoardGameRules.Entities.Board.Merchants;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Common;
using BoardGameRules.Entities.EventSourcing.Aggregates;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.NumericalResources;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.Consumables;

public class ConsumingBeer
{
	private IAggregateRoot AggregateRoot { get; }

	protected BoardGameRules.Entities.Board.Board Board { get; }

	protected PlayersTrack PlayersTrack { get; }

	public ConsumingBeer(IAggregateRoot aggregateRoot, BoardGameRules.Entities.Board.Board board, PlayersTrack playersTrack)
	{
		AggregateRoot = aggregateRoot;
		Board = board;
		PlayersTrack = playersTrack;
		AggregateRoot.RegisterApplyEvent<BeerSupplied>(ApplyEvent);
	}

	public void SupplyBeer(IBeerSource beerSource, int amount, int neededBeer, Player player, List<Region> regions)
	{
		if (neededBeer <= 0)
		{
			throw new InvalidOperationException("No more beer needed.");
		}
		if (neededBeer < amount)
		{
			throw new InvalidOperationException("You want to supply more beer than needed.");
		}
		if (player == null)
		{
			throw new InvalidOperationException("Player is not set up.");
		}
		if (beerSource == null)
		{
			throw new InvalidOperationException("Given source is null.");
		}
		if (beerSource.AvailableBeer < amount)
		{
			throw new InvalidOperationException("Given source doesn't have enough beer.");
		}
		if (amount == 0)
		{
			throw new InvalidOperationException("Amount is equal 0.");
		}
		if (!regions.Any((Region r) => r != null && CanBeerBeSuppliedToSlotFromSource(beerSource, r, player)))
		{
			throw new InvalidOperationException("Beer- can't be supplied from given source.");
		}
		AggregateRoot.ApplyEvent(new BeerSupplied(beerSource.UniqueId, amount));
		if (!beerSource.HasBeer && beerSource is Brewery)
		{
			BaseIndustry baseIndustry = (BaseIndustry)beerSource;
			RegionName name = Board.GetRegionContainingGivenIndustry(baseIndustry).Name;
			AggregateRoot.ApplyEvent(new IndustryFlipped(baseIndustry.UniqueId, name));
			PlayersTrack.GetPlayerByColor(baseIndustry.Color).ChangeIncomeMarkerPositionBy(baseIndustry.IncomeIncrease, baseIndustry.UniqueId);
		}
		if (beerSource is MerchantSlot)
		{
			BorderRegion regionOfGivenMerchantTile = Board.GetRegionOfGivenMerchantTile((MerchantSlot)beerSource);
			AggregateRoot.ApplyEvent(new MerchantBonusReceived(regionOfGivenMerchantTile.MerchantBeerBonusType, player.Color, regionOfGivenMerchantTile.Name));
		}
	}

	public bool CanBeerBeSuppliedToSlotFromSource(IBeerSource source, Region usedRegion, Player player)
	{
		if (source is Brewery { Color: var color } brewery)
		{
			if (color.Equals(player.Color))
			{
				return true;
			}
			Region regionContainingGivenIndustry = Board.GetRegionContainingGivenIndustry(brewery);
			if (regionContainingGivenIndustry == usedRegion)
			{
				return true;
			}
			return Board.DoesRouteExists(regionContainingGivenIndustry, usedRegion);
		}
		BorderRegion regionOfGivenMerchantTile = Board.GetRegionOfGivenMerchantTile((MerchantSlot)source);
		if (regionOfGivenMerchantTile.Name == usedRegion.Name)
		{
			return true;
		}
		return Board.DoesRouteExists(regionOfGivenMerchantTile, usedRegion);
	}

	public bool CanBeerBeSuppliedToSlotFromSource(IBeerSource source, BuildingSlot buildingSlot, Player player)
	{
		Region regionContainingGivenSlot = Board.GetRegionContainingGivenSlot(buildingSlot);
		return CanBeerBeSuppliedToSlotFromSource(source, regionContainingGivenSlot, player);
	}

	private void ApplyEvent(BeerSupplied beerSupplied)
	{
		Board.GetBeerSourceById(beerSupplied.BeerSourceId).ConsumeBeer(beerSupplied.Amount);
	}
}
