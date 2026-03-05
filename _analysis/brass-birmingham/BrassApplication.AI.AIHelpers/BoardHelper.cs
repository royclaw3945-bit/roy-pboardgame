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
using BoardGameRules.Entities.GamePhases;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.Players;
using BrassApplication.AI.RuleBasedSystem.Rules;
using BrassApplication.Game;

namespace BrassApplication.AI.AIHelpers;

public static class BoardHelper
{
	public static HashSet<Link> possibleLinks = null;

	private static Dictionary<BuildingType, HashSet<BuildingSlot>> possibleBuildingSlots = null;

	private static HashSet<BuildingSlot> possibleBuildingSlotsCottonManufacturePottery = null;

	private static BrassApplicationGame brassApplicationGame = null;

	private static AIType aiDifficulty = AIType.Hard;

	public static Dictionary<BuildingType, HashSet<BuildingSlot>> PossibleBuildingSlots
	{
		get
		{
			if (possibleBuildingSlots == null)
			{
				BuildAction buildAction = brassApplicationGame.Game.TurnFlow.BuildAction;
				Board board = brassApplicationGame.Game.Board;
				GameProgressInformation gameProgressInformation = brassApplicationGame.Game.GameProgressInformation;
				possibleBuildingSlots = buildAction.GetBuildingSlotsWhereBuildingIsPossible(board, gameProgressInformation);
			}
			return possibleBuildingSlots;
		}
	}

	public static HashSet<BuildingSlot> PossibleBuildingSlotsCottonManufacturePottery
	{
		get
		{
			if (possibleBuildingSlotsCottonManufacturePottery == null)
			{
				possibleBuildingSlotsCottonManufacturePottery = new HashSet<BuildingSlot>(brassApplicationGame.Game.TurnFlow.BuildAction.GetBuildingSlotsWhereBuildingIsPossible(brassApplicationGame.Game.Board, brassApplicationGame.Game.GameProgressInformation, (BuildingType bt) => bt == BuildingType.CottonMill || bt == BuildingType.Manufacturer || bt == BuildingType.Pottery));
			}
			return possibleBuildingSlotsCottonManufacturePottery;
		}
	}

	public static void Init(BrassApplicationGame brassApplicationGame, AIType aiDifficulty)
	{
		BoardHelper.brassApplicationGame = brassApplicationGame;
		BoardHelper.aiDifficulty = aiDifficulty;
		NetworkAction networkAction = brassApplicationGame.Game.TurnFlow.NetworkAction;
		Board board = brassApplicationGame.Game.Board;
		GameProgressInformation gameProgressInformation = brassApplicationGame.Game.GameProgressInformation;
		possibleLinks = new HashSet<Link>(networkAction.GetValidConnectionSlots(board, gameProgressInformation));
		possibleBuildingSlots = null;
		possibleBuildingSlotsCottonManufacturePottery = null;
	}

	public static List<Region> GetRegionNeighbours(BrassApplicationGame brassApplicationGame, Region region)
	{
		List<Region> list = new List<Region>();
		foreach (Link link in region.Links)
		{
			if (!IsLinkApplicableForCurrentEra(brassApplicationGame, link))
			{
				continue;
			}
			foreach (Region item in link.GetOtherEnd(region))
			{
				list.Add(item);
			}
		}
		return list;
	}

	public static bool IsLinkApplicableForCurrentEra(BrassApplicationGame brassApplicationGame, Link link)
	{
		if (brassApplicationGame.Game.GameProgressInformation.CurrentGameEra != GameEra.CanalEra || !(link is Canal))
		{
			if (brassApplicationGame.Game.GameProgressInformation.CurrentGameEra == GameEra.RailEra)
			{
				return link is Rail;
			}
			return false;
		}
		return true;
	}

	public static BaseCard BuildActionBestCard(IEnumerable<BaseCard> cards, BrassApplicationGame brassApplicationGame)
	{
		foreach (BaseCard item in new List<BaseCard>(cards))
		{
			if (item is LocationCard)
			{
				return item;
			}
		}
		return LowestValueCard(cards, brassApplicationGame);
	}

	public static BaseCard LowestValueCard(IEnumerable<BaseCard> cards, BrassApplicationGame brassApplicationGame, BuildingType industryToKeep = BuildingType.Undefined, Region[] regionToKeep = null)
	{
		List<BaseCard> list = new List<BaseCard>(cards);
		_ = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
		if (aiDifficulty == AIType.Easy)
		{
			return list[0];
		}
		Dictionary<RegionName, int> dictionary = new Dictionary<RegionName, int>();
		Dictionary<BuildingType, int> dictionary2 = new Dictionary<BuildingType, int>();
		foreach (BaseCard item in list)
		{
			if (item is LocationCard)
			{
				RegionName region = ((LocationCard)item).Region;
				if (dictionary.ContainsKey(region))
				{
					dictionary[region]++;
				}
				else
				{
					dictionary.Add(region, 0);
				}
			}
			else if (item is IndustryCard)
			{
				BuildingType key = ((IndustryCard)item).BuildingTypes[0];
				if (dictionary2.ContainsKey(key))
				{
					dictionary2[key]++;
				}
				else
				{
					dictionary2.Add(key, 0);
				}
			}
		}
		if (dictionary.Count > 0)
		{
			RegionName regionName = new List<RegionName>(dictionary.Keys)[0];
			foreach (RegionName key2 in dictionary.Keys)
			{
				if (dictionary[key2] > dictionary[regionName])
				{
					regionName = key2;
				}
			}
			if (dictionary[regionName] > 0)
			{
				foreach (BaseCard item2 in list)
				{
					if (item2 is LocationCard && ((LocationCard)item2).Region == regionName)
					{
						return item2;
					}
				}
			}
		}
		if (dictionary2.Count > 0)
		{
			BuildingType buildingType = new List<BuildingType>(dictionary2.Keys)[0];
			foreach (BuildingType key3 in dictionary2.Keys)
			{
				if (dictionary2[key3] > dictionary2[buildingType])
				{
					buildingType = key3;
				}
			}
			if (dictionary2[buildingType] > 0)
			{
				foreach (BaseCard item3 in list)
				{
					if (item3 is IndustryCard && ((IndustryCard)item3).BuildingTypes[0] == buildingType)
					{
						return item3;
					}
				}
			}
		}
		if (brassApplicationGame.Game.GameProgressInformation.CurrentGameEra == GameEra.CanalEra)
		{
			foreach (Region region2 in brassApplicationGame.Game.Board.Regions)
			{
				if (!dictionary.ContainsKey(region2.Name))
				{
					continue;
				}
				foreach (BuildingSlot buildingSlot in region2.BuildingSlots)
				{
					if (buildingSlot.BuildedIndustry == null || buildingSlot.BuildedIndustry.Color != brassApplicationGame.Game.GameProgressInformation.CurrentPlayer.Color)
					{
						continue;
					}
					foreach (BaseCard item4 in list)
					{
						if (item4 is LocationCard && ((LocationCard)item4).Region == region2.Name)
						{
							return item4;
						}
					}
				}
			}
		}
		foreach (Region region3 in brassApplicationGame.Game.Board.Regions)
		{
			if (!dictionary.ContainsKey(region3.Name))
			{
				continue;
			}
			bool flag = false;
			foreach (BuildingSlot buildingSlot2 in region3.BuildingSlots)
			{
				if (buildingSlot2.BuildedIndustry == null)
				{
					flag = true;
				}
			}
			if (flag)
			{
				continue;
			}
			foreach (BaseCard item5 in list)
			{
				if (item5 is LocationCard && ((LocationCard)item5).Region == region3.Name)
				{
					return item5;
				}
			}
		}
		foreach (BaseCard item6 in list)
		{
			if (item6 is IndustryCard && (((IndustryCard)item6).BuildingTypes.Contains(BuildingType.CottonMill) || ((IndustryCard)item6).BuildingTypes.Contains(BuildingType.Manufacturer) || ((IndustryCard)item6).BuildingTypes.Contains(BuildingType.Pottery)) && !((IndustryCard)item6).BuildingTypes.Contains(industryToKeep))
			{
				return item6;
			}
		}
		if (brassApplicationGame.Game.GameProgressInformation.CurrentGameEra == GameEra.CanalEra)
		{
			foreach (BaseCard item7 in list)
			{
				if (item7 is IndustryCard && ((IndustryCard)item7).BuildingTypes.Contains(BuildingType.Brewery) && !((IndustryCard)item7).BuildingTypes.Contains(industryToKeep))
				{
					return item7;
				}
			}
			foreach (BaseCard item8 in list)
			{
				if (item8 is IndustryCard && !((IndustryCard)item8).BuildingTypes.Contains(industryToKeep))
				{
					return item8;
				}
			}
		}
		foreach (BaseCard item9 in list)
		{
			if (!(item9 is LocationCard) || ((LocationCard)item9).Color != RegionColor.Red)
			{
				continue;
			}
			int num = 0;
			foreach (Link link in brassApplicationGame.Game.Board.GetRegionByName(((LocationCard)item9).Region).Links)
			{
				if (link.HasOwner)
				{
					num++;
				}
			}
			if (num == 0)
			{
				return item9;
			}
		}
		foreach (BaseCard item10 in list)
		{
			if (!(item10 is LocationCard))
			{
				continue;
			}
			int num2 = 0;
			Region regionByName = brassApplicationGame.Game.Board.GetRegionByName(((LocationCard)item10).Region);
			foreach (Link link2 in regionByName.Links)
			{
				if (link2.HasOwner)
				{
					num2++;
				}
			}
			if (num2 == 0 && (regionToKeep == null || !regionToKeep.Contains(regionByName)))
			{
				return item10;
			}
		}
		foreach (BaseCard item11 in list)
		{
			if (item11 is LocationCard && ((LocationCard)item11).Color == RegionColor.Red)
			{
				return item11;
			}
		}
		foreach (BaseCard item12 in list)
		{
			if (!(item12 is IndustryCard) || !((IndustryCard)item12).BuildingTypes.Contains(industryToKeep))
			{
				return item12;
			}
		}
		return list[0];
	}

	public static HashSet<Region> NetworkedRegions(BrassApplicationGame brassApplicationGame, Region region, PlayerColor playerColor)
	{
		HashSet<Region> hashSet = new HashSet<Region>();
		Queue<Region> queue = new Queue<Region>();
		hashSet.Add(region);
		queue.Enqueue(region);
		while (queue.Count > 0)
		{
			Region region2 = queue.Dequeue();
			foreach (Link link in region2.Links)
			{
				if (!link.HasOwner || link.Owner.Color != playerColor)
				{
					continue;
				}
				foreach (Region item in link.GetOtherEnd(region2))
				{
					if (!hashSet.Contains(item))
					{
						hashSet.Add(item);
						queue.Enqueue(item);
					}
				}
			}
		}
		return hashSet;
	}

	public static HashSet<Region> ConnectedRegions(BrassApplicationGame brassApplicationGame, Region region)
	{
		HashSet<Region> hashSet = new HashSet<Region>();
		Queue<Region> queue = new Queue<Region>();
		hashSet.Add(region);
		queue.Enqueue(region);
		while (queue.Count > 0)
		{
			Region region2 = queue.Dequeue();
			foreach (Link link in region2.Links)
			{
				if (!link.HasOwner)
				{
					continue;
				}
				foreach (Region item in link.GetOtherEnd(region2))
				{
					if (!hashSet.Contains(item))
					{
						hashSet.Add(item);
						queue.Enqueue(item);
					}
				}
			}
		}
		return hashSet;
	}

	public static Dictionary<Region, int> MissingConnectionsCount(BrassApplicationGame brassApplicationGame, HashSet<Region> init, bool includeNetworks = false)
	{
		Dictionary<Region, int> dictionary = new Dictionary<Region, int>();
		HashSet<Region> hashSet = new HashSet<Region>();
		foreach (Region item in init)
		{
			hashSet.UnionWith(ConnectedRegions(brassApplicationGame, item));
		}
		Queue<Region> queue = new Queue<Region>(hashSet);
		foreach (Region item2 in hashSet)
		{
			dictionary.Add(item2, 0);
		}
		while (queue.Count > 0)
		{
			Region region = queue.Dequeue();
			if (includeNetworks)
			{
				foreach (Region item3 in ConnectedRegions(brassApplicationGame, region).ToList())
				{
					if (!dictionary.ContainsKey(item3))
					{
						dictionary.Add(item3, dictionary[region]);
						queue.Enqueue(item3);
					}
				}
			}
			foreach (Region regionNeighbour in GetRegionNeighbours(brassApplicationGame, region))
			{
				if (!dictionary.ContainsKey(regionNeighbour))
				{
					dictionary.Add(regionNeighbour, dictionary[region] + 1);
					queue.Enqueue(regionNeighbour);
				}
			}
		}
		return dictionary;
	}

	public static HashSet<Region> RegionsWithMerchants(BrassApplicationGame brassApplicationGame, BuildingType buildingType, bool mustHaveBeer = false)
	{
		HashSet<Region> hashSet = new HashSet<Region>();
		foreach (BorderRegion borderRegion in brassApplicationGame.Game.Board.GetBorderRegions((BorderRegion r) => true))
		{
			int num = 0;
			foreach (MerchantSlot merchantSlot in borderRegion.MerchantSlots)
			{
				if (merchantSlot.MerchantTile != null && merchantSlot.MerchantTile.BuildingTypes.Contains(buildingType) && merchantSlot.HasBeer)
				{
					num++;
				}
			}
			if (mustHaveBeer && num <= 0)
			{
				continue;
			}
			HashSet<Region> hashSet2 = ConnectedRegions(brassApplicationGame, borderRegion);
			HashSet<BuildingType> hashSet3 = new HashSet<BuildingType>();
			foreach (Region item in hashSet2)
			{
				foreach (BuildingSlot buildingSlot in item.BuildingSlots)
				{
					if (buildingSlot.BuildedIndustry != null && !buildingSlot.BuildedIndustry.IsFlipped && !hashSet3.Contains(buildingSlot.BuildedIndustry.BuildingType))
					{
						hashSet3.Add(buildingSlot.BuildedIndustry.BuildingType);
					}
				}
			}
			foreach (MerchantSlot merchantSlot2 in borderRegion.MerchantSlots)
			{
				if (merchantSlot2.MerchantTile == null)
				{
					continue;
				}
				foreach (BuildingType buildingType2 in merchantSlot2.MerchantTile.BuildingTypes)
				{
					hashSet3.Contains(buildingType2);
				}
				foreach (BuildingType buildingType3 in merchantSlot2.MerchantTile.BuildingTypes)
				{
					if ((buildingType == BuildingType.Undefined || buildingType3 == buildingType) && !hashSet.Contains(borderRegion))
					{
						hashSet.Add(borderRegion);
					}
				}
			}
		}
		return hashSet;
	}

	public static bool ConnectSets(Link link, HashSet<Region> set1, HashSet<Region> set2)
	{
		bool flag = false;
		bool flag2 = false;
		if (set1.Contains(link.Region1) || set1.Contains(link.Region2) || set1.Contains(link.Region3))
		{
			flag = true;
		}
		if (set2.Contains(link.Region1) || set2.Contains(link.Region2) || set2.Contains(link.Region3))
		{
			flag2 = true;
		}
		return flag && flag2;
	}

	public static bool CanIndustryBeSoldButLackingBeer(BrassApplicationGame brassApplicationGame, BaseIndustry industry, Player player, Board board)
	{
		if (!(industry is BaseSellableIndustry))
		{
			throw new InvalidOperationException("Chosen industry is of wrong type.");
		}
		List<BorderRegion> borderRegions = board.GetBorderRegions((BorderRegion r) => r.MerchantSlots.Any((MerchantSlot s) => s.MerchantTile != null && s.MerchantTile.BuildingTypes.Contains(industry.BuildingType)));
		Region regionWithIndustry = board.GetRegionContainingGivenIndustry(industry);
		List<BorderRegion> source = borderRegions.Where((BorderRegion r) => board.DoesRouteExists(r, regionWithIndustry)).ToList();
		bool flag = false;
		if (source.Any())
		{
			IEnumerable<Brewery> source2 = from b in board.GetUnflippedBreweries()
				where brassApplicationGame.Game.TurnFlow.GeneralAction.ConsumingBeer.CanBeerBeSuppliedToSlotFromSource(b, regionWithIndustry, player)
				select b;
			flag = source.Sum((BorderRegion m) => m.MerchantSlots.Sum((MerchantSlot ms) => (ms.MerchantTile.BuildingTypes.Contains(industry.BuildingType) && ms.HasBeer) ? 1 : 0)) + source2.Sum((Brewery b) => b.AvailableBeer) >= ((BaseSellableIndustry)industry).CostOfBeerBarrelsToSellIndustry;
		}
		if (source.Any())
		{
			return !flag;
		}
		return false;
	}

	public static Link Find2ndLink()
	{
		if (brassApplicationGame.Game.GameProgressInformation.CurrentGameEra == GameEra.CanalEra)
		{
			return null;
		}
		NetworkAction networkAction = brassApplicationGame.Game.TurnFlow.NetworkAction;
		Board board = brassApplicationGame.Game.Board;
		GameProgressInformation gameProgressInformation = brassApplicationGame.Game.GameProgressInformation;
		_ = gameProgressInformation.CurrentPlayer;
		List<Link> list = new List<Link>(networkAction.GetValidConnectionSlots(board, gameProgressInformation));
		if (list.Count > 0)
		{
			return BestLink(list);
		}
		return null;
	}

	public static Link BestLink(List<Link> links)
	{
		Board board = brassApplicationGame.Game.Board;
		Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
		HashSet<Region> hashSet = new HashSet<Region>();
		foreach (CoalMine unflippedCoalMine in board.GetUnflippedCoalMines())
		{
			if (unflippedCoalMine.Color == currentPlayer.Color)
			{
				Region regionContainingGivenIndustry = board.GetRegionContainingGivenIndustry(unflippedCoalMine);
				hashSet.UnionWith(ConnectedRegions(brassApplicationGame, regionContainingGivenIndustry));
			}
		}
		Dictionary<Link, int> dictionary = new Dictionary<Link, int>();
		Dictionary<Region, int> dictionary2 = MissingConnectionsCount(brassApplicationGame, RegionsWithMerchants(brassApplicationGame, BuildingType.Undefined, mustHaveBeer: true), includeNetworks: true);
		foreach (Link link3 in links)
		{
			if (hashSet.Contains(link3.Region1) || hashSet.Contains(link3.Region2) || hashSet.Contains(link3.Region3))
			{
				int num = 100;
				if (dictionary2.ContainsKey(link3.Region1))
				{
					num = dictionary2[link3.Region1];
				}
				if (dictionary2.ContainsKey(link3.Region2) && dictionary2[link3.Region2] < num)
				{
					num = dictionary2[link3.Region2];
				}
				if (link3.Region3 != null && dictionary2.ContainsKey(link3.Region3) && dictionary2[link3.Region3] < num)
				{
					num = dictionary2[link3.Region3];
				}
				dictionary.Add(link3, num);
			}
		}
		if (dictionary.Count > 0)
		{
			Link link = null;
			{
				foreach (Link key in dictionary.Keys)
				{
					if (link == null || dictionary[key] < dictionary[link])
					{
						link = key;
					}
				}
				return link;
			}
		}
		if (brassApplicationGame.Game.TurnFlow.GetRemainingActionsCount() >= 2 && brassApplicationGame.Game.Board.CoalMarket.GetSellingPrice() >= 4 && brassApplicationGame.Game.GameProgressInformation.CurrentRoundNumber <= 3)
		{
			Link link2 = ConnectCoalForHighDemand.ApplicableLink(brassApplicationGame, new HashSet<Link>(links));
			if (link2 != null)
			{
				return link2;
			}
		}
		return links[0];
	}

	public static BuildingSlot BestBuildingSlot(List<BuildingSlot> slots, BrassApplicationGame brassApplicationGame)
	{
		Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
		foreach (BuildingSlot slot in slots)
		{
			if (slot.BuildedIndustry != null && slot.BuildedIndustry.Color != currentPlayer.Color)
			{
				return slot;
			}
		}
		foreach (BuildingSlot slot2 in slots)
		{
			if (slot2.BuildedIndustry == null)
			{
				return slot2;
			}
		}
		return slots[0];
	}

	public static HashSet<Region> RegionsWithCoalAccess(Board board)
	{
		HashSet<Region> hashSet = new HashSet<Region>(board.GetBorderRegions((BorderRegion r) => true));
		foreach (CoalMine unflippedCoalMine in board.GetUnflippedCoalMines())
		{
			Region regionContainingGivenIndustry = board.GetRegionContainingGivenIndustry(unflippedCoalMine);
			if (!hashSet.Contains(regionContainingGivenIndustry))
			{
				hashSet.Add(regionContainingGivenIndustry);
			}
		}
		HashSet<Region> hashSet2 = new HashSet<Region>();
		foreach (Region item in hashSet)
		{
			HashSet<Region> other = ConnectedRegions(brassApplicationGame, item);
			hashSet2.UnionWith(other);
		}
		return hashSet2;
	}
}
