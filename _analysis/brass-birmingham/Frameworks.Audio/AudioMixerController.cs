using UnityEngine;
using UnityEngine.Audio;

namespace Frameworks.Audio;

public class AudioMixerController : MonoBehaviour
{
	public AudioMixer MainAudioMixer;

	private readonly string MusicVolumeProperty = "MusicVolume";

	private readonly string SoundVolumeProperty = "SoundVolume";

	private void Start()
	{
		RestoreSavedVolumeSettings();
	}

	public void RestoreSavedVolumeSettings()
	{
		SetMusicVolume(LoadMusicVolume());
		SetSoundVolume(LoadSoundVolume());
	}

	public void SetMusicVolume(float musicVolume)
	{
		MainAudioMixer.SetFloat(MusicVolumeProperty, AudioUnitsConversion.LinearToDecibel(musicVolume));
		MainAudioMixer.GetFloat(MusicVolumeProperty, out var _);
	}

	public float GetMusicVolume()
	{
		float value = 1f;
		if (MainAudioMixer.GetFloat(MusicVolumeProperty, out value))
		{
			value = AudioUnitsConversion.DecibelToLinear(value);
		}
		return value;
	}

	private float LoadMusicVolume()
	{
		if (!PlayerPrefs.HasKey("MusicVolume"))
		{
			return 1f;
		}
		return PlayerPrefs.GetFloat("MusicVolume");
	}

	private void SaveMusicVolume(float musicVolume)
	{
		PlayerPrefs.SetFloat("MusicVolume", musicVolume);
		PlayerPrefs.Save();
	}

	public void SetSoundVolume(float soundVolume)
	{
		MainAudioMixer.SetFloat(SoundVolumeProperty, AudioUnitsConversion.LinearToDecibel(soundVolume));
	}

	public float GetSoundVolume()
	{
		float value = 1f;
		if (MainAudioMixer.GetFloat(SoundVolumeProperty, out value))
		{
			value = AudioUnitsConversion.DecibelToLinear(value);
		}
		return value;
	}

	private void SaveSoundVolume(float soundVolume)
	{
		PlayerPrefs.SetFloat("SoundVolume", soundVolume);
		PlayerPrefs.Save();
	}

	private float LoadSoundVolume()
	{
		if (!PlayerPrefs.HasKey("SoundVolume"))
		{
			return 1f;
		}
		return PlayerPrefs.GetFloat("SoundVolume");
	}
}
