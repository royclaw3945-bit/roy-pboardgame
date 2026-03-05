using BrassApplication.UseCases.Game.Actions.BaseAction;
using BrassApplication.UseCases.Game.Actions.BeerDelivery;
using BrassApplication.UseCases.Game.Replay;

namespace BrassApplication.UseCases.Game.Actions.SellAction;

public interface ISellActionInput : IBaseActionInput, IActionInProgressCancelDelegate, IReplayDelegate, ISelectCardDelegate, ISelectIndustryDelegate, ISelectIndustryOfTypeDelegate, IBeerDeliveryResultDelegate
{
	void EndSellAction();

	void SelectNextIndustryToSell();
}
