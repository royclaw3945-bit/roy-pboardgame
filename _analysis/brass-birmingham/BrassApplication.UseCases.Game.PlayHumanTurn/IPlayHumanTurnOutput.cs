using BrassApplication.Game;
using BrassApplication.UseCases.Game.Replay;

namespace BrassApplication.UseCases.Game.PlayHumanTurn;

public interface IPlayHumanTurnOutput
{
	void ReplayUnseenActions(PlayerMetadata replayWatchingPlayerMetadata, IReplayDelegate replayDelegate);

	void ShowSkipToMyNextTurnView();

	void RestorePlayerActionsView();
}
