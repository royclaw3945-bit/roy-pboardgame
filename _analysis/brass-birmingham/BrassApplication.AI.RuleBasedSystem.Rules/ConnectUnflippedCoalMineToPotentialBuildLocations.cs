using System;
using System.Collections.Generic;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Links;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.Players;
using BrassApplication.AI.AIHelpers;
using BrassApplication.Game;

namespace BrassApplication.AI.RuleBasedSystem.Rules;

internal class ConnectUnflippedCoalMineToPotentialBuildLocations : BaseRule
{
	public ConnectUnflippedCoalMineToPotentialBuildLocations()
	{
		base.Id = RuleId.ConnectUnflippedCoalMineToPotentialBuildLocations;
		base.AILevel = AIType.Hard;
	}

	public override int GetPriority(BrassApplicationGame brassApplicationGame)
	{
		return -1610;
	}

	public override bool IsApplicable(BrassApplicationGame brassApplicationGame)
	{
		if (!brassApplicationGame.Game.GameProgressInformation.CurrentPlayer.HasLinkTiles())
		{
			return false;
		}
		return ApplicableLinks(brassApplicationGame).Count > 0;
	}

	public override Action MakeAction(BrassApplicationGame brassApplicationGame, bool printOnly)
	{
		return delegate
		{
			_ = brassApplicationGame.Game.Board;
			Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
			NetworkAction networkAction = brassApplicationGame.Game.TurnFlow.StartNetworkAction();
			Dictionary<Link, int> dictionary = ApplicableLinks(brassApplicationGame);
			Link link = null;
			foreach (Link key in dictionary.Keys)
			{
				if (link == null)
				{
					link = key;
				}
				else if (dictionary[key] > dictionary[link])
				{
					link = key;
				}
			}
			BaseCard baseCard = BoardHelper.LowestValueCard(currentPlayer.Hand, brassApplicationGame);
			BaseRule.Logger.Info(string.Concat(BaseRule.LOG_PREFIX, GetPriority(brassApplicationGame), ": ", base.Id.ToString(), BaseRule.LOG_NEWLINE, "Place link: ", link, " with card: ", baseCard));
			if (!printOnly)
			{
				NetworkActionAIHelper.PlaceLink(networkAction, baseCard, link, brassApplicationGame.Game.Board, currentPlayer);
			}
		};
	}

	private Dictionary<Link, int> ApplicableLinks(BrassApplicationGame brassApplicationGame)
	{
		Board board = brassApplicationGame.Game.Board;
		Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
		HashSet<Region> hashSet = new HashSet<Region>();
		Dictionary<Link, int> dictionary = new Dictionary<Link, int>();
		foreach (CoalMine unflippedCoalMine in board.GetUnflippedCoalMines())
		{
			if (unflippedCoalMine.Color != currentPlayer.Color)
			{
				continue;
			}
			Region regionContainingGivenIndustry = board.GetRegionContainingGivenIndustry(unflippedCoalMine);
			if (hashSet.Contains(regionContainingGivenIndustry))
			{
				continue;
			}
			bool flag = false;
			foreach (Link link in regionContainingGivenIndustry.Links)
			{
				if (link.HasOwner)
				{
					flag = true;
				}
			}
			if (flag)
			{
				continue;
			}
			hashSet.Add(regionContainingGivenIndustry);
			foreach (Link link2 in regionContainingGivenIndustry.Links)
			{
				if (!BoardHelper.possibleLinks.Contains(link2) || dictionary.ContainsKey(link2))
				{
					continue;
				}
				List<Region> otherEnd = link2.GetOtherEnd(regionContainingGivenIndustry);
				int num = 0;
				foreach (Region item in otherEnd)
				{
					bool flag2 = false;
					foreach (BuildingSlot buildingSlot in item.BuildingSlots)
					{
						if (buildingSlot.BuildedIndustry != null)
						{
							continue;
						}
						foreach (BuildingType buildingType in buildingSlot.BuildingTypes)
						{
							if (buildingType == BuildingType.IronWorks || buildingType == BuildingType.Manufacturer || buildingType == BuildingType.Pottery)
							{
								flag2 = true;
							}
						}
					}
					if (flag2)
					{
						num++;
					}
				}
				dictionary.Add(link2, num);
			}
		}
		return dictionary;
	}

	public override void PrintAction(BrassApplicationGame brassApplicationGame)
	{
		MakeAction(brassApplicationGame, printOnly: true)();
	}
}
