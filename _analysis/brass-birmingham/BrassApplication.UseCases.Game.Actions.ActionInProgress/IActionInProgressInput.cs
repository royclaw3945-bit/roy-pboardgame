namespace BrassApplication.UseCases.Game.Actions.ActionInProgress;

public interface IActionInProgressInput
{
	void CancelAction();

	void FinishAction();
}
