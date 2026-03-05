using BoardGameRules.Entities.EventSourcing.Snapshot;

namespace BrassApplication.Persistence.RepositoryBackends;

public interface ISnapshotStorage
{
	ISnapshot LoadSnapshot(string gameId, int maxRevisionNumber = int.MaxValue);

	void SaveSnapshot(ISnapshot snapshot);

	void DeleteSnapshot(ISnapshot snapshot);

	void DeleteSnapshot(string gameId, int revision);

	void DeleteSnapshots(string gameId);

	void MigrateOldSnapshotsToNewDirectory(string gameId);
}
