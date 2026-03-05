using BrassApplication.UseCases.Game.PlayGame;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.PlayGame;

public class PlayGameController : IController
{
	private readonly IPlayGameInput _useCase;

	public PlayGameController(IPlayGameInput useCase)
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
