using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using RestSharp.Extensions;

namespace RestSharp;

public class Http : IHttp, IHttpFactory
{
	private const string _lineBreak = "\r\n";

	private static readonly Encoding _defaultEncoding = Encoding.UTF8;

	private const string FormBoundary = "-----------------------------28947758029299";

	private readonly IDictionary<string, Action<HttpWebRequest, string>> _restrictedHeaderActions;

	protected bool HasParameters => Parameters.Any();

	protected bool HasCookies => Cookies.Any();

	protected bool HasBody
	{
		get
		{
			if (RequestBodyBytes == null)
			{
				return !string.IsNullOrEmpty(RequestBody);
			}
			return true;
		}
	}

	protected bool HasFiles => Files.Any();

	public bool AlwaysMultipartFormData { get; set; }

	public string UserAgent { get; set; }

	public int Timeout { get; set; }

	public int ReadWriteTimeout { get; set; }

	public ICredentials Credentials { get; set; }

	public CookieContainer CookieContainer { get; set; }

	public Action<Stream> ResponseWriter { get; set; }

	public IList<HttpFile> Files { get; private set; }

	public bool FollowRedirects { get; set; }

	public X509CertificateCollection ClientCertificates { get; set; }

	public int? MaxRedirects { get; set; }

	public bool UseDefaultCredentials { get; set; }

	public IList<HttpHeader> Headers { get; private set; }

	public IList<HttpParameter> Parameters { get; private set; }

	public IList<HttpCookie> Cookies { get; private set; }

	public string RequestBody { get; set; }

	public string RequestContentType { get; set; }

	public byte[] RequestBodyBytes { get; set; }

	public Uri Url { get; set; }

	public bool PreAuthenticate { get; set; }

	public IWebProxy Proxy { get; set; }

	public IHttp Create()
	{
		return new Http();
	}

	public Http()
	{
		Headers = new List<HttpHeader>();
		Files = new List<HttpFile>();
		Parameters = new List<HttpParameter>();
		Cookies = new List<HttpCookie>();
		_restrictedHeaderActions = new Dictionary<string, Action<HttpWebRequest, string>>(StringComparer.OrdinalIgnoreCase);
		AddSharedHeaderActions();
		AddSyncHeaderActions();
	}

	private void AddSyncHeaderActions()
	{
		_restrictedHeaderActions.Add("Connection", delegate(HttpWebRequest r, string v)
		{
			r.Connection = v;
		});
		_restrictedHeaderActions.Add("Content-Length", delegate(HttpWebRequest r, string v)
		{
			r.ContentLength = Convert.ToInt64(v);
		});
		_restrictedHeaderActions.Add("Expect", delegate(HttpWebRequest r, string v)
		{
			r.Expect = v;
		});
		_restrictedHeaderActions.Add("If-Modified-Since", delegate(HttpWebRequest r, string v)
		{
			r.IfModifiedSince = Convert.ToDateTime(v);
		});
		_restrictedHeaderActions.Add("Referer", delegate(HttpWebRequest r, string v)
		{
			r.Referer = v;
		});
		_restrictedHeaderActions.Add("Transfer-Encoding", delegate(HttpWebRequest r, string v)
		{
			r.TransferEncoding = v;
			r.SendChunked = true;
		});
		_restrictedHeaderActions.Add("User-Agent", delegate(HttpWebRequest r, string v)
		{
			r.UserAgent = v;
		});
	}

	private void AddSharedHeaderActions()
	{
		_restrictedHeaderActions.Add("Accept", delegate(HttpWebRequest r, string v)
		{
			r.Accept = v;
		});
		_restrictedHeaderActions.Add("Content-Type", delegate(HttpWebRequest r, string v)
		{
			r.ContentType = v;
		});
		_restrictedHeaderActions.Add("Date", delegate
		{
		});
		_restrictedHeaderActions.Add("Host", delegate
		{
		});
		_restrictedHeaderActions.Add("Range", delegate(HttpWebRequest r, string v)
		{
			AddRange(r, v);
		});
	}

	private static string GetMultipartFormContentType()
	{
		return string.Format("multipart/form-data; boundary={0}", "-----------------------------28947758029299");
	}

	private static string GetMultipartFileHeader(HttpFile file)
	{
		return string.Format("--{0}{4}Content-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"{4}Content-Type: {3}{4}{4}", "-----------------------------28947758029299", file.Name, file.FileName, file.ContentType ?? "application/octet-stream", "\r\n");
	}

	private string GetMultipartFormData(HttpParameter param)
	{
		return string.Format((param.Name == RequestContentType) ? "--{0}{3}Content-Type: {1}{3}Content-Disposition: form-data; name=\"{1}\"{3}{3}{2}{3}" : "--{0}{3}Content-Disposition: form-data; name=\"{1}\"{3}{3}{2}{3}", "-----------------------------28947758029299", param.Name, param.Value, "\r\n");
	}

	private static string GetMultipartFooter()
	{
		return string.Format("--{0}--{1}", "-----------------------------28947758029299", "\r\n");
	}

	private void AppendHeaders(HttpWebRequest webRequest)
	{
		foreach (HttpHeader header in Headers)
		{
			if (_restrictedHeaderActions.ContainsKey(header.Name))
			{
				_restrictedHeaderActions[header.Name](webRequest, header.Value);
			}
			else
			{
				webRequest.Headers.Add(header.Name, header.Value);
			}
		}
	}

	private void AppendCookies(HttpWebRequest webRequest)
	{
		webRequest.CookieContainer = CookieContainer ?? new CookieContainer();
		foreach (HttpCookie cookie2 in Cookies)
		{
			Cookie cookie = new Cookie
			{
				Name = cookie2.Name,
				Value = cookie2.Value,
				Domain = webRequest.RequestUri.Host
			};
			webRequest.CookieContainer.Add(cookie);
		}
	}

	private string EncodeParameters()
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (HttpParameter parameter in Parameters)
		{
			if (stringBuilder.Length > 1)
			{
				stringBuilder.Append("&");
			}
			stringBuilder.AppendFormat("{0}={1}", parameter.Name.UrlEncode(), parameter.Value.UrlEncode());
		}
		return stringBuilder.ToString();
	}

	private void PreparePostBody(HttpWebRequest webRequest)
	{
		if (HasFiles || AlwaysMultipartFormData)
		{
			webRequest.ContentType = GetMultipartFormContentType();
		}
		else if (HasParameters)
		{
			webRequest.ContentType = "application/x-www-form-urlencoded";
			RequestBody = EncodeParameters();
		}
		else if (HasBody)
		{
			webRequest.ContentType = RequestContentType;
		}
	}

	private static void WriteStringTo(Stream stream, string toWrite)
	{
		byte[] bytes = _defaultEncoding.GetBytes(toWrite);
		stream.Write(bytes, 0, bytes.Length);
	}

	private void WriteMultipartFormData(Stream requestStream)
	{
		foreach (HttpParameter parameter in Parameters)
		{
			WriteStringTo(requestStream, GetMultipartFormData(parameter));
		}
		foreach (HttpFile file in Files)
		{
			WriteStringTo(requestStream, GetMultipartFileHeader(file));
			file.Writer(requestStream);
			WriteStringTo(requestStream, "\r\n");
		}
		WriteStringTo(requestStream, GetMultipartFooter());
	}

	private void ExtractResponseData(HttpResponse response, HttpWebResponse webResponse)
	{
		using (webResponse)
		{
			response.ContentEncoding = webResponse.ContentEncoding;
			response.Server = webResponse.Server;
			response.ContentType = webResponse.ContentType;
			response.ContentLength = webResponse.ContentLength;
			Stream responseStream = webResponse.GetResponseStream();
			ProcessResponseStream(responseStream, response);
			response.StatusCode = webResponse.StatusCode;
			response.StatusDescription = webResponse.StatusDescription;
			response.ResponseUri = webResponse.ResponseUri;
			response.ResponseStatus = ResponseStatus.Completed;
			if (webResponse.Cookies != null)
			{
				foreach (Cookie cookie in webResponse.Cookies)
				{
					response.Cookies.Add(new HttpCookie
					{
						Comment = cookie.Comment,
						CommentUri = cookie.CommentUri,
						Discard = cookie.Discard,
						Domain = cookie.Domain,
						Expired = cookie.Expired,
						Expires = cookie.Expires,
						HttpOnly = cookie.HttpOnly,
						Name = cookie.Name,
						Path = cookie.Path,
						Port = cookie.Port,
						Secure = cookie.Secure,
						TimeStamp = cookie.TimeStamp,
						Value = cookie.Value,
						Version = cookie.Version
					});
				}
			}
			string[] allKeys = webResponse.Headers.AllKeys;
			foreach (string name in allKeys)
			{
				string value = webResponse.Headers[name];
				response.Headers.Add(new HttpHeader
				{
					Name = name,
					Value = value
				});
			}
			webResponse.Close();
		}
	}

	private void ProcessResponseStream(Stream webResponseStream, HttpResponse response)
	{
		if (ResponseWriter == null)
		{
			response.RawBytes = webResponseStream.ReadAsBytes();
		}
		else
		{
			ResponseWriter(webResponseStream);
		}
	}

	private void AddRange(HttpWebRequest r, string range)
	{
		Match match = Regex.Match(range, "=(\\d+)-(\\d+)$");
		if (match.Success)
		{
			int num = Convert.ToInt32(match.Groups[1].Value);
			int to = Convert.ToInt32(match.Groups[2].Value);
			r.AddRange(num, to);
		}
	}

	public HttpResponse Post()
	{
		return PostPutInternal("POST");
	}

	public HttpResponse Put()
	{
		return PostPutInternal("PUT");
	}

	public HttpResponse Get()
	{
		return GetStyleMethodInternal("GET");
	}

	public HttpResponse Head()
	{
		return GetStyleMethodInternal("HEAD");
	}

	public HttpResponse Options()
	{
		return GetStyleMethodInternal("OPTIONS");
	}

	public HttpResponse Delete()
	{
		return GetStyleMethodInternal("DELETE");
	}

	public HttpResponse Patch()
	{
		return PostPutInternal("PATCH");
	}

	public HttpResponse Merge()
	{
		return PostPutInternal("MERGE");
	}

	public HttpResponse AsGet(string httpMethod)
	{
		return GetStyleMethodInternal(httpMethod.ToUpperInvariant());
	}

	public HttpResponse AsPost(string httpMethod)
	{
		return PostPutInternal(httpMethod.ToUpperInvariant());
	}

	private HttpResponse GetStyleMethodInternal(string method)
	{
		HttpWebRequest httpWebRequest = ConfigureWebRequest(method, Url);
		if (HasBody && (method == "DELETE" || method == "OPTIONS"))
		{
			httpWebRequest.ContentType = RequestContentType;
			WriteRequestBody(httpWebRequest);
		}
		return GetResponse(httpWebRequest);
	}

	private HttpResponse PostPutInternal(string method)
	{
		HttpWebRequest httpWebRequest = ConfigureWebRequest(method, Url);
		PreparePostData(httpWebRequest);
		WriteRequestBody(httpWebRequest);
		return GetResponse(httpWebRequest);
	}

	private void ExtractErrorResponse(HttpResponse httpResponse, Exception ex)
	{
		if (ex is WebException { Status: WebExceptionStatus.Timeout } ex2)
		{
			httpResponse.ResponseStatus = ResponseStatus.TimedOut;
			httpResponse.ErrorMessage = ex.Message;
			httpResponse.ErrorException = ex2;
		}
		else
		{
			httpResponse.ErrorMessage = ex.Message;
			httpResponse.ErrorException = ex;
			httpResponse.ResponseStatus = ResponseStatus.Error;
		}
	}

	private HttpResponse GetResponse(HttpWebRequest request)
	{
		HttpResponse httpResponse = new HttpResponse
		{
			ResponseStatus = ResponseStatus.None
		};
		try
		{
			HttpWebResponse rawResponse = GetRawResponse(request);
			ExtractResponseData(httpResponse, rawResponse);
		}
		catch (Exception ex)
		{
			ExtractErrorResponse(httpResponse, ex);
		}
		return httpResponse;
	}

	private static HttpWebResponse GetRawResponse(HttpWebRequest request)
	{
		try
		{
			return (HttpWebResponse)request.GetResponse();
		}
		catch (WebException ex)
		{
			if (ex.Response is HttpWebResponse)
			{
				return ex.Response as HttpWebResponse;
			}
			throw;
		}
	}

	private void PreparePostData(HttpWebRequest webRequest)
	{
		if (HasFiles || AlwaysMultipartFormData)
		{
			webRequest.ContentType = GetMultipartFormContentType();
			using Stream requestStream = webRequest.GetRequestStream();
			WriteMultipartFormData(requestStream);
		}
		PreparePostBody(webRequest);
	}

	private void WriteRequestBody(HttpWebRequest webRequest)
	{
		if (!HasBody)
		{
			return;
		}
		byte[] array = ((RequestBodyBytes == null) ? _defaultEncoding.GetBytes(RequestBody) : RequestBodyBytes);
		webRequest.ContentLength = array.Length;
		using Stream stream = webRequest.GetRequestStream();
		stream.Write(array, 0, array.Length);
	}

	private HttpWebRequest ConfigureWebRequest(string method, Uri url)
	{
		HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
		httpWebRequest.UseDefaultCredentials = UseDefaultCredentials;
		httpWebRequest.PreAuthenticate = PreAuthenticate;
		ServicePointManager.Expect100Continue = false;
		AppendHeaders(httpWebRequest);
		AppendCookies(httpWebRequest);
		httpWebRequest.Method = method;
		if (!HasFiles && !AlwaysMultipartFormData)
		{
			httpWebRequest.ContentLength = 0L;
		}
		httpWebRequest.AutomaticDecompression = DecompressionMethods.Deflate;
		if (ClientCertificates != null)
		{
			httpWebRequest.ClientCertificates.AddRange(ClientCertificates);
		}
		if (UserAgent.HasValue())
		{
			httpWebRequest.UserAgent = UserAgent;
		}
		if (Timeout != 0)
		{
			httpWebRequest.Timeout = Timeout;
		}
		if (ReadWriteTimeout != 0)
		{
			httpWebRequest.ReadWriteTimeout = ReadWriteTimeout;
		}
		if (Credentials != null)
		{
			httpWebRequest.Credentials = Credentials;
		}
		if (Proxy != null)
		{
			httpWebRequest.Proxy = Proxy;
		}
		httpWebRequest.AllowAutoRedirect = FollowRedirects;
		if (FollowRedirects && MaxRedirects.HasValue)
		{
			httpWebRequest.MaximumAutomaticRedirections = MaxRedirects.Value;
		}
		return httpWebRequest;
	}
}
