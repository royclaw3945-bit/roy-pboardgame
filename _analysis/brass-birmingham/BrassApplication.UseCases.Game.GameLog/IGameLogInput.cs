using System;
using System.Collections.Generic;
using BoardGameRules.Entities.EventSourcing.Events;

namespace BrassApplication.UseCases.Game.GameLog;

public interface IGameLogInput
{
	void UpdateGameLog();

	void UpdateReplaySummary(IList<IEvent> skippedEvents, Action onDismiss);

	void CloseGameLog();
}
