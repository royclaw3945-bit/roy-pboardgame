using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Actions.Events;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.Common;
using BoardGameRules.Entities.Consumables;
using BoardGameRules.Entities.EventSourcing.Aggregates;
using BoardGameRules.Entities.Game;
using BoardGameRules.Entities.GamePhases;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.NumericalResources;
using BoardGameRules.Entities.NumericalResources.Events;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.Actions;

public class BuildAction : BaseAction
{
	private readonly GameProgressInformation _gameProgressInformation;

	private BuildingSlot _chosenSlot;

	private BuildingType _chosenBuildingType;

	private BaseIndustry _chosenIndustry;

	public int NeededCoal { get; private set; }

	public int NeededIron { get; private set; }

	public BuildAction(IAggregateRoot aggregateRoot, PlayersTrack playersTrack, BoardGameRules.Entities.Board.Board board, GeneralAction generalAction, GameProgressInformation gameProgressInformation)
		: base(aggregateRoot, playersTrack, board, generalAction)
	{
		_gameProgressInformation = gameProgressInformation;
		aggregateRoot.RegisterApplyEvent<IndustryBuilt>(ApplyEvent);
	}

	public void Build(BuildingSlot slot, BuildingType buildingType)
	{
		if (slot == null)
		{
			throw new InvalidOperationException("Building slot is null.");
		}
		if (buildingType == BuildingType.Undefined)
		{
			throw new InvalidOperationException("Building type is null.");
		}
		if (_player == null)
		{
			throw new InvalidOperationException("Player is not set up.");
		}
		_chosenIndustry = _player.Mat.GetFirstBuildingOfType(buildingType);
		if (_chosenIndustry == null)
		{
			throw new InvalidOperationException("Industry is null. Check Player Mat if it has industry of wanted type.");
		}
		if (_chosenIndustry.IsForCanalEra && _gameProgressInformation.CurrentGameEra == GameEra.RailEra)
		{
			throw new InvalidOperationException("This industry can be build only in canal era.");
		}
		if (_chosenIndustry.IsForRailEra && _gameProgressInformation.CurrentGameEra == GameEra.CanalEra)
		{
			throw new InvalidOperationException("This industry can be build only in rail era.");
		}
		if (_player.Money < _chosenIndustry.MoneyCost)
		{
			throw new InvalidOperationException("Player doesn't have enough money to build this industry.");
		}
		if (!GetPossibleSlotsToBuildInWithGivenCardByGivenPlayer(_chosenCard, _player.Color, base.Board, _gameProgressInformation).Contains(slot) && !GetPossibleSlotsToOverBuildInWithGivenCardByGivenPlayer(_chosenCard, _player, base.Board).Contains(slot))
		{
			throw new InvalidOperationException("Player can't build or overbuild in given slot with chosen card.");
		}
		if (!IsGivenSlotTheBestOneInGivenRegionForGivenType(slot, buildingType, base.Board))
		{
			throw new InvalidOperationException("Player should build on space displaying only that industry's icon.");
		}
		NeededCoal = _chosenIndustry.CoalCost;
		NeededIron = _chosenIndustry.IronCost;
		_chosenSlot = slot;
		_chosenBuildingType = buildingType;
		base.AggregateRoot.ApplyEvent(new IndustryBuilt(_player.Color, buildingType, slot.UniqueId));
		base.AggregateRoot.ApplyEvent(new MoneySpent(_player.Color, _chosenIndustry.MoneyCost));
		if (NeededCoal == 0 && NeededIron == 0)
		{
			SellResourcesIfPossible(_chosenIndustry, slot, _player);
		}
	}

	private void SellResourcesIfPossible(BaseIndustry industry, BuildingSlot buildingSlot, Player player)
	{
		int num = base.Board.IronMarket.AmountOfResourcePossibleToSell();
		if (industry is IronWorks ironWorks && num > 0)
		{
			int amount = Math.Min(num, ironWorks.AvailableIron);
			base.AggregateRoot.ApplyEvent(new IronSold(buildingSlot.UniqueId, amount, base.Board.IronMarket.GetSellingPrice(amount), player.Color));
			if (ironWorks.AvailableIron == 0)
			{
				RegionName name = base.Board.GetRegionContainingGivenSlot(buildingSlot).Name;
				base.AggregateRoot.ApplyEvent(new IndustryFlipped(ironWorks.UniqueId, name));
				player.ChangeIncomeMarkerPositionBy(industry.IncomeIncrease, industry.UniqueId);
			}
		}
		if (!(industry is CoalMine coalMine))
		{
			return;
		}
		int num2 = base.Board.CoalMarket.AmountOfResourcePossibleToSell();
		Region targetRegion = base.Board.GetRegionContainingGivenSlot(buildingSlot);
		bool flag = base.Board.GetBorderRegions((BorderRegion r) => true).Any((BorderRegion r) => base.Board.GetRouteLength(r, targetRegion) > 0);
		if (num2 > 0 && flag)
		{
			int amount2 = Math.Min(num2, coalMine.AvailableCoal);
			base.AggregateRoot.ApplyEvent(new CoalSold(buildingSlot.UniqueId, amount2, base.Board.CoalMarket.GetSellingPrice(amount2), player.Color));
			if (coalMine.AvailableCoal == 0)
			{
				RegionName name2 = base.Board.GetRegionContainingGivenSlot(buildingSlot).Name;
				base.AggregateRoot.ApplyEvent(new IndustryFlipped(coalMine.UniqueId, name2));
				player.ChangeIncomeMarkerPositionBy(industry.IncomeIncrease, industry.UniqueId);
			}
		}
	}

	public bool IsIronNeeded()
	{
		if (_chosenSlot == null)
		{
			throw new InvalidOperationException("Building slot is not set up.");
		}
		if (_chosenBuildingType == BuildingType.Undefined)
		{
			throw new InvalidOperationException("Building type is not set up.");
		}
		return NeededIron != 0;
	}

	public bool IsCoalNeeded()
	{
		if (_chosenSlot == null)
		{
			throw new InvalidOperationException("Building slot is not set up.");
		}
		if (_chosenBuildingType == BuildingType.Undefined)
		{
			throw new InvalidOperationException("Building type is not set up.");
		}
		return NeededCoal != 0;
	}

	public override void SupplyCoal(ICoalSource coalSource, int amount)
	{
		base.GeneralAction.ConsumingCoal.SupplyCoal(coalSource, amount, NeededCoal, _player, new List<Region> { base.Board.GetRegionContainingGivenSlot(_chosenSlot) });
		NeededCoal -= amount;
		if (NeededCoal == 0 && NeededIron == 0)
		{
			SellResourcesIfPossible(_chosenIndustry, _chosenSlot, _player);
		}
	}

	public override void SupplyIron(IIronSource ironSource, int amount)
	{
		base.GeneralAction.ConsumingIron.SupplyIron(ironSource, amount, NeededIron, _player);
		NeededIron -= amount;
		if (NeededCoal == 0 && NeededIron == 0)
		{
			SellResourcesIfPossible(_chosenIndustry, _chosenSlot, _player);
		}
	}

	public override void ResetAction()
	{
		base.ResetAction();
		_chosenSlot = null;
	}

	private void ApplyEvent(IndustryBuilt industryBuilt)
	{
		Player obj = base.PlayersTrack.GetPlayerByColor(industryBuilt.Player) ?? throw new InvalidOperationException();
		BaseIndustry firstBuildingOfType = obj.Mat.GetFirstBuildingOfType(industryBuilt.Type);
		if (firstBuildingOfType == null)
		{
			throw new InvalidOperationException();
		}
		BuildingSlot buildingSlotByUniqueId = base.Board.GetBuildingSlotByUniqueId(industryBuilt.SlotUniqueId);
		if (buildingSlotByUniqueId.BuildedIndustry != null)
		{
			base.Board.RemovedIndustries.Add(buildingSlotByUniqueId.BuildedIndustry);
		}
		buildingSlotByUniqueId.BuildIndustry(firstBuildingOfType);
		firstBuildingOfType.Build();
		obj.Mat.RemoveGivenBuildingFromMat(firstBuildingOfType);
	}

	public List<BuildingSlot> GetPossibleSlotsToBuildInWithGivenCardByGivenPlayer(BaseCard card, PlayerColor player, BoardGameRules.Entities.Board.Board board, GameProgressInformation gameProgressInformation)
	{
		return GetPossibleSlotsToBuildInWithGivenCardByGivenPlayerStatic(card, player, board, gameProgressInformation);
	}

	public static List<BuildingSlot> GetPossibleSlotsToBuildInWithGivenCardByGivenPlayerStatic(BaseCard card, PlayerColor player, BoardGameRules.Entities.Board.Board board, GameProgressInformation gameProgressInformation)
	{
		if (card == null)
		{
			throw new InvalidOperationException("Card is null.");
		}
		if (board == null)
		{
			throw new InvalidOperationException("Board is null.");
		}
		if (card.GetType() == typeof(LocationCard))
		{
			Region regionByName = board.GetRegionByName(((LocationCard)card).Region);
			if (!CheckPlayerCanBuildOnlyOneBuildingInRegionDuringCanalEra(regionByName, gameProgressInformation, player))
			{
				return new List<BuildingSlot>();
			}
			return regionByName.BuildingSlots.Where((BuildingSlot b) => b.BuildedIndustry == null).ToList();
		}
		if (card.GetType() == typeof(IndustryCard))
		{
			IndustryCard industryCard = (IndustryCard)card;
			return (from r in board.GetYourNetwork(player)
				where r.GetType() != typeof(BorderRegion)
				where CheckPlayerCanBuildOnlyOneBuildingInRegionDuringCanalEra(r, gameProgressInformation, player)
				select r).SelectMany((Region r) => r.BuildingSlots.Where((BuildingSlot s) => s.BuildedIndustry == null && s.BuildingTypes.Any((BuildingType t) => industryCard.BuildingTypes.Contains(t)))).ToList();
		}
		if (card.GetType() == typeof(WildIndustryCard))
		{
			return (from r in board.GetYourNetwork(player)
				where CheckPlayerCanBuildOnlyOneBuildingInRegionDuringCanalEra(r, gameProgressInformation, player)
				select r).ToList().SelectMany((Region r) => r.BuildingSlots.Where((BuildingSlot s) => s.BuildedIndustry == null)).ToList();
		}
		if (card.GetType() == typeof(WildLocationCard))
		{
			return (from r in board.Regions
				where r.Name != RegionName.SouthWest && r.Name != RegionName.Central
				where CheckPlayerCanBuildOnlyOneBuildingInRegionDuringCanalEra(r, gameProgressInformation, player)
				select r).SelectMany((Region r) => r.BuildingSlots.Where((BuildingSlot b) => b.BuildedIndustry == null)).ToList();
		}
		return new List<BuildingSlot>();
	}

	public static bool CheckPlayerCanBuildOnlyOneBuildingInRegionDuringCanalEra(Region region, GameProgressInformation gameProgressInformation, PlayerColor playerColor, BuildingSlot slotToIgnore = null)
	{
		if (gameProgressInformation.CurrentGameEra != GameEra.RailEra)
		{
			if (gameProgressInformation.CurrentGameEra == GameEra.CanalEra)
			{
				return !region.BuildingSlots.Any((BuildingSlot s) => s != slotToIgnore && s.BuildedIndustry != null && s.BuildedIndustry.Color.Equals(playerColor));
			}
			return false;
		}
		return true;
	}

	public bool IsGivenSlotTheBestOneInGivenRegionForGivenType(BuildingSlot slot, BuildingType type, BoardGameRules.Entities.Board.Board board)
	{
		return IsGivenSlotTheBestOneInGivenRegionForGivenTypeStatic(slot, type, board);
	}

	public static bool IsGivenSlotTheBestOneInGivenRegionForGivenTypeStatic(BuildingSlot slot, BuildingType type, BoardGameRules.Entities.Board.Board board)
	{
		if (slot.BuildingTypes.Count == 1)
		{
			return true;
		}
		return !board.GetRegionContainingGivenSlot(slot).BuildingSlots.Any((BuildingSlot s) => s.BuildingTypes.Count == 1 && s.BuildingTypes.First() == type && s.BuildedIndustry == null);
	}

	public static List<BuildingSlot> GetPossibleSlotsToOverBuildInWithGivenCardByGivenPlayer(BaseCard card, Player player, BoardGameRules.Entities.Board.Board board)
	{
		if (card == null)
		{
			throw new InvalidOperationException("Card is null.");
		}
		if (board == null)
		{
			throw new InvalidOperationException("Board is null.");
		}
		PlayerColor color = player.Color;
		if (card is LocationCard locationCard)
		{
			Region regionByName = board.GetRegionByName(locationCard.Region);
			List<BuildingSlot> list = new List<BuildingSlot>();
			{
				foreach (BuildingSlot buildingSlot in regionByName.BuildingSlots)
				{
					if (buildingSlot.BuildedIndustry != null && CanBeOverbuild(buildingSlot, buildingSlot.BuildedIndustry.BuildingType, player, board))
					{
						list.Add(buildingSlot);
					}
				}
				return list;
			}
		}
		IndustryCard industryCard;
		if ((industryCard = card as IndustryCard) != null)
		{
			IEnumerable<BuildingSlot> enumerable = (from r in board.GetYourNetwork(color)
				where r.GetType() != typeof(BorderRegion)
				select r).SelectMany((Region r) => r.BuildingSlots);
			List<BuildingSlot> list2 = new List<BuildingSlot>();
			{
				foreach (BuildingSlot item in enumerable)
				{
					if (item.BuildingTypes.Any((BuildingType t) => industryCard.BuildingTypes.Contains(t)) && item.BuildedIndustry != null && CanBeOverbuild(item, item.BuildedIndustry.BuildingType, player, board))
					{
						list2.Add(item);
					}
				}
				return list2;
			}
		}
		if (card is WildIndustryCard)
		{
			return board.GetYourNetwork(color).SelectMany((Region r) => r.BuildingSlots.Where((BuildingSlot s) => s.BuildedIndustry != null && CanBeOverbuild(s, s.BuildedIndustry.BuildingType, player, board))).ToList();
		}
		if (card is WildLocationCard)
		{
			return board.Regions.Where((Region r) => r.Name != RegionName.SouthWest && r.Name != RegionName.Central).SelectMany((Region r) => r.BuildingSlots.Where((BuildingSlot s) => s.BuildedIndustry != null && CanBeOverbuild(s, s.BuildedIndustry.BuildingType, player, board))).ToList();
		}
		return new List<BuildingSlot>();
	}

	public List<CantUseBuildActionReason> GetBuildActionNotPossibleReasons(BoardGameRules.Entities.Board.Board board, GameProgressInformation gameProgressInformation)
	{
		if (board == null)
		{
			throw new InvalidOperationException("Board is null.");
		}
		if (gameProgressInformation == null)
		{
			throw new InvalidOperationException("GameProgressInformation is null.");
		}
		Player curPlayer = gameProgressInformation.CurrentPlayer;
		List<CantUseBuildActionReason> list = new List<CantUseBuildActionReason>();
		if (!HasPlayerAnyIndustryToBuild(curPlayer))
		{
			list.Add(CantUseBuildActionReason.NO_MORE_INDUSTRIES_TO_BUILD);
			return list;
		}
		if ((from s in board.Regions.SelectMany((Region r) => r.BuildingSlots)
			where s.BuildingTypes.Any((BuildingType t) => !GetCantUseBuildActionReasonForGivenSlot(s, curPlayer.Mat.GetFirstBuildingOfType(t), curPlayer, board, gameProgressInformation).Any())
			select s).Any((BuildingSlot s) => s.BuildingTypes.Any((BuildingType bt) => GetValidCardsForBuildingInSlotStatic(s, bt, curPlayer, board, gameProgressInformation).Any())))
		{
			return list;
		}
		list.Add(CantUseBuildActionReason.NO_PLACE_TO_BUILD_INDUSTRY_ON_BOARD);
		return list;
	}

	public bool CanUseBuildActionForGivenIndustry(BaseIndustry industry, BoardGameRules.Entities.Board.Board board, GameProgressInformation gameProgressInformation)
	{
		Player currentPlayer = gameProgressInformation.CurrentPlayer;
		foreach (BuildingSlot item in from s in board.Regions.SelectMany((Region r) => r.BuildingSlots)
			where s.BuildingTypes.Contains(industry.BuildingType)
			select s)
		{
			if (CanUseBuildActionForGivenSlot(item, industry, currentPlayer, board, gameProgressInformation))
			{
				return true;
			}
		}
		return false;
	}

	public List<CantUseBuildActionReason> GetCantUseBuildActionReasonForGivenIndustry(BaseIndustry industry, BoardGameRules.Entities.Board.Board board, GameProgressInformation gameProgressInformation)
	{
		if (industry == null)
		{
			return new List<CantUseBuildActionReason> { CantUseBuildActionReason.NO_MORE_INDUSTRIES_TO_BUILD };
		}
		Player currentPlayer = gameProgressInformation.CurrentPlayer;
		List<CantUseBuildActionReason> list = new List<CantUseBuildActionReason>();
		IEnumerable<BuildingSlot> enumerable = from s in board.Regions.SelectMany((Region r) => r.BuildingSlots)
			where s.BuildingTypes.Contains(industry.BuildingType)
			select s;
		bool flag = false;
		foreach (BuildingSlot item in enumerable)
		{
			List<CantUseBuildActionReason> cantUseBuildActionReasonForGivenSlot = GetCantUseBuildActionReasonForGivenSlot(item, industry, currentPlayer, board, gameProgressInformation);
			if (cantUseBuildActionReasonForGivenSlot.Any())
			{
				list.AddRange(cantUseBuildActionReasonForGivenSlot);
			}
			else
			{
				flag = true;
			}
		}
		if (!flag)
		{
			return list.Distinct().ToList();
		}
		return new List<CantUseBuildActionReason>();
	}

	public static bool CanUseBuildActionForGivenSlot(BuildingSlot buildingSlot, BaseIndustry industry, Player currentPlayer, BoardGameRules.Entities.Board.Board board, GameProgressInformation gameProgressInformation)
	{
		Region regionContainingGivenSlot = board.GetRegionContainingGivenSlot(buildingSlot);
		if (industry != null)
		{
			List<CantUseBuildActionReason> buildingProblems = GetBuildingProblems(buildingSlot, industry.BuildingType, currentPlayer, board, gameProgressInformation);
			if (buildingProblems != null && buildingProblems.Count > 0)
			{
				return false;
			}
			if (!CheckPlayerCanBuildOnlyOneBuildingInRegionDuringCanalEra(regionContainingGivenSlot, gameProgressInformation, currentPlayer.Color, buildingSlot))
			{
				return false;
			}
			if (!IsSingleSlotOrDoubleSlotButLastPossibleStatic(buildingSlot, industry.BuildingType, regionContainingGivenSlot))
			{
				return false;
			}
			int num = industry.MoneyCost;
			if (industry.NeedsCoal)
			{
				int num2 = CostOfCoalForGivenIndustry(board.GetRegionContainingGivenSlot(buildingSlot), board, industry.CoalCost);
				if (num2 == -1)
				{
					return false;
				}
				num += num2;
			}
			if (industry.NeedsIron)
			{
				int amountOfIronInIronWorks = board.GetAmountOfIronInIronWorks();
				if (amountOfIronInIronWorks < industry.IronCost)
				{
					num += board.IronMarket.GetBuyingPrice(industry.IronCost - amountOfIronInIronWorks);
				}
			}
			if (currentPlayer.Money < num)
			{
				return false;
			}
			if (!HasValidCardsForBuildingInSlot(buildingSlot, industry.BuildingType, currentPlayer, board, gameProgressInformation))
			{
				return false;
			}
			return true;
		}
		return false;
	}

	public List<CantUseBuildActionReason> GetCantUseBuildActionReasonForGivenSlot(BuildingSlot buildingSlot, BaseIndustry industry, Player currentPlayer, BoardGameRules.Entities.Board.Board board, GameProgressInformation gameProgressInformation)
	{
		List<CantUseBuildActionReason> list = new List<CantUseBuildActionReason>();
		Region regionContainingGivenSlot = board.GetRegionContainingGivenSlot(buildingSlot);
		if (industry != null)
		{
			List<CantUseBuildActionReason> buildingProblems = GetBuildingProblems(buildingSlot, industry.BuildingType, currentPlayer, board, gameProgressInformation);
			list.AddRange(buildingProblems);
			int num = industry.MoneyCost;
			if (currentPlayer.Money < num)
			{
				list.Add(CantUseBuildActionReason.NO_MONEY);
			}
			if (industry.NeedsCoal)
			{
				int num2 = CostOfCoalForGivenIndustry(board.GetRegionContainingGivenSlot(buildingSlot), board, industry.CoalCost);
				if (num2 == -1)
				{
					list.Add(CantUseBuildActionReason.NO_COAL);
				}
				else
				{
					num += num2;
				}
				if (currentPlayer.Money < num)
				{
					list.Add(CantUseBuildActionReason.NO_MONEY_COAL);
				}
			}
			if (industry.NeedsIron)
			{
				int amountOfIronInIronWorks = board.GetAmountOfIronInIronWorks();
				if (amountOfIronInIronWorks < industry.IronCost)
				{
					num += board.IronMarket.GetBuyingPrice(industry.IronCost - amountOfIronInIronWorks);
				}
				if (currentPlayer.Money < num)
				{
					list.Add(CantUseBuildActionReason.NO_MONEY_IRON);
				}
			}
			if (!CheckPlayerCanBuildOnlyOneBuildingInRegionDuringCanalEra(regionContainingGivenSlot, gameProgressInformation, currentPlayer.Color, buildingSlot))
			{
				list.Add(CantUseBuildActionReason.ONLY_ONE_INDUSTRY_IN_REGION);
			}
			if (!regionContainingGivenSlot.BuildingSlots.Any((BuildingSlot slot) => slot.BuildedIndustry == null || (slot.BuildedIndustry != null && CanBeOverbuild(slot, slot.BuildedIndustry.BuildingType, currentPlayer, board))))
			{
				list.Add(CantUseBuildActionReason.NO_PLACE_TO_BUILD_INDUSTRY_IN_THE_SLOT);
			}
			if (!HasValidCardsForBuildingInSlot(buildingSlot, industry.BuildingType, currentPlayer, board, gameProgressInformation))
			{
				list.Add(CantUseBuildActionReason.NO_VALID_CARD_TO_BUILD);
			}
			if (!IsSingleSlotOrDoubleSlotButLastPossible(buildingSlot, industry.BuildingType, regionContainingGivenSlot))
			{
				list.Add(CantUseBuildActionReason.FIRST_BUILD_IN_SINGLE_SLOT);
			}
		}
		else
		{
			list.Add(CantUseBuildActionReason.NO_MORE_INDUSTRIES_TO_BUILD);
		}
		return list;
	}

	public Dictionary<BuildingType, HashSet<BuildingSlot>> GetBuildingSlotsWhereBuildingIsPossible(BoardGameRules.Entities.Board.Board board, GameProgressInformation gameProgressInformation)
	{
		return GetBuildingSlotsWhereBuildingIsPossibleStatic(board, gameProgressInformation);
	}

	public static Dictionary<BuildingType, HashSet<BuildingSlot>> GetBuildingSlotsWhereBuildingIsPossibleStatic(BoardGameRules.Entities.Board.Board board, GameProgressInformation gameProgressInformation)
	{
		Dictionary<BuildingType, HashSet<BuildingSlot>> dictionary = new Dictionary<BuildingType, HashSet<BuildingSlot>>();
		foreach (BuildingType value in Enum.GetValues(typeof(BuildingType)))
		{
			dictionary.Add(value, new HashSet<BuildingSlot>());
		}
		Player currentPlayer = gameProgressInformation.CurrentPlayer;
		foreach (BuildingSlot item in board.Regions.SelectMany((Region r) => r.BuildingSlots))
		{
			foreach (BuildingType buildingType in item.BuildingTypes)
			{
				if (!dictionary[buildingType].Contains(item) && IsGivenSlotTheBestOneInGivenRegionForGivenTypeStatic(item, buildingType, board) && CanUseBuildActionForGivenSlot(item, currentPlayer.Mat.GetFirstBuildingOfType(buildingType), currentPlayer, board, gameProgressInformation))
				{
					dictionary[buildingType].Add(item);
					dictionary[BuildingType.Undefined].Add(item);
				}
			}
		}
		return dictionary;
	}

	public IEnumerable<BuildingSlot> GetBuildingSlotsWhereBuildingIsPossible(BoardGameRules.Entities.Board.Board board, GameProgressInformation gameProgressInformation, Func<BuildingType, bool> typeFilter)
	{
		return GetBuildingSlotsWhereBuildingIsPossibleStatic(board, gameProgressInformation, typeFilter);
	}

	public static IEnumerable<BuildingSlot> GetBuildingSlotsWhereBuildingIsPossibleStatic(BoardGameRules.Entities.Board.Board board, GameProgressInformation gameProgressInformation, Func<BuildingType, bool> typeFilter)
	{
		Player curPlayer = gameProgressInformation.CurrentPlayer;
		return from s in board.Regions.SelectMany((Region r) => r.BuildingSlots)
			where s.BuildingTypes.Any((BuildingType t) => typeFilter(t) && IsGivenSlotTheBestOneInGivenRegionForGivenTypeStatic(s, t, board) && CanUseBuildActionForGivenSlot(s, curPlayer.Mat.GetFirstBuildingOfType(t), curPlayer, board, gameProgressInformation))
			where s.BuildingTypes.Any((BuildingType bt) => GetValidCardsForBuildingInSlotStatic(s, bt, curPlayer, board, gameProgressInformation).Any())
			select s;
	}

	public static bool IsCardValidForBuildingInSlot(BaseCard card, BuildingSlot slot, BuildingType buildingType, Player player, BoardGameRules.Entities.Board.Board board, GameProgressInformation gameProgressInformation)
	{
		if (card is IndustryCard industryCard && !industryCard.BuildingTypes.Contains(buildingType))
		{
			return false;
		}
		if (!GetPossibleSlotsToBuildInWithGivenCardByGivenPlayerStatic(card, player.Color, board, gameProgressInformation).Contains(slot))
		{
			return GetPossibleSlotsToOverBuildInWithGivenCardByGivenPlayer(card, player, board).Contains(slot);
		}
		return true;
	}

	public IEnumerable<BaseCard> GetValidCardsForBuildingInSlot(BuildingSlot slot, BuildingType buildingType, Player player, BoardGameRules.Entities.Board.Board board)
	{
		return GetValidCardsForBuildingInSlotStatic(slot, buildingType, player, board, _gameProgressInformation);
	}

	public static IEnumerable<BaseCard> GetValidCardsForBuildingInSlotStatic(BuildingSlot slot, BuildingType buildingType, Player player, BoardGameRules.Entities.Board.Board board, GameProgressInformation gameProgressInformation)
	{
		List<BaseCard> list = new List<BaseCard>();
		foreach (BaseCard item in player.Hand)
		{
			if (IsCardValidForBuildingInSlot(item, slot, buildingType, player, board, gameProgressInformation))
			{
				list.Add(item);
			}
		}
		return list;
	}

	public static bool HasValidCardsForBuildingInSlot(BuildingSlot slot, BuildingType buildingType, Player player, BoardGameRules.Entities.Board.Board board, GameProgressInformation gameProgressInformation)
	{
		foreach (BaseCard item in player.Hand)
		{
			if (IsCardValidForBuildingInSlot(item, slot, buildingType, player, board, gameProgressInformation))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsSingleSlotOrDoubleSlotButLastPossible(BuildingSlot slot, BuildingType buildingType, Region region)
	{
		return IsSingleSlotOrDoubleSlotButLastPossibleStatic(slot, buildingType, region);
	}

	public static bool IsSingleSlotOrDoubleSlotButLastPossibleStatic(BuildingSlot slot, BuildingType buildingType, Region region)
	{
		if (slot.BuildingTypes.Count == 1)
		{
			return true;
		}
		return !region.BuildingSlots.Where((BuildingSlot s) => !object.Equals(s, slot)).Any((BuildingSlot s) => s.BuildedIndustry == null && s.BuildingTypes.Count == 1 && s.BuildingTypes.Contains(buildingType));
	}

	private bool HasPlayerAnyIndustryToBuild(Player player)
	{
		if (player.Mat.GetFirstBuildingOfType(BuildingType.CoalMine) == null && player.Mat.GetFirstBuildingOfType(BuildingType.Manufacturer) == null && player.Mat.GetFirstBuildingOfType(BuildingType.IronWorks) == null && player.Mat.GetFirstBuildingOfType(BuildingType.Pottery) == null && player.Mat.GetFirstBuildingOfType(BuildingType.CottonMill) == null)
		{
			return player.Mat.GetFirstBuildingOfType(BuildingType.Brewery) != null;
		}
		return true;
	}

	private static int CostOfCoalForGivenIndustry(Region region, BoardGameRules.Entities.Board.Board board, int coalNeeded)
	{
		int num = (from s in board.Regions.SelectMany((Region r) => r.BuildingSlots)
			where s.BuildedIndustry is CoalMine coalMine && coalMine.HasCoal
			select (ICoalSource)s.BuildedIndustry into s
			where ConsumingCoal.CanCoalBeSuppliedToRegionFromSource(s, region, board)
			select s).Sum((ICoalSource s) => s.AvailableCoal);
		int num2 = 0;
		if (num < coalNeeded)
		{
			num2 = ((!ConsumingCoal.CanCoalBeSuppliedToRegionFromSource(board.CoalMarket, region, board, ignoreCoalOnBoard: true)) ? (-1) : (num2 + board.CoalMarket.CoalPrice(coalNeeded - num)));
		}
		return num2;
	}

	private static List<CantUseBuildActionReason> GetBuildingProblems(BuildingSlot slot, BuildingType buildingType, Player currentPlayer, BoardGameRules.Entities.Board.Board board, GameProgressInformation gameProgressInformation)
	{
		List<CantUseBuildActionReason> list = new List<CantUseBuildActionReason>();
		if (slot.BuildedIndustry != null && !CanBeOverbuild(slot, buildingType, currentPlayer, board))
		{
			list.Add(CantUseBuildActionReason.NO_PLACE_TO_BUILD_INDUSTRY_IN_THE_SLOT);
		}
		if (!slot.CanBuildIndustryType(buildingType))
		{
			list.Add(CantUseBuildActionReason.INVALID_INDUSTRY_TYPE);
		}
		if (currentPlayer.Mat.GetFirstBuildingOfType(buildingType) == null)
		{
			list.Add(CantUseBuildActionReason.NO_MORE_INDUSTRIES_TO_BUILD);
		}
		BaseIndustry firstBuildingOfType = currentPlayer.Mat.GetFirstBuildingOfType(buildingType);
		if (gameProgressInformation.CurrentGameEra == GameEra.CanalEra && firstBuildingOfType != null && firstBuildingOfType.IsForRailEra)
		{
			list.Add(CantUseBuildActionReason.CAN_BUILD_IN_RAIL_ERA_ONLY);
		}
		if (gameProgressInformation.CurrentGameEra == GameEra.RailEra && firstBuildingOfType != null && firstBuildingOfType.IsForCanalEra)
		{
			list.Add(CantUseBuildActionReason.CAN_BUILD_IN_CANAL_ERA_ONLY);
		}
		return list;
	}

	private static bool CanBeOverbuild(BuildingSlot slot, BuildingType t, Player currentPlayer, BoardGameRules.Entities.Board.Board board)
	{
		if (slot.BuildedIndustry == null || slot.BuildedIndustry.BuildingType != t)
		{
			return false;
		}
		if (!currentPlayer.Mat.GetIndustryList(t).Any())
		{
			return false;
		}
		if (slot.BuildedIndustry.Color != currentPlayer.Color)
		{
			if (t != BuildingType.CoalMine && t != BuildingType.IronWorks)
			{
				return false;
			}
			bool flag = board.GetAmountOfCoalOnBoard() > 0;
			if (t == BuildingType.CoalMine && flag)
			{
				return false;
			}
			bool flag2 = board.GetAmountOfIronOnBoard() > 0;
			if (t == BuildingType.IronWorks && flag2)
			{
				return false;
			}
		}
		return slot.BuildedIndustry.Level < currentPlayer.Mat.GetFirstBuildingOfType(t).Level;
	}
}
