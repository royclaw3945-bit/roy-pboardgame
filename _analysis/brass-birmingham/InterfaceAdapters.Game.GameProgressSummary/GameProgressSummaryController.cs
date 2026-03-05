using BrassApplication.UseCases.Game.GameProgressSummary;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.GameProgressSummary;

public class GameProgressSummaryController : IController
{
	private readonly IGameProgressSummaryInput _gameProgressSummaryUseCase;

	public GameProgressSummaryController(IGameProgressSummaryInput gameProgressSummaryUseCase)
	{
		_gameProgressSummaryUseCase = gameProgressSummaryUseCase;
	}

	public void OnViewCreated()
	{
		_gameProgressSummaryUseCase.UpdateGameProgressSummary();
	}

	public void OnViewDestroyed()
	{
	}

	public void OnEarningsTrackClicked()
	{
		_gameProgressSummaryUseCase.EarningsTrackClicked();
	}

	public void OnGameLogClicked()
	{
		_gameProgressSummaryUseCase.GameLogClicked();
	}

	internal void OnMouseEnter(string element, float x, float y)
	{
		_gameProgressSummaryUseCase.ShowTooltip(element, x, y);
	}

	internal void OnMouseExit()
	{
		_gameProgressSummaryUseCase.HideTooltip();
	}
}
