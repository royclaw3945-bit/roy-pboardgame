using System;
using System.Collections.Generic;
using System.Net;
using BoardGameRules.Utils.Extensions;
using BrassApplication.Network;
using BrassApplication.Persistence.Serialization;
using Frameworks.Network.Rest;
using Frameworks.Persistence.RepositoryBackends.Common;
using RestSharp;

namespace Frameworks.Persistence.RepositoryBackends.Network;

public class NetworkBackendClientBase
{
	public const string APIVersion = "/1.0.0";

	protected readonly IRestClient _restClient;

	protected readonly IRestRequestFactory _restRequestFactory;

	protected readonly ISerialize _serializer;

	protected NetworkBackendClientBase(IRestClientFactory restClientFactory, IRestRequestFactory restRequestFactory, ISerialize serializer)
	{
		_restClient = restClientFactory.CreateRestClient();
		_restRequestFactory = restRequestFactory;
		_serializer = serializer;
	}

	public static List<NetworkBackendStatus> CheckForCommonStatuses(IRestResponse response)
	{
		if (response.ResponseStatus.Equals(ResponseStatus.Completed) && (response.StatusCode.Equals(HttpStatusCode.OK) || response.StatusCode.Equals(HttpStatusCode.Created)))
		{
			return new List<NetworkBackendStatus> { NetworkBackendStatus.Success };
		}
		if (response.ResponseStatus.Equals(ResponseStatus.TimedOut))
		{
			return new List<NetworkBackendStatus> { NetworkBackendStatus.Timeout };
		}
		if (response.ResponseStatus.Equals(ResponseStatus.Error))
		{
			return new List<NetworkBackendStatus> { NetworkBackendStatus.Error };
		}
		if (response.StatusCode.Equals(HttpStatusCode.Unauthorized))
		{
			return new List<NetworkBackendStatus> { NetworkBackendStatus.UnauthorizedAndNeedToLoginAgain };
		}
		if (response.StatusCode.Equals(HttpStatusCode.NotFound))
		{
			return new List<NetworkBackendStatus> { NetworkBackendStatus.NotFound };
		}
		if (response.StatusCode.Equals(HttpStatusCode.BadGateway))
		{
			throw new ServerException("Bad Gateway");
		}
		return new List<NetworkBackendStatus>();
	}

	protected void CheckResponseAndThrowOnError(IRestResponse response)
	{
		if (response.ResponseStatus.Equals(ResponseStatus.Completed) && (response.StatusCode.Equals(HttpStatusCode.OK) || response.StatusCode.Equals(HttpStatusCode.Created)))
		{
			return;
		}
		if (response.ResponseStatus.Equals(ResponseStatus.TimedOut))
		{
			throw new TimeoutException();
		}
		if (response.Content.Contains("The password and game.password must match.") || response.Content.Contains("The password field is required when game.is private is 1."))
		{
			throw new ServerException("WrongGamePassword");
		}
		if (response.StatusCode.Equals(HttpStatusCode.Unauthorized))
		{
			throw new UnauthorizedAccessException();
		}
		if (response.StatusCode.Equals(HttpStatusCode.NotFound))
		{
			throw new ServerException("Path not found");
		}
		if (response.StatusCode.Equals(HttpStatusCode.BadGateway))
		{
			throw new ServerException("Bad Gateway");
		}
		throw new ServerException("Unknown Error");
	}

	public T Deserialise<T>(string value, T anonymousTypeObject)
	{
		return _serializer.Deserialize<T>(value.ToStream());
	}
}
