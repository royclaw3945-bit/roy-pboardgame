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
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.Players;
using BrassApplication.AI.AIHelpers;
using BrassApplication.Game;

namespace BrassApplication.AI.RuleBasedSystem.Rules;

internal class ConnectCoalToManufactureForMerchantBeer : BaseRule
{
	public ConnectCoalToManufactureForMerchantBeer()
	{
		base.Id = RuleId.ConnectCoalToManufactureForMerchantBeer;
		base.AILevel = AIType.Hard;
	}

	public override int GetPriority(BrassApplicationGame brassApplicationGame)
	{
		return 2200;
	}

	public override bool IsApplicable(BrassApplicationGame brassApplicationGame)
	{
		Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
		currentPlayer.Mat.GetFirstBuildingOfType(BuildingType.Manufacturer);
		if (!currentPlayer.HasLinkTiles())
		{
			return false;
		}
		if (brassApplicationGame.Game.TurnFlow.SellAction.GetValidBuildingSlotsForSellingIndustries(brassApplicationGame.Game.Board, currentPlayer).Count >= 2)
		{
			return false;
		}
		if (brassApplicationGame.Game.TurnFlow.GetRemainingActionsCount() == 2)
		{
			return ApplicableCoalLinksNearManufactureForBeer(brassApplicationGame).Count > 0;
		}
		return false;
	}

	public override Action MakeAction(BrassApplicationGame brassApplicationGame, bool printOnly)
	{
		return delegate
		{
			Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
			NetworkAction networkAction = brassApplicationGame.Game.TurnFlow.StartNetworkAction();
			Link link = ApplicableCoalLinksNearManufactureForBeer(brassApplicationGame)[0];
			BaseCard baseCard = BoardHelper.LowestValueCard(currentPlayer.Hand, brassApplicationGame, BuildingType.Manufacturer, new Region[3] { link.Region1, link.Region2, link.Region3 });
			BaseRule.Logger.Info(string.Concat(BaseRule.LOG_PREFIX, GetPriority(brassApplicationGame), ": ", base.Id.ToString(), BaseRule.LOG_NEWLINE, "Place link: ", link, " with card: ", baseCard));
			if (!printOnly)
			{
				NetworkActionAIHelper.PlaceLink(networkAction, baseCard, link, brassApplicationGame.Game.Board, currentPlayer);
			}
		};
	}

	private List<Link> ApplicableCoalLinksNearManufactureForBeer(BrassApplicationGame brassApplicationGame)
	{
		Board board = brassApplicationGame.Game.Board;
		_ = brassApplicationGame.Game.GameProgressInformation;
		IEnumerable<MerchantSlot> merchantsWithBeer = board.GetMerchantsWithBeer();
		List<MerchantSlot> list = new List<MerchantSlot>();
		foreach (MerchantSlot item in merchantsWithBeer)
		{
			if (item.MerchantTile.BuildingTypes.Contains(BuildingType.Manufacturer))
			{
				list.Add(item);
			}
		}
		HashSet<Region> hashSet = new HashSet<Region>();
		HashSet<Region> hashSet2 = new HashSet<Region>();
		foreach (MerchantSlot item2 in list)
		{
			Region regionOfGivenMerchantTile = board.GetRegionOfGivenMerchantTile(item2);
			foreach (Region regionNeighbour in BoardHelper.GetRegionNeighbours(brassApplicationGame, regionOfGivenMerchantTile))
			{
				if (!hashSet.Contains(regionNeighbour))
				{
					hashSet.Add(regionNeighbour);
				}
			}
		}
		HashSet<Region> hashSet3 = new HashSet<Region>();
		HashSet<Region> hashSet4 = new HashSet<Region>();
		foreach (Region item3 in hashSet)
		{
			bool flag = false;
			foreach (BuildingSlot buildingSlot in item3.BuildingSlots)
			{
				if (buildingSlot.BuildedIndustry == null && buildingSlot.CanBuildIndustryType(BuildingType.Manufacturer))
				{
					flag = true;
				}
			}
			if (flag)
			{
				hashSet3.Add(item3);
			}
		}
		foreach (Region item4 in hashSet)
		{
			foreach (Region regionNeighbour2 in BoardHelper.GetRegionNeighbours(brassApplicationGame, item4))
			{
				if (!hashSet.Contains(regionNeighbour2) && !hashSet2.Contains(regionNeighbour2))
				{
					hashSet2.Add(regionNeighbour2);
				}
			}
		}
		foreach (Region item5 in hashSet2)
		{
			bool flag2 = false;
			foreach (BuildingSlot buildingSlot2 in item5.BuildingSlots)
			{
				if (buildingSlot2.BuildedIndustry == null && buildingSlot2.CanBuildIndustryType(BuildingType.Manufacturer))
				{
					flag2 = true;
				}
			}
			if (flag2)
			{
				hashSet4.Add(item5);
			}
		}
		HashSet<CoalMine> source = new HashSet<CoalMine>(board.GetUnflippedCoalMines());
		HashSet<Link> hashSet5 = new HashSet<Link>();
		foreach (Region item6 in hashSet3)
		{
			foreach (Link link in item6.Links)
			{
				if (!BoardHelper.possibleLinks.Contains(link) || hashSet5.Contains(link))
				{
					continue;
				}
				int num = 0;
				foreach (Region item7 in link.GetOtherEnd(item6))
				{
					foreach (BuildingSlot buildingSlot3 in item7.BuildingSlots)
					{
						if (buildingSlot3.BuildedIndustry != null && source.Contains(buildingSlot3.BuildedIndustry))
						{
							num += ((CoalMine)buildingSlot3.BuildedIndustry).AvailableCoal;
						}
					}
				}
				if (num >= 1)
				{
					hashSet5.Add(link);
				}
			}
		}
		foreach (Region item8 in hashSet4)
		{
			foreach (Link link2 in item8.Links)
			{
				if (!BoardHelper.possibleLinks.Contains(link2) || hashSet5.Contains(link2))
				{
					continue;
				}
				int num2 = 0;
				foreach (Region item9 in link2.GetOtherEnd(item8))
				{
					foreach (BuildingSlot buildingSlot4 in item9.BuildingSlots)
					{
						if (buildingSlot4.BuildedIndustry != null && source.Contains(buildingSlot4.BuildedIndustry))
						{
							num2 += ((CoalMine)buildingSlot4.BuildedIndustry).AvailableCoal;
						}
					}
				}
				if (num2 >= 1)
				{
					hashSet5.Add(link2);
				}
			}
		}
		return hashSet5.ToList();
	}

	public override void PrintAction(BrassApplicationGame brassApplicationGame)
	{
		MakeAction(brassApplicationGame, printOnly: true)();
	}
}
