using BrassApplication.Game;

namespace BrassApplication.UseCases.Menu.OfflineGame.CreateOfflineGame;

public interface ICreateOfflineGameInput
{
	void UpdateResumeOfflineGameView();

	void CreateNewGameConfiguration();

	BrassApplicationGame StartGame();

	void BackClicked();

	void ContinueClicked();

	void SetIntroductoryGame(bool isIntroductoryGame);

	void ShowIntroductoryGameTooltip(float x, float y);

	void HideTooltip();

	void StartGameClicked();
}
