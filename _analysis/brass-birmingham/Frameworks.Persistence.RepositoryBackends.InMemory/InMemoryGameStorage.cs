using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BoardGameRules.Entities.EventSourcing.Events;
using BrassApplication.Game;
using BrassApplication.Persistence.Repository;
using BrassApplication.Persistence.RepositoryBackends;

namespace Frameworks.Persistence.RepositoryBackends.InMemory;

public class InMemoryGameStorage : IGameStorage
{
	private readonly Dictionary<string, GameMetadata> storageMetadata;

	private readonly Dictionary<string, List<IEvent>> events;

	public InMemoryGameStorage()
	{
		events = new Dictionary<string, List<IEvent>>();
		storageMetadata = new Dictionary<string, GameMetadata>();
	}

	public ReadOnlyCollection<IEvent> LoadEvents(string gameId, int minRevision, int maxRevision)
	{
		if (string.IsNullOrEmpty(gameId))
		{
			throw new ArgumentException("gameId can't be null or empty");
		}
		if (!events.ContainsKey(gameId))
		{
			events[gameId] = new List<IEvent>();
		}
		if (minRevision > maxRevision)
		{
			throw new ArgumentException("MinRevision can't be greater than MaxRevision");
		}
		if (minRevision > events[gameId].Count)
		{
			return new List<IEvent>().AsReadOnly();
		}
		int val = events[gameId].Count - minRevision;
		int count = Math.Min(maxRevision - minRevision, val);
		return events[gameId].GetRange(minRevision, count).AsReadOnly();
	}

	public void SaveGame(string gameId, IList<IEvent> eventsToAppend, GameMetadata metadata)
	{
		SaveEvents(gameId, eventsToAppend);
		SaveGameMetadata(gameId, metadata);
	}

	public void SaveEvents(string gameId, IList<IEvent> eventsToAppend)
	{
		if (string.IsNullOrEmpty(gameId))
		{
			throw new ArgumentException("gameId can't be null or empty");
		}
		if (!events.ContainsKey(gameId))
		{
			events[gameId] = new List<IEvent>();
		}
		events[gameId].AddRange(eventsToAppend);
	}

	public void DeleteEvents(string gameId)
	{
		if (string.IsNullOrEmpty(gameId))
		{
			throw new ArgumentException("gameId can't be null or empty");
		}
		if (events.ContainsKey(gameId))
		{
			events.Remove(gameId);
		}
	}

	public string CreateGame(GameMetadata gameMetadata)
	{
		string text = (gameMetadata.GameId = Guid.NewGuid().ToString());
		storageMetadata[text] = gameMetadata;
		return text;
	}

	public ReadOnlyCollection<GameMetadata> GetGamesMetadata(IGamesFilter gamesFilter)
	{
		return (from gameId in GetGameIds()
			select LoadGameMetadata(gameId)).ToList().AsReadOnly();
	}

	private ReadOnlyCollection<string> GetGameIds()
	{
		List<string> list = new List<string>();
		list.AddRange(storageMetadata.Keys);
		return list.AsReadOnly();
	}

	private void SaveGameMetadata(string gameId, GameMetadata metadata)
	{
		if (string.IsNullOrEmpty(gameId))
		{
			throw new ArgumentException("gameId can't be null or empty");
		}
		if (metadata != null)
		{
			GameMetadata gameMetadata = metadata.Clone() as GameMetadata;
			gameMetadata.GameId = gameId;
			storageMetadata[gameId] = gameMetadata;
		}
	}

	public void SavePlayerMetadata(string gameId, PlayerMetadata playerMetadata)
	{
		if (string.IsNullOrEmpty(gameId))
		{
			throw new ArgumentException("gameId can't be null or empty");
		}
		storageMetadata[gameId].PlayerMetadata.Find((PlayerMetadata pm) => pm.PlayerNetworkId.Equals(playerMetadata.PlayerNetworkId)).LastSeenEventRevision = playerMetadata.LastSeenEventRevision;
	}

	public GameMetadata LoadGameMetadata(string gameId)
	{
		if (string.IsNullOrEmpty(gameId))
		{
			throw new ArgumentException("gameId can't be null or empty");
		}
		if (!storageMetadata.ContainsKey(gameId))
		{
			return null;
		}
		return storageMetadata[gameId];
	}

	public void DeleteGameMetadata(string gameId)
	{
		if (string.IsNullOrEmpty(gameId))
		{
			throw new ArgumentException("gameId can't be null or empty");
		}
		storageMetadata.Remove(gameId);
	}

	public void JoinGame(string gameId, string password, PlayerMetadata playerMetadata)
	{
		if (string.IsNullOrEmpty(gameId))
		{
			throw new ArgumentException("gameId can't be null or empty");
		}
		storageMetadata[gameId].JoinGame(playerMetadata);
	}

	public void LeaveGame(string gameId, string playerId, bool isThisAnAbandonRequest)
	{
		if (string.IsNullOrEmpty(gameId))
		{
			throw new ArgumentException("gameId can't be null or empty");
		}
		storageMetadata[gameId].LeaveGame(playerId);
	}

	public void DeleteAll()
	{
		events.Clear();
		storageMetadata.Clear();
	}
}
