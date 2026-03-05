namespace BrassApplication.UseCases.Menu.Settings;

public interface ISettingsInput
{
	void BackClicked();

	void LoadSettings();

	void SetSoundVolume(float volume);

	void SetMusicVolume(float volume);

	void SetLanguage(string language);

	void OnMapChanged(bool isNightMap);
}
