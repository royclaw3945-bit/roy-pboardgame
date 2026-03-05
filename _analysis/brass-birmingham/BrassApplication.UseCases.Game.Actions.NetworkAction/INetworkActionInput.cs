using BrassApplication.UseCases.Game.Actions.BaseAction;
using BrassApplication.UseCases.Game.Actions.BeerDelivery;
using BrassApplication.UseCases.Game.Actions.CoalDelivery;
using BrassApplication.UseCases.Game.Replay;

namespace BrassApplication.UseCases.Game.Actions.NetworkAction;

public interface INetworkActionInput : IBaseActionInput, IActionInProgressCancelDelegate, IReplayDelegate, IBeerDeliveryResultDelegate, ICoalDeliveryResultDelegate, ILinkSelectedDelegate, ISelectCardDelegate
{
	void StartNetworkAction();

	void EndNetworkAction();

	void SelectSecondLink();
}
