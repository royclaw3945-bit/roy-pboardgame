using BrassApplication.UseCases.Game.Actions.ScoutAction;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.Actions.ScoutAction;

public class ScoutActionController : IController
{
	private IScoutActionInput _useCase;

	public ScoutActionController(IScoutActionInput scoutActionUseCase)
	{
		_useCase = scoutActionUseCase;
	}

	public void OnViewCreated()
	{
	}

	public void OnViewDestroyed()
	{
	}
}
