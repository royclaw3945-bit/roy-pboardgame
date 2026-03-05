using BrassApplication.UseCases.Game.ExitToMenu;
using InterfaceAdapters.Menu.Settings;

namespace InterfaceAdapters.Game.ExitToMenu;

public class ExitToMenuController : SettingsController
{
	private IExitToMenuInput _useCase;

	public ExitToMenuController(IExitToMenuInput useCase)
		: base(useCase)
	{
		_useCase = useCase;
	}

	public void OnBackToMenuClicked()
	{
		_useCase.BackToMenu();
	}

	public string GetTranslation(string key)
	{
		return _useCase.GetTranslation(key);
	}

	public (string metadata, string events) GetSave()
	{
		return _useCase.GetSaveForReport();
	}
}
