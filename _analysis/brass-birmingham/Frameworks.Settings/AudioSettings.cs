using BrassApplication.ApplicationSettings;
using UnityEngine;

namespace Frameworks.Settings;

public class AudioSettings : IAudioSettings
{
	public const string MUSIC_PLAYERPREF_KEY = "MusicVolume";

	public const string SOUND_PLAYERPREF_KEY = "SoundVolume";

	public float GetMusicVolume()
	{
		if (PlayerPrefs.HasKey("MusicVolume"))
		{
			return PlayerPrefs.GetFloat("MusicVolume");
		}
		return 0f;
	}

	public float GetSoundVolume()
	{
		if (PlayerPrefs.HasKey("SoundVolume"))
		{
			return PlayerPrefs.GetFloat("SoundVolume");
		}
		return 0f;
	}

	public void SetMusicVolume(float volume)
	{
		PlayerPrefs.SetFloat("MusicVolume", volume);
	}

	public void SetSoundVolume(float volume)
	{
		PlayerPrefs.SetFloat("SoundVolume", volume);
	}
}
