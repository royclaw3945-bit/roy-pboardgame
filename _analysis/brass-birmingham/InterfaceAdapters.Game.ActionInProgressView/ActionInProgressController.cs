using BrassApplication.UseCases.Game.Actions.ActionInProgress;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.ActionInProgressView;

public class ActionInProgressController : IController
{
	private readonly IActionInProgressInput _useCase;

	public ActionInProgressController(IActionInProgressInput actionInProgressUseCase)
	{
		_useCase = actionInProgressUseCase;
	}

	public void OnViewCreated()
	{
	}

	public void OnViewDestroyed()
	{
	}

	public void OnCancelClicked()
	{
		_useCase.CancelAction();
	}

	public void OnFinishClicked()
	{
		_useCase.FinishAction();
	}
}
