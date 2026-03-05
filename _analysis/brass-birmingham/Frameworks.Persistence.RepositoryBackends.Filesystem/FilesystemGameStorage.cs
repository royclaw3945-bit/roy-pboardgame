using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using BoardGameRules.Entities.EventSourcing.Events;
using BrassApplication.Game;
using BrassApplication.Persistence.Repository;
using BrassApplication.Persistence.RepositoryBackends;
using BrassApplication.Persistence.Serialization;

namespace Frameworks.Persistence.RepositoryBackends.Filesystem;

public class FilesystemGameStorage : IGameStorage
{
	private const string MetadataFileName = "metadata.storage";

	private const string EventsFileName = "events.storage";

	private string RootPath { get; set; }

	private ISerialize Serializer { get; set; }

	public FilesystemGameStorage(string rootPath, ISerialize serializer)
	{
		RootPath = rootPath;
		Serializer = serializer;
	}

	public ReadOnlyCollection<IEvent> LoadEvents(string gameId, int minRevision, int maxRevision)
	{
		if (string.IsNullOrEmpty(gameId))
		{
			throw new ArgumentException("gameId can't be null or empty");
		}
		if (minRevision > maxRevision)
		{
			throw new ArgumentException("MinRevision can't be greater than MaxRevision");
		}
		List<IEvent> list = ReadEventsFromFile(gameId);
		if (minRevision > list.Count)
		{
			return new List<IEvent>().AsReadOnly();
		}
		int val = list.Count - minRevision;
		int count = Math.Min(maxRevision - minRevision, val);
		return list.GetRange(minRevision, count).AsReadOnly();
	}

	public void SaveGame(string gameId, IList<IEvent> eventsToAppend, GameMetadata metadata)
	{
		SaveEvents(gameId, eventsToAppend);
		SaveGameMetadata(gameId, metadata);
	}

	private void SaveEvents(string gameId, IList<IEvent> eventsToAppend)
	{
		if (string.IsNullOrEmpty(gameId))
		{
			throw new ArgumentException("gameId can't be null or empty");
		}
		string path = BuildEventsPath(gameId);
		string directoryName = Path.GetDirectoryName(path);
		if (!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		List<IEvent> list = ReadEventsFromFile(gameId);
		list.AddRange(eventsToAppend);
		using FileStream output = new FileStream(path, FileMode.Create);
		Serializer.Serialize(output, list);
	}

	public void DeleteEvents(string gameId)
	{
		if (string.IsNullOrEmpty(gameId))
		{
			throw new ArgumentException("gameId can't be null or empty");
		}
		string text = BuildEventsPath(gameId);
		if (File.Exists(text))
		{
			FileInfo fileInfo = new FileInfo(text);
			fileInfo.Delete();
			fileInfo.Refresh();
			while (fileInfo.Exists)
			{
				Thread.Sleep(100);
				fileInfo.Refresh();
			}
		}
	}

	private List<IEvent> ReadEventsFromFile(string gameId)
	{
		List<IEvent> result = new List<IEvent>();
		string path = BuildEventsPath(gameId);
		if (!File.Exists(path))
		{
			return result;
		}
		using FileStream input = new FileStream(path, FileMode.Open);
		return Serializer.Deserialize<List<IEvent>>(input);
	}

	private string BuildEventsPath(string gameId)
	{
		return Path.Combine(Path.Combine(RootPath, gameId), "events.storage");
	}

	private string BuildOldEventsPath()
	{
		return Path.Combine(RootPath, "events.storage");
	}

	public string CreateGame(GameMetadata gameMetadata)
	{
		string text = (gameMetadata.GameId = Guid.NewGuid().ToString());
		SaveGameMetadata(text, gameMetadata);
		return text;
	}

	public ReadOnlyCollection<GameMetadata> GetGamesMetadata(IGamesFilter gamesFilter)
	{
		return (from gameId in GetGameIds()
			select LoadGameMetadata(gameId)).ToList().AsReadOnly();
	}

	private void SaveGameMetadata(string gameId, GameMetadata metadata)
	{
		if (string.IsNullOrEmpty(gameId))
		{
			throw new ArgumentException("gameId can't be null or empty");
		}
		if (metadata == null)
		{
			return;
		}
		GameMetadata gameMetadata = metadata.Clone() as GameMetadata;
		gameMetadata.GameId = gameId;
		string path = BuildMetadataPath(gameId);
		string directoryName = Path.GetDirectoryName(path);
		if (!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		using FileStream output = new FileStream(path, FileMode.Create);
		Serializer.Serialize(output, gameMetadata);
	}

	public void SavePlayerMetadata(string gameId, PlayerMetadata playerMetadata)
	{
		GameMetadata gameMetadata = LoadGameMetadata(gameId);
		gameMetadata.PlayerMetadata.Find((PlayerMetadata pm) => pm.PlayerNetworkId.Equals(playerMetadata.PlayerNetworkId)).LastSeenEventRevision = playerMetadata.LastSeenEventRevision;
		SaveGameMetadata(gameId, gameMetadata);
	}

	public GameMetadata LoadGameMetadata(string gameId)
	{
		if (string.IsNullOrEmpty(gameId))
		{
			throw new ArgumentException("gameId can't be null or empty");
		}
		string text = BuildMetadataPath(gameId);
		if (!File.Exists(text) || new FileInfo(text).Length == 0L)
		{
			DeleteGameMetadata(gameId);
			return null;
		}
		GameMetadata gameMetadata = null;
		using FileStream input = new FileStream(text, FileMode.Open);
		return Serializer.Deserialize<GameMetadata>(input);
	}

	public void DeleteGameMetadata(string gameId)
	{
		if (string.IsNullOrEmpty(gameId))
		{
			throw new ArgumentException("gameId can't be null or empty");
		}
		string directoryName = Path.GetDirectoryName(BuildMetadataPath(gameId));
		if (Directory.Exists(directoryName))
		{
			Directory.Delete(directoryName, recursive: true);
		}
	}

	public void JoinGame(string gameId, string password, PlayerMetadata playerMetadata)
	{
		GameMetadata gameMetadata = LoadGameMetadata(gameId);
		gameMetadata.JoinGame(playerMetadata);
		SaveGameMetadata(gameId, gameMetadata);
	}

	public void LeaveGame(string gameId, string playerId, bool isThisAnAbandonRequest)
	{
		GameMetadata gameMetadata = LoadGameMetadata(gameId);
		gameMetadata.LeaveGame(playerId);
		SaveGameMetadata(gameId, gameMetadata);
	}

	private IList<string> GetGameIds()
	{
		List<string> list = new List<string>();
		if (Directory.Exists(RootPath))
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(RootPath);
			list.AddRange(from d in directoryInfo.GetDirectories("*", SearchOption.TopDirectoryOnly)
				select d.Name);
			list.Sort();
		}
		return list;
	}

	private string BuildMetadataPath(string gameId)
	{
		return Path.Combine(Path.Combine(RootPath, gameId), "metadata.storage");
	}

	public void DeleteAll()
	{
		if (Directory.Exists(RootPath))
		{
			Directory.Delete(RootPath, recursive: true);
		}
	}
}
