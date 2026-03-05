using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Actions;
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

internal class ConnectIronForHighDemand : BaseRule
{
	public ConnectIronForHighDemand()
	{
		base.Id = RuleId.ConnectIronForHighDemand;
		base.AILevel = AIType.Medium;
	}

	public override int GetPriority(BrassApplicationGame brassApplicationGame)
	{
		BaseIndustry firstBuildingOfType = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer.Mat.GetFirstBuildingOfType(BuildingType.IronWorks);
		int sellingPrice = brassApplicationGame.Game.Board.IronMarket.GetSellingPrice();
		if (firstBuildingOfType == null)
		{
			return -1220;
		}
		if (sellingPrice >= 3)
		{
			return 1900;
		}
		if (sellingPrice >= 2 && firstBuildingOfType.IsForCanalEra && !firstBuildingOfType.IsForRailEra && brassApplicationGame.Game.GameProgressInformation.CurrentGameEra == GameEra.CanalEra)
		{
			return 1900;
		}
		return 1220;
	}

	public Link ApplicableLink(BrassApplicationGame brassApplicationGame)
	{
		BuildAction buildAction = brassApplicationGame.Game.TurnFlow.BuildAction;
		NetworkAction networkAction = brassApplicationGame.Game.TurnFlow.NetworkAction;
		Player player = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
		BaseIndustry ironWorks = player.Mat.GetFirstBuildingOfType(BuildingType.IronWorks);
		List<BuildingSlot> list = brassApplicationGame.Game.Board.Regions.SelectMany((Region r) => r.BuildingSlots.Where((BuildingSlot s) => s.BuildedIndustry == null && s.BuildingTypes.Contains(BuildingType.IronWorks))).Where(delegate(BuildingSlot s)
		{
			brassApplicationGame.Game.Board.GetRegionContainingGivenSlot(s);
			List<CantUseBuildActionReason> cantUseBuildActionReasonForGivenSlot = buildAction.GetCantUseBuildActionReasonForGivenSlot(s, ironWorks, player, brassApplicationGame.Game.Board, brassApplicationGame.Game.GameProgressInformation);
			return cantUseBuildActionReasonForGivenSlot.Contains(CantUseBuildActionReason.NO_COAL) && !cantUseBuildActionReasonForGivenSlot.Contains(CantUseBuildActionReason.NO_VALID_CARD_TO_BUILD);
		}).ToList();
		if (!list.Any())
		{
			return null;
		}
		HashSet<Region> hashSet = new HashSet<Region>();
		foreach (BuildingSlot item in list)
		{
			Region regionContainingGivenSlot = brassApplicationGame.Game.Board.GetRegionContainingGivenSlot(item);
			if (!hashSet.Contains(regionContainingGivenSlot))
			{
				hashSet.Add(regionContainingGivenSlot);
			}
		}
		HashSet<Link> hashSet2 = new HashSet<Link>(hashSet.SelectMany((Region r) => r.Links.Where((Link l) => !l.HasOwner)));
		if (networkAction.GetNetworkActionNotPossibleReasons(brassApplicationGame.Game.Board, brassApplicationGame.Game.GameProgressInformation).Any())
		{
			return null;
		}
		HashSet<Link> other = new HashSet<Link>(BoardHelper.possibleLinks.Where((Link l) => networkAction.GetConnectionCost(l) <= player.Money));
		hashSet2.IntersectWith(other);
		HashSet<Region> set = BoardHelper.RegionsWithCoalAccess(brassApplicationGame.Game.Board);
		HashSet<Link> hashSet3 = new HashSet<Link>();
		foreach (Link item2 in hashSet2)
		{
			if (BoardHelper.ConnectSets(item2, hashSet, set))
			{
				hashSet3.Add(item2);
			}
		}
		if (hashSet3.Count == 0)
		{
			return null;
		}
		return hashSet3.First();
	}

	public override bool IsApplicable(BrassApplicationGame brassApplicationGame)
	{
		Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
		int remainingActionsCount = brassApplicationGame.Game.TurnFlow.GetRemainingActionsCount();
		Link link = ApplicableLink(brassApplicationGame);
		int num = int.MaxValue;
		if (link != null)
		{
			num = brassApplicationGame.Game.TurnFlow.StartNetworkAction().GetConnectionCost(link) + currentPlayer.Mat.GetFirstBuildingOfType(BuildingType.IronWorks).MoneyCost;
		}
		if (remainingActionsCount == 2 && link != null)
		{
			return num <= currentPlayer.Money;
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
			BaseCard baseCard = BoardHelper.LowestValueCard(currentPlayer.Hand, brassApplicationGame, BuildingType.IronWorks);
			Link link = ApplicableLink(brassApplicationGame);
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
