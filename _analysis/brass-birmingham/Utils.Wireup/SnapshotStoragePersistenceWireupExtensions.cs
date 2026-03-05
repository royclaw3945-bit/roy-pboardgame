using BrassApplication.Persistence.RepositoryBackends;
using Frameworks.Persistence.RepositoryBackends.InMemory;

namespace Utils.Wireup;

public static class SnapshotStoragePersistenceWireupExtensions
{
	public static SnapshotStoragePersistenceWireup UsingInMemorySnapshotStoragePersistence(this Wireup wireup)
	{
		wireup.With((ISnapshotStorage)new InMemorySnapshotStorage());
		return new SnapshotStoragePersistenceWireup(wireup);
	}

	public static SnapshotStoragePersistenceWireup UsingFilesystemSnapshotStoragePersistence(this Wireup wireup, string rootPath)
	{
		return new SnapshotStoragePersistenceWireup(wireup, rootPath, cached: false);
	}

	public static SnapshotStoragePersistenceWireup UsingCachedFilesystemSnapshotStoragePersistence(this Wireup wireup, string rootPath)
	{
		return new SnapshotStoragePersistenceWireup(wireup, rootPath, cached: true);
	}
}
