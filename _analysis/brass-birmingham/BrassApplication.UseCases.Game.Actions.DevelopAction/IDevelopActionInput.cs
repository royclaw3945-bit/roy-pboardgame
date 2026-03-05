using BrassApplication.UseCases.Game.Actions.BaseAction;
using BrassApplication.UseCases.Game.Actions.IronDelivery;
using BrassApplication.UseCases.Game.Replay;

namespace BrassApplication.UseCases.Game.Actions.DevelopAction;

public interface IDevelopActionInput : IBaseActionInput, IActionInProgressCancelDelegate, IReplayDelegate, ISelectCardDelegate, ISelectIndustryOfTypeDelegate, IIronDeliveryResultDelegate, IActionInProgressFinishDelegate
{
	void StartDevelopAction();

	void ConfirmNotEnoughResourcesForSecondDevelop();
}
