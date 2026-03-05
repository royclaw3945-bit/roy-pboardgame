using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Utils.Logging;
using BrassApplication.Game;
using BrassApplication.Persistence.Repository;
using BrassApplication.Persistence.RepositoryBackends;
using Frameworks.Persistence.RepositoryBackends.Network;

namespace Frameworks.Persistence.RepositoryBackends.GuaranteedNetworkDelivery;

public class GuaranteedDeliveryNetworkGameStorageInternal : IGameStorage
{
	private static readonly ILog Logger = LogFactory.BuildLogger(typeof(GuaranteedDeliveryNetworkGameStorageInternal));

	private readonly IGuaranteedDeliveryJobQueue _jobQueue;

	private readonly NetworkGameStorage _networkGameStorage;

	public GuaranteedDeliveryNetworkGameStorageInternal(IGuaranteedDeliveryJobQueue jobQueue, NetworkGameStorage networkGameStorage)
	{
		_jobQueue = jobQueue;
		_networkGameStorage = networkGameStorage;
	}

	public ReadOnlyCollection<IEvent> LoadEvents(string gameId, int minRevision, int maxRevision)
	{
		Logger.Debug("LoadEvents");
		return _networkGameStorage.LoadEvents(gameId, minRevision, maxRevision);
	}

	public void SaveGame(string gameId, IList<IEvent> eventsToAppend, GameMetadata metadata)
	{
		Logger.Debug("SaveGame");
		_jobQueue.Enqueue(new SaveGameJob(gameId, eventsToAppend, metadata));
	}

	public void DeleteEvents(string gameId)
	{
		Logger.Debug("DeleteEvents");
		_jobQueue.Enqueue(new DeleteEventsJob(gameId));
	}

	public string CreateGame(GameMetadata gameStateMetadata)
	{
		Logger.Debug("CreateGame");
		return _networkGameStorage.CreateGame(gameStateMetadata);
	}

	public ReadOnlyCollection<GameMetadata> GetGamesMetadata(IGamesFilter gamesFilter)
	{
		Logger.Debug("GetGamesMetadata");
		return _networkGameStorage.GetGamesMetadata(gamesFilter);
	}

	public void SavePlayerMetadata(string gameId, PlayerMetadata metadata)
	{
		Logger.Debug("SavePlayerMetadata");
		_jobQueue.Enqueue(new SavePlayerMetadataJob(gameId, metadata));
	}

	public GameMetadata LoadGameMetadata(string gameId)
	{
		Logger.Debug("LoadGameMetadata");
		return _networkGameStorage.LoadGameMetadata(gameId);
	}

	public void DeleteGameMetadata(string gameId)
	{
		Logger.Debug("DeleteGameMetadata");
		_jobQueue.Enqueue(new DeleteGameMetadataJob(gameId));
	}

	public void JoinGame(string gameId, string password, PlayerMetadata playerMetadata)
	{
		Logger.Debug("JoinGame");
		_networkGameStorage.JoinGame(gameId, password, playerMetadata);
	}

	public void LeaveGame(string gameId, string playerId, bool isThisAnAbandonRequest)
	{
		Logger.Debug("LeaveGame");
		_networkGameStorage.LeaveGame(gameId, playerId, isThisAnAbandonRequest);
	}

	public void DeleteAll()
	{
		throw new NotImplementedException();
	}
}
