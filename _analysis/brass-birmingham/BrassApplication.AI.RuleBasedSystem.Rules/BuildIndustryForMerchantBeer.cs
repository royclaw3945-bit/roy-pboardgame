using System;
using System.Collections.Generic;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Merchants;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.Game;
using BoardGameRules.Entities.GamePhases;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.Players;
using BrassApplication.AI.AIHelpers;
using BrassApplication.Game;

namespace BrassApplication.AI.RuleBasedSystem.Rules;

internal class BuildIndustryForMerchantBeer : BaseRule
{
	public BuildIndustryForMerchantBeer()
	{
		base.Id = RuleId.BuildIndustryForMerchantBeer;
		base.AILevel = AIType.Hard;
	}

	public override int GetPriority(BrassApplicationGame brassApplicationGame)
	{
		BestSlot(brassApplicationGame, out var bestRank);
		Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
		brassApplicationGame.Game.TurnFlow.GetRemainingActionsCount();
		brassApplicationGame.Game.Board.IronMarket.GetSellingPrice();
		brassApplicationGame.Game.Board.CoalMarket.GetSellingPrice();
		if (bestRank >= 500 && currentPlayer.Money >= 26 && currentPlayer.IncomeLevel >= 3 && brassApplicationGame.Game.GameProgressInformation.CurrentGameEra == GameEra.CanalEra)
		{
			return 2600;
		}
		return 2000;
	}

	public override bool IsApplicable(BrassApplicationGame brassApplicationGame)
	{
		int bestRank;
		return BestSlot(brassApplicationGame, out bestRank) != null;
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
			BaseRule.Logger.Info(string.Concat(BaseRule.LOG_PREFIX, GetPriority(brassApplicationGame), ": ", base.Id.ToString(), BaseRule.LOG_NEWLINE, "Build ", item2, " in ", regionContainingGivenSlot.Name, " (", item, ") with card: ", baseCard));
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
		bestRank = -1000;
		IEnumerable<BaseIndustry> industriesOnBoardOfGivenPlayer = board.GetIndustriesOnBoardOfGivenPlayer(currentPlayer.Color);
		Dictionary<Region, int> dictionary = BoardHelper.MissingConnectionsCount(brassApplicationGame, BoardHelper.RegionsWithMerchants(brassApplicationGame, BuildingType.CottonMill, mustHaveBeer: true), includeNetworks: true);
		Dictionary<Region, int> dictionary2 = BoardHelper.MissingConnectionsCount(brassApplicationGame, BoardHelper.RegionsWithMerchants(brassApplicationGame, BuildingType.Manufacturer, mustHaveBeer: true), includeNetworks: true);
		Dictionary<Region, int> dictionary3 = BoardHelper.MissingConnectionsCount(brassApplicationGame, BoardHelper.RegionsWithMerchants(brassApplicationGame, BuildingType.Pottery, mustHaveBeer: true), includeNetworks: true);
		if (dictionary.Count == 0 && dictionary2.Count == 0 && dictionary3.Count == 0)
		{
			return null;
		}
		Dictionary<BuildingType, Dictionary<Region, int>> dictionary4 = new Dictionary<BuildingType, Dictionary<Region, int>>();
		dictionary4.Add(BuildingType.CottonMill, dictionary);
		dictionary4.Add(BuildingType.Manufacturer, dictionary2);
		dictionary4.Add(BuildingType.Pottery, dictionary3);
		BuildAction buildAction = brassApplicationGame.Game.TurnFlow.StartBuildAction();
		Tuple<BuildingSlot, BuildingType> result = null;
		foreach (BuildingSlot item in BoardHelper.PossibleBuildingSlotsCottonManufacturePottery)
		{
			foreach (BuildingType key in dictionary4.Keys)
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
					if (!(item2 is BorderRegion))
					{
						continue;
					}
					foreach (MerchantSlot merchantSlot in ((BorderRegion)item2).MerchantSlots)
					{
						if (merchantSlot.AvailableBeer > 0 && merchantSlot.MerchantTile.BuildingTypes.Contains(key))
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
				num = (dictionary4[key].ContainsKey(regionContainingGivenSlot) ? (num - 100 * dictionary4[key][regionContainingGivenSlot]) : (num - 10000));
				foreach (BaseIndustry item3 in industriesOnBoardOfGivenPlayer)
				{
					if (item3.BuildingType == key && !item3.IsFlipped)
					{
						num -= 100;
					}
				}
				if (num > bestRank)
				{
					bestRank = num;
					result = new Tuple<BuildingSlot, BuildingType>(item, key);
				}
			}
		}
		if (bestRank > -150)
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
