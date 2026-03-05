using BrassApplication.ApplicationSettings;
using BrassApplication.UseCases.Menu.Settings;
using InterfaceAdapters.Common;
using InterfaceAdapters.Menu.MainMenu;
using InterfaceAdapters.Routing;

namespace InterfaceAdapters.Menu.Settings;

public class SettingsPresenter : IPresenter, ISettingsOutput
{
	protected readonly IRouter _router;

	public SettingsPresenter(IRouter router)
	{
		_router = router;
	}

	public virtual void HideView()
	{
		_router.HideView<ISettingsView>().OnComplete(delegate
		{
			_router.ShowView<IMainMenuView>();
		});
	}

	public void RefreshSettings(IAudioSettings audioSettings, ILanguageSettings languageSettings, IGameSettings gameSettings)
	{
		SettingsViewModel viewModel = new SettingsViewModel
		{
			MusicVolume = audioSettings.GetMusicVolume(),
			SoundVolume = audioSettings.GetSoundVolume(),
			LanguageSelected = languageSettings.GetSelectedLanguage(),
			AvailableLanguages = languageSettings.GetAvailableLanguages(),
			IsNightMap = gameSettings.GetIsNightMap()
		};
		_router.GetView<ISettingsView>().RefreshSettings(viewModel);
	}
}
