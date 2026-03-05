using BrassApplication.UseCases.Game.PlayAiTurn;
using BrassApplication.UseCases.Game.PlayGame;
using InterfaceAdapters.Common;
using InterfaceAdapters.Routing;

namespace InterfaceAdapters.Game.PlayAiTurn;

public class PlayAITurnPresenter : IPresenter, IPlayAITurnOutput
{
	private readonly IRouter _router;

	public PlayAITurnPresenter(IRouter router)
	{
		_router = router;
	}

	public void EndTurn()
	{
		_router.UseCaseBuilder.GetUseCase<PlayGameUseCase>().EndTurn();
	}
}
