using System;
using System.Linq;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.Players;
using BrassApplication.AI.AIHelpers;
using BrassApplication.Game;

namespace BrassApplication.AI.RuleBasedSystem.Rules;

internal class TakeLoanBeforeCoal : BaseRule
{
	public TakeLoanBeforeCoal()
	{
		base.Id = RuleId.TakeLoanBeforeCoal;
		base.AILevel = AIType.Hard;
	}

	public override int GetPriority(BrassApplicationGame brassApplicationGame)
	{
		return 2410;
	}

	public override bool IsApplicable(BrassApplicationGame brassApplicationGame)
	{
		Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
		bool flag = !brassApplicationGame.Game.TurnFlow.LoanAction.GetLoanActionNotPossibleReasons(brassApplicationGame.Game.GameProgressInformation).Any();
		if (BuildCoalForHighDemand.ApplicableSlot(brassApplicationGame) != null && BuildCoalForHighDemand.GetPriorityStatic(brassApplicationGame) == 2400 && flag && brassApplicationGame.Game.TurnFlow.GetRemainingActionsCount() >= 2 && currentPlayer.Money < 20)
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
