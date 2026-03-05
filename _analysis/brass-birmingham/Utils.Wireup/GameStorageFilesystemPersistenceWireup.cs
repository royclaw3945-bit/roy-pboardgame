using System;
using BoardGameRules.Utils.Logging;
using BrassApplication.Persistence.RepositoryBackends;
using BrassApplication.Persistence.Serialization;
using Frameworks.Persistence.RepositoryBackends.Cached;
using Frameworks.Persistence.RepositoryBackends.Filesystem;

namespace Utils.Wireup;

public class GameStorageFilesystemPersistenceWireup : GameStoragePersistenceWireup
{
	private static readonly ILog Logger = LogFactory.BuildLogger(typeof(GameStorageFilesystemPersistenceWireup));

	public GameStorageFilesystemPersistenceWireup(Wireup inner)
		: base(inner)
	{
		Logger.Verbose("Registering in memory event storage");
	}

	public GameStorageFilesystemPersistenceWireup(Wireup inner, string rootPath, bool cached)
		: base(inner)
	{
		GameStorageFilesystemPersistenceWireup gameStorageFilesystemPersistenceWireup = this;
		if (cached)
		{
			Logger.Verbose("Registering cached filesystem event storage.");
			Container.Register((Func<NanoContainer, IGameStorage>)((NanoContainer c) => new CachedFilesystemGameStorage(rootPath, gameStorageFilesystemPersistenceWireup.Container.ResolveNamed<ISerialize>("GameStorage"))));
		}
		else
		{
			Logger.Verbose("Registering filesystem event storage.");
			Container.Register((Func<NanoContainer, IGameStorage>)((NanoContainer c) => new FilesystemGameStorage(rootPath, gameStorageFilesystemPersistenceWireup.Container.ResolveNamed<ISerialize>("GameStorage"))));
		}
	}
}
