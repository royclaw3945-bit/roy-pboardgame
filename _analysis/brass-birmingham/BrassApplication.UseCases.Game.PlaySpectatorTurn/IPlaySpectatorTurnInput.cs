using BrassApplication.UseCases.Game.Replay;

namespace BrassApplication.UseCases.Game.PlaySpectatorTurn;

public interface IPlaySpectatorTurnInput : IReplayDelegate
{
	void OnViewDestroyed();
}
