using System;
using System.Linq;
using System.Text;

namespace RestSharp;

public class HttpBasicAuthenticator : IAuthenticator
{
	private readonly string _username;

	private readonly string _password;

	public HttpBasicAuthenticator(string username, string password)
	{
		_password = password;
		_username = username;
	}

	public void Authenticate(IRestClient client, IRestRequest request)
	{
		if (!request.Parameters.Any((Parameter p) => p.Name.Equals("Authorization", StringComparison.OrdinalIgnoreCase)))
		{
			string arg = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_username}:{_password}"));
			string value = $"Basic {arg}";
			request.AddParameter("Authorization", value, ParameterType.HttpHeader);
		}
	}
}
