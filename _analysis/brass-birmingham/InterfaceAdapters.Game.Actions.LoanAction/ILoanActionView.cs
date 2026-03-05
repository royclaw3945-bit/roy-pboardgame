using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.Actions.LoanAction;

public interface ILoanActionView : IBaseView
{
	void ShowIncomePanel(LoanActionViewModel loanActionViewModel, bool isDelayed);

	void SetLoanButtonVisibility(bool isActive);

	IAnimation CreateLoanActionAnimation(LoanActionViewModel loanActionViewModel, float delay);
}
