using BrassApplication.UseCases.Game.Actions.PassAction;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.Actions.PassAction;

public class PassActionController : IController
{
	private IPassActionInput _useCase;

	public PassActionController(IPassActionInput buildActionUseCase)
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
