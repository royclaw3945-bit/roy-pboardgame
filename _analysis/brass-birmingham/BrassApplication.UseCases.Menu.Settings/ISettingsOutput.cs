using BrassApplication.ApplicationSettings;

namespace BrassApplication.UseCases.Menu.Settings;

public interface ISettingsOutput
{
	void HideView();

	void RefreshSettings(IAudioSettings audioSettings, ILanguageSettings languageSettings, IGameSettings gameSettings);
}
