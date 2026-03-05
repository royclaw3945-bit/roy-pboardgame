using BrassApplication.UseCases.Menu.MainMenu;
using InterfaceAdapters.Common;
using InterfaceAdapters.Common.CreateGame;
using InterfaceAdapters.Menu.Connection;
using InterfaceAdapters.Menu.Credits;
using InterfaceAdapters.Menu.Login;
using InterfaceAdapters.Menu.OnlineGameMenu;
using InterfaceAdapters.Menu.Settings;
using InterfaceAdapters.Menu.Tutorial;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Utils;

namespace InterfaceAdapters.Menu.MainMenu;

public class MainMenuPresenter : IPresenter, IMainMenuOutput
{
	private readonly IRouter _router;

	private readonly ILocalization _localization;

	public MainMenuPresenter(IRouter router, ILocalization localization)
	{
		_router = router;
		_localization = localization;
	}

	public void ShowMainMenu()
	{
		_router.ShowView<IMainMenuView>();
	}

	public void ShowOfflineGameView()
	{
		_router.HideView<IMainMenuView>().OnComplete(delegate
		{
			_router.ShowView<ICreateOfflineGameView>();
		});
	}

	public void StartTutorial()
	{
		_router.HideView<IMainMenuView>().OnComplete(delegate
		{
			_router.GetView<ITutorialView>().StartTutorial();
		});
	}

	public void ShowOnlineGamesView()
	{
		_router.HideView<IMainMenuView>().OnComplete(delegate
		{
			_router.ShowView<IListPlayerOnlineGamesMenuView>();
		});
	}

	public void ShowLoginView()
	{
		_router.HideView<IMainMenuView>().OnComplete(delegate
		{
			_router.ShowView<ILoginView>();
		});
	}

	public void ShowTestGamesView()
	{
	}

	public void ShowRulebookView()
	{
	}

	public void ShowSettingsView()
	{
		_router.HideView<IMainMenuView>().OnComplete(delegate
		{
			_router.ShowView<ISettingsView>();
		});
	}

	public void ShowCreditsView()
	{
		_router.HideView<IMainMenuView>().OnComplete(delegate
		{
			_router.ShowView<ICreditsView>();
		});
	}

	public void ShowQuitGameQuestion()
	{
		_router.ShowView<IQuestionPopupView>();
		string translation = _localization.GetTranslation("UI/DO_YOU_WANT_QUIT");
		string translation2 = _localization.GetTranslation("UI/YES");
		string translation3 = _localization.GetTranslation("UI/NO");
		_router.GetView<IQuestionPopupView>().Setup(translation, isConfirmNegative: true, translation2, translation3, _router.GetView<IMainMenuView>().QuitTheGame, delegate
		{
			_router.HideView<IQuestionPopupView>();
		});
	}

	public void ShowConnectionView()
	{
		_router.ShowView<IConnectionView>();
	}

	public void HideConnectionView()
	{
		_router.HideView<IConnectionView>();
	}
}
