using BoardGameRules.Utils.Logging;
using BrassApplication.Network;
using BrassApplication.Persistence.RepositoryBackends;
using BrassApplication.Persistence.Serialization;
using Frameworks.Network.Rest;
using Frameworks.Network.UserManagement;
using Frameworks.Persistence.RepositoryBackends.Cached;
using Frameworks.Persistence.RepositoryBackends.GuaranteedNetworkDelivery;
using Frameworks.Persistence.RepositoryBackends.Network;
using InterfaceAdapters.Utils.MainThreadDispatcher;

namespace Frameworks.Network;

public class NetworkBackendClient : INetworkBackendClient
{
	private static readonly ILog Logger = LogFactory.BuildLogger(typeof(NetworkBackendClient));

	private readonly IRestRequestFactory _restRequestFactory;

	public string AuthToken
	{
		get
		{
			return _restRequestFactory.AuthToken;
		}
		set
		{
			_restRequestFactory.AuthToken = value;
		}
	}

	public INetworkBackendUserManagement UserManagement { get; private set; }

	public IGameStorage GameStorage { get; private set; }

	public NetworkBackendClient(IRestClientFactory restClientFactory, IRestRequestFactory restRequestFactory, ISerialize serializer, IMainThreadDispatcher dispatcher, IGuaranteedDeliveryJobQueue jobQueue)
	{
		UserManagement = new NetworkBackendUserManagement(restClientFactory, restRequestFactory, serializer, dispatcher);
		CachedNetworkGameStorage gameStorage = new CachedNetworkGameStorage(new NetworkGameStorage(restClientFactory, restRequestFactory, serializer));
		GameStorage = gameStorage;
		_restRequestFactory = restRequestFactory;
		Logger.Debug("NetworkBackendClient created");
	}
}
