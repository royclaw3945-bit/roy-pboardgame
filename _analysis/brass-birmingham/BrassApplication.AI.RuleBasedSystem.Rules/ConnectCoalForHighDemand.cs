using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Links;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.GamePhases;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.Players;
using BrassApplication.AI.AIHelpers;
using BrassApplication.Game;

namespace BrassApplication.AI.RuleBasedSystem.Rules;

internal class ConnectCoalForHighDemand : BaseRule
{
	public ConnectCoalForHighDemand()
	{
		base.Id = RuleId.ConnectCoalForHighDemand;
		base.AILevel = AIType.Medium;
	}

	public override int GetPriority(BrassApplicationGame brassApplicationGame)
	{
		BaseIndustry firstBuildingOfType = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer.Mat.GetFirstBuildingOfType(BuildingType.CoalMine);
		int sellingPrice = brassApplicationGame.Game.Board.CoalMarket.GetSellingPrice();
		if (sellingPrice >= 3)
		{
			return 1640;
		}
		if (sellingPrice >= 2 && firstBuildingOfType != null && firstBuildingOfType.IsForCanalEra && !firstBuildingOfType.IsForRailEra && brassApplicationGame.Game.GameProgressInformation.CurrentGameEra == GameEra.CanalEra)
		{
			return 1640;
		}
		return 1400;
	}

	public static Link ApplicableLink(BrassApplicationGame brassApplicationGame, HashSet<Link> possibleLinks)
	{
		NetworkAction networkAction = brassApplicationGame.Game.TurnFlow.NetworkAction;
		Board board = brassApplicationGame.Game.Board;
		Player player = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
		int sellingPrice = brassApplicationGame.Game.Board.CoalMarket.GetSellingPrice();
		if (!player.HasLinkTiles())
		{
			return null;
		}
		if (sellingPrice <= 1 && brassApplicationGame.Game.GameProgressInformation.CurrentGameEra == GameEra.CanalEra)
		{
			return null;
		}
		HashSet<Region> hashSet = new HashSet<Region>();
		foreach (BorderRegion borderRegion in board.GetBorderRegions((BorderRegion t) => true))
		{
			HashSet<Region> other = BoardHelper.ConnectedRegions(brassApplicationGame, borderRegion);
			hashSet.UnionWith(other);
		}
		HashSet<Region> hashSet2 = new HashSet<Region>();
		foreach (BuildingSlot item in BoardHelper.PossibleBuildingSlots[BuildingType.CoalMine])
		{
			Region regionContainingGivenSlot = board.GetRegionContainingGivenSlot(item);
			if (!hashSet.Contains(regionContainingGivenSlot) && !hashSet2.Contains(regionContainingGivenSlot))
			{
				hashSet2.Add(regionContainingGivenSlot);
			}
		}
		possibleLinks = new HashSet<Link>(possibleLinks.Where((Link l) => networkAction.GetConnectionCost(l) <= player.Money));
		foreach (Link possibleLink in possibleLinks)
		{
			if (BoardHelper.ConnectSets(possibleLink, hashSet, hashSet2))
			{
				return possibleLink;
			}
		}
		if (hashSet2.Count == 0)
		{
			bool flag = false;
			foreach (BaseCard item2 in player.Hand)
			{
				if (item2 is IndustryCard && ((IndustryCard)item2).BuildingTypes.Contains(BuildingType.CoalMine))
				{
					flag = true;
				}
			}
			if (flag)
			{
				foreach (Region region in brassApplicationGame.Game.Board.Regions)
				{
					if (hashSet.Contains(region) || hashSet2.Contains(region))
					{
						continue;
					}
					bool flag2 = false;
					if (brassApplicationGame.Game.GameProgressInformation.CurrentGameEra == GameEra.CanalEra)
					{
						foreach (BuildingSlot buildingSlot in region.BuildingSlots)
						{
							if (buildingSlot.BuildedIndustry != null && buildingSlot.BuildedIndustry.Color == player.Color)
							{
								flag2 = true;
							}
						}
					}
					if (flag2)
					{
						continue;
					}
					foreach (BuildingSlot buildingSlot2 in region.BuildingSlots)
					{
						if (buildingSlot2.BuildedIndustry == null && buildingSlot2.CanBuildIndustryType(BuildingType.CoalMine))
						{
							hashSet2.Add(region);
						}
					}
				}
			}
			else
			{
				foreach (BaseCard item3 in player.Hand)
				{
					if (!(item3 is LocationCard))
					{
						continue;
					}
					Region regionByName = brassApplicationGame.Game.Board.GetRegionByName(((LocationCard)item3).Region);
					if (hashSet.Contains(regionByName) || hashSet2.Contains(regionByName))
					{
						continue;
					}
					bool flag3 = false;
					if (brassApplicationGame.Game.GameProgressInformation.CurrentGameEra == GameEra.CanalEra)
					{
						foreach (BuildingSlot buildingSlot3 in regionByName.BuildingSlots)
						{
							if (buildingSlot3.BuildedIndustry != null && buildingSlot3.BuildedIndustry.Color == player.Color)
							{
								flag3 = true;
							}
						}
					}
					if (flag3)
					{
						continue;
					}
					foreach (BuildingSlot buildingSlot4 in regionByName.BuildingSlots)
					{
						if (buildingSlot4.BuildedIndustry == null && buildingSlot4.CanBuildIndustryType(BuildingType.CoalMine))
						{
							hashSet2.Add(regionByName);
						}
					}
				}
			}
			foreach (Link possibleLink2 in possibleLinks)
			{
				if (BoardHelper.ConnectSets(possibleLink2, hashSet, hashSet2))
				{
					return possibleLink2;
				}
			}
		}
		return null;
	}

	public override bool IsApplicable(BrassApplicationGame brassApplicationGame)
	{
		if (brassApplicationGame.Game.TurnFlow.GetRemainingActionsCount() == 2)
		{
			return ApplicableLink(brassApplicationGame, BoardHelper.possibleLinks) != null;
		}
		return false;
	}

	public override Action MakeAction(BrassApplicationGame brassApplicationGame, bool printOnly)
	{
		return delegate
		{
			Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
			_ = brassApplicationGame.Game.TurnFlow.BuildAction;
			NetworkAction networkAction = brassApplicationGame.Game.TurnFlow.StartNetworkAction();
			BaseCard baseCard = BoardHelper.LowestValueCard(currentPlayer.Hand, brassApplicationGame, BuildingType.CoalMine);
			Link link = ApplicableLink(brassApplicationGame, BoardHelper.possibleLinks);
			BaseRule.Logger.Info(string.Concat(BaseRule.LOG_PREFIX, GetPriority(brassApplicationGame), ": ", base.Id.ToString(), BaseRule.LOG_NEWLINE, "Place link: ", link, " with card: ", baseCard));
			if (!printOnly)
			{
				NetworkActionAIHelper.PlaceLink(networkAction, baseCard, link, brassApplicationGame.Game.Board, currentPlayer);
			}
		};
	}

	public override void PrintAction(BrassApplicationGame brassApplicationGame)
	{
		MakeAction(brassApplicationGame, printOnly: true)();
	}
}
