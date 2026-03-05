using System;
using BoardGameRules.Entities.EventSourcing.Snapshot;
using BrassApplication.Persistence.RepositoryBackends;

namespace Frameworks.Persistence.RepositoryBackends.Cached;

public class CachedSnapshotStorage : ISnapshotStorage
{
	protected ISnapshotStorage _cachedSnapshotStorage;

	protected ISnapshotStorage _persistentSnapshotStorage;

	public CachedSnapshotStorage(ISnapshotStorage cachedSnapshotStorage, ISnapshotStorage persistentSnapshotStorage)
	{
		_cachedSnapshotStorage = cachedSnapshotStorage;
		_persistentSnapshotStorage = persistentSnapshotStorage;
	}

	public ISnapshot LoadSnapshot(string gameId, int maxRevisionNumber = int.MaxValue)
	{
		try
		{
			ISnapshot snapshot = _cachedSnapshotStorage.LoadSnapshot(gameId, maxRevisionNumber);
			if (snapshot != null)
			{
				return snapshot;
			}
		}
		catch (ArgumentException)
		{
		}
		return _persistentSnapshotStorage.LoadSnapshot(gameId, maxRevisionNumber);
	}

	public void SaveSnapshot(ISnapshot snapshot)
	{
		try
		{
			_cachedSnapshotStorage.SaveSnapshot(snapshot);
		}
		catch (ArgumentException)
		{
		}
		_persistentSnapshotStorage.SaveSnapshot(snapshot);
	}

	public void DeleteSnapshot(ISnapshot snapshot)
	{
		try
		{
			_cachedSnapshotStorage.DeleteSnapshot(snapshot);
		}
		catch (ArgumentException)
		{
		}
		_persistentSnapshotStorage.DeleteSnapshot(snapshot);
	}

	public void DeleteSnapshot(string gameId, int revision)
	{
		try
		{
			_cachedSnapshotStorage.DeleteSnapshot(gameId, revision);
		}
		catch (ArgumentException)
		{
		}
		_persistentSnapshotStorage.DeleteSnapshot(gameId, revision);
	}

	public void DeleteSnapshots(string gameId)
	{
		try
		{
			_cachedSnapshotStorage.DeleteSnapshots(gameId);
		}
		catch (ArgumentException)
		{
		}
		_persistentSnapshotStorage.DeleteSnapshots(gameId);
	}

	public void MigrateOldSnapshotsToNewDirectory(string gameId)
	{
		try
		{
			_cachedSnapshotStorage.MigrateOldSnapshotsToNewDirectory(gameId);
		}
		catch (ArgumentException)
		{
		}
		_persistentSnapshotStorage.MigrateOldSnapshotsToNewDirectory(gameId);
	}
}
