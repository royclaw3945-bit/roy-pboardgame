using System.Collections.Generic;
using BrassApplication.ApplicationSettings;
using BrassApplication.UseCases.Common;
using BrassApplication.Utils;

namespace BrassApplication.UseCases.Menu.Settings;

public class SettingsUseCase : IUseCase, ISettingsInput
{
	private readonly ISettingsOutput _presenter;

	private readonly IAudioSettings _audioSettings;

	private readonly ILanguageSettings _languageSettings;

	private readonly IGameSettings _gameSettings;

	private readonly IAnalytics _analytics;

	public SettingsUseCase(ISettingsOutput presenter, IAudioSettings audioSettings, ILanguageSettings languageSettings, IGameSettings gameSettings, IAnalytics analytics)
	{
		_presenter = presenter;
		_audioSettings = audioSettings;
		_languageSettings = languageSettings;
		_gameSettings = gameSettings;
		_analytics = analytics;
	}

	public void BackClicked()
	{
		_presenter.HideView();
	}

	public void LoadSettings()
	{
		_presenter.RefreshSettings(_audioSettings, _languageSettings, _gameSettings);
		_analytics.ScreenVisit("Settings");
	}

	public void SetSoundVolume(float volume)
	{
		_audioSettings.SetSoundVolume(volume);
	}

	public void SetMusicVolume(float volume)
	{
		_audioSettings.SetMusicVolume(volume);
	}

	public IList<string> GetAvailableLanguages()
	{
		return _languageSettings.GetAvailableLanguages();
	}

	public void SetLanguage(string language)
	{
		_languageSettings.SetSelectedLanguage(language);
	}

	public void OnMapChanged(bool isNightMap)
	{
		_gameSettings.SetMap(isNightMap);
	}
}
