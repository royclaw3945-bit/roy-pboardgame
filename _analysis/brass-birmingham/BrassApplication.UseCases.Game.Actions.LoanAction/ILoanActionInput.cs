using BoardGameRules.Entities.Players;
using BrassApplication.UseCases.Game.Actions.BaseAction;
using BrassApplication.UseCases.Game.Replay;

namespace BrassApplication.UseCases.Game.Actions.LoanAction;

public interface ILoanActionInput : IBaseActionInput, IActionInProgressCancelDelegate, IReplayDelegate, ISelectCardDelegate
{
	void StartLoanAction();

	void TakeLoan();

	void HideIncomePanel();

	void ShowTooltip(string element, float x, float y, PlayerColor? playerColor);

	void HideTooltip();
}
