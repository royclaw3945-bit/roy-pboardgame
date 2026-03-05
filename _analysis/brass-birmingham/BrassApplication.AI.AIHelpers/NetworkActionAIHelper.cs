using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.Links;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.Common;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.Players;

namespace BrassApplication.AI.AIHelpers;

public static class NetworkActionAIHelper
{
	public static void PlaceLink(NetworkAction networkAction, BaseCard card, Link link, Board board, Player player, bool is2nd = false)
	{
		if (!is2nd)
		{
			networkAction.DiscardCard(card, player);
		}
		networkAction.PlaceLink(link);
		while (networkAction.IsCoalNeeded())
		{
			List<Region> targetRegions = new List<Region> { link.Region1, link.Region2, link.Region3 };
			ICoalSource coalSource = (from c in board.GetUnflippedCoalMines()
				where targetRegions.Any((Region r) => networkAction.GeneralAction.ConsumingCoal.CanCoalBeSuppliedToRegionFromSource(c, r))
				select c).FirstOrDefault();
			ICoalSource coalSource2 = coalSource ?? board.CoalMarket;
			networkAction.SupplyCoal(coalSource2, 1);
		}
		while (networkAction.IsBeerNeeded())
		{
			List<Region> targetRegions2 = new List<Region> { link.Region1, link.Region2 };
			if (link.Region3 != null)
			{
				targetRegions2.Add(link.Region3);
			}
			Brewery beerSource = (from b in board.GetUnflippedBreweries()
				where targetRegions2.Any((Region r) => networkAction.GeneralAction.ConsumingBeer.CanBeerBeSuppliedToSlotFromSource(b, r, player))
				select b).First();
			networkAction.SupplyBeer(beerSource, 1);
		}
		if (!is2nd)
		{
			Link link2 = BoardHelper.Find2ndLink();
			if (link2 != null)
			{
				PlaceLink(networkAction, null, link2, board, player, is2nd: true);
			}
		}
	}
}
