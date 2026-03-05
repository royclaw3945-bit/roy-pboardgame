namespace BrassApplication.UseCases.Menu.MainMenu;

public interface IMainMenuOutput
{
	void ShowMainMenu();

	void ShowOfflineGameView();

	void StartTutorial();

	void ShowOnlineGamesView();

	void ShowLoginView();

	void ShowTestGamesView();

	void ShowRulebookView();

	void ShowSettingsView();

	void ShowCreditsView();

	void ShowQuitGameQuestion();

	void ShowConnectionView();

	void HideConnectionView();
}
