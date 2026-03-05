using BrassApplication.Persistence.Serialization;
using Frameworks.Persistence.RepositoryBackends.Filesystem;
using Frameworks.Persistence.RepositoryBackends.InMemory;

namespace Frameworks.Persistence.RepositoryBackends.Cached;

public class CachedFilesystemSnapshotStorage : CachedSnapshotStorage
{
	public CachedFilesystemSnapshotStorage(string rootPath, ISerialize serializer)
		: base(new InMemorySnapshotStorage(InMemorySnapshotStorage.StorageStrategy.KeepOneRecentlySaved), new FilesystemSnapshotStorage(rootPath, serializer))
	{
	}
}
