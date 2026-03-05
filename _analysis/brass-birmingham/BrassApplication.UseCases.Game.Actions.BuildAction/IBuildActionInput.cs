using BrassApplication.UseCases.Game.Actions.BaseAction;
using BrassApplication.UseCases.Game.Actions.CoalDelivery;
using BrassApplication.UseCases.Game.Actions.IronDelivery;
using BrassApplication.UseCases.Game.Replay;

namespace BrassApplication.UseCases.Game.Actions.BuildAction;

public interface IBuildActionInput : IBaseActionInput, IActionInProgressCancelDelegate, IReplayDelegate, IIronDeliveryResultDelegate, ICoalDeliveryResultDelegate, ISelectBuildingDelegate, ISelectCardDelegate
{
	void StartBuildAction();
}
