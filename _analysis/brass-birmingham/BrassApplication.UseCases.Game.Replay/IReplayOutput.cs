using System;
using System.Collections.Generic;
using BoardGameRules.Entities.EventSourcing.Events;
using BrassApplication.Game;

namespace BrassApplication.UseCases.Game.Replay;

public interface IReplayOutput
{
	void UpdateCurrentPlayer(string playerColor);

	void UpdateCurrentPlayerAction(IEvent eventToReplay);

	void UpdateCurrentPlayerActionText(string text);

	void UpdateGamePresentation(PlayerMetadata replayWatchingPlayerMetadata);

	void ReplayEvent(IEvent eventToReplay, IEventReplayDelegate eventReplayDelegate);

	void CancelCurrentReplay();

	void ShowReplaySummary(IList<IEvent> skippedEvents, Action onDismiss);

	void ShowIndustrySellingSummary(IEvent industrySellingEndedEvent, Action onDismiss);

	void UpdateCurrentPlayerActionsView();

	void FinishReplay();
}
