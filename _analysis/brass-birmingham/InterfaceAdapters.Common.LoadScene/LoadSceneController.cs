using BrassApplication.UseCases.Common.LoadScene;

namespace InterfaceAdapters.Common.LoadScene;

public class LoadSceneController : IController
{
	private readonly ILoadSceneInput _useCase;

	public LoadSceneController(ILoadSceneInput useCase)
	{
		_useCase = useCase;
	}

	public void OnViewCreated()
	{
		_useCase.GenerateGameTip();
	}

	public void OnViewDestroyed()
	{
	}
}
