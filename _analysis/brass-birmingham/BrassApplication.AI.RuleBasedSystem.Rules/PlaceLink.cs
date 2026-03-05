using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Links;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.Game;
using BoardGameRules.Entities.Players;
using BrassApplication.AI.AIHelpers;
using BrassApplication.Game;

namespace BrassApplication.AI.RuleBasedSystem.Rules;

internal class PlaceLink : BaseRule
{
	public PlaceLink()
	{
		base.Id = RuleId.PlaceLink;
		base.AILevel = AIType.Easy;
	}

	public override int GetPriority(BrassApplicationGame brassApplicationGame)
	{
		return 500;
	}

	public override bool IsApplicable(BrassApplicationGame brassApplicationGame)
	{
		return PossibleLinks(brassApplicationGame).Count > 0;
	}

	public List<Link> PossibleLinks(BrassApplicationGame brassApplicationGame)
	{
		GameProgressInformation gameProgressInformation = brassApplicationGame.Game.GameProgressInformation;
		Player player = gameProgressInformation.CurrentPlayer;
		NetworkAction networkAction = brassApplicationGame.Game.TurnFlow.NetworkAction;
		if (!player.HasLinkTiles())
		{
			return new List<Link>();
		}
		return (from l in networkAction.GetValidConnectionSlots(brassApplicationGame.Game.Board, brassApplicationGame.Game.GameProgressInformation)
			where networkAction.GetConnectionCost(l) <= player.Money
			select l).ToList();
	}

	public static Link FindBestLink(List<Link> links)
	{
		IDictionary<Link, int> dictionary = new Dictionary<Link, int>();
		foreach (Link link in links)
		{
			int num = 0;
			num += link.Region1.BuildingSlots.Where((BuildingSlot s) => s.BuildedIndustry != null).Sum((BuildingSlot s) => s.BuildedIndustry.VPPerLink);
			if (link.Region1 is BorderRegion)
			{
				num += 2;
			}
			num += link.Region2.BuildingSlots.Where((BuildingSlot s) => s.BuildedIndustry != null).Sum((BuildingSlot s) => s.BuildedIndustry.VPPerLink);
			if (link.Region2 is BorderRegion)
			{
				num += 2;
			}
			if (link.Region3 != null)
			{
				num += link.Region3.BuildingSlots.Where((BuildingSlot s) => s.BuildedIndustry != null).Sum((BuildingSlot s) => s.BuildedIndustry.VPPerLink);
				if (link.Region3 is BorderRegion)
				{
					num += 2;
				}
			}
			dictionary.Add(link, num);
		}
		return dictionary.OrderByDescending((KeyValuePair<Link, int> v) => v.Value).First().Key;
	}

	public override Action MakeAction(BrassApplicationGame brassApplicationGame, bool printOnly)
	{
		return delegate
		{
			Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
			NetworkAction networkAction = brassApplicationGame.Game.TurnFlow.StartNetworkAction();
			Link link = FindBestLink(PossibleLinks(brassApplicationGame));
			BaseCard baseCard = BoardHelper.LowestValueCard(currentPlayer.Hand, brassApplicationGame);
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
