using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace RestSharp;

public interface IHttp
{
	Action<Stream> ResponseWriter { get; set; }

	CookieContainer CookieContainer { get; set; }

	ICredentials Credentials { get; set; }

	bool AlwaysMultipartFormData { get; set; }

	string UserAgent { get; set; }

	int Timeout { get; set; }

	int ReadWriteTimeout { get; set; }

	bool FollowRedirects { get; set; }

	X509CertificateCollection ClientCertificates { get; set; }

	int? MaxRedirects { get; set; }

	bool UseDefaultCredentials { get; set; }

	IList<HttpHeader> Headers { get; }

	IList<HttpParameter> Parameters { get; }

	IList<HttpFile> Files { get; }

	IList<HttpCookie> Cookies { get; }

	string RequestBody { get; set; }

	string RequestContentType { get; set; }

	bool PreAuthenticate { get; set; }

	byte[] RequestBodyBytes { get; set; }

	Uri Url { get; set; }

	IWebProxy Proxy { get; set; }

	HttpResponse Delete();

	HttpResponse Get();

	HttpResponse Head();

	HttpResponse Options();

	HttpResponse Post();

	HttpResponse Put();

	HttpResponse Patch();

	HttpResponse Merge();

	HttpResponse AsPost(string httpMethod);

	HttpResponse AsGet(string httpMethod);
}
