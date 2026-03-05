using BrassApplication.UseCases.Menu.Settings;

namespace BrassApplication.UseCases.Game.ExitToMenu;

public interface IExitToMenuOutput : ISettingsOutput
{
	void ShowLoadingViewAndLoadMenuScene();

	string GetTranslation(string key);
}
