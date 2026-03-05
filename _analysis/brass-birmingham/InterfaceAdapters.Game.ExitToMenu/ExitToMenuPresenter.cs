using BrassApplication.UseCases.Game.ExitToMenu;
using BrassApplication.UseCases.Menu.Settings;
using InterfaceAdapters.Common.LoadScene;
using InterfaceAdapters.Menu.Settings;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Utils;

namespace InterfaceAdapters.Game.ExitToMenu;

public class ExitToMenuPresenter : SettingsPresenter, IExitToMenuOutput, ISettingsOutput
{
	private ILocalization _localization;

	public ExitToMenuPresenter(IRouter router, ILocalization localization)
		: base(router)
	{
		_localization = localization;
	}

	public void ShowLoadingViewAndLoadMenuScene()
	{
		_router.ShowView<ILoadSceneView>().OnComplete(delegate
		{
			_router.MoveOnTop<ILoadSceneView>();
			_router.GetView<ILoadSceneView>().LoadScene("MenuScene");
		});
	}

	public override void HideView()
	{
		_router.HideView<IExitToMenuView>();
	}

	public string GetTranslation(string key)
	{
		return _localization.GetTranslation(key);
	}
}
