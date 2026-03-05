using BoardGameRules.Entities.Players;

namespace BrassApplication.UseCases.Game.Replay;

public interface IReplayInput : IEventReplayDelegate
{
	void SkipReplay();

	PlayerColor GetReplayWatchingPlayerColor();
}
