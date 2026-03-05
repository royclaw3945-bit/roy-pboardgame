using BrassApplication.UseCases.Game.PlayHumanTurn;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.PlayHumanTurn;

public class PlayHumanTurnController : IController
{
	private readonly IPlayHumanTurnInput _useCase;

	public PlayHumanTurnController(IPlayHumanTurnInput useCase)
	{
		_useCase = useCase;
	}

	public void OnViewCreated()
	{
	}

	public void OnViewDestroyed()
	{
	}
}
