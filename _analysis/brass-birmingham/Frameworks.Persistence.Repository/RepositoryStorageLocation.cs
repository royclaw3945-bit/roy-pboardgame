using UnityEngine;

namespace Frameworks.Persistence.Repository;

public class RepositoryStorageLocation
{
	public static readonly string GuaranteedDeliveryJobQueueStoragePath = GetPersistentDataDirectory() + "/games/jobs";

	private const string GamesStoragePath = "/games/local/";

	private const string TestGamesStoragePath = "/games/test_games/";

	private const string JobQueueStoragePath = "/games/jobs";

	private const string LocalGamePath = "local_game";

	private const string TutorialGamePath = "tutorial_game";

	public const int NetworkBackendTimeoutInSeconds = 15;

	public const string NetworkBackendUrl = "https://brass-birmingham.api.cublogames.com/index.php";

	public static string LocalGameStoragePath => GetPersistentDataDirectory() + "/games/local/local_game";

	public static string TutorialGameStoragePath => GetPersistentDataDirectory() + "/games/local/tutorial_game";

	public static string TestGameStoragePath => GetPersistentDataDirectory() + "/games/test_games/";

	public static string GetPersistentDataDirectory()
	{
		return Application.persistentDataPath;
	}
}
