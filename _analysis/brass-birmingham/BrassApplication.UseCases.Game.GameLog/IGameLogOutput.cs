using System;
using System.Collections.Generic;
using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Entities.Game;
using BrassApplication.Game;

namespace BrassApplication.UseCases.Game.GameLog;

public interface IGameLogOutput
{
	void UpdateGameLogView(IList<IEvent> eventsList, GameProgressInformation gameProgressInformation, GameMetadata metadata);

	void UpdateReplaySummaryView(IList<IEvent> gameLogList, GameProgressInformation gameProgressInformation, GameMetadata metadata, Action onDismiss);

	void CloseGameLogView();
}
