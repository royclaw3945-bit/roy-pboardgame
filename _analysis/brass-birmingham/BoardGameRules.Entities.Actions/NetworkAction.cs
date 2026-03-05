using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Actions.Events;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Links;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Common;
using BoardGameRules.Entities.EventSourcing.Aggregates;
using BoardGameRules.Entities.Game;
using BoardGameRules.Entities.GamePhases;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.NumericalResources;
using BoardGameRules.Entities.NumericalResources.Events;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.Actions;

public class NetworkAction : BaseAction
{
	private int _linkCounter;

	private Link Link;

	private readonly GameProgressInformation _gameProgressInformation;

	public int NeededBeer { get; set; }

	public int NeededCoal { get; set; }

	public NetworkAction(IAggregateRoot aggregateRoot, PlayersTrack playersTrack, BoardGameRules.Entities.Board.Board board, GeneralAction generalAction, GameProgressInformation gameProgressInformation)
		: base(aggregateRoot, playersTrack, board, generalAction)
	{
		_gameProgressInformation = gameProgressInformation;
		aggregateRoot.RegisterApplyEvent<LinkPlaced>(ApplyEvent);
	}

	public void PlaceLink(Link link)
	{
		if (link == null)
		{
			throw new InvalidOperationException("Given link is null.");
		}
		if (_gameProgressInformation.CurrentGameEra == GameEra.CanalEra && link is Rail)
		{
			throw new InvalidOperationException("Can't place rail link in canal era.");
		}
		if (_gameProgressInformation.CurrentGameEra == GameEra.RailEra && link is Canal)
		{
			throw new InvalidOperationException("Can't place canal link in rail era.");
		}
		if (_gameProgressInformation.CurrentGameEra == GameEra.CanalEra && _linkCounter == 1)
		{
			throw new InvalidOperationException("You can only place one link per action in canal era.");
		}
		if (_gameProgressInformation.CurrentGameEra == GameEra.RailEra && _linkCounter == 2)
		{
			throw new InvalidOperationException("You can only place two links per action in rail era.");
		}
		Link = link;
		_linkCounter++;
		base.AggregateRoot.ApplyEvent(new LinkPlaced(_player.Color, link.LinkId));
		if (_gameProgressInformation.CurrentGameEra == GameEra.RailEra)
		{
			NeededCoal++;
			if (_linkCounter == 1)
			{
				base.AggregateRoot.ApplyEvent(new MoneySpent(_player.Color, 5));
				return;
			}
			NeededBeer = 1;
			base.AggregateRoot.ApplyEvent(new MoneySpent(_player.Color, 10));
		}
		else
		{
			base.AggregateRoot.ApplyEvent(new MoneySpent(_player.Color, 3));
		}
	}

	public override void SupplyCoal(ICoalSource coalSource, int amount)
	{
		base.GeneralAction.ConsumingCoal.SupplyCoal(coalSource, amount, NeededCoal, _player, new List<Region> { Link.Region1, Link.Region2, Link.Region3 });
		NeededCoal -= amount;
	}

	public override void SupplyBeer(IBeerSource beerSource, int amount)
	{
		base.GeneralAction.ConsumingBeer.SupplyBeer(beerSource, amount, NeededBeer, _player, new List<Region> { Link.Region1, Link.Region2, Link.Region3 });
		NeededBeer -= amount;
	}

	public List<CantUseLinkActionReason> GetNetworkActionNotPossibleReasons(BoardGameRules.Entities.Board.Board board, GameProgressInformation gameProgressInformation)
	{
		List<CantUseLinkActionReason> list = new List<CantUseLinkActionReason>();
		Player currentPlayer = gameProgressInformation.CurrentPlayer;
		if (GetValidConnectionSlots(board, gameProgressInformation).Any())
		{
			return list;
		}
		if (!currentPlayer.HasLinkTiles())
		{
			list.Add(CantUseLinkActionReason.NO_MORE_CONNECTIONS_TO_BUILD);
			return list;
		}
		int baseConnectionCost = GetBaseConnectionCost();
		if (currentPlayer.Money < baseConnectionCost)
		{
			list.Add(CantUseLinkActionReason.NO_MONEY);
		}
		bool flag = true;
		bool flag2 = gameProgressInformation.CurrentGameEra == GameEra.RailEra;
		bool flag3 = gameProgressInformation.CurrentGameEra == GameEra.RailEra && _linkCounter == 1;
		IEnumerable<Link> enumerable = board.Regions.SelectMany((Region r) => r.Links).Distinct();
		IList<Region> yourNetwork = board.GetYourNetwork(currentPlayer.Color);
		foreach (Link item in enumerable)
		{
			if (item.HasOwner || !IsConnectionBuildableInCurrentEra(item, gameProgressInformation) || (!yourNetwork.Contains(item.Region1) && !yourNetwork.Contains(item.Region2) && !yourNetwork.Contains(item.Region3)))
			{
				continue;
			}
			flag = false;
			if (flag2 && GetValidCoalSources(new Region[3] { item.Region1, item.Region2, item.Region3 }, board).Any())
			{
				flag2 = false;
				if (!flag3)
				{
					break;
				}
				if (GetValidBeerSources(new Region[3] { item.Region1, item.Region2, item.Region3 }, board, _player).Any())
				{
					flag3 = false;
					break;
				}
			}
		}
		if (flag)
		{
			list.Add(CantUseLinkActionReason.NO_PLACE_TO_BUILD_CONNECTION);
			return list;
		}
		if (flag2)
		{
			list.Add(CantUseLinkActionReason.NO_COAL);
		}
		if (flag3)
		{
			list.Add(CantUseLinkActionReason.NO_BEER);
		}
		if (!flag2 && !list.Contains(CantUseLinkActionReason.NO_MONEY))
		{
			list.Add(CantUseLinkActionReason.NO_MONEY_COAL);
		}
		return list;
	}

	public bool IsBeerNeeded()
	{
		if (Link == null)
		{
			throw new InvalidOperationException("Link is not chosen.");
		}
		return NeededBeer != 0;
	}

	public bool IsCoalNeeded()
	{
		if (Link == null)
		{
			throw new InvalidOperationException("Link is not chosen.");
		}
		return NeededCoal != 0;
	}

	public List<Link> GetValidConnectionSlots(BoardGameRules.Entities.Board.Board board, GameProgressInformation gameProgressInformation)
	{
		List<Link> list = new List<Link>();
		Player currentPlayer = gameProgressInformation.CurrentPlayer;
		if (currentPlayer.LinkTileCount <= 0)
		{
			return list;
		}
		IEnumerable<Link> enumerable = from l in board.Regions.SelectMany((Region r) => r.Links).Distinct()
			where !l.HasOwner
			select l;
		IList<Region> yourNetwork = board.GetYourNetwork(currentPlayer.Color);
		foreach (Link item in enumerable)
		{
			int connectionCost = GetConnectionCost(item);
			if (!item.HasOwner && IsConnectionBuildableInCurrentEra(item, gameProgressInformation) && currentPlayer.Money >= connectionCost && (gameProgressInformation.CurrentGameEra != GameEra.RailEra || (GetValidCoalSources(new Region[3] { item.Region1, item.Region2, item.Region3 }, board).Any() && (_linkCounter != 1 || GetValidBeerSources(new Region[3] { item.Region1, item.Region2, item.Region3 }, board, currentPlayer).Any()))) && (yourNetwork.Contains(item.Region1) || yourNetwork.Contains(item.Region2) || yourNetwork.Contains(item.Region3)))
			{
				list.Add(item);
			}
		}
		return list;
	}

	private int GetBaseConnectionCost()
	{
		if (_gameProgressInformation.CurrentGameEra == GameEra.CanalEra)
		{
			return 3;
		}
		if (_linkCounter != 0)
		{
			return 10;
		}
		return 5;
	}

	public int GetConnectionCost(Link link)
	{
		if (_gameProgressInformation.CurrentGameEra == GameEra.CanalEra)
		{
			return 3;
		}
		int num = ((_linkCounter == 0) ? 5 : 10);
		List<Region> list = new List<Region> { link.Region1, link.Region2 };
		if (link.Region3 != null)
		{
			list.Add(link.Region3);
		}
		return num + CostOfCoalForGivenLink(list, 1);
	}

	private int CostOfCoalForGivenLink(List<Region> regions, int coalNeeded)
	{
		HashSet<Region> hashSet = new HashSet<Region>();
		foreach (Region region in regions)
		{
			if (!hashSet.Contains(region))
			{
				hashSet.Add(region);
			}
			foreach (Region connectedRegion in base.Board.GetConnectedRegions(region))
			{
				if (!hashSet.Contains(connectedRegion))
				{
					hashSet.Add(connectedRegion);
				}
			}
		}
		int num = 0;
		foreach (Region item in hashSet)
		{
			foreach (BuildingSlot buildingSlot in item.BuildingSlots)
			{
				if (buildingSlot.BuildedIndustry is CoalMine { HasCoal: not false } coalMine)
				{
					num += coalMine.AvailableCoal;
				}
			}
		}
		int num2 = 0;
		if (num < coalNeeded)
		{
			num2 = ((!hashSet.Any((Region r) => r is BorderRegion)) ? 100000 : (num2 + base.Board.CoalMarket.CoalPrice(coalNeeded - num)));
		}
		return num2;
	}

	private bool IsConnectionBuildableInCurrentEra(Link c, GameProgressInformation gameProgressInformation)
	{
		GameEra currentGameEra = gameProgressInformation.CurrentGameEra;
		int num = (int)currentGameEra;
		if (num != 0)
		{
			if (num == 1 && c is Rail)
			{
				goto IL_0024;
			}
		}
		else if (c is Canal)
		{
			goto IL_0024;
		}
		return false;
		IL_0024:
		return true;
	}

	private IEnumerable<ICoalSource> GetValidCoalSources(Region[] regions, BoardGameRules.Entities.Board.Board board)
	{
		List<ICoalSource> list = (from s in board.GetUnflippedCoalMines().Select((Func<CoalMine, ICoalSource>)((CoalMine cm) => cm)).ToList()
			where regions.Any((Region r) => base.GeneralAction.ConsumingCoal.CanCoalBeSuppliedToRegionFromSource(s, r))
			select s).ToList();
		if (regions.Any((Region r) => base.GeneralAction.ConsumingCoal.CanCoalBeSuppliedToRegionFromSource(board.CoalMarket, r)))
		{
			list.Add(board.CoalMarket);
		}
		if (regions.Any((Region r) => r is BorderRegion))
		{
			list.Add(board.CoalMarket);
		}
		return list;
	}

	private IEnumerable<IBeerSource> GetValidBeerSources(Region[] regions, BoardGameRules.Entities.Board.Board board, Player player)
	{
		return (from s in board.GetUnflippedBreweries().Select((Func<Brewery, IBeerSource>)((Brewery b) => b)).ToList()
			where regions.Where((Region r) => r != null).Any((Region r) => base.GeneralAction.ConsumingBeer.CanBeerBeSuppliedToSlotFromSource(s, r, player))
			select s).ToList();
	}

	public override void ResetAction()
	{
		base.ResetAction();
		NeededBeer = 0;
		NeededCoal = 0;
		_linkCounter = 0;
	}

	private void ApplyEvent(LinkPlaced linkPlaced)
	{
		Link linkById = base.Board.GetLinkById(linkPlaced.LinkId);
		Player playerByColor = base.PlayersTrack.GetPlayerByColor(linkPlaced.Player);
		linkById.SetOwner(playerByColor);
		playerByColor.TakeLinkTiles();
	}
}
