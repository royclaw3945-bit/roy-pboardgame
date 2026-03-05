using BrassApplication.UseCases.Game.PlayAiTurn;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.PlayAiTurn;

public class PlayAiTurnController : IController
{
	private readonly IPlayAITurnInput _useCase;

	public PlayAiTurnController(IPlayAITurnInput useCase)
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
