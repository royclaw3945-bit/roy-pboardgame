using BoardGameRules.Utils.Logging;
using BrassApplication.Persistence.Repository;
using Frameworks.Persistence.GameFactory;
using Utils;
using Utils.Wireup;

namespace Frameworks.Persistence.Repository;

public class RepositoryFactory : IRepositoryFactory
{
	private readonly ILog Logger = LogFactory.BuildLogger(typeof(RepositoryFactory));

	public IRepository CreateLocalGameRepository()
	{
		Logger.Debug("RepositoryFactory.CreateLocalGameRepository(): storage path = " + RepositoryStorageLocation.LocalGameStoragePath);
		return CreateOfflineGameRepository(RepositoryStorageLocation.LocalGameStoragePath);
	}

	public IRepository CreateTutorialGameRepository()
	{
		Logger.Debug("RepositoryFactory.CreateTutorialGameRepository(): storage path = " + RepositoryStorageLocation.LocalGameStoragePath);
		return CreateOfflineGameRepository(RepositoryStorageLocation.TutorialGameStoragePath);
	}

	public IRepository CreateTestGamesRepository()
	{
		Logger.Debug("RepositoryFactory.CreateTestGamesRepository(): storage path = " + RepositoryStorageLocation.LocalGameStoragePath);
		return CreateOfflineGameRepository(RepositoryStorageLocation.TestGameStoragePath);
	}

	public IRepository CreateInMemoryGameRepository()
	{
		Logger.Debug("RepositoryFactory.CreateInMemoryGameRepository()");
		return Wireup.Init().UsingInMemoryGameStoragePersistence().UsingJsonSerialization()
			.UsingInMemorySnapshotStoragePersistence()
			.UsingJsonSerialization()
			.With((IGameFactory)new BrassBirminghamFactory())
			.BuildRepository();
	}

	private IRepository CreateOfflineGameRepository(string storagePath)
	{
		return Wireup.Init().UsingCachedFilesystemGameStoragePersistence(storagePath).UsingJsonSerialization()
			.UsingCachedFilesystemSnapshotStoragePersistence(storagePath)
			.UsingJsonSerialization()
			.With((IGameFactory)new BrassBirminghamFactory())
			.BuildRepository();
	}

	public IRepository CreateOnlineGameRepository()
	{
		return Wireup.Init().With(Global.Instance.NetworkBackendClient.GameStorage).UsingInMemorySnapshotStoragePersistence()
			.With((IGameFactory)new BrassBirminghamFactory())
			.BuildRepository();
	}
}
