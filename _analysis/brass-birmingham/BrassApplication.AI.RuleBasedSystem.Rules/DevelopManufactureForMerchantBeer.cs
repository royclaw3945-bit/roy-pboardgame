using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Merchants;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.Common;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.Players;
using BrassApplication.AI.AIHelpers;
using BrassApplication.Game;

namespace BrassApplication.AI.RuleBasedSystem.Rules;

internal class DevelopManufactureForMerchantBeer : BaseRule
{
	public DevelopManufactureForMerchantBeer()
	{
		base.Id = RuleId.DevelopManufactureForMerchantBeer;
		base.AILevel = AIType.Hard;
	}

	public override int GetPriority(BrassApplicationGame brassApplicationGame)
	{
		return 1800;
	}

	public override bool IsApplicable(BrassApplicationGame brassApplicationGame)
	{
		Board board = brassApplicationGame.Game.Board;
		IIronSource ironSource = board.GetUnflippedIronWorks().FirstOrDefault();
		IIronSource ironSource2 = ironSource ?? board.IronMarket;
		Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
		BaseIndustry firstBuildingOfType = currentPlayer.Mat.GetFirstBuildingOfType(BuildingType.Manufacturer);
		bool flag = false;
		foreach (BaseCard item in currentPlayer.Hand)
		{
			if (!(item is IndustryCard))
			{
				continue;
			}
			foreach (BuildingType buildingType in ((IndustryCard)item).BuildingTypes)
			{
				if (buildingType == BuildingType.Manufacturer)
				{
					flag = true;
				}
			}
		}
		if (flag && brassApplicationGame.Game.GameProgressInformation.CurrentRoundNumber < 2 && ironSource2.IronPrice() <= currentPlayer.Money && firstBuildingOfType.Level == 1)
		{
			return HasApplicableManufactureSlotsForBeer(brassApplicationGame);
		}
		return false;
	}

	private bool HasApplicableManufactureSlotsForBeer(BrassApplicationGame brassApplicationGame)
	{
		Board board = brassApplicationGame.Game.Board;
		_ = brassApplicationGame.Game.GameProgressInformation;
		IEnumerable<MerchantSlot> merchantsWithBeer = board.GetMerchantsWithBeer();
		List<MerchantSlot> list = new List<MerchantSlot>();
		foreach (MerchantSlot item in merchantsWithBeer)
		{
			if (item.MerchantTile.BuildingTypes.Count == 1 && item.MerchantTile.BuildingTypes[0] == BuildingType.Manufacturer)
			{
				list.Add(item);
			}
		}
		HashSet<Region> hashSet = new HashSet<Region>();
		foreach (MerchantSlot item2 in list)
		{
			Region regionOfGivenMerchantTile = board.GetRegionOfGivenMerchantTile(item2);
			foreach (Region regionNeighbour in BoardHelper.GetRegionNeighbours(brassApplicationGame, regionOfGivenMerchantTile))
			{
				if (!hashSet.Contains(regionNeighbour))
				{
					hashSet.Add(regionNeighbour);
				}
			}
		}
		HashSet<Region> hashSet2 = new HashSet<Region>();
		foreach (Region item3 in hashSet)
		{
			bool flag = false;
			bool flag2 = false;
			foreach (BuildingSlot buildingSlot in item3.BuildingSlots)
			{
				if (buildingSlot.BuildedIndustry == null && buildingSlot.CanBuildIndustryType(BuildingType.Manufacturer))
				{
					flag = true;
				}
				if (buildingSlot.BuildedIndustry != null && buildingSlot.BuildedIndustry.Color == brassApplicationGame.Game.GameProgressInformation.CurrentPlayer.Color)
				{
					flag2 = true;
				}
			}
			if (flag && !flag2)
			{
				hashSet2.Add(item3);
			}
		}
		return hashSet2.Count > 0;
	}

	public List<BuildingType> OtherApplicableIndustries(BrassApplicationGame brassApplicationGame)
	{
		List<BuildingType> list = new List<BuildingType>();
		Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
		BaseIndustry firstBuildingOfType = currentPlayer.Mat.GetFirstBuildingOfType(BuildingType.CoalMine);
		BaseIndustry firstBuildingOfType2 = currentPlayer.Mat.GetFirstBuildingOfType(BuildingType.IronWorks);
		currentPlayer.Mat.GetFirstBuildingOfType(BuildingType.Manufacturer);
		BaseIndustry firstBuildingOfType3 = currentPlayer.Mat.GetFirstBuildingOfType(BuildingType.Brewery);
		BaseIndustry firstBuildingOfType4 = currentPlayer.Mat.GetFirstBuildingOfType(BuildingType.CottonMill);
		if (firstBuildingOfType4 != null && firstBuildingOfType4.IsForCanalEra && !firstBuildingOfType4.IsForRailEra)
		{
			list.Add(BuildingType.CottonMill);
		}
		if (firstBuildingOfType3 != null && firstBuildingOfType3.IsForCanalEra && !firstBuildingOfType3.IsForRailEra)
		{
			list.Add(BuildingType.Brewery);
		}
		if (firstBuildingOfType != null && firstBuildingOfType.IsForCanalEra && !firstBuildingOfType.IsForRailEra)
		{
			list.Add(BuildingType.CoalMine);
		}
		if (firstBuildingOfType2 != null && firstBuildingOfType2.IsForCanalEra && !firstBuildingOfType2.IsForRailEra)
		{
			list.Add(BuildingType.IronWorks);
		}
		return list;
	}

	public override Action MakeAction(BrassApplicationGame brassApplicationGame, bool printOnly)
	{
		return delegate
		{
			Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
			DevelopAction developAction = brassApplicationGame.Game.TurnFlow.StartDevelopAction();
			BaseCard baseCard = BoardHelper.LowestValueCard(currentPlayer.Hand, brassApplicationGame, BuildingType.Manufacturer);
			List<BuildingType> list = OtherApplicableIndustries(brassApplicationGame);
			BuildingType buildingType = BuildingType.Manufacturer;
			BuildingType buildingType2 = BuildingType.Undefined;
			if (list.Count >= 1)
			{
				buildingType2 = list[0];
			}
			BaseRule.Logger.Info(string.Concat(BaseRule.LOG_PREFIX, GetPriority(brassApplicationGame), ": ", base.Id.ToString(), BaseRule.LOG_NEWLINE, "Develop Industries: ", buildingType, ", ", buildingType2, " with card: ", baseCard));
			if (!printOnly)
			{
				DevelopActionAIHelper.DevelopIndustry(developAction, baseCard, buildingType, buildingType2, brassApplicationGame.Game.Board, currentPlayer);
			}
		};
	}

	public override void PrintAction(BrassApplicationGame brassApplicationGame)
	{
		MakeAction(brassApplicationGame, printOnly: true)();
	}
}
