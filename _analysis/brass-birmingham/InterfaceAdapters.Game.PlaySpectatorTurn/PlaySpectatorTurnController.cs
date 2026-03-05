using BrassApplication.UseCases.Game.PlaySpectatorTurn;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.PlaySpectatorTurn;

public class PlaySpectatorTurnController : IController
{
	private readonly IPlaySpectatorTurnInput _useCase;

	public PlaySpectatorTurnController(IPlaySpectatorTurnInput useCase)
	{
		_useCase = useCase;
	}

	public void OnViewCreated()
	{
	}

	public void OnViewDestroyed()
	{
		_useCase.OnViewDestroyed();
	}
}
