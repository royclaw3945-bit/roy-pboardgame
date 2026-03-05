using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Utils.Extensions;
using BoardGameRules.Utils.Logging;
using BrassApplication.Game;
using BrassApplication.Persistence.Repository;
using BrassApplication.Persistence.RepositoryBackends;
using BrassApplication.Persistence.Serialization;
using Frameworks.Network.Rest;
using Frameworks.Persistence.RepositoryBackends.Common;
using Newtonsoft.Json;
using RestSharp;

namespace Frameworks.Persistence.RepositoryBackends.Network;

public class NetworkGameStorage : NetworkBackendClientBase, IGameStorage
{
	private static readonly ILog Logger = LogFactory.BuildLogger(typeof(NetworkGameStorage));

	public NetworkGameStorage(IRestClientFactory restClientFactory, IRestRequestFactory restRequestFactory, ISerialize serializer)
		: base(restClientFactory, restRequestFactory, serializer)
	{
	}

	public ReadOnlyCollection<IEvent> LoadEvents(string gameId, int minRevision, int maxRevision)
	{
		IRestRequest restRequest = _restRequestFactory.CreateRestRequest("/1.0.0/games/" + gameId + "/events", Method.GET);
		restRequest.AddParameter("perPage", "0", ParameterType.GetOrPost);
		IRestResponse restResponse = _restClient.Execute(restRequest);
		Logger.Debug("LoadEvents() - Response status code = " + restResponse.StatusCode);
		CheckResponseAndThrowOnError(restResponse);
		var anonymousTypeObject = new
		{
			data = new List<IEvent>()
		};
		List<IEvent> data = Deserialise(restResponse.Content, anonymousTypeObject).data;
		int val = data.Count - minRevision;
		int count = Math.Min(maxRevision - minRevision, val);
		return data.GetRange(minRevision, count).AsReadOnly();
	}

	public void SaveGame(string gameId, IList<IEvent> eventsToAppend, GameMetadata metadata)
	{
		using MemoryStream memoryStream = new MemoryStream();
		using MemoryStream memoryStream2 = new MemoryStream();
		_serializer.Serialize(memoryStream, eventsToAppend);
		_serializer.Serialize(memoryStream2, metadata);
		string text = "{ ";
		if (eventsToAppend.Count > 0)
		{
			text = text + "\"events\": " + memoryStream.ConvertToString() + ",";
		}
		text = text + "\"metadata\": " + memoryStream2.ConvertToString() + "}";
		IRestRequest request = _restRequestFactory.CreateRestRequest("/1.0.0/games/" + gameId + "/events-and-metadata", Method.POST, text);
		IRestResponse restResponse = _restClient.Execute(request);
		Logger.Debug("SaveGame() - Response status code = " + restResponse.StatusCode);
		CheckResponseAndThrowOnError(restResponse);
	}

	public void DeleteEvents(string gameId)
	{
		IRestRequest request = _restRequestFactory.CreateRestRequest("/1.0.0/games/" + gameId + "/events", Method.DELETE);
		IRestResponse restResponse = _restClient.Execute(request);
		Logger.Debug("DeleteEvents() - Response status code = " + restResponse.StatusCode);
		CheckResponseAndThrowOnError(restResponse);
	}

	public string CreateGame(GameMetadata gameMetadata)
	{
		string text = null;
		try
		{
			using MemoryStream memoryStream = new MemoryStream();
			_serializer.Serialize(memoryStream, gameMetadata);
			string data = "{ \"metadata\": " + memoryStream.ConvertToString() + "}";
			IRestRequest request = _restRequestFactory.CreateRestRequest("/1.0.0/games", Method.POST, data);
			IRestResponse restResponse = _restClient.Execute(request);
			CheckResponseAndThrowOnError(restResponse);
			var anonymousTypeObject = new
			{
				data = new
				{
					id = ""
				}
			};
			return Deserialise(restResponse.Content, anonymousTypeObject).data.id;
		}
		catch (JsonReaderException ex)
		{
			throw new DataParsingException(ex.Message);
		}
	}

	public ReadOnlyCollection<GameMetadata> GetGamesMetadata(IGamesFilter gamesFilter)
	{
		List<GameMetadata> list = new List<GameMetadata>();
		try
		{
			IRestRequest restRequest = _restRequestFactory.CreateRestRequest("/1.0.0/games", Method.GET);
			restRequest.AddParameter("filter[]", "updated_at:gt:" + DateTime.Now.AddDays(-7.0).ToString("yyyy-MM-dd"), ParameterType.GetOrPost);
			if (gamesFilter != null)
			{
				if (!string.IsNullOrEmpty(gamesFilter.GameId))
				{
					restRequest.AddParameter("filter[]", "id:eq:" + gamesFilter.GameId, ParameterType.GetOrPost);
				}
				if (!gamesFilter.TwoPlayers)
				{
					restRequest.AddParameter("filter[]", "metadata->GameStartConfiguration->NumberOfPlayersAsInt:ne:2", ParameterType.GetOrPost);
				}
				if (!gamesFilter.ThreePlayers)
				{
					restRequest.AddParameter("filter[]", "metadata->GameStartConfiguration->NumberOfPlayersAsInt:ne:3", ParameterType.GetOrPost);
				}
				if (!gamesFilter.FourPlayers)
				{
					restRequest.AddParameter("filter[]", "metadata->GameStartConfiguration->NumberOfPlayersAsInt:ne:4", ParameterType.GetOrPost);
				}
				if (gamesFilter.PrivateGames)
				{
					restRequest.AddParameter("filter[]", "metadata->NetworkGameStartConfiguration->IsPrivate:eq:true", ParameterType.GetOrPost);
				}
				if (gamesFilter.ShowOnlyOpenGames)
				{
					restRequest.AddParameter("filter[]", "metadata->NetworkGameProgressInformation->CurrentGameStatus:eq:Open", ParameterType.GetOrPost);
				}
				if (gamesFilter.ShowExceptOpenGames)
				{
					restRequest.AddParameter("filter[]", "metadata->NetworkGameProgressInformation->CurrentGameStatus:ne:Open", ParameterType.GetOrPost);
				}
				if (!string.IsNullOrEmpty(gamesFilter.ShowOnlyGamesThatDoNotBelongToPlayerId))
				{
					restRequest.AddParameter("filter[]", "metadata->PlayerMetadata:nct:" + gamesFilter.ShowOnlyGamesThatDoNotBelongToPlayerId, ParameterType.GetOrPost);
				}
				if (!string.IsNullOrEmpty(gamesFilter.ShowOnlyGamesThatBelongToPlayerId))
				{
					restRequest.AddParameter("filter[]", "metadata->PlayerMetadata:ct:" + gamesFilter.ShowOnlyGamesThatBelongToPlayerId, ParameterType.GetOrPost);
				}
			}
			restRequest.AddParameter("sort[]", "updated_at:DSC", ParameterType.GetOrPost);
			restRequest.AddParameter("perPage", "0", ParameterType.GetOrPost);
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			IRestResponse restResponse = _restClient.Execute(restRequest);
			stopwatch.Stop();
			Logger.Debug(string.Concat("GetGamesMetadata() - Response status code = ", restResponse.StatusCode, ", elapsed = ", stopwatch.ElapsedMilliseconds, "ms"));
			CheckResponseAndThrowOnError(restResponse);
			var anonymousTypeObject = new
			{
				data = new[]
				{
					new
					{
						id = "",
						metadata = (new object() as GameMetadata)
					}
				}
			};
			list = (from metadata in Deserialise(restResponse.Content, anonymousTypeObject).data.Select(data =>
				{
					data.metadata.GameId = data.id;
					return data.metadata;
				})
				orderby metadata.NetworkGameProgressInformation.CurrentGameStatus
				select metadata).ToList();
			if (gamesFilter != null && !string.IsNullOrEmpty(gamesFilter.ShowOnlyGamesThatBelongToPlayerId))
			{
				list = list.Where((GameMetadata metadata) => metadata.PlayerMetadata.Exists((PlayerMetadata pm) => pm?.PlayerNetworkId.Equals(gamesFilter.ShowOnlyGamesThatBelongToPlayerId) ?? false)).ToList();
			}
			if (gamesFilter != null && !string.IsNullOrEmpty(gamesFilter.ShowOnlyGamesThatDoNotBelongToPlayerId))
			{
				list = list.Where((GameMetadata metadata) => !metadata.PlayerMetadata.Exists((PlayerMetadata pm) => pm?.PlayerNetworkId.Equals(gamesFilter.ShowOnlyGamesThatDoNotBelongToPlayerId) ?? false)).ToList();
			}
		}
		catch (JsonReaderException ex)
		{
			throw new DataParsingException(ex.Message);
		}
		return list.AsReadOnly();
	}

	public void SavePlayerMetadata(string gameId, PlayerMetadata playerMetadata)
	{
		string data = "{ \"lastSeenRevision\": " + playerMetadata.LastSeenEventRevision + "}";
		IRestRequest request = _restRequestFactory.CreateRestRequest("/1.0.0/games/" + gameId + "/metadata/lastSeenRevision", Method.PUT, data);
		IRestResponse restResponse = _restClient.Execute(request);
		Logger.Debug("SavePlayerMetadata() - Response status code = " + restResponse.StatusCode);
		CheckResponseAndThrowOnError(restResponse);
	}

	public GameMetadata LoadGameMetadata(string gameId)
	{
		IRestRequest request = _restRequestFactory.CreateRestRequest("/1.0.0/games?filter[]=id:eq:" + gameId, Method.GET);
		IRestResponse restResponse = _restClient.Execute(request);
		Logger.Debug("LoadGameMetadata() - Response status code = " + restResponse.StatusCode);
		GameMetadata result = null;
		CheckResponseAndThrowOnError(restResponse);
		var anonymousTypeObject = new
		{
			data = new[]
			{
				new
				{
					id = "",
					metadata = (new object() as GameMetadata)
				}
			}
		};
		List<GameMetadata> list = Deserialise(restResponse.Content, anonymousTypeObject).data.Select(data =>
		{
			data.metadata.GameId = data.id;
			return data.metadata;
		}).ToList();
		if (list.Count > 0)
		{
			result = list.ElementAt(0);
		}
		return result;
	}

	public void DeleteGameMetadata(string gameId)
	{
		throw new NotImplementedException();
	}

	public void JoinGame(string gameId, string password, PlayerMetadata playerMetadata)
	{
		try
		{
			using MemoryStream memoryStream = new MemoryStream();
			_serializer.Serialize(memoryStream, playerMetadata);
			string data = "{ " + (string.IsNullOrEmpty(password) ? "" : ("\"password\": \"" + password + "\",")) + " \"metadata\": " + memoryStream.ConvertToString() + "}";
			IRestRequest request = _restRequestFactory.CreateRestRequest("/1.0.0/games/" + gameId + "/players", Method.PUT, data);
			IRestResponse restResponse = _restClient.Execute(request);
			Logger.Debug("JoinGame() - Response status code = " + restResponse.StatusCode);
			CheckResponseAndThrowOnError(restResponse);
		}
		catch (JsonReaderException ex)
		{
			throw new DataParsingException(ex.Message);
		}
	}

	public void LeaveGame(string gameId, string playerId, bool isThisAnAbandonRequest)
	{
		try
		{
			IRestRequest restRequest = _restRequestFactory.CreateRestRequest("/1.0.0/games/" + gameId + "/players", Method.DELETE);
			restRequest.AddParameter("playerId", playerId, ParameterType.GetOrPost);
			if (isThisAnAbandonRequest)
			{
				restRequest.AddParameter("abandon", "true", ParameterType.GetOrPost);
			}
			IRestResponse restResponse = _restClient.Execute(restRequest);
			Logger.Debug("LeaveGame() - Response status code = " + restResponse.StatusCode);
			CheckResponseAndThrowOnError(restResponse);
		}
		catch (JsonReaderException ex)
		{
			throw new DataParsingException(ex.Message);
		}
	}

	public void DeleteAll()
	{
		throw new NotImplementedException();
	}
}
