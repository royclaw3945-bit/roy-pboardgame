namespace BrassApplication.ApplicationSettings;

public interface IMenuSettings
{
	bool ShouldShowSplashScreen();

	void SetShouldShowSplashScreen(bool newSetting);
}
