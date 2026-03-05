using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Entities.GamePhases.Events;
using BoardGameRules.Utils.Logging;
using BrassApplication.Game;
using BrassApplication.GameHistory;
using BrassApplication.UseCases.Game.Board;
using BrassApplication.UseCases.Game.GameLog;
using BrassApplication.UseCases.Game.GameProgressSummary;
using BrassApplication.UseCases.Game.HandOfCards;
using BrassApplication.UseCases.Game.IndustriesSummary;
using BrassApplication.UseCases.Game.Market;
using BrassApplication.UseCases.Game.PlayerActions;
using BrassApplication.UseCases.Game.PlayersSummary;
using BrassApplication.UseCases.Game.Replay;
using BrassApplication.UseCases.Game.SellIndustryFlow;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.EndOfEra;
using InterfaceAdapters.Game.PlayerActions;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Utils;
using InterfaceAdapters.Utils.MainThreadDispatcher;

namespace InterfaceAdapters.Game.Replay;

public class ReplayPresenter : IPresenter, IReplayOutput
{
	protected readonly IRouter _router;

	protected readonly IMainThreadDispatcher _mainThreadDispatcher;

	private readonly ChangeEventToTextHelper _changeEventToTextHelper;

	protected static readonly ILog Logger = LogFactory.BuildLogger(typeof(ReplayPresenter));

	public ReplayPresenter(IRouter router, IMainThreadDispatcher mainThreadDispatcher, IGameHistory gameHistory, ILocalization localization)
	{
		_router = router;
		_mainThreadDispatcher = mainThreadDispatcher;
		_changeEventToTextHelper = new ChangeEventToTextHelper(gameHistory, localization);
	}

	public void UpdateGamePresentation(PlayerMetadata replayWatchingPlayerMetadata)
	{
		_router.UseCaseBuilder.GetUseCase<BoardUseCase>().UpdateBoard();
		_router.UseCaseBuilder.GetUseCase<MarketUseCase>().UpdateMarket();
		_router.UseCaseBuilder.GetUseCase<PlayerActionsUseCase>().UpdatePlayerActions();
		_router.UseCaseBuilder.GetUseCase<GameProgressSummaryUseCase>().UpdateGameProgressSummary();
		_router.UseCaseBuilder.GetUseCase<PlayersSummaryUseCase>().UpdatePlayers();
		_router.UseCaseBuilder.GetUseCase<IndustriesSummaryUseCase>().UpdateIndustries();
		_router.UseCaseBuilder.GetUseCase<HandOfCardsUseCase>().UpdateCards(replayWatchingPlayerMetadata);
	}

	public void UpdateCurrentPlayer(string playerColor)
	{
		if (_router.IsViewVisible<IReplayView>())
		{
			_router.GetView<IReplayView>().SetCurrentPlayerName(playerColor);
		}
	}

	public void UpdateCurrentPlayerAction(IEvent eventToReplay)
	{
		string item = _changeEventToTextHelper.ChangeEventToText(eventToReplay).Item3;
		if (item != null && _router.IsViewVisible<IReplayView>())
		{
			_router.GetView<IReplayView>().SetCurrentPlayerAction(item);
		}
	}

	public void UpdateCurrentPlayerActionText(string text)
	{
		if (_router.IsViewVisible<IReplayView>())
		{
			_router.GetView<IReplayView>().SetCurrentPlayerAction(text);
		}
	}

	public virtual void ReplayEvent(IEvent eventToReplay, IEventReplayDelegate eventReplayDelegate)
	{
		Logger.Debug("ReplayEvent(): event = " + eventToReplay);
		IAnimation animation = _router.GetView<IReplayView>().CreateAnimation(eventToReplay);
		if (!(eventToReplay is CanalEraEnded))
		{
			animation.OnComplete(delegate
			{
				Logger.Debug("ReplayEventCompleted(): event = " + eventToReplay);
				eventReplayDelegate.EventReplayCompleted(eventToReplay);
			});
		}
		else
		{
			_router.GetView<IEndOfEraView>().SetOnOkButtonClicked(delegate
			{
				_router.HideView<IEndOfEraView>();
				Logger.Debug("ReplayEventCompleted(): event = " + eventToReplay);
				eventReplayDelegate.EventReplayCompleted(eventToReplay);
			});
		}
		Task.Run(delegate
		{
			_mainThreadDispatcher.Invoke(delegate
			{
				animation.Play();
			});
		});
	}

	public void CancelCurrentReplay()
	{
		_router.GetView<IReplayView>().KillAllAnimations();
	}

	public void ShowReplaySummary(IList<IEvent> skippedEvents, Action onDismiss)
	{
		_router.UseCaseBuilder.GetUseCase<GameLogUseCase>().UpdateReplaySummary(skippedEvents, onDismiss);
	}

	public void ShowIndustrySellingSummary(IEvent industrySellingEndedEvent, Action onDismiss)
	{
		_router.UseCaseBuilder.GetUseCase<SellIndustryFlowUseCase>().ShowIndustrySellingSummary(industrySellingEndedEvent, onDismiss);
	}

	public void UpdateCurrentPlayerActionsView()
	{
		_router.GetView<IPlayerActionsView>().HideEndTurnButton();
	}

	public void FinishReplay()
	{
		_router.GetView<IReplayView>().RestoreDefaultAnimationSpeed();
	}
}
