using BrassApplication.UseCases.Menu.SplashScreen;
using InterfaceAdapters.Common;
using InterfaceAdapters.Routing;

namespace InterfaceAdapters.Menu.MainMenu;

public class SplashScreenPresenter : IPresenter, ISplashScreenOutput
{
	private readonly IRouter _router;

	public SplashScreenPresenter(IRouter router)
	{
		_router = router;
	}

	public void ShowSplashScreen()
	{
		_router.ShowView<ISplashScreenView>().Play();
	}

	public void ShowMainMenu()
	{
		_router.ShowView<IMainMenuView>();
	}

	public void HideSplashScreen()
	{
		_router.HideView<ISplashScreenView>();
	}
}
