using BrassApplication.ApplicationSettings;

namespace Frameworks.Settings;

public class MenuSettings : IMenuSettings
{
	private static bool _shouldShowSplashScreen = true;

	public bool ShouldShowSplashScreen()
	{
		return _shouldShowSplashScreen;
	}

	public void SetShouldShowSplashScreen(bool newSetting)
	{
		_shouldShowSplashScreen = newSetting;
	}
}
