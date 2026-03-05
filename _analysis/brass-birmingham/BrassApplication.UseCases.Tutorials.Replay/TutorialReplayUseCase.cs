using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Actions.Events;
using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Entities.GamePhases.Events;
using BoardGameRules.Entities.NumericalResources.Events;
using BrassApplication.Game;
using BrassApplication.GameHistory;
using BrassApplication.UseCases.Game.Replay;

namespace BrassApplication.UseCases.Tutorials.Replay;

public class TutorialReplayUseCase : ReplayUseCase
{
	private bool _awaitsManualInteraction;

	private bool wasFirstManualReplayWatched;

	private bool wasFirstIronSoldWatched;

	private IEvent eventForManualReplay;

	public bool AutoSkipReplay { get; set; }

	public bool ManualReplay { get; set; }

	public TutorialReplayUseCase(IReplayOutput presenter, IGameHistory gameHistory)
		: base(presenter, gameHistory)
	{
	}

	public override void ReplayUnseenEvents(PlayerMetadata replayWatchingPlayerMetadata, IReplayDelegate replayDelegate)
	{
		if (!ManualReplay)
		{
			base.ReplayUnseenEvents(replayWatchingPlayerMetadata, replayDelegate);
			if (_isReplayInProgress && AutoSkipReplay)
			{
				SkipReplayWithoutSummary();
			}
			return;
		}
		_replayWatchingPlayerMetadata = replayWatchingPlayerMetadata;
		_replayDelegate = replayDelegate;
		if (wasFirstManualReplayWatched)
		{
			_awaitsManualInteraction = false;
			ShowEventsBeforeTurnStarted();
		}
		else
		{
			wasFirstManualReplayWatched = true;
			_awaitsManualInteraction = true;
		}
	}

	public void ReplayNextAction()
	{
		ReplayEventsUntil(typeof(ActionCountDecreased), inclusive: true);
	}

	public void ReplayEndOfEraStart()
	{
		ReplayEventsUntil(typeof(CanalEraEndStarted), inclusive: true);
	}

	public void ReplayEndOfEraScoringLinks()
	{
		ReplayEventsUntil(typeof(ScoreFlippedIndustryTilesPhaseStarted), inclusive: false);
	}

	public void ReplayEndOfEraScoringIndustries()
	{
		ReplayEventsUntil(typeof(ObsoleteIndustriesRemoved), inclusive: false);
	}

	public void ReplayEndOfEndOfEra()
	{
		ReplayEventsUntil(typeof(CanalEraEnded), inclusive: true);
	}

	public void ReplayStartOfRailEra()
	{
		ReplayEventsUntil(typeof(RailEraStarted), inclusive: true);
	}

	private void ReplayEventsUntil(Type eventToStopType, bool inclusive)
	{
		_awaitsManualInteraction = false;
		IReadOnlyCollection<IEvent> unseenEvents = _gameHistory.GetUnseenEvents(_replayWatchingPlayerMetadata.PlayerStartConfiguration.Color);
		ReplayUseCase.Logger.Debug("Replay not seen events for player: {0}, lastSeenRevision: {1}, notSeenEvents: {2}", _replayWatchingPlayerMetadata.NickName, _replayWatchingPlayerMetadata.LastSeenEventRevision, unseenEvents?.Count);
		_gameHistory.MoveGameToRevision(_replayWatchingPlayerMetadata.LastSeenEventRevision);
		UpdateGamePresentationImpl(_replayWatchingPlayerMetadata);
		if (inclusive)
		{
			PrepareEventsUntilGivenEventInclusive(eventToStopType, unseenEvents, _replayWatchingPlayerMetadata, _replayDelegate);
		}
		else
		{
			PrepareEventsUntilGivenEventExclusive(eventToStopType, unseenEvents, _replayWatchingPlayerMetadata, _replayDelegate);
		}
		SetNewLastSeenRevision(_replayWatchingPlayerMetadata, _replayEvents);
		SaveLastSeenRevision(_replayWatchingPlayerMetadata);
		ReplayNextEventWithoutFinish();
	}

	public override void EventReplayCompleted(IEvent eventToReplay)
	{
		if (_isReplayInProgress)
		{
			_gameHistory.MoveGameToRevision(eventToReplay.Metadata.Revision);
			if (ManualReplay)
			{
				ReplayNextEventWithoutFinish();
			}
			else
			{
				ReplayNextEvent();
			}
		}
	}

	public void ReplayRemaining()
	{
		_awaitsManualInteraction = false;
		IReadOnlyCollection<IEvent> unseenEvents = _gameHistory.GetUnseenEvents(_replayWatchingPlayerMetadata.PlayerStartConfiguration.Color);
		ReplayUseCase.Logger.Debug("Replay not seen events for player: {0}, lastSeenRevision: {1}, notSeenEvents: {2}", _replayWatchingPlayerMetadata.NickName, _replayWatchingPlayerMetadata.LastSeenEventRevision, unseenEvents?.Count);
		_gameHistory.MoveGameToRevision(_replayWatchingPlayerMetadata.LastSeenEventRevision);
		UpdateGamePresentationImpl(_replayWatchingPlayerMetadata);
		PrepareEventsToReplay(unseenEvents, _replayWatchingPlayerMetadata, _replayDelegate);
		SetNewLastSeenRevision(_replayWatchingPlayerMetadata, _replayEvents);
		SaveLastSeenRevision(_replayWatchingPlayerMetadata);
		ReplayNextEvent();
	}

	public void UpdateGameHistoryAndFinishReplay()
	{
		_awaitsManualInteraction = false;
		UpdateGameHistory();
		FinishReplay();
	}

	public void SkipReplayWithoutSummary()
	{
		if (!_isReplayInProgress)
		{
			throw new InvalidOperationException("There is no replay in progress");
		}
		_presenter.CancelCurrentReplay();
		if (IsEndOfEraStartedEventOnTheList(_replayEvents))
		{
			ReplayUseCase.Logger.Debug("End of Era Started event on the list. Stopping at end of era events.");
			_replayEvents = RemoveEventsBeforeEndOfEraStarted(_replayEvents);
		}
		else if (IsEndOfEraEndedEventOnTheList(_replayEvents))
		{
			ReplayUseCase.Logger.Debug("End of Era Ended event on the list. Stopping at end of era summary.");
			_replayEvents = RemoveEventsBeforeEndOfEraEnded(_replayEvents);
			ReplayNextEvent();
		}
		else
		{
			UpdateGameHistory();
			_replayEvents.Clear();
			FinishReplay();
		}
	}

	public bool IsAwaitingManualInteraction()
	{
		return _awaitsManualInteraction;
	}

	public void ShowEventsBeforeTurnStarted()
	{
		if (_isReplayInProgress)
		{
			throw new InvalidOperationException("Replay is already in progress");
		}
		_isReplayInProgress = true;
		ReplayEventsUntil(typeof(TurnStarted), inclusive: true);
	}

	private void PrepareEraEndedToReplay(IReadOnlyCollection<IEvent> notSeenEvents, PlayerMetadata replayWatchingPlayerMetadata, IReplayDelegate replayDelegate)
	{
		PrepareEventsUntilGivenEventInclusive(typeof(CanalEraEnded), notSeenEvents, replayWatchingPlayerMetadata, replayDelegate);
	}

	private void PrepareEventsUntilGivenEventInclusive(Type eventToStopType, IReadOnlyCollection<IEvent> notSeenEvents, PlayerMetadata replayWatchingPlayerMetadata, IReplayDelegate replayDelegate)
	{
		_replayDelegate = replayDelegate;
		_replayDelegate?.ReplayStarted();
		List<IEvent> list = ((notSeenEvents != null) ? new List<IEvent>(notSeenEvents.Where((IEvent e) => e.Metadata.Revision > replayWatchingPlayerMetadata.LastSeenEventRevision)) : new List<IEvent>());
		_replayEvents = new List<IEvent>();
		for (int num = 0; num < list.Count; num++)
		{
			_replayEvents.Add(list[num]);
			if (list[num].GetType() == eventToStopType)
			{
				num = list.Count;
			}
		}
		_replaySummaryEvents = new List<IEvent>(_replayEvents);
	}

	private void PrepareEventsUntilGivenEventExclusive(Type eventToStopType, IReadOnlyCollection<IEvent> notSeenEvents, PlayerMetadata replayWatchingPlayerMetadata, IReplayDelegate replayDelegate)
	{
		_replayDelegate = replayDelegate;
		_replayDelegate?.ReplayStarted();
		List<IEvent> list = ((notSeenEvents != null) ? new List<IEvent>(notSeenEvents.Where((IEvent e) => e.Metadata.Revision > replayWatchingPlayerMetadata.LastSeenEventRevision)) : new List<IEvent>());
		_replayEvents = new List<IEvent>();
		for (int num = 0; num < list.Count; num++)
		{
			if (list[num].GetType() == eventToStopType)
			{
				num = list.Count;
			}
			else
			{
				_replayEvents.Add(list[num]);
			}
		}
		_replaySummaryEvents = new List<IEvent>(_replayEvents);
	}

	protected override void ReplayNextEvent()
	{
		if (_replayEvents == null)
		{
			return;
		}
		if (_replayEvents.Any())
		{
			IEvent obj = _replayEvents.First();
			if (!(obj is IronSold) || wasFirstIronSoldWatched)
			{
				_replayEvents.RemoveAt(0);
				if (obj is TurnStarted)
				{
					UpdateGamePresentationImpl(_replayWatchingPlayerMetadata);
				}
				else if (!(obj is CanalEraEndStarted))
				{
					_ = obj is RailEraEndStarted;
				}
				_presenter.UpdateCurrentPlayerAction(obj);
				_presenter.ReplayEvent(obj, this);
			}
			else
			{
				_replayEvents.RemoveAt(0);
				wasFirstIronSoldWatched = true;
				_awaitsManualInteraction = true;
				eventForManualReplay = obj;
			}
		}
		else
		{
			UpdateGameHistory();
			FinishReplay();
		}
	}

	public void ManualReplayOfPreparedEvent()
	{
		_awaitsManualInteraction = false;
		_presenter.UpdateCurrentPlayerAction(eventForManualReplay);
		_presenter.ReplayEvent(eventForManualReplay, this);
	}

	private void ReplayNextEventWithoutFinish()
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
			}
			else if (!(obj is CanalEraEndStarted))
			{
				_ = obj is RailEraEndStarted;
			}
			_presenter.UpdateCurrentPlayerAction(obj);
			_presenter.ReplayEvent(obj, this);
		}
		else
		{
			_awaitsManualInteraction = true;
		}
	}
}
