using System;
using System.Linq;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.GamePhases;
using BoardGameRules.Entities.Players;
using BrassApplication.AI.AIHelpers;
using BrassApplication.Game;

namespace BrassApplication.AI.RuleBasedSystem.Rules;

internal class TakeLoan : BaseRule
{
	public TakeLoan()
	{
		base.Id = RuleId.TakeLoan;
		base.AILevel = AIType.Easy;
	}

	public override int GetPriority(BrassApplicationGame brassApplicationGame)
	{
		Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
		if (brassApplicationGame.Game.GameProgressInformation.CurrentGameEra == GameEra.CanalEra)
		{
			if (currentPlayer.Money < 4)
			{
				return 2300;
			}
		}
		else if (currentPlayer.Money < 5)
		{
			return 2300;
		}
		if (currentPlayer.Money < 12 && brassApplicationGame.Game.TurnFlow.GetRemainingActionsCount() == 2)
		{
			return 1610;
		}
		return -450;
	}

	public override bool IsApplicable(BrassApplicationGame brassApplicationGame)
	{
		if (brassApplicationGame.Game.GameProgressInformation.CurrentPlayer.IncomeLevel >= 15)
		{
			return false;
		}
		if (!brassApplicationGame.Game.TurnFlow.LoanAction.GetLoanActionNotPossibleReasons(brassApplicationGame.Game.GameProgressInformation).Any())
		{
			return GetPriority(brassApplicationGame) > 0;
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
