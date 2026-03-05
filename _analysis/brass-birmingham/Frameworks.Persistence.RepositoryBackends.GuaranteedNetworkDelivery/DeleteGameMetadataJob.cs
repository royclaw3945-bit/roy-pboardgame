using BoardGameRules.Utils.Logging;
using BrassApplication.Persistence.RepositoryBackends;

namespace Frameworks.Persistence.RepositoryBackends.GuaranteedNetworkDelivery;

public class DeleteGameMetadataJob : IJob
{
	private static readonly ILog Logger = LogFactory.BuildLogger(typeof(DeleteGameMetadataJob));

	public string GameId { get; set; }

	public DeleteGameMetadataJob(string gameId)
	{
		GameId = gameId;
	}

	public void ProcessUsing(IGameStorage gameStorage)
	{
		Logger.Debug("DeleteGameMetadataJob");
		gameStorage.DeleteGameMetadata(GameId);
	}
}
