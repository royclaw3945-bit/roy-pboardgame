using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Links;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.Game;
using BoardGameRules.Entities.GamePhases;
using BoardGameRules.Entities.Players;
using BrassApplication.AI.AIHelpers;
using BrassApplication.Game;

namespace BrassApplication.AI.RuleBasedSystem.Rules;

internal class PlaceLinkDouble : BaseRule
{
	public PlaceLinkDouble()
	{
		base.Id = RuleId.PlaceLinkDouble;
		base.AILevel = AIType.Hard;
	}

	public override int GetPriority(BrassApplicationGame brassApplicationGame)
	{
		return 1420;
	}

	public override bool IsApplicable(BrassApplicationGame brassApplicationGame)
	{
		Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
		if ((from s in brassApplicationGame.Game.Board.Regions.SelectMany((Region r) => r.BuildingSlots)
			where s.BuildedIndustry != null
			select s.BuildedIndustry).Count() > 0 && currentPlayer.Money >= 29 && currentPlayer.IncomeLevel >= 11 && brassApplicationGame.Game.GameProgressInformation.CurrentGameEra == GameEra.RailEra && brassApplicationGame.Game.TurnFlow.GetRemainingActionsCount() == 1 && brassApplicationGame.Game.GameProgressInformation.CurrentRoundNumber >= 4)
		{
			return PossibleLinks(brassApplicationGame).Count > 0;
		}
		return false;
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

	public override Action MakeAction(BrassApplicationGame brassApplicationGame, bool printOnly)
	{
		return delegate
		{
			Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
			NetworkAction networkAction = brassApplicationGame.Game.TurnFlow.StartNetworkAction();
			IDictionary<Link, int> dictionary = new Dictionary<Link, int>();
			foreach (Link item in PossibleLinks(brassApplicationGame))
			{
				int num = 0;
				num += item.Region1.BuildingSlots.Where((BuildingSlot s) => s.BuildedIndustry != null).Sum((BuildingSlot s) => s.BuildedIndustry.VPPerLink);
				if (item.Region1 is BorderRegion)
				{
					num += 2;
				}
				num += item.Region2.BuildingSlots.Where((BuildingSlot s) => s.BuildedIndustry != null).Sum((BuildingSlot s) => s.BuildedIndustry.VPPerLink);
				if (item.Region2 is BorderRegion)
				{
					num += 2;
				}
				if (item.Region3 != null)
				{
					num += item.Region3.BuildingSlots.Where((BuildingSlot s) => s.BuildedIndustry != null).Sum((BuildingSlot s) => s.BuildedIndustry.VPPerLink);
					if (item.Region3 is BorderRegion)
					{
						num += 2;
					}
				}
				dictionary.Add(item, num);
			}
			foreach (Link item2 in dictionary.Keys.ToList())
			{
				bool flag = true;
				bool flag2 = true;
				bool flag3 = true;
				foreach (BuildingSlot buildingSlot in item2.Region1.BuildingSlots)
				{
					if (buildingSlot.BuildedIndustry != null)
					{
						flag = false;
					}
				}
				foreach (BuildingSlot buildingSlot2 in item2.Region2.BuildingSlots)
				{
					if (buildingSlot2.BuildedIndustry != null)
					{
						flag2 = false;
					}
				}
				if (item2.Region3 != null)
				{
					foreach (BuildingSlot buildingSlot3 in item2.Region3.BuildingSlots)
					{
						if (buildingSlot3.BuildedIndustry != null)
						{
							flag3 = false;
						}
					}
				}
				else
				{
					flag3 = false;
				}
				if (flag && flag2 && flag3)
				{
					dictionary[item2] += 100;
				}
			}
			Link key = dictionary.OrderByDescending((KeyValuePair<Link, int> v) => v.Value).First().Key;
			BaseCard baseCard = BoardHelper.LowestValueCard(currentPlayer.Hand, brassApplicationGame);
			BaseRule.Logger.Info(string.Concat(BaseRule.LOG_PREFIX, GetPriority(brassApplicationGame), ": ", base.Id.ToString(), BaseRule.LOG_NEWLINE, "Place link: ", key, " with card: ", baseCard));
			if (!printOnly)
			{
				NetworkActionAIHelper.PlaceLink(networkAction, baseCard, key, brassApplicationGame.Game.Board, currentPlayer);
			}
		};
	}

	public override void PrintAction(BrassApplicationGame brassApplicationGame)
	{
		MakeAction(brassApplicationGame, printOnly: true)();
	}
}
