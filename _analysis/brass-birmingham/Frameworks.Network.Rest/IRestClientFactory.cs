using RestSharp;

namespace Frameworks.Network.Rest;

public interface IRestClientFactory
{
	IRestClient CreateRestClient();
}
