using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BoardGameRules.Entities.EventSourcing.Events;
using BrassApplication.Game;
using BrassApplication.UseCases.Game.Actions.ActionInProgress;
using BrassApplication.UseCases.Game.Actions.BaseAction;
using BrassApplication.UseCases.Game.Actions.IronDelivery;
using BrassApplication.UseCases.Game.HandOfCards;
using BrassApplication.UseCases.Game.Replay;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.ActionInProgressView;
using InterfaceAdapters.Game.HandOfCards;
using InterfaceAdapters.Game.PlayerActions;
using InterfaceAdapters.Game.Replay;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Utils;

namespace InterfaceAdapters.Game.Actions.BaseAction;

public class BaseActionPresenter : IPresenter, IBaseActionOutput
{
	protected readonly IRouter _router;

	protected readonly ILocalization _localization;

	protected BaseActionPresenter(IRouter router, ILocalization localization)
	{
		_router = router;
		_localization = localization;
	}

	public virtual void HideActionInProgressView()
	{
		_router.UseCaseBuilder.GetUseCase<ActionInProgressUseCase>().SetCancelActionDelegate(null);
		_router.HideView<IActionInProgressView>();
	}

	public void ShowPlayerActionsButtons()
	{
		_router.HideView<IReplayView>();
		_router.ShowView<IPlayerActionsView>();
	}

	public void UpdateGameStatus(PlayerMetadata replayWatchingPlayerMetadata)
	{
		_router.UseCaseBuilder.GetUseCase<ReplayUseCase>().UpdateGamePresentation(replayWatchingPlayerMetadata);
	}

	public void PresentEvents(ReadOnlyCollection<IEvent> events, PlayerMetadata replayWatchingPlayerMetadata, IReplayDelegate replayDelegate)
	{
		_router.ShowView<IReplayView>();
		_router.MoveOnTop<IReplayView>();
		_router.GetView<IReplayView>().HideSkipButton();
		_router.UseCaseBuilder.GetUseCase<ReplayUseCase>().PresentEvents(events, replayWatchingPlayerMetadata, replayDelegate);
		if (_router.IsViewVisible<IReplayView>())
		{
			_router.GetView<IReplayView>().HideSkipButton();
		}
	}

	public void RestoreCardsView()
	{
		_router.GetView<IHandOfCardsView>().HideCards();
		_router.UseCaseBuilder.GetUseCase<HandOfCardsUseCase>().SetOnCardClickActionDelegate(null);
	}

	public void ShowEndTurnButton()
	{
		_router.HideView<IReplayView>();
		_router.GetView<IPlayerActionsView>().ShowEndTurnButton();
		_router.ShowView<IPlayerActionsView>();
	}

	public void HideReplayView()
	{
		_router.HideView<IReplayView>();
	}

	public async Task WaitForTweensEnd()
	{
		if (_router.GetView<IReplayView>().IsAnyTweenInProgress())
		{
			await Task.Delay(100);
		}
	}

	public void HideIronDelivery()
	{
		_router.UseCaseBuilder.GetUseCase<IronDeliveryUseCase>().HideIronDelivery();
	}
}
