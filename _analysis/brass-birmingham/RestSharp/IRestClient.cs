using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace RestSharp;

public interface IRestClient
{
	CookieContainer CookieContainer { get; set; }

	string UserAgent { get; set; }

	int Timeout { get; set; }

	int ReadWriteTimeout { get; set; }

	bool UseSynchronizationContext { get; set; }

	IAuthenticator Authenticator { get; set; }

	Uri BaseUrl { get; set; }

	bool PreAuthenticate { get; set; }

	IList<Parameter> DefaultParameters { get; }

	X509CertificateCollection ClientCertificates { get; set; }

	IWebProxy Proxy { get; set; }

	IRestResponse Execute(IRestRequest request);

	IRestResponse<T> Execute<T>(IRestRequest request) where T : new();

	Uri BuildUri(IRestRequest request);

	IRestResponse ExecuteAsGet(IRestRequest request, string httpMethod);

	IRestResponse ExecuteAsPost(IRestRequest request, string httpMethod);

	IRestResponse<T> ExecuteAsGet<T>(IRestRequest request, string httpMethod) where T : new();

	IRestResponse<T> ExecuteAsPost<T>(IRestRequest request, string httpMethod) where T : new();
}
