using BrassApplication.Persistence.RepositoryBackends;

namespace BrassApplication.Network;

public interface INetworkBackendClient
{
	INetworkBackendUserManagement UserManagement { get; }

	IGameStorage GameStorage { get; }

	string AuthToken { get; set; }
}
