using Frameworks.Persistence.RepositoryBackends.InMemory;
using Frameworks.Persistence.RepositoryBackends.Network;

namespace Frameworks.Persistence.RepositoryBackends.Cached;

public class CachedNetworkGameStorage : CachedGameStorage
{
	public CachedNetworkGameStorage(NetworkGameStorage networkGameStorage)
		: base(new InMemoryGameStorage(), networkGameStorage)
	{
	}
}
