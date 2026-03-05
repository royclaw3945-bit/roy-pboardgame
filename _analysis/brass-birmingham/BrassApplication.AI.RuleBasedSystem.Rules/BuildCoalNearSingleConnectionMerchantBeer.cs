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
using BoardGameRules.Entities.Players;
using BrassApplication.AI.AIHelpers;
using BrassApplication.Game;

namespace BrassApplication.AI.RuleBasedSystem.Rules;

internal class BuildCoalNearSingleConnectionMerchantBeer : BaseRule
{
	public BuildCoalNearSingleConnectionMerchantBeer()
	{
		base.Id = RuleId.BuildCoalNearSingleConnectionMerchantBeer;
		base.AILevel = AIType.Hard;
	}

	public override int GetPriority(BrassApplicationGame brassApplicationGame)
	{
		return 1600;
	}

	public override bool IsApplicable(BrassApplicationGame brassApplicationGame)
	{
		return ApplicableCoalSlotsNearManufactureForBeer(brassApplicationGame).Count > 0;
	}

	public override Action MakeAction(BrassApplicationGame brassApplicationGame, bool printOnly)
	{
		return delegate
		{
			Board board = brassApplicationGame.Game.Board;
			Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
			BuildAction buildAction = brassApplicationGame.Game.TurnFlow.StartBuildAction();
			BuildingSlot buildingSlot = ApplicableCoalSlotsNearManufactureForBeer(brassApplicationGame)[0];
			BuildingType buildingType = BuildingType.CoalMine;
			BaseCard baseCard = BoardHelper.LowestValueCard(buildAction.GetValidCardsForBuildingInSlot(buildingSlot, buildingType, currentPlayer, board), brassApplicationGame);
			Region regionContainingGivenSlot = board.GetRegionContainingGivenSlot(buildingSlot);
			BaseRule.Logger.Info(string.Concat(BaseRule.LOG_PREFIX, GetPriority(brassApplicationGame), ": ", base.Id.ToString(), BaseRule.LOG_NEWLINE, "Build ", buildingType.ToString(), " in ", regionContainingGivenSlot.Name, " with card: ", baseCard));
			if (!printOnly)
			{
				BuildActionAIHelper.BuildIndustry(buildAction, baseCard, buildingSlot, buildingType, board, currentPlayer);
			}
		};
	}

	private List<BuildingSlot> ApplicableCoalSlotsNearManufactureForBeer(BrassApplicationGame brassApplicationGame)
	{
		Board board = brassApplicationGame.Game.Board;
		_ = brassApplicationGame.Game.GameProgressInformation;
		IEnumerable<MerchantSlot> merchantsWithBeer = board.GetMerchantsWithBeer();
		List<MerchantSlot> list = new List<MerchantSlot>();
		foreach (MerchantSlot item in merchantsWithBeer)
		{
			bool flag = false;
			foreach (BuildingType buildingType in item.MerchantTile.BuildingTypes)
			{
				if (buildingType == BuildingType.Manufacturer || buildingType == BuildingType.Pottery)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				continue;
			}
			BorderRegion regionOfGivenMerchantTile = board.GetRegionOfGivenMerchantTile(item);
			int num = 0;
			foreach (Link link in regionOfGivenMerchantTile.Links)
			{
				if (BoardHelper.IsLinkApplicableForCurrentEra(brassApplicationGame, link))
				{
					num++;
				}
			}
			if (num == 1)
			{
				list.Add(item);
			}
		}
		HashSet<Region> hashSet = new HashSet<Region>();
		foreach (MerchantSlot item2 in list)
		{
			Region regionOfGivenMerchantTile2 = board.GetRegionOfGivenMerchantTile(item2);
			foreach (Region regionNeighbour in BoardHelper.GetRegionNeighbours(brassApplicationGame, regionOfGivenMerchantTile2))
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
			bool flag2 = false;
			foreach (BuildingSlot buildingSlot in item3.BuildingSlots)
			{
				if (buildingSlot.BuildedIndustry == null && buildingSlot.CanBuildIndustryType(BuildingType.CoalMine))
				{
					flag2 = true;
				}
			}
			if (flag2)
			{
				hashSet2.Add(item3);
			}
		}
		HashSet<BuildingSlot> hashSet3 = new HashSet<BuildingSlot>();
		foreach (Region item4 in hashSet2)
		{
			foreach (BuildingSlot buildingSlot2 in item4.BuildingSlots)
			{
				if (BoardHelper.PossibleBuildingSlots[BuildingType.CoalMine].Contains(buildingSlot2) && !hashSet3.Contains(buildingSlot2))
				{
					hashSet3.Add(buildingSlot2);
				}
			}
		}
		return hashSet3.ToList();
	}

	public override void PrintAction(BrassApplicationGame brassApplicationGame)
	{
		MakeAction(brassApplicationGame, printOnly: true)();
	}
}
