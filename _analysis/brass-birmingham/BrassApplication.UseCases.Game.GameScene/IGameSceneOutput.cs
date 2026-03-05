namespace BrassApplication.UseCases.Game.GameScene;

public interface IGameSceneOutput
{
	void ShowGamePanel();

	void ShowLoadingView();

	void ShowAndHideLoadingView();

	void ShowExitMenuView();

	void ShowBoard();

	void ShowCards();

	void ShowMarket();

	void ShowPlayersSummary();

	void ShowPlayerActions();

	void ShowIndustriesSummary();

	void ShowGameProgressSummary();

	void PlayGame();

	void ShowBackToMenuTooltip(float x, float y);

	void HideTooltip();
}
