using BrassApplication.ApplicationSettings;
using BrassApplication.UseCases.Common;

namespace BrassApplication.UseCases.Menu.SplashScreen;

public class SplashScreenUseCase : IUseCase, ISplashScreenInput
{
	private readonly ISplashScreenOutput _presenter;

	private readonly IMenuSettings _settings;

	private readonly ILanguageSettings _languageSettings;

	public SplashScreenUseCase(ISplashScreenOutput presenter, IMenuSettings settings, ILanguageSettings languageSettings)
	{
		_presenter = presenter;
		_settings = settings;
		_languageSettings = languageSettings;
	}

	public void StartApplication()
	{
		_languageSettings.SetSelectedLanguage(_languageSettings.GetSelectedLanguage());
		IMenuSettings settings = _settings;
		if (settings == null || settings.ShouldShowSplashScreen())
		{
			_settings?.SetShouldShowSplashScreen(newSetting: false);
			_presenter.ShowSplashScreen();
		}
		else
		{
			_presenter.ShowMainMenu();
		}
	}

	public void SplashPresentationCompleted()
	{
		_presenter.HideSplashScreen();
		_presenter.ShowMainMenu();
	}
}
