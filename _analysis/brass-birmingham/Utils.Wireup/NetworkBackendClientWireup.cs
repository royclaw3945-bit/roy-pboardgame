using BrassApplication.Network;
using BrassApplication.Persistence.Serialization;
using Frameworks.Network;
using Frameworks.Network.Rest;
using Frameworks.Persistence.RepositoryBackends.GuaranteedNetworkDelivery;
using InterfaceAdapters.Utils.MainThreadDispatcher;

namespace Utils.Wireup;

public class NetworkBackendClientWireup : Wireup
{
	public NetworkBackendClientWireup(Wireup inner)
		: base(inner)
	{
		Container.Register(BuildNetworkBackendClient);
	}

	public virtual INetworkBackendClient BuildNetworkBackendClient()
	{
		return Container.Resolve<INetworkBackendClient>();
	}

	private static INetworkBackendClient BuildNetworkBackendClient(NanoContainer context)
	{
		IRestClientFactory restClientFactory = context.Resolve<IRestClientFactory>();
		IRestRequestFactory restRequestFactory = context.Resolve<IRestRequestFactory>();
		ISerialize serializer = context.Resolve<ISerialize>();
		IMainThreadDispatcher dispatcher = context.Resolve<IMainThreadDispatcher>();
		IGuaranteedDeliveryJobQueue jobQueue = context.Resolve<IGuaranteedDeliveryJobQueue>();
		return new NetworkBackendClient(restClientFactory, restRequestFactory, serializer, dispatcher, jobQueue);
	}
}
