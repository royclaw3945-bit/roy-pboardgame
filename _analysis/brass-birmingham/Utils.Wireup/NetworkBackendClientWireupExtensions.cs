namespace Utils.Wireup;

public static class NetworkBackendClientWireupExtensions
{
	public static NetworkBackendClientWireup UsingNetworkBackendClientWireup(this Wireup wireup)
	{
		return new NetworkBackendClientWireup(wireup);
	}
}
