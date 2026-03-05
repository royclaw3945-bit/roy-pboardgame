using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using RestSharp.Deserializers;
using RestSharp.Extensions;

namespace RestSharp;

public class RestClient : IRestClient
{
	private static readonly Version version = new AssemblyName(Assembly.GetExecutingAssembly().FullName).Version;

	public IHttpFactory HttpFactory = new SimpleFactory<Http>();

	private IDictionary<string, IDeserializer> ContentHandlers { get; set; }

	private IList<string> AcceptTypes { get; set; }

	public IList<Parameter> DefaultParameters { get; private set; }

	public int? MaxRedirects { get; set; }

	public X509CertificateCollection ClientCertificates { get; set; }

	public IWebProxy Proxy { get; set; }

	public bool FollowRedirects { get; set; }

	public CookieContainer CookieContainer { get; set; }

	public string UserAgent { get; set; }

	public int Timeout { get; set; }

	public int ReadWriteTimeout { get; set; }

	public bool UseSynchronizationContext { get; set; }

	public IAuthenticator Authenticator { get; set; }

	public virtual Uri BaseUrl { get; set; }

	public bool PreAuthenticate { get; set; }

	public RestClient()
	{
		ContentHandlers = new Dictionary<string, IDeserializer>();
		AcceptTypes = new List<string>();
		DefaultParameters = new List<Parameter>();
		AddHandler("application/json", new JsonDeserializer());
		AddHandler("application/xml", new XmlDeserializer());
		AddHandler("text/json", new JsonDeserializer());
		AddHandler("text/x-json", new JsonDeserializer());
		AddHandler("text/javascript", new JsonDeserializer());
		AddHandler("text/xml", new XmlDeserializer());
		AddHandler("*", new XmlDeserializer());
		FollowRedirects = true;
	}

	public RestClient(Uri baseUrl)
		: this()
	{
		BaseUrl = baseUrl;
	}

	public RestClient(string baseUrl)
		: this()
	{
		if (string.IsNullOrEmpty(baseUrl))
		{
			throw new ArgumentNullException("baseUrl");
		}
		BaseUrl = new Uri(baseUrl);
	}

	public void AddHandler(string contentType, IDeserializer deserializer)
	{
		ContentHandlers[contentType] = deserializer;
		if (contentType != "*")
		{
			AcceptTypes.Add(contentType);
			string value = string.Join(", ", AcceptTypes.ToArray());
			this.RemoveDefaultParameter("Accept");
			this.AddDefaultParameter("Accept", value, ParameterType.HttpHeader);
		}
	}

	public void RemoveHandler(string contentType)
	{
		ContentHandlers.Remove(contentType);
		AcceptTypes.Remove(contentType);
		this.RemoveDefaultParameter("Accept");
	}

	public void ClearHandlers()
	{
		ContentHandlers.Clear();
		AcceptTypes.Clear();
		this.RemoveDefaultParameter("Accept");
	}

	private IDeserializer GetHandler(string contentType)
	{
		if (contentType == null)
		{
			throw new ArgumentNullException("contentType");
		}
		if (string.IsNullOrEmpty(contentType) && ContentHandlers.ContainsKey("*"))
		{
			return ContentHandlers["*"];
		}
		int num = contentType.IndexOf(';');
		if (num > -1)
		{
			contentType = contentType.Substring(0, num);
		}
		IDeserializer result = null;
		if (ContentHandlers.ContainsKey(contentType))
		{
			result = ContentHandlers[contentType];
		}
		else if (ContentHandlers.ContainsKey("*"))
		{
			result = ContentHandlers["*"];
		}
		return result;
	}

	private void AuthenticateIfNeeded(RestClient client, IRestRequest request)
	{
		if (Authenticator != null)
		{
			Authenticator.Authenticate(client, request);
		}
	}

	public Uri BuildUri(IRestRequest request)
	{
		string text = request.Resource;
		IEnumerable<Parameter> enumerable = request.Parameters.Where((Parameter p) => p.Type == ParameterType.UrlSegment);
		UriBuilder uriBuilder = new UriBuilder(BaseUrl);
		foreach (Parameter item in enumerable)
		{
			if (item.Value == null)
			{
				throw new ArgumentException($"Cannot build uri when url segment parameter '{item.Name}' value is null.", "request");
			}
			if (!string.IsNullOrEmpty(text))
			{
				text = text.Replace("{" + item.Name + "}", item.Value.ToString().UrlEncode());
			}
			uriBuilder.Path = uriBuilder.Path.UrlDecode().Replace("{" + item.Name + "}", item.Value.ToString().UrlEncode());
		}
		BaseUrl = new Uri(uriBuilder.ToString());
		if (!string.IsNullOrEmpty(text) && text.StartsWith("/"))
		{
			text = text.Substring(1);
		}
		if (BaseUrl != null && !string.IsNullOrEmpty(BaseUrl.AbsoluteUri))
		{
			if (!BaseUrl.AbsoluteUri.EndsWith("/") && !string.IsNullOrEmpty(text))
			{
				text = "/" + text;
			}
			text = (string.IsNullOrEmpty(text) ? BaseUrl.AbsoluteUri : $"{BaseUrl}{text}");
		}
		IEnumerable<Parameter> enumerable2 = ((request.Method == Method.POST || request.Method == Method.PUT || request.Method == Method.PATCH) ? request.Parameters.Where((Parameter p) => p.Type == ParameterType.QueryString).ToList() : request.Parameters.Where((Parameter p) => p.Type == ParameterType.GetOrPost || p.Type == ParameterType.QueryString).ToList());
		if (!enumerable2.Any())
		{
			return new Uri(text);
		}
		string text2 = EncodeParameters(enumerable2);
		string text3 = (text.Contains("?") ? "&" : "?");
		text = text + text3 + text2;
		return new Uri(text);
	}

	private static string EncodeParameters(IEnumerable<Parameter> parameters)
	{
		return string.Join("&", parameters.Select((Parameter x) => EncodeParameter(x)).ToArray());
	}

	private static string EncodeParameter(Parameter parameter)
	{
		if (parameter.Value != null)
		{
			return parameter.Name.UrlEncode() + "=" + parameter.Value.ToString().UrlEncode();
		}
		return parameter.Name.UrlEncode() + "=";
	}

	private void ConfigureHttp(IRestRequest request, IHttp http)
	{
		http.AlwaysMultipartFormData = request.AlwaysMultipartFormData;
		http.UseDefaultCredentials = request.UseDefaultCredentials;
		http.ResponseWriter = request.ResponseWriter;
		http.CookieContainer = CookieContainer;
		foreach (Parameter p in DefaultParameters)
		{
			if (!request.Parameters.Any((Parameter parameter2) => parameter2.Name == p.Name && parameter2.Type == p.Type))
			{
				request.AddParameter(p);
			}
		}
		if (request.Parameters.All((Parameter parameter2) => parameter2.Name.ToLowerInvariant() != "accept"))
		{
			string value = string.Join(", ", AcceptTypes.ToArray());
			request.AddParameter("Accept", value, ParameterType.HttpHeader);
		}
		http.Url = BuildUri(request);
		http.PreAuthenticate = PreAuthenticate;
		string text = UserAgent ?? http.UserAgent;
		http.UserAgent = (text.HasValue() ? text : ("RestSharp/" + version));
		int num = ((request.Timeout > 0) ? request.Timeout : Timeout);
		if (num > 0)
		{
			http.Timeout = num;
		}
		int num2 = ((request.ReadWriteTimeout > 0) ? request.ReadWriteTimeout : ReadWriteTimeout);
		if (num2 > 0)
		{
			http.ReadWriteTimeout = num2;
		}
		http.FollowRedirects = FollowRedirects;
		if (ClientCertificates != null)
		{
			http.ClientCertificates = ClientCertificates;
		}
		http.MaxRedirects = MaxRedirects;
		if (request.Credentials != null)
		{
			http.Credentials = request.Credentials;
		}
		foreach (HttpHeader item in from parameter2 in request.Parameters
			where parameter2.Type == ParameterType.HttpHeader
			select new HttpHeader
			{
				Name = parameter2.Name,
				Value = Convert.ToString(parameter2.Value)
			})
		{
			http.Headers.Add(item);
		}
		foreach (HttpCookie item2 in from parameter2 in request.Parameters
			where parameter2.Type == ParameterType.Cookie
			select new HttpCookie
			{
				Name = parameter2.Name,
				Value = Convert.ToString(parameter2.Value)
			})
		{
			http.Cookies.Add(item2);
		}
		foreach (HttpParameter item3 in from parameter2 in request.Parameters
			where parameter2.Type == ParameterType.GetOrPost && parameter2.Value != null
			select new HttpParameter
			{
				Name = parameter2.Name,
				Value = Convert.ToString(parameter2.Value)
			})
		{
			http.Parameters.Add(item3);
		}
		foreach (FileParameter file in request.Files)
		{
			http.Files.Add(new HttpFile
			{
				Name = file.Name,
				ContentType = file.ContentType,
				Writer = file.Writer,
				FileName = file.FileName,
				ContentLength = file.ContentLength
			});
		}
		Parameter parameter = request.Parameters.Where((Parameter parameter2) => parameter2.Type == ParameterType.RequestBody).FirstOrDefault();
		if (parameter != null)
		{
			http.RequestContentType = parameter.Name;
			if (!http.Files.Any())
			{
				object value2 = parameter.Value;
				if (value2 is byte[])
				{
					http.RequestBodyBytes = (byte[])value2;
				}
				else
				{
					http.RequestBody = Convert.ToString(parameter.Value);
				}
			}
			else
			{
				http.Parameters.Add(new HttpParameter
				{
					Name = parameter.Name,
					Value = Convert.ToString(parameter.Value)
				});
			}
		}
		ConfigureProxy(http);
	}

	private void ConfigureProxy(IHttp http)
	{
		if (Proxy != null)
		{
			http.Proxy = Proxy;
		}
	}

	private RestResponse ConvertToRestResponse(IRestRequest request, HttpResponse httpResponse)
	{
		RestResponse restResponse = new RestResponse
		{
			Content = httpResponse.Content,
			ContentEncoding = httpResponse.ContentEncoding,
			ContentLength = httpResponse.ContentLength,
			ContentType = httpResponse.ContentType,
			ErrorException = httpResponse.ErrorException,
			ErrorMessage = httpResponse.ErrorMessage,
			RawBytes = httpResponse.RawBytes,
			ResponseStatus = httpResponse.ResponseStatus,
			ResponseUri = httpResponse.ResponseUri,
			Server = httpResponse.Server,
			StatusCode = httpResponse.StatusCode,
			StatusDescription = httpResponse.StatusDescription,
			Request = request
		};
		foreach (HttpHeader header in httpResponse.Headers)
		{
			restResponse.Headers.Add(new Parameter
			{
				Name = header.Name,
				Value = header.Value,
				Type = ParameterType.HttpHeader
			});
		}
		foreach (HttpCookie cookie in httpResponse.Cookies)
		{
			restResponse.Cookies.Add(new RestResponseCookie
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
		return restResponse;
	}

	private IRestResponse<T> Deserialize<T>(IRestRequest request, IRestResponse raw)
	{
		request.OnBeforeDeserialization(raw);
		IRestResponse<T> restResponse = new RestResponse<T>();
		try
		{
			restResponse = raw.toAsyncResponse<T>();
			restResponse.Request = request;
			if (restResponse.ErrorException == null)
			{
				IDeserializer handler = GetHandler(raw.ContentType);
				if (handler != null)
				{
					handler.RootElement = request.RootElement;
					handler.DateFormat = request.DateFormat;
					handler.Namespace = request.XmlNamespace;
					restResponse.Data = handler.Deserialize<T>(raw);
				}
			}
		}
		catch (Exception ex)
		{
			restResponse.ResponseStatus = ResponseStatus.Error;
			restResponse.ErrorMessage = ex.Message;
			restResponse.ErrorException = ex;
		}
		return restResponse;
	}

	public byte[] DownloadData(IRestRequest request)
	{
		return Execute(request).RawBytes;
	}

	public virtual IRestResponse Execute(IRestRequest request)
	{
		string name = Enum.GetName(typeof(Method), request.Method);
		Method method = request.Method;
		if ((uint)(method - 1) <= 1u || (uint)(method - 6) <= 1u)
		{
			return Execute(request, name, DoExecuteAsPost);
		}
		return Execute(request, name, DoExecuteAsGet);
	}

	public IRestResponse ExecuteAsGet(IRestRequest request, string httpMethod)
	{
		return Execute(request, httpMethod, DoExecuteAsGet);
	}

	public IRestResponse ExecuteAsPost(IRestRequest request, string httpMethod)
	{
		request.Method = Method.POST;
		return Execute(request, httpMethod, DoExecuteAsPost);
	}

	private IRestResponse Execute(IRestRequest request, string httpMethod, Func<IHttp, string, HttpResponse> getResponse)
	{
		AuthenticateIfNeeded(this, request);
		IRestResponse restResponse = new RestResponse();
		try
		{
			IHttp http = HttpFactory.Create();
			ConfigureHttp(request, http);
			restResponse = ConvertToRestResponse(request, getResponse(http, httpMethod));
			restResponse.Request = request;
			restResponse.Request.IncreaseNumAttempts();
		}
		catch (Exception ex)
		{
			restResponse.ResponseStatus = ResponseStatus.Error;
			restResponse.ErrorMessage = ex.Message;
			restResponse.ErrorException = ex;
		}
		return restResponse;
	}

	private static HttpResponse DoExecuteAsGet(IHttp http, string method)
	{
		return http.AsGet(method);
	}

	private static HttpResponse DoExecuteAsPost(IHttp http, string method)
	{
		return http.AsPost(method);
	}

	public virtual IRestResponse<T> Execute<T>(IRestRequest request) where T : new()
	{
		return Deserialize<T>(request, Execute(request));
	}

	public IRestResponse<T> ExecuteAsGet<T>(IRestRequest request, string httpMethod) where T : new()
	{
		return Deserialize<T>(request, ExecuteAsGet(request, httpMethod));
	}

	public IRestResponse<T> ExecuteAsPost<T>(IRestRequest request, string httpMethod) where T : new()
	{
		return Deserialize<T>(request, ExecuteAsPost(request, httpMethod));
	}
}
