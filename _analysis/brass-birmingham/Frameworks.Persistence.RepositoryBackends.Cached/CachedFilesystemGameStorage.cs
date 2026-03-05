using BrassApplication.Persistence.Serialization;
using Frameworks.Persistence.RepositoryBackends.Filesystem;
using Frameworks.Persistence.RepositoryBackends.InMemory;

namespace Frameworks.Persistence.RepositoryBackends.Cached;

public class CachedFilesystemGameStorage : CachedGameStorage
{
	public CachedFilesystemGameStorage(string rootPath, ISerialize serializer)
		: base(new InMemoryGameStorage(), new FilesystemGameStorage(rootPath, serializer))
	{
	}
}
