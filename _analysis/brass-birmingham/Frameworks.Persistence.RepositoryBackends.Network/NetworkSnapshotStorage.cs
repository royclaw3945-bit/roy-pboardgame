using System;
using BoardGameRules.Entities.EventSourcing.Snapshot;
using BrassApplication.Persistence.RepositoryBackends;

namespace Frameworks.Persistence.RepositoryBackends.Network;

public class NetworkSnapshotStorage : ISnapshotStorage
{
	public ISnapshot LoadSnapshot(string gameId, int maxRevisionNumber = int.MaxValue)
	{
		throw new NotImplementedException();
	}

	public void SaveSnapshot(ISnapshot snapshot)
	{
		throw new NotImplementedException();
	}

	public void DeleteSnapshot(ISnapshot snapshot)
	{
		throw new NotImplementedException();
	}

	public void DeleteSnapshot(string gameId, int revision)
	{
		throw new NotImplementedException();
	}

	public void DeleteSnapshots(string gameId)
	{
		throw new NotImplementedException();
	}

	public void MigrateOldSnapshotsToNewDirectory(string gameId)
	{
	}
}
