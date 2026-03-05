using BrassApplication.UseCases.Game.Actions.BuildAction;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.Actions.BuildAction;

public class BuildActionController : IController
{
	private readonly IBuildActionInput _useCase;

	public BuildActionController(IBuildActionInput buildActionUseCase)
	{
		_useCase = buildActionUseCase;
	}

	public void OnViewCreated()
	{
	}

	public void OnViewDestroyed()
	{
	}
}
