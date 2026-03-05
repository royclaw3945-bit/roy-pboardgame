using BrassApplication.UseCases.Menu.Settings;

namespace BrassApplication.UseCases.Game.ExitToMenu;

public interface IExitToMenuInput : ISettingsInput
{
	void BackToMenu();

	string GetTranslation(string key);

	(string metadata, string events) GetSaveForReport();
}
