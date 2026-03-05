using BrassApplication.UseCases.Game.Replay;

namespace BrassApplication.UseCases.Game.Actions.BaseAction;

public interface IBaseActionInput : IActionInProgressCancelDelegate, IReplayDelegate
{
}
