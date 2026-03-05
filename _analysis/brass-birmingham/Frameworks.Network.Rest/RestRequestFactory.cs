using RestSharp;

namespace Frameworks.Network.Rest;

public class RestRequestFactory : IRestRequestFactory
{
	public string AuthToken { get; set; }

	public IRestRequest CreateRestRequest(string path, Method method)
	{
		return CreateRestRequest(path, method, null);
	}

	public IRestRequest CreateRestRequest(string path, Method method, string data)
	{
		RestRequest restRequest = new RestRequest(path, method);
		restRequest.AddHeader("Accept", "application/json");
		if (data != null)
		{
			restRequest.AddHeader("Content-Type", "application/json");
			restRequest.AddParameter("application/json", data, ParameterType.RequestBody);
		}
		if (!string.IsNullOrEmpty(AuthToken))
		{
			restRequest.AddHeader("Authorization", "Bearer " + AuthToken);
		}
		return restRequest;
	}
}
