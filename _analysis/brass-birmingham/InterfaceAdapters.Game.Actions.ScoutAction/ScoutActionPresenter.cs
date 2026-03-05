using System;
using System.Collections.ObjectModel;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.EventSourcing.Events;
using BrassApplication.Game;
using BrassApplication.UseCases.Game.Actions;
using BrassApplication.UseCases.Game.Actions.ActionInProgress;
using BrassApplication.UseCases.Game.Actions.BaseAction;
using BrassApplication.UseCases.Game.Actions.ScoutAction;
using BrassApplication.UseCases.Game.HandOfCards;
using InterfaceAdapters.Game.ActionInProgressView;
using InterfaceAdapters.Game.Actions.BaseAction;
using InterfaceAdapters.Game.HandOfCards;
using InterfaceAdapters.Game.PlayerActions;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Utils;

namespace InterfaceAdapters.Game.Actions.ScoutAction;

public class ScoutActionPresenter : BaseActionPresenter, IScoutActionOutput, IBaseActionOutput
{
	public ScoutActionPresenter(IRouter router, ILocalization localization)
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

	public void HideCards()
	{
		_router.UseCaseBuilder.GetUseCase<HandOfCardsUseCase>().SetOnCardClickActionDelegate(null);
		_router.GetView<IHandOfCardsView>().HideCards();
	}

	public void ShowExchangeForWildCardsView(BrassApplicationGame game, ISelectCardDelegate selectCardDelegate)
	{
		_router.UseCaseBuilder.GetUseCase<HandOfCardsUseCase>().SetOnCardClickActionDelegate(selectCardDelegate);
		_router.ShowView<IScoutActionView>();
		_router.MoveOnTop<IActionInProgressView>();
		_router.MoveOnTop<IHandOfCardsView>();
	}

	public void SetupCardInSlot(int slotIndex, BaseCard card, Action onCallback)
	{
		_router.GetView<IScoutActionView>().AnimateCardToSlot(slotIndex, _router.GetView<IHandOfCardsView>().GetCardObjectInfo(card), onCallback);
		if (slotIndex == 1)
		{
			_router.GetView<IHandOfCardsView>().TurnOffInteractions();
		}
	}

	public void HideExchangeForWildCardsView()
	{
		_router.HideView<IScoutActionView>();
	}

	public void EnableCardsLayoutGroup()
	{
		_router.GetView<IHandOfCardsView>().EnableCardsLayoutGroup();
	}

	public void ReplayEvents(ReadOnlyCollection<IEvent> events)
	{
	}
}
