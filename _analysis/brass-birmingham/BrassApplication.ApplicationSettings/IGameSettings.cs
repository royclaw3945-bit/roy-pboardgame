using BoardGameRules.Entities.Players;
using BrassApplication.AI;
using BrassApplication.Game;

namespace BrassApplication.ApplicationSettings;

public interface IGameSettings
{
	bool CameFromMenu();

	void SetMap(bool isNightMap);

	bool GetIsNightMap();

	void SetDefaultGameSettings(GameMetadata gameMetadata, int numberOfPLayers);

	int GetNumberOfPlayers();

	bool IsSavedDefaultGameSettings();

	PlayerColor GetPlayerIndexSavedColor(int index);

	PlayerAvatar GetPlayerIndexSavedAvatar(int index);

	AIType GetPlayerIndexSavedAIType(int index);
}
