using BrassApplication.Game;

namespace BrassApplication.UseCases.Game.PlaySpectatorTurn;

public interface IPlaySpectatorTurnOutput
{
	void ShowSkipToLatestTurnView();

	void ShowWaitingForOnlinePlayerView(PlayerMetadata playerMetadata);

	void PlayGame();

	void ReplayUnseenActions(PlayerMetadata replayWatchingPlayerMetadata, PlaySpectatorTurnUseCase replayDelegate);

	void DisableBuildingFromTheBoard();

	void EnableBuildingFromTheBoard();

	void ShowSpectatorCards(PlayerMetadata replayWatchingPlayerMetadata);

	void ShowSpectatorIndustries(PlayerMetadata replayWatchingPlayerMetadata);
}
