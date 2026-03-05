using BoardGameRules.Entities.Players;
using BrassApplication.UseCases.Game.Actions.LoanAction;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.IncomeTrack;

namespace InterfaceAdapters.Game.Actions.LoanAction;

public class LoanActionController : IController, IIncomeTrackController
{
	private readonly ILoanActionInput _useCase;

	public LoanActionController(ILoanActionInput loanActionUseCase)
	{
		_useCase = loanActionUseCase;
	}

	public void OnViewCreated()
	{
	}

	public void OnViewDestroyed()
	{
	}

	public void OnTakeLoanClicked()
	{
		_useCase.TakeLoan();
	}

	public void HideIncomePanel()
	{
		_useCase.HideIncomePanel();
	}

	public void OnMouseEnter(string element, float x, float y, PlayerColor? playerColor = null)
	{
		_useCase.ShowTooltip(element, x, y, playerColor);
	}

	public void OnMouseExit()
	{
		_useCase.HideTooltip();
	}
}
