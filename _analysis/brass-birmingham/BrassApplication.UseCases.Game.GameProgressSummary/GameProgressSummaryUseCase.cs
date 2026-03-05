using BrassApplication.GameHistory;
using BrassApplication.UseCases.Common;

namespace BrassApplication.UseCases.Game.GameProgressSummary;

public class GameProgressSummaryUseCase : IUseCase, IGameProgressSummaryInput
{
	private readonly IGameProgressSummaryOutput _presenter;

	private readonly IGameHistory _gameHistory;

	public GameProgressSummaryUseCase(IGameProgressSummaryOutput presenter, IGameHistory gameHistory)
	{
		_presenter = presenter;
		_gameHistory = gameHistory;
	}

	public void UpdateGameProgressSummary()
	{
		_presenter.UpdateGameProgressSummaryView(_gameHistory.CurrentGame);
	}

	public void EarningsTrackClicked()
	{
		_presenter.ShowGameProgressDetails();
	}

	public void GameLogClicked()
	{
		_presenter.ShowGameLog();
	}

	public void ShowTooltip(string element, float x, float y)
	{
		_presenter.ShowTooltip(element, x, y);
	}

	public void HideTooltip()
	{
		_presenter.HideTooltip();
	}
}
