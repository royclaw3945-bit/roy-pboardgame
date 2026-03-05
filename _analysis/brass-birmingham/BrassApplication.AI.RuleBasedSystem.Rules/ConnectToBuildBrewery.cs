using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Links;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.Game;
using BoardGameRules.Entities.GamePhases;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.Players;
using BrassApplication.AI.AIHelpers;
using BrassApplication.Game;

namespace BrassApplication.AI.RuleBasedSystem.Rules;

internal class ConnectToBuildBrewery : BaseRule
{
	public ConnectToBuildBrewery()
	{
		base.Id = RuleId.ConnectToBuildBrewery;
		base.AILevel = AIType.Hard;
	}

	public override int GetPriority(BrassApplicationGame brassApplicationGame)
	{
		return 2110;
	}

	public override bool IsApplicable(BrassApplicationGame brassApplicationGame)
	{
		Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
		BaseIndustry firstBuildingOfType = currentPlayer.Mat.GetFirstBuildingOfType(BuildingType.Brewery);
		if (firstBuildingOfType == null || currentPlayer.Money <= firstBuildingOfType.MoneyCost)
		{
			return false;
		}
		if (brassApplicationGame.Game.GameProgressInformation.CurrentGameEra == GameEra.RailEra && firstBuildingOfType != null && firstBuildingOfType.IsForCanalEra && !firstBuildingOfType.IsForRailEra)
		{
			return false;
		}
		bool flag = false;
		foreach (BaseCard item in currentPlayer.Hand)
		{
			if (!(item is IndustryCard))
			{
				continue;
			}
			foreach (BuildingType buildingType in ((IndustryCard)item).BuildingTypes)
			{
				if (buildingType == BuildingType.Brewery)
				{
					flag = true;
				}
			}
		}
		if (flag && HasIndustriesToSellButLackingBeer(brassApplicationGame))
		{
			return ApplicableLinks(brassApplicationGame).Count > 0;
		}
		return false;
	}

	public override Action MakeAction(BrassApplicationGame brassApplicationGame, bool printOnly)
	{
		return delegate
		{
			_ = brassApplicationGame.Game.Board;
			Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
			NetworkAction networkAction = brassApplicationGame.Game.TurnFlow.StartNetworkAction();
			Link link = ApplicableLinks(brassApplicationGame)[0];
			BaseCard baseCard = BoardHelper.LowestValueCard(currentPlayer.Hand, brassApplicationGame, BuildingType.Brewery);
			BaseRule.Logger.Info(string.Concat(BaseRule.LOG_PREFIX, GetPriority(brassApplicationGame), ": ", base.Id.ToString(), BaseRule.LOG_NEWLINE, "Place link: ", link, " with card: ", baseCard));
			if (!printOnly)
			{
				NetworkActionAIHelper.PlaceLink(networkAction, baseCard, link, brassApplicationGame.Game.Board, currentPlayer);
			}
		};
	}

	private List<Link> ApplicableLinks(BrassApplicationGame brassApplicationGame)
	{
		List<Link> list = new List<Link>();
		HashSet<Region> hashSet = new HashSet<Region>();
		Board board = brassApplicationGame.Game.Board;
		GameProgressInformation gameProgressInformation = brassApplicationGame.Game.GameProgressInformation;
		Player player = gameProgressInformation.CurrentPlayer;
		foreach (Region region in board.Regions)
		{
			bool flag = false;
			foreach (BuildingSlot buildingSlot in region.BuildingSlots)
			{
				if (buildingSlot.BuildedIndustry != null)
				{
					continue;
				}
				foreach (BuildingType buildingType in buildingSlot.BuildingTypes)
				{
					if (buildingType == BuildingType.Brewery)
					{
						flag = true;
					}
				}
			}
			if (flag && !board.IsRegionPartOfGivenPlayerNetworkConnection(region, player.Color))
			{
				hashSet.Add(region);
			}
		}
		NetworkAction networkAction = brassApplicationGame.Game.TurnFlow.NetworkAction;
		foreach (Link item in new HashSet<Link>(BoardHelper.possibleLinks.Where((Link l) => networkAction.GetConnectionCost(l) <= player.Money)))
		{
			if (hashSet.Contains(item.Region1) || hashSet.Contains(item.Region2) || hashSet.Contains(item.Region3))
			{
				list.Add(item);
			}
		}
		return list;
	}

	private bool HasIndustriesToSellButLackingBeer(BrassApplicationGame brassApplicationGame)
	{
		Board board = brassApplicationGame.Game.Board;
		Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
		List<BuildingSlot> list = new List<BuildingSlot>();
		foreach (BuildingSlot item in board.Regions.SelectMany((Region r) => r.BuildingSlots.Where((BuildingSlot s) => s.BuildedIndustry is BaseSellableIndustry)))
		{
			if (item.BuildedIndustry.Color == currentPlayer.Color && !item.BuildedIndustry.IsFlipped && BoardHelper.CanIndustryBeSoldButLackingBeer(brassApplicationGame, item.BuildedIndustry, currentPlayer, board))
			{
				list.Add(item);
			}
		}
		return list.Count > 0;
	}

	public override void PrintAction(BrassApplicationGame brassApplicationGame)
	{
		MakeAction(brassApplicationGame, printOnly: true)();
	}
}
