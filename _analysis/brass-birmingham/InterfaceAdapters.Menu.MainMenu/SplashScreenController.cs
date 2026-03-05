using BrassApplication.UseCases.Menu.SplashScreen;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Menu.MainMenu;

public class SplashScreenController : IController
{
	private readonly ISplashScreenInput _useCase;

	public SplashScreenController(ISplashScreenInput useCase)
	{
		_useCase = useCase;
	}

	public void OnViewCreated()
	{
	}

	public void OnViewDestroyed()
	{
	}

	public void OnApplicationStarted()
	{
		_useCase.StartApplication();
	}

	public void OnSplashPresentationCompleted()
	{
		_useCase.SplashPresentationCompleted();
	}
}
