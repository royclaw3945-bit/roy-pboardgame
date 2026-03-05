using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Entities.GamePhases.Events;
using BoardGameRules.Entities.Players;
using BoardGameRules.Entities.SellIndustry.Events;
using BoardGameRules.Utils.Logging;
using BrassApplication.Game;
using BrassApplication.GameHistory;
using BrassApplication.UseCases.Common;

namespace BrassApplication.UseCases.Game.Replay;

public class ReplayUseCase : IUseCase, IReplayInput, IEventReplayDelegate
{
	protected static readonly ILog Logger = LogFactory.BuildLogger(typeof(ReplayUseCase));

	protected readonly IReplayOutput _presenter;

	protected IList<IEvent> _replayEvents;

	protected IList<IEvent> _replaySummaryEvents;

	protected IReplayDelegate _replayDelegate;

	protected bool _isReplayInProgress;

	protected PlayerMetadata _replayWatchingPlayerMetadata;

	protected readonly IGameHistory _gameHistory;

	public ReplayUseCase(IReplayOutput presenter, IGameHistory gameHistory)
	{
		_presenter = presenter;
		_gameHistory = gameHistory;
		_isReplayInProgress = false;
		_replayWatchingPlayerMetadata = null;
	}

	public void UpdateGamePresentation(PlayerMetadata replayWatchingPlayerMetadata)
	{
		if (_isReplayInProgress)
		{
			throw new InvalidOperationException("Replay is already in progress");
		}
		UpdateGamePresentationImpl(replayWatchingPlayerMetadata);
	}

	public void PresentEvents(ReadOnlyCollection<IEvent> events, PlayerMetadata replayWatchingPlayerMetadata, IReplayDelegate replayDelegate)
	{
		if (_isReplayInProgress)
		{
			throw new InvalidOperationException("Replay is already in progress");
		}
		_isReplayInProgress = true;
		_replayWatchingPlayerMetadata = replayWatchingPlayerMetadata;
		Logger.Debug("Present events for player: {0}, events: {1}", replayWatchingPlayerMetadata.NickName, events?.Count);
		_gameHistory.MoveGameToRevision(replayWatchingPlayerMetadata.LastSeenEventRevision);
		PrepareEventsToReplay(events, replayWatchingPlayerMetadata, replayDelegate);
		SetNewLastSeenRevision(replayWatchingPlayerMetadata, _replayEvents);
		ReplayNextEvent();
	}

	public virtual void ReplayUnseenEvents(PlayerMetadata replayWatchingPlayerMetadata, IReplayDelegate replayDelegate)
	{
		if (_isReplayInProgress)
		{
			throw new InvalidOperationException("Replay is already in progress");
		}
		_isReplayInProgress = true;
		_replayWatchingPlayerMetadata = replayWatchingPlayerMetadata;
		IReadOnlyCollection<IEvent> unseenEvents = _gameHistory.GetUnseenEvents(replayWatchingPlayerMetadata.PlayerStartConfiguration.Color);
		Logger.Debug("Replay not seen events for player: {0}, lastSeenRevision: {1}, notSeenEvents: {2}", replayWatchingPlayerMetadata.NickName, replayWatchingPlayerMetadata.LastSeenEventRevision, unseenEvents?.Count);
		_gameHistory.MoveGameToRevision(replayWatchingPlayerMetadata.LastSeenEventRevision);
		UpdateGamePresentationImpl(replayWatchingPlayerMetadata);
		PrepareEventsToReplay(unseenEvents, replayWatchingPlayerMetadata, replayDelegate);
		SetNewLastSeenRevision(replayWatchingPlayerMetadata, _replayEvents);
		SaveLastSeenRevision(replayWatchingPlayerMetadata);
		ReplayNextEvent();
	}

	public void SkipReplay()
	{
		if (!_isReplayInProgress)
		{
			throw new InvalidOperationException("There is no replay in progress");
		}
		_presenter.CancelCurrentReplay();
		if (IsIndustrySellingEndedOnTheList(_replayEvents))
		{
			Logger.Debug("IndustrySellingEnded event on the list");
			_replayEvents = RemoveEventsBeforeIndustrySellingEnded(_replayEvents);
			_replaySummaryEvents = RemoveEventsBeforeIndustrySellingEnded(_replaySummaryEvents);
			IEvent industrySellingEndedEvent = _replayEvents[0];
			_replayEvents.RemoveAt(0);
			ShowSoldIndustries(industrySellingEndedEvent, delegate
			{
				Logger.Debug("Industry Selling Ended event on the list. Resuming Industry Selling Ended replay after summary.");
				ReplayNextEvent();
			});
		}
		else if (IsEndOfEraStartedEventOnTheList(_replayEvents))
		{
			Logger.Debug("End of Era Started event on the list. Stopping at end of era events.");
			List<IEvent> list = RemoveEventsAfterEndOfEraStarted(_replaySummaryEvents);
			_replayEvents = RemoveEventsBeforeEndOfEraStarted(_replayEvents);
			_replaySummaryEvents = RemoveEventsBeforeEndOfEraStarted(_replaySummaryEvents);
			UpdateGameHistoryToGivenRevision(list.Last().Metadata.Revision);
			ShowReplaySummary(list, delegate
			{
				Logger.Debug("End of Era event on the list. Resuming End of Era replay after summary.");
				ReplayNextEvent();
			});
		}
		else if (IsEndOfEraEndedEventOnTheList(_replayEvents))
		{
			Logger.Debug("End of Era Ended event on the list. Stopping at end of era summary.");
			_replayEvents = RemoveEventsBeforeEndOfEraEnded(_replayEvents);
			_replaySummaryEvents = RemoveEventsBeforeEndOfEraEnded(_replaySummaryEvents);
			ReplayNextEvent();
		}
		else
		{
			UpdateGameHistory();
			List<IEvent> skippedEvents = new List<IEvent>(_replaySummaryEvents);
			_replayEvents.Clear();
			_replaySummaryEvents.Clear();
			ShowReplaySummary(skippedEvents, delegate
			{
				FinishReplay();
			});
		}
	}

	public virtual void EventReplayCompleted(IEvent eventToReplay)
	{
		if (_isReplayInProgress)
		{
			_gameHistory.MoveGameToRevision(eventToReplay.Metadata.Revision);
			ReplayNextEvent();
		}
	}

	public PlayerColor GetReplayWatchingPlayerColor()
	{
		return _replayWatchingPlayerMetadata.PlayerStartConfiguration.Color;
	}

	public bool GetIsReplayInProgress()
	{
		return _isReplayInProgress;
	}

	protected void UpdateGamePresentationImpl(PlayerMetadata replayWatchingPlayerMetadata)
	{
		Logger.Debug($"Update game presentation, revision: {_gameHistory.CurrentGame.Game.Revision}, current player: {_gameHistory.CurrentGame.Game.GameProgressInformation?.CurrentPlayer.Color}, replay watching player: {replayWatchingPlayerMetadata.PlayerStartConfiguration?.Color}");
		_presenter.UpdateGamePresentation(replayWatchingPlayerMetadata);
		if (_gameHistory.CurrentGame.Game.GameProgressInformation != null)
		{
			string nickName = _gameHistory.CurrentGame.Metadata.GetPlayerMetadataByColor(_gameHistory.CurrentGame.Game.GameProgressInformation.CurrentPlayer.Color).NickName;
			_presenter.UpdateCurrentPlayer(nickName);
		}
		_presenter.UpdateCurrentPlayerActionText("");
		_presenter.UpdateCurrentPlayerActionsView();
	}

	protected void PrepareEventsToReplay(IReadOnlyCollection<IEvent> notSeenEvents, PlayerMetadata replayWatchingPlayerMetadata, IReplayDelegate replayDelegate)
	{
		_replayDelegate = replayDelegate;
		_replayDelegate?.ReplayStarted();
		_replayEvents = ((notSeenEvents != null) ? new List<IEvent>(notSeenEvents.Where((IEvent e) => e.Metadata.Revision > replayWatchingPlayerMetadata.LastSeenEventRevision)) : new List<IEvent>());
		_replaySummaryEvents = new List<IEvent>(_replayEvents);
	}

	protected bool IsEndOfEraStartedEventOnTheList(IList<IEvent> filteredEvents)
	{
		return filteredEvents.Any((IEvent e) => e.GetType() == typeof(CanalEraEndStarted) || e.GetType() == typeof(RailEraEndStarted));
	}

	protected bool IsEndOfEraEndedEventOnTheList(IList<IEvent> filteredEvents)
	{
		return filteredEvents.Any((IEvent e) => e.GetType() == typeof(CanalEraEnded) || e.GetType() == typeof(RailEraEnded));
	}

	protected bool IsIndustrySellingEndedOnTheList(IList<IEvent> filteredEvents)
	{
		return filteredEvents.Any((IEvent e) => e.GetType() == typeof(IndustrySellingEnded));
	}

	protected List<IEvent> RemoveEventsBeforeEndOfEraStarted(IList<IEvent> filteredEvents)
	{
		List<IEvent> list = new List<IEvent>(filteredEvents);
		int count = list.FindIndex((IEvent e) => e.GetType() == typeof(CanalEraEndStarted) || e.GetType() == typeof(RailEraEndStarted));
		list.RemoveRange(0, count);
		return list;
	}

	protected List<IEvent> RemoveEventsAfterEndOfEraStarted(IList<IEvent> filteredEvents)
	{
		List<IEvent> list = new List<IEvent>(filteredEvents);
		int num = list.FindIndex((IEvent e) => e.GetType() == typeof(CanalEraEndStarted) || e.GetType() == typeof(RailEraEndStarted));
		list.RemoveRange(num, list.Count - num);
		return list;
	}

	protected List<IEvent> RemoveEventsBeforeEndOfEraEnded(IList<IEvent> filteredEvents)
	{
		List<IEvent> list = new List<IEvent>(filteredEvents);
		int count = list.FindIndex((IEvent e) => e.GetType() == typeof(CanalEraEnded) || e.GetType() == typeof(RailEraEnded));
		list.RemoveRange(0, count);
		return list;
	}

	protected List<IEvent> RemoveEventsBeforeIndustrySellingEnded(IList<IEvent> filteredEvents)
	{
		List<IEvent> list = new List<IEvent>(filteredEvents);
		int count = list.FindIndex((IEvent e) => e.GetType() == typeof(IndustrySellingEnded));
		list.RemoveRange(0, count);
		return list;
	}

	protected List<IEvent> RemoveEventsAfterEndOfEraEnded(IList<IEvent> filteredEvents)
	{
		List<IEvent> list = new List<IEvent>(filteredEvents);
		int num = list.FindIndex((IEvent e) => e.GetType() == typeof(CanalEraEnded) || e.GetType() == typeof(RailEraEnded));
		list.RemoveRange(num, list.Count - num);
		return list;
	}

	protected virtual void ReplayNextEvent()
	{
		if (_replayEvents == null)
		{
			return;
		}
		if (_replayEvents.Any())
		{
			IEvent obj = _replayEvents.First();
			_replayEvents.RemoveAt(0);
			if (obj is TurnStarted)
			{
				UpdateGamePresentationImpl(_replayWatchingPlayerMetadata);
				_presenter.UpdateCurrentPlayerAction(obj);
				_presenter.ReplayEvent(obj, this);
				if (GetReplayWatchingPlayerColor() == _gameHistory.CurrentGame.Game.GameProgressInformation.CurrentPlayer.Color && _replayEvents == null)
				{
					UpdateGameHistory();
					FinishReplay();
				}
			}
			else if (obj is IndustrySellingEnded)
			{
				ShowSoldIndustries(obj, delegate
				{
					Logger.Debug("Industry Selling Ended event on the list. Resuming Industry Selling Ended replay after summary.");
					ReplayNextEvent();
				});
			}
			else
			{
				_presenter.UpdateCurrentPlayerAction(obj);
				_presenter.ReplayEvent(obj, this);
			}
		}
		else
		{
			UpdateGameHistory();
			FinishReplay();
		}
	}

	protected void UpdateGameHistory()
	{
		_gameHistory.MoveToLatestGame();
		UpdateGamePresentationImpl(_replayWatchingPlayerMetadata);
	}

	protected void UpdateGameHistoryToGivenRevision(int revision)
	{
		_gameHistory.MoveGameToRevision(revision);
		UpdateGamePresentationImpl(_replayWatchingPlayerMetadata);
	}

	protected void FinishReplay()
	{
		Logger.Debug("Finish Replay");
		_isReplayInProgress = false;
		_replayWatchingPlayerMetadata = null;
		_replayEvents = null;
		_replaySummaryEvents = null;
		IReplayDelegate replayDelegate = _replayDelegate;
		_replayDelegate = null;
		replayDelegate?.ReplayCompleted();
		_presenter.FinishReplay();
	}

	protected void SetNewLastSeenRevision(PlayerMetadata replayWatchingPlayerMetadata, IList<IEvent> notSeenEvents)
	{
		int num = replayWatchingPlayerMetadata.LastSeenEventRevision + (notSeenEvents?.Count ?? 0);
		if (num != replayWatchingPlayerMetadata.LastSeenEventRevision)
		{
			Logger.Debug("Updating lastSeenRevision to: {0} for player: {1}", num, replayWatchingPlayerMetadata.NickName);
			replayWatchingPlayerMetadata.LastSeenEventRevision = num;
		}
	}

	protected void SaveLastSeenRevision(PlayerMetadata replayWatchingPlayerMetadata)
	{
		if (_gameHistory.CurrentGame.Metadata.GameProgressInformation.CurrentPlayer.Color.Equals(replayWatchingPlayerMetadata.PlayerStartConfiguration.Color) && _gameHistory.CurrentGame.Metadata.GameType.Equals(GameType.Network))
		{
			Logger.Debug("Network game, skipping saving lastSeenRevision");
			return;
		}
		Logger.Debug("Saving lastSeenRevision");
		_gameHistory.Repository.Async.SaveAsync(replayWatchingPlayerMetadata);
	}

	private void ShowReplaySummary(IList<IEvent> skippedEvents, Action onDismiss = null)
	{
		if (skippedEvents.Count > 0)
		{
			_presenter.ShowReplaySummary(skippedEvents, onDismiss);
		}
		else
		{
			onDismiss();
		}
	}

	private void ShowSoldIndustries(IEvent industrySellingEndedEvent, Action onDismiss = null)
	{
		if (industrySellingEndedEvent != null)
		{
			_presenter.ShowIndustrySellingSummary(industrySellingEndedEvent, onDismiss);
		}
	}
}
