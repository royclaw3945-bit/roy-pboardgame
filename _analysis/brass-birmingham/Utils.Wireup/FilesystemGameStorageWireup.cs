using System;
using BoardGameRules.Utils.Logging;
using BrassApplication.Persistence.RepositoryBackends;
using BrassApplication.Persistence.Serialization;
using Frameworks.Persistence.RepositoryBackends.Filesystem;

namespace Utils.Wireup;

public class FilesystemGameStorageWireup : Wireup
{
	private static readonly ILog Logger = LogFactory.BuildLogger(typeof(FilesystemGameStorageWireup));

	public FilesystemGameStorageWireup(Wireup inner, string rootPath)
		: base(inner)
	{
		FilesystemGameStorageWireup filesystemGameStorageWireup = this;
		Logger.Verbose("Registering filesystem event storage.");
		Container.Register((Func<NanoContainer, IGameStorage>)((NanoContainer c) => new FilesystemGameStorage(rootPath, filesystemGameStorageWireup.Container.Resolve<ISerialize>())));
	}
}
