using System;
using System.Collections.Generic;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.GamePhases;
using BoardGameRules.Entities.Players;
using BrassApplication.AI.AIHelpers;
using BrassApplication.Game;

namespace BrassApplication.AI.RuleBasedSystem.Rules;

internal class SellIndustryToMerchant : BaseRule
{
	public SellIndustryToMerchant()
	{
		base.Id = RuleId.SellIndustryToMerchant;
		base.AILevel = AIType.Easy;
	}

	public override int GetPriority(BrassApplicationGame brassApplicationGame)
	{
		Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
		brassApplicationGame.Game.TurnFlow.SellAction.GetValidBuildingSlotsForSellingIndustries(brassApplicationGame.Game.Board, currentPlayer);
		if (brassApplicationGame.Game.GameProgressInformation.CurrentGameEra == GameEra.CanalEra)
		{
			if (currentPlayer.Money > 22)
			{
				return 2130;
			}
			return 2610;
		}
		if (currentPlayer.Money < 23)
		{
			return 2610;
		}
		return 2130;
	}

	public override bool IsApplicable(BrassApplicationGame brassApplicationGame)
	{
		Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
		return brassApplicationGame.Game.TurnFlow.SellAction.GetValidBuildingSlotsForSellingIndustries(brassApplicationGame.Game.Board, currentPlayer).Count > 0;
	}

	public override Action MakeAction(BrassApplicationGame brassApplicationGame, bool printOnly)
	{
		return delegate
		{
			Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
			SellAction sellAction = brassApplicationGame.Game.TurnFlow.StartSellAction();
			List<BuildingSlot> possibleSlotsToSell = SellActionAIHelper.PossibleSlotsToSell(sellAction, brassApplicationGame.Game.Board, currentPlayer);
			BaseCard baseCard = BoardHelper.LowestValueCard(currentPlayer.Hand, brassApplicationGame);
			BuildingSlot buildingSlot = SellActionAIHelper.SlotToSell(sellAction, baseCard, brassApplicationGame.Game.Board, currentPlayer, possibleSlotsToSell);
			Region regionContainingGivenSlot = brassApplicationGame.Game.Board.GetRegionContainingGivenSlot(buildingSlot);
			BaseRule.Logger.Info(string.Concat(BaseRule.LOG_PREFIX, GetPriority(brassApplicationGame), ": ", base.Id.ToString(), BaseRule.LOG_NEWLINE, "Use card: ", baseCard.ToString(), "; Sell region: ", regionContainingGivenSlot, "; Sell slot: ", buildingSlot));
			if (!printOnly)
			{
				SellActionAIHelper.SellIndustries(sellAction, baseCard, brassApplicationGame.Game.Board, currentPlayer, brassApplicationGame.Game.DevelopMerchantBonus, BaseRule.Logger);
			}
		};
	}

	public override void PrintAction(BrassApplicationGame brassApplicationGame)
	{
		MakeAction(brassApplicationGame, printOnly: true)();
	}
}
