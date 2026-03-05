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

internal class BuildIndustryForExtraSale : BaseRule
{
	public BuildIndustryForExtraSale()
	{
		base.Id = RuleId.BuildIndustryForExtraSale;
		base.AILevel = AIType.Hard;
	}

	public override int GetPriority(BrassApplicationGame brassApplicationGame)
	{
		return 2700;
	}

	public override bool IsApplicable(BrassApplicationGame brassApplicationGame)
	{
		if (brassApplicationGame.Game.TurnFlow.GetRemainingActionsCount() < 2)
		{
			return false;
		}
		Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
		List<BuildingSlot> validBuildingSlotsForSellingIndustries = brassApplicationGame.Game.TurnFlow.SellAction.GetValidBuildingSlotsForSellingIndustries(brassApplicationGame.Game.Board, currentPlayer);
		if (validBuildingSlotsForSellingIndustries.Count == 0 || validBuildingSlotsForSellingIndustries.Count >= 2)
		{
			return false;
		}
		return BestSlot(brassApplicationGame) != null;
	}

	public override Action MakeAction(BrassApplicationGame brassApplicationGame, bool printOnly)
	{
		return delegate
		{
			Board board = brassApplicationGame.Game.Board;
			Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
			BuildAction buildAction = brassApplicationGame.Game.TurnFlow.StartBuildAction();
			Tuple<BuildingSlot, BuildingType> tuple = BestSlot(brassApplicationGame);
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

	private Tuple<BuildingSlot, BuildingType> BestSlot(BrassApplicationGame brassApplicationGame)
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
		int num = -1000;
		foreach (BuildingSlot item in BoardHelper.PossibleBuildingSlotsCottonManufacturePottery)
		{
			foreach (BuildingType key in dictionary.Keys)
			{
				if (buildAction.GetCantUseBuildActionReasonForGivenSlot(item, currentPlayer.Mat.GetFirstBuildingOfType(key), currentPlayer, board, gameProgressInformation).Count != 0 || !buildAction.IsGivenSlotTheBestOneInGivenRegionForGivenType(item, key, board))
				{
					continue;
				}
				int num2 = 0;
				Region regionContainingGivenSlot = board.GetRegionContainingGivenSlot(item);
				if (item.BuildedIndustry != null)
				{
					num2 = ((item.BuildedIndustry.Color != currentPlayer.Color) ? (num2 + 100) : (num2 - 100));
				}
				bool flag = false;
				foreach (Region item2 in BoardHelper.ConnectedRegions(brassApplicationGame, regionContainingGivenSlot))
				{
					if (!(item2 is BorderRegion))
					{
						continue;
					}
					foreach (MerchantSlot merchantSlot in ((BorderRegion)item2).MerchantSlots)
					{
						if (merchantSlot.MerchantTile != null && merchantSlot.MerchantTile.BuildingTypes.Contains(key))
						{
							flag = true;
						}
					}
				}
				if (flag)
				{
					num2 += 50;
				}
				num2 = (dictionary[key].ContainsKey(regionContainingGivenSlot) ? (num2 - 100 * dictionary[key][regionContainingGivenSlot]) : (num2 - 1000));
				BaseIndustry firstBuildingOfType = currentPlayer.Mat.GetFirstBuildingOfType(key);
				num2 += firstBuildingOfType.VPForBuilding;
				if (firstBuildingOfType.IsForCanalEra && !firstBuildingOfType.IsForRailEra)
				{
					num2 += 4;
				}
				if (num2 > num)
				{
					num = num2;
					result = new Tuple<BuildingSlot, BuildingType>(item, key);
				}
			}
		}
		if (num >= -1)
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
