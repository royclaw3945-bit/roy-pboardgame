using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Links;
using BoardGameRules.Entities.Board.Merchants;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.Common;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.Players;
using BoardGameRules.Utils.Logging;

namespace BrassApplication.AI.AIHelpers;

public static class SellActionAIHelper
{
	public static IBeerSource FindBeerSource(SellAction sellAction, Board board, Player player, BuildingSlot slotToSell)
	{
		Region targetRegion = board.GetRegionContainingGivenSlot(slotToSell);
		BaseIndustry industryToSell = slotToSell.BuildedIndustry;
		foreach (MerchantSlot item in from m in board.GetBorderRegions((BorderRegion r) => true).SelectMany((BorderRegion r) => r.MerchantSlots)
			where m.HasBeer && m.MerchantTile.BuildingTypes.Contains(industryToSell.BuildingType)
			select m)
		{
			Region regionOfGivenMerchantTile = board.GetRegionOfGivenMerchantTile(item);
			if (board.DoesRouteExists(regionOfGivenMerchantTile, targetRegion))
			{
				return item;
			}
		}
		IEnumerable<Brewery> source = from b in board.GetUnflippedBreweries()
			where sellAction.GeneralAction.ConsumingBeer.CanBeerBeSuppliedToSlotFromSource(b, targetRegion, player)
			select b;
		if (source.Any())
		{
			return source.First();
		}
		return null;
	}

	private static int PlayerLinks(Region region, Player player)
	{
		int num = 0;
		foreach (Link link in region.Links)
		{
			if (link.HasOwner && link.Owner == player)
			{
				num++;
			}
		}
		return num;
	}

	public static List<BuildingSlot> PossibleSlotsToSell(SellAction sellAction, Board board, Player player)
	{
		return (from s in sellAction.GetValidBuildingSlotsForSellingIndustries(board, player)
			orderby s.BuildedIndustry.IncomeIncrease * 10 + PlayerLinks(board.GetRegionContainingGivenSlot(s), player) descending
			select s).ToList();
	}

	public static BuildingSlot SlotToSell(SellAction sellAction, BaseCard card, Board board, Player player, List<BuildingSlot> possibleSlotsToSell)
	{
		IEnumerable<MerchantSlot> merchantSlotsWithBeer = from m in board.GetBorderRegions((BorderRegion r) => true).SelectMany((BorderRegion r) => r.MerchantSlots)
			where m.HasBeer
			select m;
		BuildingSlot buildingSlot = null;
		if (buildingSlot == null && possibleSlotsToSell.Count > 0)
		{
			foreach (BuildingSlot item in possibleSlotsToSell)
			{
				if (item.BuildedIndustry.IsForCanalEra && !item.BuildedIndustry.IsForRailEra)
				{
					buildingSlot = item;
				}
			}
		}
		if (buildingSlot == null && possibleSlotsToSell.Count > 0)
		{
			buildingSlot = possibleSlotsToSell.FirstOrDefault((BuildingSlot s) => merchantSlotsWithBeer.Any((MerchantSlot m) => m.MerchantTile.BuildingTypes.Contains(s.BuildedIndustry.BuildingType) && sellAction.GeneralAction.ConsumingBeer.CanBeerBeSuppliedToSlotFromSource(m, s, player)));
		}
		if (buildingSlot == null && possibleSlotsToSell.Count > 0)
		{
			buildingSlot = possibleSlotsToSell.First();
		}
		if (buildingSlot != null)
		{
			BaseIndustry industryToSell = buildingSlot.BuildedIndustry;
			Region regionWithIndustry = board.GetRegionContainingGivenIndustry(industryToSell);
			List<BorderRegion> source = (from r in board.GetBorderRegions((BorderRegion r) => r.MerchantSlots.Any((MerchantSlot s) => s.MerchantTile != null && s.MerchantTile.BuildingTypes.Contains(industryToSell.BuildingType)))
				where board.DoesRouteExists(r, regionWithIndustry)
				select r).ToList();
			IEnumerable<Brewery> source2 = from b in board.GetUnflippedBreweries()
				where sellAction.GeneralAction.ConsumingBeer.CanBeerBeSuppliedToSlotFromSource(b, regionWithIndustry, player)
				select b;
			if (source.Sum((BorderRegion m) => m.MerchantSlots.Sum((MerchantSlot ms) => (ms.MerchantTile.BuildingTypes.Contains(industryToSell.BuildingType) && ms.HasBeer) ? 1 : 0)) + source2.Sum((Brewery b) => b.AvailableBeer) < ((BaseSellableIndustry)industryToSell).CostOfBeerBarrelsToSellIndustry)
			{
				return null;
			}
		}
		return buildingSlot;
	}

	public static void SellIndustries(SellAction sellAction, BaseCard card, Board board, Player player, DevelopMerchantBonus developMerchantBonus, ILog logger)
	{
		sellAction.DiscardCard(card, player);
		List<BuildingSlot> possibleSlotsToSell = PossibleSlotsToSell(sellAction, board, player);
		for (BuildingSlot buildingSlot = SlotToSell(sellAction, card, board, player, possibleSlotsToSell); buildingSlot != null; buildingSlot = SlotToSell(sellAction, card, board, player, possibleSlotsToSell))
		{
			BaseIndustry buildedIndustry = buildingSlot.BuildedIndustry;
			sellAction.StartSellProcedure(buildedIndustry);
			logger.Info(string.Concat("Sell - ", buildedIndustry.BuildingType, " with card: ", card));
			while (sellAction.IsBeerNeeded())
			{
				IBeerSource beerSource = FindBeerSource(sellAction, board, player, buildingSlot);
				sellAction.SupplyBeer(beerSource, 1);
			}
			sellAction.SellIndustry();
			if (DevelopMerchantBonus.Instance.MerchantDevelopmentNeeded)
			{
				BuildingType buildingType = BuildingType.Undefined;
				if (buildingType == BuildingType.Undefined)
				{
					BaseIndustry firstBuildingOfType = player.Mat.GetFirstBuildingOfType(BuildingType.IronWorks);
					if (firstBuildingOfType != null && firstBuildingOfType.IsForCanalEra && !firstBuildingOfType.IsForRailEra)
					{
						buildingType = BuildingType.IronWorks;
					}
				}
				if (buildingType == BuildingType.Undefined)
				{
					BaseIndustry firstBuildingOfType2 = player.Mat.GetFirstBuildingOfType(BuildingType.Brewery);
					if (firstBuildingOfType2 != null && firstBuildingOfType2.IsForCanalEra && !firstBuildingOfType2.IsForRailEra)
					{
						buildingType = BuildingType.Brewery;
					}
				}
				if (buildingType == BuildingType.Undefined && player.Mat.GetFirstBuildingOfType(BuildingType.Brewery) != null)
				{
					buildingType = BuildingType.Brewery;
				}
				if (buildingType == BuildingType.Undefined)
				{
					List<BuildingType> list = Enum.GetValues(typeof(BuildingType)).Cast<BuildingType>().ToList();
					list.Remove(BuildingType.Undefined);
					IEnumerable<BuildingType> source = list.Where((BuildingType t) => player.Mat.CanIndustryBeDeveloped(t));
					if (source.Any())
					{
						buildingType = source.First();
					}
				}
				if (buildingType != BuildingType.Undefined)
				{
					developMerchantBonus.Develop(player, buildingType);
				}
				else
				{
					developMerchantBonus.FinishDevelop();
				}
			}
			possibleSlotsToSell = PossibleSlotsToSell(sellAction, board, player);
		}
	}
}
