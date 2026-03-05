using BrassApplication.ApplicationSettings;

namespace Frameworks.Settings;

public static class SettingsFactory
{
	public static IMenuSettings CreateMenuSettings()
	{
		return new MenuSettings();
	}

	public static IAudioSettings CreateAudioSettings()
	{
		return new AudioSettings();
	}

	public static ILanguageSettings CreateLanguageSettings()
	{
		return new LanguageSettings();
	}

	public static IGameSettings CreateGameSettings()
	{
		return new GameSettings();
	}

	public static INetworkFilterSettings CreateNetworkFilterSettings()
	{
		return new NetworkFilterSettings();
	}
}
