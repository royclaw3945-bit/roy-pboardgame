using System.Linq;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.Common;
using BoardGameRules.Entities.Players;

namespace BrassApplication.AI.AIHelpers;

public static class DevelopActionAIHelper
{
	public static void DevelopIndustry(DevelopAction developAction, BaseCard card, BuildingType buildingType1, BuildingType buildingType2, Board board, Player player)
	{
		developAction.DiscardCard(card, player);
		developAction.Develop(buildingType1);
		IIronSource ironSource = board.GetUnflippedIronWorks().FirstOrDefault();
		IIronSource ironSource2 = ironSource ?? board.IronMarket;
		developAction.SupplyIron(ironSource2, 1);
		if (buildingType2 != BuildingType.Undefined)
		{
			ironSource = board.GetUnflippedIronWorks().FirstOrDefault();
			IIronSource ironSource3 = ironSource ?? board.IronMarket;
			if (ironSource3.IronPrice() <= player.Money)
			{
				developAction.Develop(buildingType2);
				developAction.SupplyIron(ironSource3, 1);
			}
		}
	}
}
