using System;
using BoardGameRules.Utils.Logging;
using BrassApplication.Persistence.RepositoryBackends;
using BrassApplication.Persistence.Serialization;
using Frameworks.Persistence.RepositoryBackends.Cached;
using Frameworks.Persistence.RepositoryBackends.Filesystem;

namespace Utils.Wireup;

public class SnapshotStoragePersistenceWireup : Wireup
{
	private static readonly ILog Logger = LogFactory.BuildLogger(typeof(SnapshotStoragePersistenceWireup));

	public static readonly string Name = "SnapshotStorage";

	public SnapshotStoragePersistenceWireup(Wireup inner)
		: base(inner)
	{
		Logger.Verbose("Registering in memory snapshot storage.");
	}

	public SnapshotStoragePersistenceWireup(Wireup inner, string rootPath, bool cached)
		: base(inner)
	{
		SnapshotStoragePersistenceWireup snapshotStoragePersistenceWireup = this;
		if (cached)
		{
			Logger.Verbose("Registering cached filesystem snapshot storage.");
			Container.Register((Func<NanoContainer, ISnapshotStorage>)((NanoContainer c) => new CachedFilesystemSnapshotStorage(rootPath, snapshotStoragePersistenceWireup.Container.ResolveNamed<ISerialize>(Name))));
		}
		else
		{
			Logger.Verbose("Registering filesystem snapshot storage.");
			Container.Register((Func<NanoContainer, ISnapshotStorage>)((NanoContainer c) => new FilesystemSnapshotStorage(rootPath, snapshotStoragePersistenceWireup.Container.ResolveNamed<ISerialize>(Name))));
		}
	}
}
