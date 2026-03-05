using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Events;
using BoardGameRules.Entities.Board.Links;
using BoardGameRules.Entities.Board.Merchants;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.Common;
using BoardGameRules.Entities.EventSourcing.Aggregates;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.Markets;
using BoardGameRules.Entities.NumericalResources;
using BoardGameRules.Entities.NumericalResources.Events;
using BoardGameRules.Entities.Players;
using BoardGameRules.Utils;
using BoardGameRules.Utils.Logging;

namespace BoardGameRules.Entities.Board;

public class Board
{
	private ILog Logger;

	private readonly PlayersTrack _playersTrack;

	private readonly Randomer _randomer;

	public IList<Region> Regions { get; }

	public List<BaseIndustry> RemovedIndustries { get; }

	public List<BaseCard> WildIndustryCardDeck { get; }

	public List<BaseCard> WildLocationCardDeck { get; }

	public List<BaseCard> CardDeck { get; }

	public CoalMarket CoalMarket { get; }

	public IronMarket IronMarket { get; }

	private List<Player> CurrentPlayerOrder { get; set; }

	public Board(IAggregateRoot aggregateRoot, PlayersTrack playersTrack, Randomer randomer)
	{
		_playersTrack = playersTrack;
		_randomer = randomer;
		aggregateRoot.RegisterApplyEvent<MerchantBonusReceived>(ApplyEvent);
		aggregateRoot.RegisterApplyEvent<MerchantBonusDevelopFinished>(ApplyEvent);
		aggregateRoot.RegisterApplyEvent<CoalSold>(ApplyEvent);
		aggregateRoot.RegisterApplyEvent<IronSold>(ApplyEvent);
		Logger = LogFactory.BuildLogger(GetType());
		WildIndustryCardDeck = new List<BaseCard>();
		WildLocationCardDeck = new List<BaseCard>();
		CardDeck = new List<BaseCard>();
		Regions = new List<Region>();
		RemovedIndustries = new List<BaseIndustry>();
		CoalMarket = new CoalMarket();
		IronMarket = new IronMarket();
		CreateBoard();
	}

	private void CreateBoard()
	{
		CreateRegions(_randomer);
		CreateLinks();
	}

	public void PrepareCardDecks(NumberOfPlayers numberOfPlayers, IRandomer randomer)
	{
		WildIndustryCardDeck.Clear();
		WildLocationCardDeck.Clear();
		CardDeck.Clear();
		CardsFactory cardsFactory = new CardsFactory(randomer);
		WildIndustryCardDeck.AddRange(cardsFactory.GetIndustryWildCards(numberOfPlayers));
		WildLocationCardDeck.AddRange(cardsFactory.GetLocationWildCards(numberOfPlayers));
		CardDeck.AddRange(cardsFactory.GetLocationAndIndustryCards(numberOfPlayers));
	}

	public void PrepareMerchantTiles(NumberOfPlayers numberOfPlayers, IRandomer randomer)
	{
		List<MerchantSlot> slots = GetBorderRegions((BorderRegion r) => r.MerchantSlots.First().MinNumberOfPlayers <= numberOfPlayers).SelectMany((BorderRegion r) => r.MerchantSlots).ToList();
		List<MerchantTile> tiles = MerchantTilesFactory.GenerateMerchantTilesForGivenNumberOfPlayers(numberOfPlayers, randomer);
		PlaceMerchantTilesInSlots(tiles, slots);
	}

	public Region GetRegionByName(RegionName name)
	{
		return Regions.First((Region r) => r.Name.Equals(name));
	}

	public Region GetRegionContainingGivenSlot(BuildingSlot slot)
	{
		return Regions.FirstOrDefault((Region r) => r.BuildingSlots.Any((BuildingSlot bs) => bs.Equals(slot)));
	}

	public Region GetRegionContainingGivenIndustry(BaseIndustry industry)
	{
		return Regions.FirstOrDefault((Region r) => r.BuildingSlots.Any((BuildingSlot bs) => industry.Equals(bs.BuildedIndustry)));
	}

	public BuildingSlot GetBuildingSlotContainingGivenIndustry(BaseIndustry industry)
	{
		return Regions.SelectMany((Region r) => r.BuildingSlots).FirstOrDefault((BuildingSlot s) => industry.Equals(s.BuildedIndustry));
	}

	public BorderRegion GetRegionOfGivenMerchantTile(MerchantSlot merchantSlot)
	{
		return GetBorderRegions((BorderRegion r) => true).First((BorderRegion r) => r.MerchantSlots.Any((MerchantSlot s) => s.Equals(merchantSlot)));
	}

	public BorderRegion GetRegionOfGivenMerchantId(string merchantUniqueId)
	{
		return GetBorderRegions((BorderRegion r) => true).First((BorderRegion r) => r.MerchantSlots.Any((MerchantSlot s) => s.UniqueId.Equals(merchantUniqueId)));
	}

	public List<BorderRegion> GetBorderRegions(Func<BorderRegion, bool> filter)
	{
		return (from r in Regions
			where r is BorderRegion arg && filter(arg)
			select (BorderRegion)r).ToList();
	}

	public Link GetLinkById(string linkId)
	{
		return Regions.SelectMany((Region r) => r.Links).FirstOrDefault((Link l) => l.LinkId.Equals(linkId));
	}

	public IBeerSource GetBeerSourceById(string beerSourceId)
	{
		return (from s in Regions.SelectMany((Region r) => r.BuildingSlots)
			where s.BuildedIndustry is IBeerSource
			select s.BuildedIndustry as IBeerSource).Concat(GetBorderRegions((BorderRegion r) => true).SelectMany((BorderRegion r) => r.MerchantSlots)).FirstOrDefault((IBeerSource bs) => bs.UniqueId.Equals(beerSourceId));
	}

	public ICoalSource GetCoalSourceById(string coalSourceId)
	{
		List<ICoalSource> list = (from s in Regions.SelectMany((Region r) => r.BuildingSlots)
			where s.BuildedIndustry is ICoalSource
			select s.BuildedIndustry as ICoalSource).ToList();
		list.Add(CoalMarket);
		return list.FirstOrDefault((ICoalSource cs) => cs.UniqueId.Equals(coalSourceId));
	}

	public IIronSource GetIronSourceById(string ironSourceId)
	{
		List<IIronSource> list = (from s in Regions.SelectMany((Region r) => r.BuildingSlots)
			where s.BuildedIndustry is IIronSource
			select s.BuildedIndustry as IIronSource).ToList();
		list.Add(IronMarket);
		return list.FirstOrDefault((IIronSource cs) => cs.UniqueId.Equals(ironSourceId));
	}

	public bool IsRegionPartOfGivenPlayerNetworkConnection(Region region, PlayerColor playerColor)
	{
		if (region.BuildingSlots == null || !region.BuildingSlots.Any((BuildingSlot s) => s.BuildedIndustry != null && s.BuildedIndustry.Color.Equals(playerColor)))
		{
			return region.Links.Any((Link c) => c.Owner != null && c.Owner.Color.Equals(playerColor));
		}
		return true;
	}

	public IList<Region> GetYourNetwork(PlayerColor playerColor)
	{
		List<Region> list = Regions.Where((Region r) => IsRegionPartOfGivenPlayerNetworkConnection(r, playerColor)).ToList();
		if (list.Count != 0)
		{
			return list;
		}
		return Regions;
	}

	public BaseIndustry GetIndustryByUniqueId(string uniqueId)
	{
		return (from s in Regions.SelectMany((Region r) => r.BuildingSlots)
			where s.BuildedIndustry != null
			select s.BuildedIndustry).FirstOrDefault((BaseIndustry i) => i?.UniqueId.Equals(uniqueId) ?? false);
	}

	public BaseIndustry GetRemovedIndustryByUniqueId(string uniqueId)
	{
		return RemovedIndustries.FirstOrDefault((BaseIndustry i) => i?.UniqueId.Equals(uniqueId) ?? false);
	}

	public BuildingSlot GetBuildingSlotByUniqueId(string uniqueId)
	{
		return Regions.SelectMany((Region r) => r.BuildingSlots).FirstOrDefault((BuildingSlot bs) => bs.UniqueId.Equals(uniqueId));
	}

	public void ResetMerchantBeer()
	{
		foreach (MerchantSlot item in GetBorderRegions((BorderRegion r) => true).SelectMany((BorderRegion r) => r.MerchantSlots))
		{
			item.ResetBeer();
		}
	}

	public int GetAmountOfCoalOnBoard()
	{
		return ((CoalMarket.CoalPrice(1) != 8) ? 1 : 0) + (from s in Regions.SelectMany((Region r) => r.BuildingSlots)
			where s.BuildedIndustry is CoalMine
			select (CoalMine)s.BuildedIndustry).Sum((CoalMine c) => c.AvailableCoal);
	}

	public int GetAmountOfIronOnBoard()
	{
		return ((IronMarket.IronPrice() != 6) ? 1 : 0) + GetAmountOfIronInIronWorks();
	}

	public IEnumerable<CoalMine> GetUnflippedCoalMines()
	{
		return from s in Regions.SelectMany((Region r) => r.BuildingSlots)
			where s.BuildedIndustry is CoalMine coalMine && coalMine.HasCoal
			select (CoalMine)s.BuildedIndustry;
	}

	public int GetAmountOfIronInIronWorks()
	{
		return (from s in Regions.SelectMany((Region r) => r.BuildingSlots)
			where s.BuildedIndustry is IronWorks
			select (IronWorks)s.BuildedIndustry).Sum((IronWorks c) => c.AvailableIron);
	}

	public IEnumerable<IronWorks> GetUnflippedIronWorks()
	{
		return from s in Regions.SelectMany((Region r) => r.BuildingSlots)
			where s.BuildedIndustry is IronWorks ironWorks && ironWorks.HasIron
			select (IronWorks)s.BuildedIndustry;
	}

	public int GetAmountOfBeerInBreweries()
	{
		return (from s in Regions.SelectMany((Region r) => r.BuildingSlots)
			where s.BuildedIndustry is Brewery
			select (Brewery)s.BuildedIndustry).Sum((Brewery c) => c.AvailableBeer);
	}

	public IEnumerable<Brewery> GetUnflippedBreweries()
	{
		return from s in Regions.SelectMany((Region r) => r.BuildingSlots)
			where s.BuildedIndustry is Brewery brewery && brewery.HasBeer
			select (Brewery)s.BuildedIndustry;
	}

	public IEnumerable<MerchantSlot> GetMerchantsWithBeer()
	{
		return GetBorderRegions((BorderRegion r) => true).SelectMany((BorderRegion r) => r.MerchantSlots.Where((MerchantSlot m) => m.HasBeer));
	}

	public IEnumerable<BaseIndustry> GetIndustriesOnBoardOfGivenPlayer(PlayerColor playerColor)
	{
		return from s in Regions.SelectMany((Region r) => r.BuildingSlots)
			where s.BuildedIndustry != null && s.BuildedIndustry.Color == playerColor
			select s.BuildedIndustry;
	}

	public List<Region>[] GetPossibleConnections(Region from, Type type)
	{
		return CreateRoutes(from, type, active: false);
	}

	private int[] BFS(Region from, Type type, bool active, out int[] parent)
	{
		int[] array = new int[Regions.Count];
		parent = new int[Regions.Count];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = (parent[i] = -1);
		}
		array[Regions.IndexOf(from)] = 0;
		Queue<Region> queue = new Queue<Region>();
		queue.Enqueue(from);
		while (queue.Count > 0)
		{
			Region region = queue.Dequeue();
			int num = Regions.IndexOf(region);
			foreach (Link link in region.Links)
			{
				if ((active && !link.HasOwner) || (!active && link.GetType() != type))
				{
					continue;
				}
				foreach (Region item in link.GetOtherEnd(region))
				{
					int num2 = Regions.IndexOf(item);
					if (array[num2] == -1)
					{
						array[num2] = array[num] + 1;
						parent[num2] = num;
						queue.Enqueue(item);
					}
				}
			}
		}
		return array;
	}

	public List<Region> GetConnectedRegions(Region from)
	{
		List<Region> list = new List<Region>();
		int[] parent = new int[Regions.Count];
		int[] array = BFS(from, null, active: true, out parent);
		for (int i = 0; i < Regions.Count; i++)
		{
			if (array[i] >= 0)
			{
				list.Add(Regions[i]);
			}
		}
		return list;
	}

	public bool DoesRouteExists(Region from, Region to)
	{
		return GetShortestRouteTo(from, to) != null;
	}

	public int GetRouteLength(Region from, Region to)
	{
		return GetShortestRouteTo(from, to)?.Count ?? (-1);
	}

	public List<Region> GetShortestRouteTo(Region from, Region to)
	{
		return CreateRoutes(from, null, active: true)[Regions.IndexOf(to)];
	}

	private void PlaceMerchantTilesInSlots(List<MerchantTile> tiles, List<MerchantSlot> slots)
	{
		for (int i = 0; i < slots.Count; i++)
		{
			slots[i].SetMerchant(tiles[i]);
		}
	}

	private List<Region>[] CreateRoutes(Region from, Type type, bool active)
	{
		if (type == null)
		{
			active = true;
		}
		int[] array = new int[Regions.Count];
		int[] array2 = new int[Regions.Count];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = (array2[i] = -1);
		}
		array[Regions.IndexOf(from)] = 0;
		Queue<Region> queue = new Queue<Region>();
		queue.Enqueue(from);
		while (queue.Count > 0)
		{
			Region region = queue.Dequeue();
			int num = Regions.IndexOf(region);
			foreach (Link link in region.Links)
			{
				if ((active && !link.HasOwner) || (!active && link.GetType() != type))
				{
					continue;
				}
				foreach (Region item2 in link.GetOtherEnd(region))
				{
					int num2 = Regions.IndexOf(item2);
					if (array[num2] == -1)
					{
						array[num2] = array[num] + 1;
						array2[num2] = num;
						queue.Enqueue(item2);
					}
				}
			}
		}
		List<Region>[] array3 = new List<Region>[Regions.Count];
		for (int j = 0; j < array3.Length; j++)
		{
			if (array[j] >= 0)
			{
				array3[j] = new List<Region>();
				int num3 = j;
				while (array[num3] > 0)
				{
					Region item = Regions[num3];
					array3[j].Add(item);
					num3 = array2[num3];
				}
				array3[j].Reverse();
			}
		}
		return array3;
	}

	private void ApplyEvent(MerchantBonusReceived merchantBonusReceived)
	{
		Player playerByColor = _playersTrack.GetPlayerByColor(merchantBonusReceived.PlayerColor);
		if (merchantBonusReceived.BonusType == BonusType.VP)
		{
			playerByColor.VP += ((merchantBonusReceived.RegionName == RegionName.Shrewsbury) ? 4 : 3);
			playerByColor.VPFromBonusesInCurrentEra += ((merchantBonusReceived.RegionName == RegionName.Shrewsbury) ? 4 : 3);
		}
		else if (merchantBonusReceived.BonusType == BonusType.Income)
		{
			playerByColor.IncomeMarkerPosition += 2;
		}
		else if (merchantBonusReceived.BonusType == BonusType.Money)
		{
			playerByColor.Money += 5;
		}
		else if (merchantBonusReceived.BonusType == BonusType.Develop)
		{
			DevelopMerchantBonus.Instance.MerchantDevelopmentNeeded = true;
		}
	}

	private void ApplyEvent(MerchantBonusDevelopFinished merchantBonusDevelopFinished)
	{
		DevelopMerchantBonus.Instance.MerchantDevelopmentNeeded = false;
	}

	private void ApplyEvent(CoalSold coalSold)
	{
		_playersTrack.GetPlayerByColor(coalSold.PlayerColor).Money += coalSold.MoneyReceived;
		CoalMarket.Sell(coalSold.Amount);
		(GetBuildingSlotByUniqueId(coalSold.SlotUniqueId).BuildedIndustry as CoalMine).ConsumeCoal(coalSold.Amount);
	}

	private void ApplyEvent(IronSold ironSold)
	{
		_playersTrack.GetPlayerByColor(ironSold.PlayerColor).Money += ironSold.MoneyReceived;
		IronMarket.Sell(ironSold.Amount);
		(GetBuildingSlotByUniqueId(ironSold.SlotUniqueId).BuildedIndustry as IronWorks).ConsumeIron(ironSold.Amount);
	}

	private void CreateRegions(Randomer randomer)
	{
		Regions.Add(new Region(RegionName.Birmingham, RegionColor.Violet, new List<BuildingSlot>
		{
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.CottonMill, BuildingType.Manufacturer),
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.Manufacturer),
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.IronWorks),
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.Manufacturer)
		}));
		Regions.Add(new Region(RegionName.Redditch, RegionColor.Violet, new List<BuildingSlot>
		{
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.Manufacturer, BuildingType.CoalMine),
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.IronWorks)
		}));
		Regions.Add(new Region(RegionName.Coventry, RegionColor.Violet, new List<BuildingSlot>
		{
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.Pottery),
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.Manufacturer, BuildingType.CoalMine),
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.IronWorks, BuildingType.Manufacturer)
		}));
		Regions.Add(new Region(RegionName.Nuneaton, RegionColor.Violet, new List<BuildingSlot>
		{
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.Manufacturer, BuildingType.Brewery),
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.CottonMill, BuildingType.CoalMine)
		}));
		Regions.Add(new Region(RegionName.Worcester, RegionColor.Yellow, new List<BuildingSlot>
		{
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.CottonMill),
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.CottonMill)
		}));
		Regions.Add(new Region(RegionName.Kidderminster, RegionColor.Yellow, new List<BuildingSlot>
		{
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.CottonMill, BuildingType.CoalMine),
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.CottonMill)
		}));
		Regions.Add(new Region(RegionName.Dudley, RegionColor.Yellow, new List<BuildingSlot>
		{
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.CoalMine),
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.IronWorks)
		}));
		Regions.Add(new Region(RegionName.Coalbrookdale, RegionColor.Yellow, new List<BuildingSlot>
		{
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.IronWorks, BuildingType.Brewery),
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.IronWorks),
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.CoalMine)
		}));
		Regions.Add(new Region(RegionName.Wolverhampton, RegionColor.Yellow, new List<BuildingSlot>
		{
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.Manufacturer),
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.Manufacturer, BuildingType.CoalMine)
		}));
		Regions.Add(new Region(RegionName.Walsall, RegionColor.Red, new List<BuildingSlot>
		{
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.IronWorks, BuildingType.Manufacturer),
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.Manufacturer, BuildingType.Brewery)
		}));
		Regions.Add(new Region(RegionName.Tamworth, RegionColor.Red, new List<BuildingSlot>
		{
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.CottonMill, BuildingType.CoalMine),
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.CottonMill, BuildingType.CoalMine)
		}));
		Regions.Add(new Region(RegionName.Cannock, RegionColor.Red, new List<BuildingSlot>
		{
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.Manufacturer, BuildingType.CoalMine),
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.CoalMine)
		}));
		Regions.Add(new Region(RegionName.Stafford, RegionColor.Red, new List<BuildingSlot>
		{
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.Manufacturer, BuildingType.Brewery),
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.Pottery)
		}));
		Regions.Add(new Region(RegionName.BurtonOnTrent, RegionColor.Red, new List<BuildingSlot>
		{
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.Manufacturer, BuildingType.CoalMine),
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.Brewery)
		}));
		Regions.Add(new Region(RegionName.Derby, RegionColor.Cyan, new List<BuildingSlot>
		{
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.CottonMill, BuildingType.Brewery),
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.CottonMill, BuildingType.Manufacturer),
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.IronWorks)
		}));
		Regions.Add(new Region(RegionName.Belper, RegionColor.Cyan, new List<BuildingSlot>
		{
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.CottonMill, BuildingType.Manufacturer),
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.CoalMine),
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.Pottery)
		}));
		Regions.Add(new Region(RegionName.Leek, RegionColor.Blue, new List<BuildingSlot>
		{
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.CottonMill, BuildingType.Manufacturer),
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.CottonMill, BuildingType.CoalMine)
		}));
		Regions.Add(new Region(RegionName.Uttoxeter, RegionColor.Blue, new List<BuildingSlot>
		{
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.Manufacturer, BuildingType.Brewery),
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.CottonMill, BuildingType.Brewery)
		}));
		Regions.Add(new Region(RegionName.Stone, RegionColor.Blue, new List<BuildingSlot>
		{
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.CottonMill, BuildingType.Brewery),
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.Manufacturer, BuildingType.CoalMine)
		}));
		Regions.Add(new Region(RegionName.StokeOnTrent, RegionColor.Blue, new List<BuildingSlot>
		{
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.Pottery, BuildingType.IronWorks),
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.Manufacturer),
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.CottonMill, BuildingType.Manufacturer)
		}));
		Regions.Add(new Region(RegionName.SouthWest, RegionColor.Violet, new List<BuildingSlot>
		{
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.Brewery)
		}));
		Regions.Add(new Region(RegionName.Central, RegionColor.Violet, new List<BuildingSlot>
		{
			new BuildingSlot(UniqueIdGenerator.GenerateBuildingSlotUniqueId(randomer), BuildingType.Brewery)
		}));
		Regions.Add(new BorderRegion(RegionName.Oxford, BonusType.Income, new List<MerchantSlot>
		{
			new MerchantSlot(UniqueIdGenerator.GenerateMerchantSlotUniqueId(randomer), NumberOfPlayers.Two),
			new MerchantSlot(UniqueIdGenerator.GenerateMerchantSlotUniqueId(randomer), NumberOfPlayers.Two)
		}));
		Regions.Add(new BorderRegion(RegionName.Gloucester, BonusType.Develop, new List<MerchantSlot>
		{
			new MerchantSlot(UniqueIdGenerator.GenerateMerchantSlotUniqueId(randomer), NumberOfPlayers.Two),
			new MerchantSlot(UniqueIdGenerator.GenerateMerchantSlotUniqueId(randomer), NumberOfPlayers.Two)
		}));
		Regions.Add(new BorderRegion(RegionName.Shrewsbury, BonusType.VP, new List<MerchantSlot>
		{
			new MerchantSlot(UniqueIdGenerator.GenerateMerchantSlotUniqueId(randomer), NumberOfPlayers.Two)
		}));
		Regions.Add(new BorderRegion(RegionName.Warrington, BonusType.Money, new List<MerchantSlot>
		{
			new MerchantSlot(UniqueIdGenerator.GenerateMerchantSlotUniqueId(randomer), NumberOfPlayers.Three),
			new MerchantSlot(UniqueIdGenerator.GenerateMerchantSlotUniqueId(randomer), NumberOfPlayers.Three)
		}));
		Regions.Add(new BorderRegion(RegionName.Nottingham, BonusType.VP, new List<MerchantSlot>
		{
			new MerchantSlot(UniqueIdGenerator.GenerateMerchantSlotUniqueId(randomer), NumberOfPlayers.Four),
			new MerchantSlot(UniqueIdGenerator.GenerateMerchantSlotUniqueId(randomer), NumberOfPlayers.Four)
		}));
	}

	private void CreateLinks()
	{
		new List<Link>
		{
			new Rail(GetRegionByName(RegionName.Birmingham), GetRegionByName(RegionName.Walsall)),
			new Canal(GetRegionByName(RegionName.Birmingham), GetRegionByName(RegionName.Walsall)),
			new Rail(GetRegionByName(RegionName.Birmingham), GetRegionByName(RegionName.Redditch)),
			new Rail(GetRegionByName(RegionName.Birmingham), GetRegionByName(RegionName.Coventry)),
			new Canal(GetRegionByName(RegionName.Birmingham), GetRegionByName(RegionName.Coventry)),
			new Rail(GetRegionByName(RegionName.Birmingham), GetRegionByName(RegionName.Nuneaton)),
			new Rail(GetRegionByName(RegionName.Birmingham), GetRegionByName(RegionName.Tamworth)),
			new Canal(GetRegionByName(RegionName.Birmingham), GetRegionByName(RegionName.Tamworth)),
			new Rail(GetRegionByName(RegionName.Birmingham), GetRegionByName(RegionName.Dudley)),
			new Canal(GetRegionByName(RegionName.Birmingham), GetRegionByName(RegionName.Dudley)),
			new Rail(GetRegionByName(RegionName.Birmingham), GetRegionByName(RegionName.Worcester)),
			new Canal(GetRegionByName(RegionName.Birmingham), GetRegionByName(RegionName.Worcester)),
			new Rail(GetRegionByName(RegionName.Birmingham), GetRegionByName(RegionName.Oxford)),
			new Canal(GetRegionByName(RegionName.Birmingham), GetRegionByName(RegionName.Oxford)),
			new Rail(GetRegionByName(RegionName.Redditch), GetRegionByName(RegionName.Oxford)),
			new Canal(GetRegionByName(RegionName.Redditch), GetRegionByName(RegionName.Oxford)),
			new Rail(GetRegionByName(RegionName.Redditch), GetRegionByName(RegionName.Gloucester)),
			new Canal(GetRegionByName(RegionName.Redditch), GetRegionByName(RegionName.Gloucester)),
			new Rail(GetRegionByName(RegionName.Nuneaton), GetRegionByName(RegionName.Tamworth)),
			new Canal(GetRegionByName(RegionName.Nuneaton), GetRegionByName(RegionName.Tamworth)),
			new Rail(GetRegionByName(RegionName.Kidderminster), GetRegionByName(RegionName.Dudley)),
			new Canal(GetRegionByName(RegionName.Kidderminster), GetRegionByName(RegionName.Dudley)),
			new Rail(GetRegionByName(RegionName.Kidderminster), GetRegionByName(RegionName.Coalbrookdale)),
			new Canal(GetRegionByName(RegionName.Kidderminster), GetRegionByName(RegionName.Coalbrookdale)),
			new Rail(GetRegionByName(RegionName.Coalbrookdale), GetRegionByName(RegionName.Wolverhampton)),
			new Canal(GetRegionByName(RegionName.Coalbrookdale), GetRegionByName(RegionName.Wolverhampton)),
			new Rail(GetRegionByName(RegionName.Dudley), GetRegionByName(RegionName.Wolverhampton)),
			new Canal(GetRegionByName(RegionName.Dudley), GetRegionByName(RegionName.Wolverhampton)),
			new Rail(GetRegionByName(RegionName.Wolverhampton), GetRegionByName(RegionName.Cannock)),
			new Canal(GetRegionByName(RegionName.Wolverhampton), GetRegionByName(RegionName.Cannock)),
			new Rail(GetRegionByName(RegionName.Wolverhampton), GetRegionByName(RegionName.Walsall)),
			new Canal(GetRegionByName(RegionName.Wolverhampton), GetRegionByName(RegionName.Walsall)),
			new Rail(GetRegionByName(RegionName.Walsall), GetRegionByName(RegionName.Cannock)),
			new Canal(GetRegionByName(RegionName.Walsall), GetRegionByName(RegionName.Cannock)),
			new Rail(GetRegionByName(RegionName.Walsall), GetRegionByName(RegionName.Tamworth)),
			new Canal(GetRegionByName(RegionName.Walsall), GetRegionByName(RegionName.BurtonOnTrent)),
			new Rail(GetRegionByName(RegionName.Coventry), GetRegionByName(RegionName.Nuneaton)),
			new Rail(GetRegionByName(RegionName.Worcester), GetRegionByName(RegionName.Gloucester)),
			new Canal(GetRegionByName(RegionName.Worcester), GetRegionByName(RegionName.Gloucester)),
			new Rail(GetRegionByName(RegionName.Worcester), GetRegionByName(RegionName.SouthWest), GetRegionByName(RegionName.Kidderminster)),
			new Canal(GetRegionByName(RegionName.Worcester), GetRegionByName(RegionName.SouthWest), GetRegionByName(RegionName.Kidderminster)),
			new Rail(GetRegionByName(RegionName.Coalbrookdale), GetRegionByName(RegionName.Shrewsbury)),
			new Canal(GetRegionByName(RegionName.Coalbrookdale), GetRegionByName(RegionName.Shrewsbury)),
			new Rail(GetRegionByName(RegionName.Cannock), GetRegionByName(RegionName.Central)),
			new Canal(GetRegionByName(RegionName.Cannock), GetRegionByName(RegionName.Central)),
			new Rail(GetRegionByName(RegionName.Cannock), GetRegionByName(RegionName.Stafford)),
			new Canal(GetRegionByName(RegionName.Cannock), GetRegionByName(RegionName.Stafford)),
			new Rail(GetRegionByName(RegionName.Cannock), GetRegionByName(RegionName.BurtonOnTrent)),
			new Rail(GetRegionByName(RegionName.Tamworth), GetRegionByName(RegionName.BurtonOnTrent)),
			new Canal(GetRegionByName(RegionName.Tamworth), GetRegionByName(RegionName.BurtonOnTrent)),
			new Rail(GetRegionByName(RegionName.Stone), GetRegionByName(RegionName.Stafford)),
			new Canal(GetRegionByName(RegionName.Stone), GetRegionByName(RegionName.Stafford)),
			new Rail(GetRegionByName(RegionName.Stone), GetRegionByName(RegionName.Uttoxeter)),
			new Rail(GetRegionByName(RegionName.Stone), GetRegionByName(RegionName.BurtonOnTrent)),
			new Canal(GetRegionByName(RegionName.Stone), GetRegionByName(RegionName.BurtonOnTrent)),
			new Rail(GetRegionByName(RegionName.Stone), GetRegionByName(RegionName.StokeOnTrent)),
			new Canal(GetRegionByName(RegionName.Stone), GetRegionByName(RegionName.StokeOnTrent)),
			new Rail(GetRegionByName(RegionName.StokeOnTrent), GetRegionByName(RegionName.Warrington)),
			new Canal(GetRegionByName(RegionName.StokeOnTrent), GetRegionByName(RegionName.Warrington)),
			new Rail(GetRegionByName(RegionName.StokeOnTrent), GetRegionByName(RegionName.Leek)),
			new Canal(GetRegionByName(RegionName.StokeOnTrent), GetRegionByName(RegionName.Leek)),
			new Rail(GetRegionByName(RegionName.Leek), GetRegionByName(RegionName.Belper)),
			new Rail(GetRegionByName(RegionName.Belper), GetRegionByName(RegionName.Derby)),
			new Canal(GetRegionByName(RegionName.Belper), GetRegionByName(RegionName.Derby)),
			new Rail(GetRegionByName(RegionName.Derby), GetRegionByName(RegionName.Nottingham)),
			new Canal(GetRegionByName(RegionName.Derby), GetRegionByName(RegionName.Nottingham)),
			new Rail(GetRegionByName(RegionName.Derby), GetRegionByName(RegionName.Uttoxeter)),
			new Rail(GetRegionByName(RegionName.Derby), GetRegionByName(RegionName.BurtonOnTrent)),
			new Canal(GetRegionByName(RegionName.Derby), GetRegionByName(RegionName.BurtonOnTrent))
		};
	}
}
