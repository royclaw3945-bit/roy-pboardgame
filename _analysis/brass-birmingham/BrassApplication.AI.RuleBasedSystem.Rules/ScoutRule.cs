using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.GamePhases;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.Players;
using BrassApplication.AI.AIHelpers;
using BrassApplication.Game;

namespace BrassApplication.AI.RuleBasedSystem.Rules;

internal class ScoutRule : BaseRule
{
	public ScoutRule()
	{
		base.Id = RuleId.ScoutRule;
		base.AILevel = AIType.Easy;
	}

	public override int GetPriority(BrassApplicationGame brassApplicationGame)
	{
		Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
		Board board = brassApplicationGame.Game.Board;
		if (brassApplicationGame.Game.TurnFlow.GetRemainingActionsCount() >= 2)
		{
			int sellingPrice = brassApplicationGame.Game.Board.CoalMarket.GetSellingPrice();
			if (sellingPrice >= 7 && BuildCoalForHighDemand.ApplicableSlot(brassApplicationGame) == null && brassApplicationGame.Game.GameProgressInformation.CurrentGameEra == GameEra.RailEra)
			{
				HashSet<Region> hashSet = new HashSet<Region>();
				foreach (BorderRegion borderRegion in board.GetBorderRegions((BorderRegion t) => true))
				{
					HashSet<Region> other = BoardHelper.ConnectedRegions(brassApplicationGame, borderRegion);
					hashSet.UnionWith(other);
				}
				foreach (Region item in hashSet)
				{
					foreach (BuildingSlot buildingSlot in item.BuildingSlots)
					{
						if (buildingSlot.BuildedIndustry == null && buildingSlot.BuildingTypes.Contains(BuildingType.CoalMine) && !BoardHelper.PossibleBuildingSlots[BuildingType.CoalMine].Contains(buildingSlot))
						{
							return 1810;
						}
					}
				}
			}
			if (brassApplicationGame.Game.GameProgressInformation.CurrentRoundNumber <= 1)
			{
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				Dictionary<RegionName, int> dictionary = new Dictionary<RegionName, int>();
				Dictionary<BuildingType, int> dictionary2 = new Dictionary<BuildingType, int>();
				foreach (BaseCard item2 in currentPlayer.Hand)
				{
					if (item2 is LocationCard)
					{
						num++;
						RegionName region = ((LocationCard)item2).Region;
						if (dictionary.ContainsKey(region))
						{
							dictionary[region]++;
							num3++;
							if (dictionary[region] > num2)
							{
								num2 = dictionary[region];
							}
						}
						else
						{
							dictionary.Add(region, 0);
						}
					}
					else
					{
						if (!(item2 is IndustryCard))
						{
							continue;
						}
						BuildingType key = ((IndustryCard)item2).BuildingTypes[0];
						if (dictionary2.ContainsKey(key))
						{
							dictionary2[key]++;
							num3++;
							if (dictionary2[key] > num2)
							{
								num2 = dictionary2[key];
							}
						}
						else
						{
							dictionary2.Add(key, 0);
						}
					}
				}
				if (num3 >= 3)
				{
					return 1810;
				}
			}
			int sellingPrice2 = brassApplicationGame.Game.Board.IronMarket.GetSellingPrice();
			int num4 = 0;
			foreach (BaseIndustry item3 in brassApplicationGame.Game.Board.GetIndustriesOnBoardOfGivenPlayer(currentPlayer.Color))
			{
				if (item3.BuildingType == BuildingType.IronWorks)
				{
					num4++;
				}
			}
			if (brassApplicationGame.Game.GameProgressInformation.CurrentGameEra == GameEra.CanalEra && sellingPrice2 >= 5 && num4 >= 1 && currentPlayer.Mat.GetFirstBuildingOfType(BuildingType.IronWorks) != null)
			{
				return 1810;
			}
			if (sellingPrice >= 5 && BuildCoalForHighDemand.ApplicableSlot(brassApplicationGame) == null)
			{
				HashSet<Region> hashSet2 = new HashSet<Region>();
				foreach (BorderRegion borderRegion2 in board.GetBorderRegions((BorderRegion t) => true))
				{
					HashSet<Region> other2 = BoardHelper.ConnectedRegions(brassApplicationGame, borderRegion2);
					hashSet2.UnionWith(other2);
				}
				foreach (Region item4 in hashSet2)
				{
					foreach (BuildingSlot buildingSlot2 in item4.BuildingSlots)
					{
						if (buildingSlot2.BuildedIndustry == null && buildingSlot2.BuildingTypes.Contains(BuildingType.CoalMine) && !BoardHelper.PossibleBuildingSlots[BuildingType.CoalMine].Contains(buildingSlot2))
						{
							return 510;
						}
					}
				}
			}
		}
		return 250;
	}

	public override bool IsApplicable(BrassApplicationGame brassApplicationGame)
	{
		return !brassApplicationGame.Game.TurnFlow.ScoutAction.GetScoutActionNotPossibleReasons(brassApplicationGame.Game.Board, brassApplicationGame.Game.GameProgressInformation).Any();
	}

	public override Action MakeAction(BrassApplicationGame brassApplicationGame, bool printOnly)
	{
		return delegate
		{
			Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
			ScoutAction scoutAction = brassApplicationGame.Game.TurnFlow.StartScoutAction();
			HashSet<BaseCard> hashSet = new HashSet<BaseCard>(currentPlayer.Hand);
			BaseCard baseCard = BoardHelper.LowestValueCard(hashSet.ToList(), brassApplicationGame);
			hashSet.Remove(baseCard);
			BaseCard baseCard2 = BoardHelper.LowestValueCard(hashSet.ToList(), brassApplicationGame);
			hashSet.Remove(baseCard2);
			BaseCard baseCard3 = BoardHelper.LowestValueCard(hashSet.ToList(), brassApplicationGame);
			BaseRule.Logger.Info(string.Concat(BaseRule.LOG_PREFIX, GetPriority(brassApplicationGame), ": ", base.Id.ToString(), BaseRule.LOG_NEWLINE, "Scout with card: ", baseCard, "; Other cards: ", baseCard2, ", ", baseCard3));
			if (!printOnly)
			{
				scoutAction.DiscardCard(baseCard, currentPlayer);
				scoutAction.Scout(new List<BaseCard> { baseCard2, baseCard3 }, currentPlayer);
			}
		};
	}

	public override void PrintAction(BrassApplicationGame brassApplicationGame)
	{
		MakeAction(brassApplicationGame, printOnly: true)();
	}
}
