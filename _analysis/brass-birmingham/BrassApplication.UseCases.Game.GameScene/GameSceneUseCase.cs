using BrassApplication.ApplicationSettings;
using BrassApplication.UseCases.Common;

namespace BrassApplication.UseCases.Game.GameScene;

public class GameSceneUseCase : IUseCase, IGameSceneInput
{
	private readonly IGameSceneOutput _presenter;

	protected readonly IGameSettings _gameSettings;

	public GameSceneUseCase(IGameSceneOutput presenter, IGameSettings gameSettings)
	{
		_presenter = presenter;
		_gameSettings = gameSettings;
	}

	public virtual void StartGameScene()
	{
		_presenter.ShowLoadingView();
		_presenter.ShowBoard();
		_presenter.ShowGameProgressSummary();
		_presenter.ShowCards();
		_presenter.ShowMarket();
		_presenter.ShowPlayersSummary();
		_presenter.ShowIndustriesSummary();
		_presenter.ShowGamePanel();
		_presenter.PlayGame();
	}

	public void OpenMenu()
	{
		_presenter.ShowExitMenuView();
	}

	public void ShowBackToMenuTooltip(float x, float y)
	{
		_presenter.ShowBackToMenuTooltip(x, y);
	}

	public void HideTooltip()
	{
		_presenter.HideTooltip();
	}
}
