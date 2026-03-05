using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Links;
using BoardGameRules.Entities.Board.Merchants;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.Game;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.Players;
using BrassApplication.AI.AIHelpers;
using BrassApplication.Game;

namespace BrassApplication.AI.RuleBasedSystem.Rules;

internal class ConnectIndustryForMerchantBeer : BaseRule
{
	public ConnectIndustryForMerchantBeer()
	{
		base.Id = RuleId.ConnectIndustryForMerchantBeer;
		base.AILevel = AIType.Hard;
	}

	public override int GetPriority(BrassApplicationGame brassApplicationGame)
	{
		return 2100;
	}

	public override bool IsApplicable(BrassApplicationGame brassApplicationGame)
	{
		if (!brassApplicationGame.Game.GameProgressInformation.CurrentPlayer.HasLinkTiles())
		{
			return false;
		}
		if (brassApplicationGame.Game.TurnFlow.GetRemainingActionsCount() < 2 && brassApplicationGame.Game.GameProgressInformation.CurrentRoundNumber == 8)
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
			Link link = ApplicableLinks(brassApplicationGame)[0];
			BaseCard baseCard = BoardHelper.LowestValueCard(currentPlayer.Hand, brassApplicationGame);
			BaseRule.Logger.Info(string.Concat(BaseRule.LOG_PREFIX, GetPriority(brassApplicationGame), ": ", base.Id.ToString(), BaseRule.LOG_NEWLINE, "Place link: ", link, " with card: ", baseCard));
			if (!printOnly)
			{
				NetworkActionAIHelper.PlaceLink(networkAction, baseCard, link, brassApplicationGame.Game.Board, currentPlayer);
			}
		};
	}

	private List<Link> ApplicableLinks(BrassApplicationGame brassApplicationGame)
	{
		Board board = brassApplicationGame.Game.Board;
		GameProgressInformation gameProgressInformation = brassApplicationGame.Game.GameProgressInformation;
		Player player = gameProgressInformation.CurrentPlayer;
		NetworkAction networkAction = brassApplicationGame.Game.TurnFlow.NetworkAction;
		HashSet<Link> hashSet = new HashSet<Link>(BoardHelper.possibleLinks.Where((Link l) => networkAction.GetConnectionCost(l) <= player.Money));
		Dictionary<BuildingType, HashSet<Region>> dictionary = new Dictionary<BuildingType, HashSet<Region>>();
		Dictionary<BuildingType, HashSet<Region>> dictionary2 = new Dictionary<BuildingType, HashSet<Region>>();
		dictionary.Add(BuildingType.CottonMill, new HashSet<Region>());
		dictionary.Add(BuildingType.Manufacturer, new HashSet<Region>());
		dictionary.Add(BuildingType.Pottery, new HashSet<Region>());
		dictionary2.Add(BuildingType.CottonMill, new HashSet<Region>());
		dictionary2.Add(BuildingType.Manufacturer, new HashSet<Region>());
		dictionary2.Add(BuildingType.Pottery, new HashSet<Region>());
		foreach (BaseIndustry item in board.GetIndustriesOnBoardOfGivenPlayer(player.Color))
		{
			if (item.IsFlipped || (item.BuildingType != BuildingType.CottonMill && item.BuildingType != BuildingType.Manufacturer && item.BuildingType != BuildingType.Pottery))
			{
				continue;
			}
			HashSet<Region> hashSet2 = BoardHelper.ConnectedRegions(brassApplicationGame, board.GetRegionContainingGivenIndustry(item));
			bool flag = false;
			foreach (Region item2 in hashSet2)
			{
				if (!(item2 is BorderRegion borderRegion))
				{
					continue;
				}
				foreach (MerchantSlot merchantSlot in borderRegion.MerchantSlots)
				{
					if (merchantSlot.MerchantTile == null)
					{
						continue;
					}
					foreach (BuildingType buildingType in merchantSlot.MerchantTile.BuildingTypes)
					{
						if (buildingType == item.BuildingType)
						{
							flag = true;
						}
					}
				}
			}
			if (flag)
			{
				continue;
			}
			foreach (Region item3 in hashSet2)
			{
				if (!dictionary[item.BuildingType].Contains(item3))
				{
					dictionary[item.BuildingType].Add(item3);
				}
			}
		}
		foreach (BorderRegion borderRegion2 in board.GetBorderRegions((BorderRegion t) => true))
		{
			HashSet<Region> hashSet3 = BoardHelper.ConnectedRegions(brassApplicationGame, borderRegion2);
			int num = 0;
			foreach (MerchantSlot merchantSlot2 in borderRegion2.MerchantSlots)
			{
				if (merchantSlot2.HasBeer)
				{
					num++;
				}
			}
			if (num <= 0)
			{
				continue;
			}
			foreach (MerchantSlot merchantSlot3 in borderRegion2.MerchantSlots)
			{
				if (merchantSlot3.MerchantTile == null)
				{
					continue;
				}
				foreach (BuildingType buildingType2 in merchantSlot3.MerchantTile.BuildingTypes)
				{
					foreach (Region item4 in hashSet3)
					{
						if (!dictionary2[buildingType2].Contains(item4))
						{
							dictionary2[buildingType2].Add(item4);
						}
					}
				}
			}
		}
		HashSet<Link> hashSet4 = new HashSet<Link>();
		foreach (Link item5 in hashSet)
		{
			if (BoardHelper.ConnectSets(item5, dictionary2[BuildingType.CottonMill], dictionary[BuildingType.CottonMill]) || BoardHelper.ConnectSets(item5, dictionary2[BuildingType.Manufacturer], dictionary[BuildingType.Manufacturer]) || BoardHelper.ConnectSets(item5, dictionary2[BuildingType.Pottery], dictionary[BuildingType.Pottery]))
			{
				hashSet4.Add(item5);
			}
		}
		return hashSet4.ToList();
	}

	public override void PrintAction(BrassApplicationGame brassApplicationGame)
	{
		MakeAction(brassApplicationGame, printOnly: true)();
	}
}
