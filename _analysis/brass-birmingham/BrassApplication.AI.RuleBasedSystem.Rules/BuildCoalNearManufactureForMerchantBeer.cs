using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Merchants;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.Players;
using BrassApplication.AI.AIHelpers;
using BrassApplication.Game;

namespace BrassApplication.AI.RuleBasedSystem.Rules;

internal class BuildCoalNearManufactureForMerchantBeer : BaseRule
{
	public BuildCoalNearManufactureForMerchantBeer()
	{
		base.Id = RuleId.BuildCoalNearManufactureForMerchantBeer;
		base.AILevel = AIType.Hard;
	}

	public override int GetPriority(BrassApplicationGame brassApplicationGame)
	{
		return 1210;
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
		if (hashSet2.Count == 0)
		{
			HashSet<Region> hashSet3 = new HashSet<Region>();
			foreach (Region item4 in hashSet)
			{
				foreach (Region regionNeighbour2 in BoardHelper.GetRegionNeighbours(brassApplicationGame, item4))
				{
					if (!hashSet.Contains(regionNeighbour2) && !hashSet3.Contains(regionNeighbour2))
					{
						hashSet3.Add(regionNeighbour2);
					}
				}
			}
			foreach (Region item5 in hashSet3)
			{
				bool flag3 = false;
				bool flag4 = false;
				foreach (BuildingSlot buildingSlot2 in item5.BuildingSlots)
				{
					if (buildingSlot2.BuildedIndustry == null && buildingSlot2.CanBuildIndustryType(BuildingType.Manufacturer))
					{
						flag3 = true;
					}
					if (buildingSlot2.BuildedIndustry != null && buildingSlot2.BuildedIndustry.Color == brassApplicationGame.Game.GameProgressInformation.CurrentPlayer.Color)
					{
						flag4 = true;
					}
				}
				if (flag3 && !flag4)
				{
					hashSet2.Add(item5);
				}
			}
		}
		HashSet<Region> hashSet4 = new HashSet<Region>();
		foreach (Region item6 in hashSet2)
		{
			foreach (Region regionNeighbour3 in BoardHelper.GetRegionNeighbours(brassApplicationGame, item6))
			{
				if (!hashSet4.Contains(regionNeighbour3))
				{
					hashSet4.Add(regionNeighbour3);
				}
			}
		}
		HashSet<BuildingSlot> hashSet5 = new HashSet<BuildingSlot>();
		foreach (Region item7 in hashSet4)
		{
			foreach (BuildingSlot buildingSlot3 in item7.BuildingSlots)
			{
				if (BoardHelper.PossibleBuildingSlots[BuildingType.CoalMine].Contains(buildingSlot3) && !hashSet5.Contains(buildingSlot3))
				{
					hashSet5.Add(buildingSlot3);
				}
			}
		}
		return hashSet5.ToList();
	}

	public override void PrintAction(BrassApplicationGame brassApplicationGame)
	{
		MakeAction(brassApplicationGame, printOnly: true)();
	}
}
