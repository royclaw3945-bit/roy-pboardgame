using BrassApplication.Network;

namespace Frameworks.Network;

public static class NetworkFactory
{
	public static INetworkCredentialsStorage CreateNetworkCredentialsStorage()
	{
		return new NetworkCredentialsStorage();
	}
}
