using BrassApplication.UseCases.Menu.Settings;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Menu.Settings;

public class SettingsController : IController
{
	private readonly ISettingsInput _useCase;

	public SettingsController(ISettingsInput useCase)
	{
		_useCase = useCase;
	}

	public void OnViewCreated()
	{
		_useCase.LoadSettings();
	}

	public void OnViewDestroyed()
	{
	}

	public void OnBackClicked()
	{
		_useCase.BackClicked();
	}

	public void OnSoundChanged(float volume)
	{
		_useCase.SetSoundVolume(volume);
	}

	public void OnMusicChanged(float volume)
	{
		_useCase.SetMusicVolume(volume);
	}

	public void OnLanguageChanged(string language)
	{
		_useCase.SetLanguage(language);
	}

	public void OnMapChanged(bool isNightMap)
	{
		_useCase.OnMapChanged(isNightMap);
	}
}
