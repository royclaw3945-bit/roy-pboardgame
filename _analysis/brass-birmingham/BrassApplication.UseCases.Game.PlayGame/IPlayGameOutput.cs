using BrassApplication.Game;
using BrassApplication.UseCases.Common.SaveGame;

namespace BrassApplication.UseCases.Game.PlayGame;

public interface IPlayGameOutput
{
	void StartHumanTurn();

	void StartAITurn(PlayerMetadata playerMetadata);

	void StartSellingIndustries();

	void SaveGame(BrassApplicationGame game, ISaveGameDelegate saveGameDelegate);

	void StartGameReview(PlayerMetadata localPlayerMetadata);

	void StartNetworkHumanTurn();

	void StartNetworkSpectatorTurn(PlayerMetadata replayWatchingPlayerMetadata);
}
