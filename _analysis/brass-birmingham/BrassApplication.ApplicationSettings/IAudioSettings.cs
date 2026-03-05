namespace BrassApplication.ApplicationSettings;

public interface IAudioSettings
{
	float GetMusicVolume();

	float GetSoundVolume();

	void SetMusicVolume(float volume);

	void SetSoundVolume(float volume);
}
