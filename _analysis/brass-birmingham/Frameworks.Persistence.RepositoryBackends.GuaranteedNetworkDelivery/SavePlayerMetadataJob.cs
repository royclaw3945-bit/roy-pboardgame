using BoardGameRules.Utils.Logging;
using BrassApplication.Game;
using BrassApplication.Persistence.RepositoryBackends;

namespace Frameworks.Persistence.RepositoryBackends.GuaranteedNetworkDelivery;

public class SavePlayerMetadataJob : IJob
{
	private static readonly ILog Logger = LogFactory.BuildLogger(typeof(SavePlayerMetadataJob));

	public string GameId { get; set; }

	public PlayerMetadata Metadata { get; set; }

	public SavePlayerMetadataJob(string gameId, PlayerMetadata metadata)
	{
		GameId = gameId;
		Metadata = metadata;
	}

	public void ProcessUsing(IGameStorage gameStorage)
	{
		Logger.Debug("SavePlayerMetadataJob");
		gameStorage.SavePlayerMetadata(GameId, Metadata);
	}
}
