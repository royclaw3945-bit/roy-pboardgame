using BrassApplication.UseCases.Common;

namespace BrassApplication.UseCases.Game.Actions.ActionInProgress;

public class ActionInProgressUseCase : IUseCase, IActionInProgressInput
{
	private readonly IActionInProgressOutput _presenter;

	private IActionInProgressCancelDelegate _cancelActionDelegate;

	private IActionInProgressFinishDelegate _finishDelegate;

	public ActionInProgressUseCase(IActionInProgressOutput presenter)
	{
		_presenter = presenter;
	}

	public void CancelAction()
	{
		_cancelActionDelegate?.CancelAction();
	}

	public void FinishAction()
	{
		_finishDelegate?.FinishAction();
	}

	public void SetCancelActionDelegate(IActionInProgressCancelDelegate cancelActionDelegate)
	{
		_cancelActionDelegate = cancelActionDelegate;
	}

	public void SetFinishActionDelegate(IActionInProgressFinishDelegate finishDelegate)
	{
		_finishDelegate = finishDelegate;
	}
}
