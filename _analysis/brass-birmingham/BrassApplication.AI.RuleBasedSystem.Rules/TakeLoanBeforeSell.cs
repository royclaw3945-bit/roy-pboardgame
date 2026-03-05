using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.Players;
using BrassApplication.AI.AIHelpers;
using BrassApplication.Game;

namespace BrassApplication.AI.RuleBasedSystem.Rules;

internal class TakeLoanBeforeSell : BaseRule
{
	public TakeLoanBeforeSell()
	{
		base.Id = RuleId.TakeLoanBeforeSell;
		base.AILevel = AIType.Hard;
	}

	public override int GetPriority(BrassApplicationGame brassApplicationGame)
	{
		return 2620;
	}

	public override bool IsApplicable(BrassApplicationGame brassApplicationGame)
	{
		Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
		List<BuildingSlot> validBuildingSlotsForSellingIndustries = brassApplicationGame.Game.TurnFlow.SellAction.GetValidBuildingSlotsForSellingIndustries(brassApplicationGame.Game.Board, currentPlayer);
		bool flag = !brassApplicationGame.Game.TurnFlow.LoanAction.GetLoanActionNotPossibleReasons(brassApplicationGame.Game.GameProgressInformation).Any();
		if (validBuildingSlotsForSellingIndustries.Count > 0 && flag && currentPlayer.Money < 8 && currentPlayer.IncomeLevel >= 0)
		{
			return currentPlayer.IncomeLevel <= 3;
		}
		return false;
	}

	public override Action MakeAction(BrassApplicationGame brassApplicationGame, bool printOnly)
	{
		return delegate
		{
			Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
			LoanAction loanAction = brassApplicationGame.Game.TurnFlow.StartLoanAction();
			BaseCard baseCard = BoardHelper.LowestValueCard(currentPlayer.Hand, brassApplicationGame);
			BaseRule.Logger.Info(BaseRule.LOG_PREFIX + GetPriority(brassApplicationGame) + ": " + base.Id.ToString() + BaseRule.LOG_NEWLINE + "Take a Loan with a card: " + baseCard);
			if (!printOnly)
			{
				loanAction.DiscardCard(baseCard, currentPlayer);
				loanAction.Loan(currentPlayer.Color);
			}
		};
	}

	public override void PrintAction(BrassApplicationGame brassApplicationGame)
	{
		MakeAction(brassApplicationGame, printOnly: true)();
	}
}
