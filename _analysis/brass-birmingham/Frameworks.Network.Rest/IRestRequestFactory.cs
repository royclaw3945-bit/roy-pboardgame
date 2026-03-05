using RestSharp;

namespace Frameworks.Network.Rest;

public interface IRestRequestFactory
{
	string AuthToken { get; set; }

	IRestRequest CreateRestRequest(string path, Method method);

	IRestRequest CreateRestRequest(string path, Method method, string data);
}
