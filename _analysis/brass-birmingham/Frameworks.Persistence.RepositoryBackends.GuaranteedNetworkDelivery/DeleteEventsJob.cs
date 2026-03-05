using BoardGameRules.Utils.Logging;
using BrassApplication.Persistence.RepositoryBackends;

namespace Frameworks.Persistence.RepositoryBackends.GuaranteedNetworkDelivery;

public class DeleteEventsJob : IJob
{
	private static readonly ILog Logger = LogFactory.BuildLogger(typeof(SaveGameJob));

	public string GameId { get; set; }

	public DeleteEventsJob(string gameId)
	{
		GameId = gameId;
	}

	public void ProcessUsing(IGameStorage gameStorage)
	{
		Logger.Debug("DeleteEventsJob");
		gameStorage.DeleteEvents(GameId);
	}
}
