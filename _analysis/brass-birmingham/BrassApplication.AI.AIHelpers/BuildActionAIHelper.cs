using System.Linq;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.Common;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.Players;

namespace BrassApplication.AI.AIHelpers;

public static class BuildActionAIHelper
{
	public static void BuildIndustry(BuildAction buildAction, BaseCard card, BuildingSlot buildingSlot, BuildingType buildingType, Board board, Player player)
	{
		Region targetRegion = board.GetRegionContainingGivenSlot(buildingSlot);
		buildAction.DiscardCard(card, player);
		buildAction.Build(buildingSlot, buildingType);
		while (buildAction.IsIronNeeded())
		{
			IIronSource ironSource = board.GetUnflippedIronWorks().FirstOrDefault();
			IIronSource ironSource2 = ironSource ?? board.IronMarket;
			buildAction.SupplyIron(ironSource2, 1);
		}
		while (buildAction.IsCoalNeeded())
		{
			ICoalSource coalSource = (from c in board.GetUnflippedCoalMines()
				where buildAction.GeneralAction.ConsumingCoal.CanCoalBeSuppliedToRegionFromSource(c, targetRegion)
				select c).FirstOrDefault();
			ICoalSource coalSource2 = coalSource ?? board.CoalMarket;
			buildAction.SupplyCoal(coalSource2, 1);
		}
	}
}
