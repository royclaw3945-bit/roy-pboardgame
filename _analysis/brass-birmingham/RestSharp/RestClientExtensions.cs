using System;
using System.Linq;

namespace RestSharp;

public static class RestClientExtensions
{
	public static IRestResponse<T> Get<T>(this IRestClient client, IRestRequest request) where T : new()
	{
		request.Method = Method.GET;
		return client.Execute<T>(request);
	}

	public static IRestResponse<T> Post<T>(this IRestClient client, IRestRequest request) where T : new()
	{
		request.Method = Method.POST;
		return client.Execute<T>(request);
	}

	public static IRestResponse<T> Put<T>(this IRestClient client, IRestRequest request) where T : new()
	{
		request.Method = Method.PUT;
		return client.Execute<T>(request);
	}

	public static IRestResponse<T> Head<T>(this IRestClient client, IRestRequest request) where T : new()
	{
		request.Method = Method.HEAD;
		return client.Execute<T>(request);
	}

	public static IRestResponse<T> Options<T>(this IRestClient client, IRestRequest request) where T : new()
	{
		request.Method = Method.OPTIONS;
		return client.Execute<T>(request);
	}

	public static IRestResponse<T> Patch<T>(this IRestClient client, IRestRequest request) where T : new()
	{
		request.Method = Method.PATCH;
		return client.Execute<T>(request);
	}

	public static IRestResponse<T> Delete<T>(this IRestClient client, IRestRequest request) where T : new()
	{
		request.Method = Method.DELETE;
		return client.Execute<T>(request);
	}

	public static IRestResponse Get(this IRestClient client, IRestRequest request)
	{
		request.Method = Method.GET;
		return client.Execute(request);
	}

	public static IRestResponse Post(this IRestClient client, IRestRequest request)
	{
		request.Method = Method.POST;
		return client.Execute(request);
	}

	public static IRestResponse Put(this IRestClient client, IRestRequest request)
	{
		request.Method = Method.PUT;
		return client.Execute(request);
	}

	public static IRestResponse Head(this IRestClient client, IRestRequest request)
	{
		request.Method = Method.HEAD;
		return client.Execute(request);
	}

	public static IRestResponse Options(this IRestClient client, IRestRequest request)
	{
		request.Method = Method.OPTIONS;
		return client.Execute(request);
	}

	public static IRestResponse Patch(this IRestClient client, IRestRequest request)
	{
		request.Method = Method.PATCH;
		return client.Execute(request);
	}

	public static IRestResponse Delete(this IRestClient client, IRestRequest request)
	{
		request.Method = Method.DELETE;
		return client.Execute(request);
	}

	public static void AddDefaultParameter(this IRestClient restClient, Parameter p)
	{
		if (p.Type == ParameterType.RequestBody)
		{
			throw new NotSupportedException("Cannot set request body from default headers. Use Request.AddBody() instead.");
		}
		restClient.DefaultParameters.Add(p);
	}

	public static void RemoveDefaultParameter(this IRestClient restClient, string name)
	{
		Parameter parameter = restClient.DefaultParameters.SingleOrDefault((Parameter p) => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
		if (parameter != null)
		{
			restClient.DefaultParameters.Remove(parameter);
		}
	}

	public static void AddDefaultParameter(this IRestClient restClient, string name, object value)
	{
		restClient.AddDefaultParameter(new Parameter
		{
			Name = name,
			Value = value,
			Type = ParameterType.GetOrPost
		});
	}

	public static void AddDefaultParameter(this IRestClient restClient, string name, object value, ParameterType type)
	{
		restClient.AddDefaultParameter(new Parameter
		{
			Name = name,
			Value = value,
			Type = type
		});
	}

	public static void AddDefaultHeader(this IRestClient restClient, string name, string value)
	{
		restClient.AddDefaultParameter(name, value, ParameterType.HttpHeader);
	}

	public static void AddDefaultUrlSegment(this IRestClient restClient, string name, string value)
	{
		restClient.AddDefaultParameter(name, value, ParameterType.UrlSegment);
	}
}
