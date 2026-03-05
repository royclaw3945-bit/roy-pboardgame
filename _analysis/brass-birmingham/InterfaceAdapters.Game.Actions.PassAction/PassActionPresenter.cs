using BrassApplication.UseCases.Game.Actions;
using BrassApplication.UseCases.Game.Actions.ActionInProgress;
using BrassApplication.UseCases.Game.Actions.BaseAction;
using BrassApplication.UseCases.Game.Actions.PassAction;
using BrassApplication.UseCases.Game.HandOfCards;
using InterfaceAdapters.Game.ActionInProgressView;
using InterfaceAdapters.Game.Actions.BaseAction;
using InterfaceAdapters.Game.HandOfCards;
using InterfaceAdapters.Game.PlayerActions;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Utils;

namespace InterfaceAdapters.Game.Actions.PassAction;

public class PassActionPresenter : BaseActionPresenter, IPassActionOutput, IBaseActionOutput
{
	public PassActionPresenter(IRouter router, ILocalization localization)
		: base(router, localization)
	{
	}

	public void ShowPlayerInfoToSelectCardToDiscard(IActionInProgressCancelDelegate cancelActionDelegate, int currentActionNumber, int actionsMaxCount)
	{
		_router.HideView<IPlayerActionsView>();
		_router.GetView<IActionInProgressView>().SetActionInfoText(_localization.GetTranslation("UI/SELECT_CARD_TO_DISCARD"));
		_router.GetView<IActionInProgressView>().UpdateActionCounter(currentActionNumber, actionsMaxCount);
		_router.ShowView<IActionInProgressView>();
		_router.UseCaseBuilder.GetUseCase<ActionInProgressUseCase>().SetCancelActionDelegate(cancelActionDelegate);
	}

	public void ShowCardsToDiscard(ISelectCardDelegate selectCardDelegate)
	{
		_router.UseCaseBuilder.GetUseCase<HandOfCardsUseCase>().SetOnCardClickActionDelegate(selectCardDelegate);
		_router.GetView<IHandOfCardsView>().ShowCards();
	}
}
