using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Utils.Logging;
using BrassApplication.Game;
using BrassApplication.Persistence.Repository;
using BrassApplication.Persistence.RepositoryBackends;

namespace Frameworks.Persistence.RepositoryBackends.Cached;

public class CachedGameStorage : IGameStorage, ICachedGameStorage
{
	protected readonly IGameStorage CacheGameStorage;

	protected readonly IGameStorage PersistentGameStorage;

	private readonly IDictionary<string, bool> _haveCachedEvents = new Dictionary<string, bool>();

	private readonly IDictionary<string, bool> _haveCachedMetadata = new Dictionary<string, bool>();

	private static readonly ILog Logger = LogFactory.BuildLogger(typeof(CachedGameStorage));

	protected CachedGameStorage(IGameStorage cacheGameStorage, IGameStorage persistentGameStorage)
	{
		CacheGameStorage = cacheGameStorage;
		PersistentGameStorage = persistentGameStorage;
	}

	public ReadOnlyCollection<IEvent> LoadEvents(string gameId, int minRevision, int maxRevision)
	{
		if (string.IsNullOrEmpty(gameId))
		{
			throw new ArgumentException("gameId can't be null or empty");
		}
		try
		{
			if (!_haveCachedEvents.ContainsKey(gameId) || !_haveCachedEvents[gameId])
			{
				PrecacheAllEventsAndMetadata(gameId);
			}
			Logger.Debug("Load events from cache");
			return CacheGameStorage.LoadEvents(gameId, minRevision, maxRevision);
		}
		catch (ArgumentException)
		{
			_haveCachedEvents[gameId] = false;
		}
		Logger.Debug("Load events from persistent storage");
		return PersistentGameStorage.LoadEvents(gameId, minRevision, maxRevision);
	}

	public void SaveGame(string gameId, IList<IEvent> eventsToAppend, GameMetadata metadata)
	{
		if (string.IsNullOrEmpty(gameId))
		{
			throw new ArgumentException("gameId can't be null or empty");
		}
		try
		{
			if (!_haveCachedEvents.ContainsKey(gameId) || !_haveCachedEvents[gameId] || !_haveCachedMetadata.ContainsKey(gameId) || !_haveCachedMetadata[gameId])
			{
				PrecacheAllEventsAndMetadata(gameId);
			}
			Logger.Debug("Save game to cache");
			CacheGameStorage.SaveGame(gameId, eventsToAppend, metadata);
		}
		catch (ArgumentException)
		{
			_haveCachedEvents[gameId] = false;
			_haveCachedMetadata[gameId] = false;
		}
		Logger.Debug("Save game to persistent storage");
		PersistentGameStorage.SaveGame(gameId, eventsToAppend, metadata);
	}

	public void DeleteEvents(string gameId)
	{
		if (string.IsNullOrEmpty(gameId))
		{
			throw new ArgumentException("gameId can't be null or empty");
		}
		try
		{
			_haveCachedEvents.Remove(gameId);
			CacheGameStorage.DeleteEvents(gameId);
		}
		catch (ArgumentException)
		{
		}
		PersistentGameStorage.DeleteEvents(gameId);
	}

	private void PrecacheAllEventsAndMetadata(string gameId)
	{
		Logger.Debug("Precache all events and metadata");
		try
		{
			ReadOnlyCollection<IEvent> eventsToAppend = PersistentGameStorage.LoadEvents(gameId, 0, int.MaxValue);
			GameMetadata metadata = PersistentGameStorage.LoadGameMetadata(gameId);
			CacheGameStorage.SaveGame(gameId, eventsToAppend, metadata);
			_haveCachedEvents[gameId] = true;
			_haveCachedMetadata[gameId] = true;
		}
		catch (Exception)
		{
		}
	}

	public void FlushCaches()
	{
		Logger.Debug("Flush cache");
		foreach (string key in _haveCachedEvents.Keys)
		{
			CacheGameStorage.DeleteEvents(key);
			CacheGameStorage.DeleteGameMetadata(key);
		}
		_haveCachedEvents.Clear();
		_haveCachedMetadata.Clear();
	}

	public string CreateGame(GameMetadata gameMetadata)
	{
		return PersistentGameStorage.CreateGame(gameMetadata);
	}

	public ReadOnlyCollection<GameMetadata> GetGamesMetadata(IGamesFilter gamesFilter)
	{
		return PersistentGameStorage.GetGamesMetadata(gamesFilter);
	}

	public void SavePlayerMetadata(string gameId, PlayerMetadata metadata)
	{
		if (string.IsNullOrEmpty(gameId))
		{
			throw new ArgumentException("gameId can't be null or empty");
		}
		try
		{
			if (!_haveCachedMetadata.ContainsKey(gameId) || !_haveCachedMetadata[gameId])
			{
				PrecacheAllEventsAndMetadata(gameId);
			}
			Logger.Debug("Save player metadata to cache");
			CacheGameStorage.SavePlayerMetadata(gameId, metadata);
		}
		catch (ArgumentException)
		{
			_haveCachedEvents[gameId] = false;
			_haveCachedMetadata[gameId] = false;
		}
		Logger.Debug("Save player metadata to persistent storage");
		PersistentGameStorage.SavePlayerMetadata(gameId, metadata);
	}

	public GameMetadata LoadGameMetadata(string gameId)
	{
		if (string.IsNullOrEmpty(gameId))
		{
			throw new ArgumentException("gameId can't be null or empty");
		}
		try
		{
			if (!_haveCachedMetadata.ContainsKey(gameId) || !_haveCachedMetadata[gameId])
			{
				PrecacheAllEventsAndMetadata(gameId);
			}
			Logger.Debug("Load game metadata from cache");
			return CacheGameStorage.LoadGameMetadata(gameId);
		}
		catch (ArgumentException)
		{
			_haveCachedMetadata[gameId] = false;
		}
		Logger.Debug("Load game metadata from persistent storage");
		return PersistentGameStorage.LoadGameMetadata(gameId);
	}

	public void DeleteGameMetadata(string gameId)
	{
		if (string.IsNullOrEmpty(gameId))
		{
			throw new ArgumentException("gameId can't be null or empty");
		}
		_haveCachedMetadata.Remove(gameId);
		CacheGameStorage.DeleteGameMetadata(gameId);
		PersistentGameStorage.DeleteGameMetadata(gameId);
	}

	public void JoinGame(string gameId, string password, PlayerMetadata playerMetadata)
	{
		try
		{
			if (!_haveCachedMetadata.ContainsKey(gameId) || !_haveCachedMetadata[gameId])
			{
				PrecacheAllEventsAndMetadata(gameId);
			}
			Logger.Debug("Join game metadata from cache");
			CacheGameStorage.JoinGame(gameId, password, playerMetadata);
		}
		catch (ArgumentException)
		{
			_haveCachedMetadata[gameId] = false;
		}
		Logger.Debug("Join game metadata from persistent storage");
		PersistentGameStorage.JoinGame(gameId, password, playerMetadata);
	}

	public void LeaveGame(string gameId, string playerId, bool isThisAnAbandonRequest)
	{
		try
		{
			if (!_haveCachedMetadata.ContainsKey(gameId) || !_haveCachedMetadata[gameId])
			{
				PrecacheAllEventsAndMetadata(gameId);
			}
			Logger.Debug("Leave game metadata from cache");
			CacheGameStorage.LeaveGame(gameId, playerId, isThisAnAbandonRequest);
		}
		catch (ArgumentException)
		{
			_haveCachedMetadata[gameId] = false;
		}
		Logger.Debug("Leave game metadata from persistent storage");
		PersistentGameStorage.LeaveGame(gameId, playerId, isThisAnAbandonRequest);
	}

	public void DeleteAll()
	{
		_haveCachedEvents.Clear();
		_haveCachedMetadata.Clear();
		CacheGameStorage.DeleteAll();
		PersistentGameStorage.DeleteAll();
	}
}
