using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.EventSourcing.Snapshot;
using BoardGameRules.Utils.Logging;
using BrassApplication.Persistence.RepositoryBackends;

namespace Frameworks.Persistence.RepositoryBackends.InMemory;

public class InMemorySnapshotStorage : ISnapshotStorage
{
	public enum StorageStrategy
	{
		KeepAll,
		KeepOneRecentlySaved,
		KeepOneGratestRevision
	}

	private readonly StorageStrategy _storageStrategy;

	private readonly Dictionary<KeyValuePair<string, int>, ISnapshot> storage = new Dictionary<KeyValuePair<string, int>, ISnapshot>();

	private static readonly ILog Logger = LogFactory.BuildLogger(typeof(InMemorySnapshotStorage));

	public InMemorySnapshotStorage()
		: this(StorageStrategy.KeepAll)
	{
	}

	public InMemorySnapshotStorage(StorageStrategy storageStrategy)
	{
		_storageStrategy = storageStrategy;
	}

	public ISnapshot LoadSnapshot(string gameId, int maxRevisionNumber = int.MaxValue)
	{
		if (string.IsNullOrEmpty(gameId))
		{
			throw new ArgumentException("gameId can't be null or empty");
		}
		KeyValuePair<string, int> key = new KeyValuePair<string, int>("", -1);
		int num = 0;
		foreach (KeyValuePair<KeyValuePair<string, int>, ISnapshot> item in storage)
		{
			string key2 = item.Key.Key;
			int value = item.Key.Value;
			if (key2.Equals(gameId) && value <= maxRevisionNumber && value > num)
			{
				key = item.Key;
				num = value;
				Logger.Info("Found snapshot for {0}, revision {1}", key.Key, key.Value);
			}
		}
		if (key.Key.Equals("") && key.Value == -1)
		{
			Logger.Info("There is no in-memory snapshot for given gameId " + gameId + " and revision " + maxRevisionNumber + ".");
			return null;
		}
		return storage[key].Clone() as ISnapshot;
	}

	public void SaveSnapshot(ISnapshot snapshot)
	{
		if (snapshot == null)
		{
			throw new ArgumentException("Snapshot is null");
		}
		if (snapshot.Metadata == null)
		{
			throw new ArgumentException("Snapshot.Metadata is null");
		}
		string gameId = snapshot.Metadata.GameId;
		KeyValuePair<string, int> key = new KeyValuePair<string, int>(gameId, snapshot.Metadata.Revision);
		if (storage.ContainsKey(key))
		{
			throw new ArgumentException("There is already snapshot for given gameId and revision");
		}
		Logger.Info("Saving snapshot for {0}, revision {1}", key.Key, key.Value);
		StoreWithStrategy(key, snapshot.Clone() as ISnapshot);
	}

	private void StoreWithStrategy(KeyValuePair<string, int> key, ISnapshot snapshot)
	{
		switch (_storageStrategy)
		{
		case StorageStrategy.KeepAll:
			storage[key] = snapshot;
			break;
		case StorageStrategy.KeepOneRecentlySaved:
			storage.Clear();
			storage[key] = snapshot;
			break;
		case StorageStrategy.KeepOneGratestRevision:
		{
			if (storage.Count == 0)
			{
				storage[key] = snapshot;
				break;
			}
			KeyValuePair<string, int> keyValuePair = storage.Keys.ElementAt(0);
			if (keyValuePair.Key.Equals(snapshot.Metadata.GameId) && keyValuePair.Value < snapshot.Metadata.Revision)
			{
				storage.Clear();
				storage[key] = snapshot;
			}
			break;
		}
		default:
			Logger.Error("Wrong storage strategy. Not saving.");
			break;
		}
	}

	public void DeleteSnapshot(ISnapshot snapshot)
	{
		List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>();
		foreach (KeyValuePair<KeyValuePair<string, int>, ISnapshot> item in storage)
		{
			if (item.Value.Equals(snapshot))
			{
				list.Add(item.Key);
			}
		}
		if (list.Count == 0)
		{
			throw new ArgumentException("There is no such snapshot in the store");
		}
		foreach (KeyValuePair<string, int> item2 in list)
		{
			storage.Remove(item2);
		}
	}

	public void DeleteSnapshot(string gameId, int revision)
	{
		if (string.IsNullOrEmpty(gameId))
		{
			throw new ArgumentException("gameId can't be null or empty");
		}
		KeyValuePair<string, int> key = new KeyValuePair<string, int>(gameId, revision);
		if (!storage.ContainsKey(key))
		{
			throw new ArgumentException("There is no snapshot for given gameId and revision");
		}
		storage.Remove(key);
	}

	public void DeleteSnapshots(string gameId)
	{
		if (string.IsNullOrEmpty(gameId))
		{
			throw new ArgumentException("gameId can't be null or empty");
		}
		List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>();
		foreach (KeyValuePair<KeyValuePair<string, int>, ISnapshot> item in storage)
		{
			if (item.Key.Key.Equals(gameId))
			{
				list.Add(item.Key);
			}
		}
		if (list.Count == 0)
		{
			throw new ArgumentException("There are no snapshots for given gameId in the store");
		}
		foreach (KeyValuePair<string, int> item2 in list)
		{
			storage.Remove(item2);
		}
	}

	public void MigrateOldSnapshotsToNewDirectory(string gameId)
	{
	}
}
