using System;
using System.Collections.Generic;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Merchants;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.Game;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.Players;
using BrassApplication.AI.AIHelpers;
using BrassApplication.Game;

namespace BrassApplication.AI.RuleBasedSystem.Rules;

internal class BuildIndustryForSale : BaseRule
{
	public BuildIndustryForSale()
	{
		base.Id = RuleId.BuildIndustryForSale;
		base.AILevel = AIType.Easy;
	}

	public override int GetPriority(BrassApplicationGame brassApplicationGame)
	{
		BestSlot(brassApplicationGame, out var bestRank);
		if (bestRank >= 0)
		{
			return 1630;
		}
		return 600;
	}

	public override bool IsApplicable(BrassApplicationGame brassApplicationGame)
	{
		bool flag = false;
		if (BestSlot(brassApplicationGame, out var bestRank) != null)
		{
			if (bestRank < 0)
			{
				if (flag)
				{
					return bestRank >= -10;
				}
				return false;
			}
			return true;
		}
		return false;
	}

	public override Action MakeAction(BrassApplicationGame brassApplicationGame, bool printOnly)
	{
		return delegate
		{
			Board board = brassApplicationGame.Game.Board;
			Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
			BuildAction buildAction = brassApplicationGame.Game.TurnFlow.StartBuildAction();
			int bestRank;
			Tuple<BuildingSlot, BuildingType> tuple = BestSlot(brassApplicationGame, out bestRank);
			BuildingSlot item = tuple.Item1;
			BuildingType item2 = tuple.Item2;
			BaseCard baseCard = BoardHelper.LowestValueCard(buildAction.GetValidCardsForBuildingInSlot(item, item2, currentPlayer, board), brassApplicationGame);
			Region regionContainingGivenSlot = board.GetRegionContainingGivenSlot(item);
			BaseRule.Logger.Info(string.Concat(BaseRule.LOG_PREFIX, GetPriority(brassApplicationGame), ": ", base.Id.ToString(), BaseRule.LOG_NEWLINE, "Build ", item2, " in ", regionContainingGivenSlot.Name, " with card: ", baseCard));
			if (!printOnly)
			{
				BuildActionAIHelper.BuildIndustry(buildAction, baseCard, item, item2, board, currentPlayer);
			}
		};
	}

	private Tuple<BuildingSlot, BuildingType> BestSlot(BrassApplicationGame brassApplicationGame, out int bestRank)
	{
		Board board = brassApplicationGame.Game.Board;
		GameProgressInformation gameProgressInformation = brassApplicationGame.Game.GameProgressInformation;
		Player currentPlayer = gameProgressInformation.CurrentPlayer;
		_ = brassApplicationGame.Game.TurnFlow.NetworkAction;
		Dictionary<Region, int> value = BoardHelper.MissingConnectionsCount(brassApplicationGame, BoardHelper.RegionsWithMerchants(brassApplicationGame, BuildingType.CottonMill));
		Dictionary<Region, int> value2 = BoardHelper.MissingConnectionsCount(brassApplicationGame, BoardHelper.RegionsWithMerchants(brassApplicationGame, BuildingType.Manufacturer));
		Dictionary<Region, int> value3 = BoardHelper.MissingConnectionsCount(brassApplicationGame, BoardHelper.RegionsWithMerchants(brassApplicationGame, BuildingType.Pottery));
		Dictionary<BuildingType, Dictionary<Region, int>> dictionary = new Dictionary<BuildingType, Dictionary<Region, int>>();
		dictionary.Add(BuildingType.CottonMill, value);
		dictionary.Add(BuildingType.Manufacturer, value2);
		dictionary.Add(BuildingType.Pottery, value3);
		BuildAction buildAction = brassApplicationGame.Game.TurnFlow.StartBuildAction();
		Tuple<BuildingSlot, BuildingType> result = null;
		bestRank = -1000;
		foreach (BuildingSlot item in BoardHelper.PossibleBuildingSlotsCottonManufacturePottery)
		{
			foreach (BuildingType key in dictionary.Keys)
			{
				if (buildAction.GetCantUseBuildActionReasonForGivenSlot(item, currentPlayer.Mat.GetFirstBuildingOfType(key), currentPlayer, board, gameProgressInformation).Count != 0 || !buildAction.IsGivenSlotTheBestOneInGivenRegionForGivenType(item, key, board))
				{
					continue;
				}
				int num = 0;
				if (item.BuildedIndustry != null)
				{
					num = ((item.BuildedIndustry.Color != currentPlayer.Color) ? (num + 1000) : (num - 1000));
				}
				bool flag = false;
				foreach (Region item2 in BoardHelper.ConnectedRegions(brassApplicationGame, board.GetRegionContainingGivenSlot(item)))
				{
					if (!(item2 is BorderRegion borderRegion))
					{
						continue;
					}
					foreach (MerchantSlot merchantSlot in borderRegion.MerchantSlots)
					{
						if (merchantSlot.MerchantTile != null && merchantSlot.MerchantTile.BuildingTypes.Contains(key))
						{
							flag = true;
						}
					}
				}
				if (flag)
				{
					num += 500;
				}
				Region regionContainingGivenSlot = board.GetRegionContainingGivenSlot(item);
				num = (dictionary[key].ContainsKey(regionContainingGivenSlot) ? (num - 10 * dictionary[key][regionContainingGivenSlot]) : (num - 10000));
				BaseIndustry firstBuildingOfType = currentPlayer.Mat.GetFirstBuildingOfType(key);
				num += firstBuildingOfType.IncomeIncrease;
				if (num > bestRank)
				{
					bestRank = num;
					result = new Tuple<BuildingSlot, BuildingType>(item, key);
				}
			}
		}
		if (bestRank >= -150)
		{
			return result;
		}
		return null;
	}

	public override void PrintAction(BrassApplicationGame brassApplicationGame)
	{
		MakeAction(brassApplicationGame, printOnly: true)();
	}
}
