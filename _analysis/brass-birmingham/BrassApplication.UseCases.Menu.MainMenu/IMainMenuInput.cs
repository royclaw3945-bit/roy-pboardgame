namespace BrassApplication.UseCases.Menu.MainMenu;

public interface IMainMenuInput
{
	void ShowOfflineGameView();

	void StartTutorial();

	void ShowOnlineGamesView();

	void ShowTestGamesView();

	void ShowRulebookView();

	void ShowSettingsView();

	void ShowCreditsView();

	void ShowQuitGameQuestion();
}
