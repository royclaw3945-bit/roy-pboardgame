using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using RestSharp;

namespace Frameworks.Network.Rest;

public class RestClientFactory : IRestClientFactory
{
	private readonly string _backendUrl;

	private readonly int _timeoutInSeconds;

	public RestClientFactory(string backendUrl, int timeoutInSeconds)
	{
		_backendUrl = backendUrl;
		_timeoutInSeconds = timeoutInSeconds;
	}

	public IRestClient CreateRestClient()
	{
		ServicePointManager.ServerCertificateValidationCallback = (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true;
		return new RestClient(_backendUrl)
		{
			Timeout = 1000 * _timeoutInSeconds
		};
	}
}
