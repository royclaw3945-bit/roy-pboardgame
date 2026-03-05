using BoardGameRules.Entities.Players;
using BrassApplication.UseCases.Game.Replay;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.Replay;

public class ReplayController : IController
{
	private readonly IReplayInput _useCase;

	public ReplayController(IReplayInput useCase)
	{
		_useCase = useCase;
	}

	public void OnViewCreated()
	{
	}

	public void OnViewDestroyed()
	{
	}

	public void SkipToMyTurnClicked()
	{
		_useCase.SkipReplay();
	}

	public PlayerColor GetReplayWatchingPlayerColor()
	{
		return _useCase.GetReplayWatchingPlayerColor();
	}
}
