using System.Collections.Generic;

namespace InterfaceAdapters.Menu.Settings;

public class SettingsViewModel
{
	public float MusicVolume { get; set; }

	public float SoundVolume { get; set; }

	public string LanguageSelected { get; set; }

	public IList<string> AvailableLanguages { get; set; }

	public bool IsNightMap { get; set; }
}
