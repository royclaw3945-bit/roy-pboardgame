using Frameworks.Persistence.RepositoryBackends.Cached;
using Frameworks.Persistence.RepositoryBackends.InMemory;
using Frameworks.Persistence.RepositoryBackends.Network;

namespace Frameworks.Persistence.RepositoryBackends.GuaranteedNetworkDelivery;

public class GuaranteedDeliveryNetworkGameStorage : CachedGameStorage
{
	public GuaranteedDeliveryNetworkGameStorage(IGuaranteedDeliveryJobQueue jobQueue, NetworkGameStorage networkGameStorage)
		: base(new InMemoryGameStorage(), new GuaranteedDeliveryNetworkGameStorageInternal(jobQueue, networkGameStorage))
	{
	}
}
