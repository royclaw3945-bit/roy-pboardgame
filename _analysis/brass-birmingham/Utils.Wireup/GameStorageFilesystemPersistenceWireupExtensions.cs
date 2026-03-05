using BrassApplication.Persistence.RepositoryBackends;
using Frameworks.Persistence.RepositoryBackends.InMemory;

namespace Utils.Wireup;

public static class GameStorageFilesystemPersistenceWireupExtensions
{
	public static GameStorageFilesystemPersistenceWireup UsingInMemoryGameStoragePersistence(this Wireup wireup)
	{
		wireup.With((IGameStorage)new InMemoryGameStorage());
		return new GameStorageFilesystemPersistenceWireup(wireup);
	}

	public static GameStorageFilesystemPersistenceWireup UsingFilesystemGameStoragePersistence(this Wireup wireup, string rootPath)
	{
		return new GameStorageFilesystemPersistenceWireup(wireup, rootPath, cached: false);
	}

	public static GameStorageFilesystemPersistenceWireup UsingCachedFilesystemGameStoragePersistence(this Wireup wireup, string rootPath)
	{
		return new GameStorageFilesystemPersistenceWireup(wireup, rootPath, cached: true);
	}
}
