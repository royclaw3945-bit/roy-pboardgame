using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Actions.Events;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Merchants;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Common;
using BoardGameRules.Entities.EventSourcing.Aggregates;
using BoardGameRules.Entities.Game;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.NumericalResources;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.Actions;

public class SellAction : BaseAction
{
	private BaseIndustry _industry;

	private bool inProgress;

	public int NeededBeer { get; private set; }

	public SellAction(IAggregateRoot aggregateRoot, PlayersTrack playersTrack, BoardGameRules.Entities.Board.Board board, GeneralAction generalAction)
		: base(aggregateRoot, playersTrack, board, generalAction)
	{
	}

	public void StartSellProcedure(BaseIndustry industry)
	{
		if (industry == null)
		{
			throw new InvalidOperationException("Chosen industry is null.");
		}
		if (!CanIndustryBeSold(industry, _player))
		{
			throw new InvalidOperationException("Chosen industry can't be sold.");
		}
		if (!inProgress)
		{
			_industry = industry;
			NeededBeer = ((BaseSellableIndustry)industry).CostOfBeerBarrelsToSellIndustry;
			inProgress = true;
		}
		if (!IsBeerNeeded())
		{
			SellIndustry();
		}
	}

	public override void SupplyBeer(IBeerSource beerSource, int amount)
	{
		base.GeneralAction.ConsumingBeer.SupplyBeer(beerSource, amount, NeededBeer, _player, new List<Region> { base.Board.GetRegionContainingGivenIndustry(_industry) });
		NeededBeer -= amount;
	}

	public bool IsBeerNeeded()
	{
		return NeededBeer != 0;
	}

	public List<CantUseSellActionReason> GetSellActionNotPossibleReasons(BoardGameRules.Entities.Board.Board board, GameProgressInformation gameProgressInformation)
	{
		Player currentPlayer = gameProgressInformation.CurrentPlayer;
		List<CantUseSellActionReason> list = new List<CantUseSellActionReason>();
		List<BuildingSlot> list2 = board.Regions.SelectMany((Region r) => r.BuildingSlots.Where((BuildingSlot s) => s.BuildedIndustry is BaseSellableIndustry && !s.BuildedIndustry.IsFlipped && s.BuildedIndustry.Color.Equals(currentPlayer.Color))).ToList();
		if (!list2.Any())
		{
			list.Add(CantUseSellActionReason.NO_INDUSTRY_TO_SELL);
			return list;
		}
		IEnumerable<Brewery> unflippedBreweries = board.GetUnflippedBreweries();
		bool flag = true;
		bool flag2 = false;
		foreach (BuildingSlot slot in list2)
		{
			IEnumerable<BorderRegion> source = from r in board.GetBorderRegions((BorderRegion r) => true)
				where r.MerchantSlots.Any((MerchantSlot s) => s.MerchantTile != null && s.MerchantTile.BuildingTypes.Contains(slot.BuildedIndustry.BuildingType))
				select r;
			Region regionWithIndustry = board.GetRegionContainingGivenIndustry(slot.BuildedIndustry);
			List<BorderRegion> source2 = source.Where((BorderRegion r) => board.DoesRouteExists(r, regionWithIndustry)).ToList();
			BaseSellableIndustry baseSellableIndustry = slot.BuildedIndustry as BaseSellableIndustry;
			if (!source2.Any())
			{
				continue;
			}
			flag = false;
			if (baseSellableIndustry != null && baseSellableIndustry.CostOfBeerBarrelsToSellIndustry == 0)
			{
				flag2 = true;
				break;
			}
			int num = (source2.Any((BorderRegion r) => r.MerchantSlots.Any((MerchantSlot s) => s.MerchantTile.BuildingTypes.Contains(slot.BuildedIndustry.BuildingType) && s.HasBeer)) ? 1 : 0);
			int num2 = unflippedBreweries.Where((Brewery b) => base.GeneralAction.ConsumingBeer.CanBeerBeSuppliedToSlotFromSource(b, regionWithIndustry, currentPlayer)).Sum((Brewery b) => b.AvailableBeer);
			if (num + num2 >= baseSellableIndustry?.CostOfBeerBarrelsToSellIndustry)
			{
				flag2 = true;
				break;
			}
		}
		if (flag)
		{
			list.Add(CantUseSellActionReason.NO_ROUTE_TO_MARKET);
		}
		if (!flag2 && !flag)
		{
			list.Add(CantUseSellActionReason.NO_BEER);
		}
		return list;
	}

	public List<BuildingSlot> GetValidBuildingSlotsForSellingIndustries(BoardGameRules.Entities.Board.Board board, Player curPlayer)
	{
		List<BuildingSlot> list = new List<BuildingSlot>();
		foreach (BuildingSlot item in board.Regions.SelectMany((Region r) => r.BuildingSlots.Where((BuildingSlot s) => s.BuildedIndustry is BaseSellableIndustry)))
		{
			if (item.BuildedIndustry.Color == curPlayer.Color && !item.BuildedIndustry.IsFlipped && CanIndustryBeSold(item.BuildedIndustry, curPlayer))
			{
				list.Add(item);
			}
		}
		return list;
	}

	private bool CanIndustryBeSold(BaseIndustry industry, Player player)
	{
		if (!(industry is BaseSellableIndustry))
		{
			throw new InvalidOperationException("Chosen industry is of wrong type.");
		}
		List<BorderRegion> borderRegions = base.Board.GetBorderRegions((BorderRegion r) => r.MerchantSlots.Any((MerchantSlot s) => s.MerchantTile != null && s.MerchantTile.BuildingTypes.Contains(industry.BuildingType)));
		Region regionWithIndustry = base.Board.GetRegionContainingGivenIndustry(industry);
		List<BorderRegion> source = borderRegions.Where((BorderRegion r) => base.Board.DoesRouteExists(r, regionWithIndustry)).ToList();
		bool flag = false;
		if (source.Any())
		{
			IEnumerable<Brewery> source2 = from b in base.Board.GetUnflippedBreweries()
				where base.GeneralAction.ConsumingBeer.CanBeerBeSuppliedToSlotFromSource(b, regionWithIndustry, player)
				select b;
			flag = source.Sum((BorderRegion m) => m.MerchantSlots.Sum((MerchantSlot ms) => (ms.MerchantTile.BuildingTypes.Contains(industry.BuildingType) && ms.HasBeer) ? 1 : 0)) + source2.Sum((Brewery b) => b.AvailableBeer) >= ((BaseSellableIndustry)industry).CostOfBeerBarrelsToSellIndustry;
		}
		return source.Any() && flag;
	}

	public void SellIndustry()
	{
		if (inProgress)
		{
			RegionName name = base.Board.GetRegionContainingGivenIndustry(_industry).Name;
			base.AggregateRoot.ApplyEvent(new IndustryFlipped(_industry.UniqueId, name));
			_player.ChangeIncomeMarkerPositionBy(_industry.IncomeIncrease, _industry.UniqueId);
			inProgress = false;
		}
	}
}
