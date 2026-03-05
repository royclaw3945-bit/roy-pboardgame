using System.Collections.Generic;
using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Utils.Logging;
using BrassApplication.Game;
using BrassApplication.Persistence.RepositoryBackends;

namespace Frameworks.Persistence.RepositoryBackends.GuaranteedNetworkDelivery;

public class SaveGameJob : IJob
{
	private static readonly ILog Logger = LogFactory.BuildLogger(typeof(SaveGameJob));

	public string GameId { get; set; }

	public IList<IEvent> Events { get; set; }

	public GameMetadata Metadata { get; set; }

	public SaveGameJob(string gameId, IList<IEvent> events, GameMetadata metadata)
	{
		GameId = gameId;
		Events = events;
		Metadata = metadata;
	}

	public void ProcessUsing(IGameStorage gameStorage)
	{
		Logger.Debug("SaveEventsJob");
		gameStorage.SaveGame(GameId, Events, Metadata);
	}
}
