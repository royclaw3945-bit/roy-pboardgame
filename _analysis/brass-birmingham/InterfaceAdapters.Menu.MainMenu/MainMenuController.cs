using BrassApplication.UseCases.Menu.MainMenu;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Menu.MainMenu;

public class MainMenuController : IController
{
	private readonly IMainMenuInput _useCase;

	public MainMenuController(IMainMenuInput useCase)
	{
		_useCase = useCase;
	}

	public void OnViewCreated()
	{
	}

	public void OnViewDestroyed()
	{
	}

	public void ShowOfflineGameView()
	{
		_useCase.ShowOfflineGameView();
	}

	public void StartTutorial()
	{
		_useCase.StartTutorial();
	}

	public void ShowOnlineGamesView()
	{
		_useCase.ShowOnlineGamesView();
	}

	public void ShowTestGamesView()
	{
		_useCase.ShowTestGamesView();
	}

	public void ShowRulebookView()
	{
		_useCase.ShowRulebookView();
	}

	public void ShowSettingsView()
	{
		_useCase.ShowSettingsView();
	}

	public void ShowCreditsView()
	{
		_useCase.ShowCreditsView();
	}

	public void ShowQuitGameQuestion()
	{
		_useCase.ShowQuitGameQuestion();
	}
}
