using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using BoardGameRules.Entities.EventSourcing.Events;
using BrassApplication.GameHistory;
using BrassApplication.Persistence.Repository;
using BrassApplication.UseCases.Common;

namespace BrassApplication.UseCases.Game.GameLog;

public class GameLogUseCase : IUseCase, IGameLogInput
{
	private readonly IGameLogOutput _presenter;

	private readonly IRepository _repository;

	private readonly IGameHistory _gameHistory;

	public GameLogUseCase(IGameLogOutput presenter, IRepository repository, IGameHistory gameHistory)
	{
		_presenter = presenter;
		_repository = repository;
		_gameHistory = gameHistory;
	}

	public void UpdateGameLog()
	{
		ReadOnlyCollection<IEvent> eventsList = _repository.Advanced.GameStorage.LoadEvents(_gameHistory.CurrentGame.Metadata.GameId, 0, int.MaxValue);
		_presenter.UpdateGameLogView(eventsList, _gameHistory.CurrentGame.Game.GameProgressInformation, _gameHistory.CurrentGame.Metadata);
	}

	public void UpdateReplaySummary(IList<IEvent> skippedEvents, Action onDismiss)
	{
		_presenter.UpdateReplaySummaryView(skippedEvents, _gameHistory.CurrentGame.Game.GameProgressInformation, _gameHistory.CurrentGame.Metadata, onDismiss);
	}

	public void CloseGameLog()
	{
		_presenter.CloseGameLogView();
	}
}
