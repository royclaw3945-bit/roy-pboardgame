using BrassApplication.UseCases.Game.Actions.BaseAction;
using BrassApplication.UseCases.Game.Replay;

namespace BrassApplication.UseCases.Game.Actions.ScoutAction;

public interface IScoutActionInput : IBaseActionInput, IActionInProgressCancelDelegate, IReplayDelegate, ISelectCardDelegate
{
	void StartScoutAction();
}
