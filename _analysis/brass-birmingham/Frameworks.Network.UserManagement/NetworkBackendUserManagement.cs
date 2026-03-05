using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BoardGameRules.Utils.Logging;
using BrassApplication.Network;
using BrassApplication.Persistence.Serialization;
using Frameworks.Network.Rest;
using Frameworks.Persistence.RepositoryBackends.Network;
using InterfaceAdapters.Utils.MainThreadDispatcher;
using Newtonsoft.Json;
using RestSharp;

namespace Frameworks.Network.UserManagement;

public class NetworkBackendUserManagement : NetworkBackendClientBase, INetworkBackendUserManagement
{
	private static readonly ILog Logger = LogFactory.BuildLogger(typeof(NetworkBackendUserManagement));

	private readonly IMainThreadDispatcher _dispatcher;

	public string UserId { get; private set; }

	public string UserNickname { get; private set; }

	public NetworkBackendUserManagement(IRestClientFactory restClientFactory, IRestRequestFactory restRequestFactory, ISerialize serializer, IMainThreadDispatcher dispatcher)
		: base(restClientFactory, restRequestFactory, serializer)
	{
		_dispatcher = dispatcher;
	}

	public Task Register(string email, string nickname, string password, Action<IEnumerable<NetworkBackendStatus>> registerCompleted)
	{
		return Task.Run(delegate
		{
			List<NetworkBackendStatus> backendStatuses = new List<NetworkBackendStatus>();
			try
			{
				string value = email.ToLower();
				IRestRequest restRequest = _restRequestFactory.CreateRestRequest("/register", Method.POST);
				restRequest.AddParameter("email", value, ParameterType.GetOrPost);
				restRequest.AddParameter("name", nickname, ParameterType.GetOrPost);
				restRequest.AddParameter("password", password, ParameterType.GetOrPost);
				restRequest.AddParameter("password_confirmation", password, ParameterType.GetOrPost);
				IRestResponse response = _restClient.Execute(restRequest);
				backendStatuses = CheckRegisterResponse(response);
			}
			catch (JsonReaderException)
			{
				backendStatuses.Clear();
				backendStatuses.Add(NetworkBackendStatus.ErrorParsingResponse);
			}
			finally
			{
				_dispatcher.InvokeAsync(delegate
				{
					Logger.Debug("Register(): backendStatuses: ");
					foreach (NetworkBackendStatus item in backendStatuses)
					{
						Logger.Debug("    " + item);
					}
					registerCompleted(backendStatuses);
				});
			}
		});
	}

	private List<NetworkBackendStatus> CheckRegisterResponse(IRestResponse response)
	{
		List<NetworkBackendStatus> list = new List<NetworkBackendStatus>();
		list = NetworkBackendClientBase.CheckForCommonStatuses(response);
		if (list.Count > 0)
		{
			return list;
		}
		if (response.StatusCode.Equals((HttpStatusCode)422))
		{
			var anonymousTypeObject = new
			{
				errors = new
				{
					email = new string[1] { "" },
					password = new string[1] { "" },
					name = new string[1] { "" }
				}
			};
			var anon = Deserialise(response.Content, anonymousTypeObject);
			if (anon.errors.email != null && anon.errors.email.Contains("The email field is required."))
			{
				list.Add(NetworkBackendStatus.EmailHasToBeProvided);
			}
			if (anon.errors.email != null && anon.errors.email.Contains("The email must be a valid email address."))
			{
				list.Add(NetworkBackendStatus.ProvidedEmailIsNotValidEmailAddress);
			}
			if (anon.errors.email != null && anon.errors.email.Contains("The email has already been taken."))
			{
				list.Add(NetworkBackendStatus.RegisterEmailIsAlreadyTaken);
			}
			if (anon.errors.password != null && anon.errors.password.Contains("The password field is required."))
			{
				list.Add(NetworkBackendStatus.PasswordHasToBeProvided);
			}
			if (anon.errors.password != null && anon.errors.password.Contains("The password must be at least 6 characters."))
			{
				list.Add(NetworkBackendStatus.RegisterPasswordHasToBeAtLeast6Characters);
			}
			if (anon.errors.name != null && anon.errors.name.Contains("The name field is required."))
			{
				list.Add(NetworkBackendStatus.NameHasToBeProvided);
			}
		}
		return list;
	}

	public Task Login(string email, string password, Action<IEnumerable<NetworkBackendStatus>, string> loginCompleted)
	{
		return Task.Run(delegate
		{
			List<NetworkBackendStatus> backendStatuses = new List<NetworkBackendStatus>();
			string authToken = null;
			try
			{
				string value = email.ToLower();
				IRestRequest restRequest = _restRequestFactory.CreateRestRequest("/login/with/e-mail", Method.POST);
				restRequest.AddParameter("email", value, ParameterType.GetOrPost);
				restRequest.AddParameter("password", password, ParameterType.GetOrPost);
				IRestResponse restResponse = _restClient.Execute(restRequest);
				backendStatuses = NetworkBackendClientBase.CheckForCommonStatuses(restResponse);
				if (backendStatuses.Contains(NetworkBackendStatus.Success))
				{
					var anonymousTypeObject = new
					{
						access_token = ""
					};
					var anon = Deserialise(restResponse.Content, anonymousTypeObject);
					authToken = anon.access_token;
				}
			}
			catch (JsonReaderException)
			{
				backendStatuses.Clear();
				backendStatuses.Add(NetworkBackendStatus.ErrorParsingResponse);
			}
			finally
			{
				_dispatcher.InvokeAsync(delegate
				{
					if (!string.IsNullOrEmpty(authToken))
					{
						_restRequestFactory.AuthToken = authToken;
					}
					Logger.Debug("Login(): AuthToken:" + _restRequestFactory.AuthToken);
					Logger.Debug("Login(): backendStatuses: ");
					foreach (NetworkBackendStatus item in backendStatuses)
					{
						Logger.Debug("    " + item);
					}
					loginCompleted(backendStatuses, _restRequestFactory.AuthToken);
				});
			}
		});
	}

	public Task Logout()
	{
		_restRequestFactory.AuthToken = null;
		UserId = null;
		return Task.Run(delegate
		{
			IRestRequest request = _restRequestFactory.CreateRestRequest("/logout", Method.POST);
			_restClient.Execute(request);
		});
	}

	public Task ResetPassword(string email, Action<IEnumerable<NetworkBackendStatus>> resetPasswordCompleted)
	{
		return Task.Run(delegate
		{
			List<NetworkBackendStatus> backendStatuses = new List<NetworkBackendStatus>();
			try
			{
				string value = email.ToLower();
				IRestRequest restRequest = _restRequestFactory.CreateRestRequest("/password/email", Method.POST);
				restRequest.AddParameter("email", value, ParameterType.GetOrPost);
				IRestResponse response = _restClient.Execute(restRequest);
				backendStatuses = CheckResetPasswordResponse(response);
			}
			catch (JsonReaderException)
			{
				backendStatuses.Clear();
				backendStatuses.Add(NetworkBackendStatus.ErrorParsingResponse);
			}
			finally
			{
				_dispatcher.InvokeAsync(delegate
				{
					resetPasswordCompleted(backendStatuses);
				});
			}
		});
	}

	private List<NetworkBackendStatus> CheckResetPasswordResponse(IRestResponse response)
	{
		List<NetworkBackendStatus> list = new List<NetworkBackendStatus>();
		list = NetworkBackendClientBase.CheckForCommonStatuses(response);
		if (list.Count > 0)
		{
			return list;
		}
		if (response.StatusCode.Equals((HttpStatusCode)422))
		{
			var anonymousTypeObject = new
			{
				errors = new
				{
					email = new string[1] { "" },
					password = new string[1] { "" }
				}
			};
			var anon = Deserialise(response.Content, anonymousTypeObject);
			if (anon.errors.email != null && anon.errors.email.Contains("The email must be a valid email address."))
			{
				list.Add(NetworkBackendStatus.ProvidedEmailIsNotValidEmailAddress);
			}
			else if (anon.errors.email != null && anon.errors.email.Contains("The email field is required."))
			{
				list.Add(NetworkBackendStatus.EmailHasToBeProvided);
			}
		}
		return list;
	}

	public Task Me(Action<IEnumerable<NetworkBackendStatus>, string> meCompleted)
	{
		return Task.Run(delegate
		{
			List<NetworkBackendStatus> backendStatuses = new List<NetworkBackendStatus>();
			string id = null;
			string name = null;
			try
			{
				IRestRequest request = _restRequestFactory.CreateRestRequest("/me", Method.GET);
				IRestResponse restResponse = _restClient.Execute(request);
				backendStatuses = NetworkBackendClientBase.CheckForCommonStatuses(restResponse);
				if (backendStatuses.Contains(NetworkBackendStatus.Success))
				{
					var anonymousTypeObject = new
					{
						id = "",
						email = "",
						name = ""
					};
					var anon = Deserialise(restResponse.Content, anonymousTypeObject);
					id = anon.id;
					name = anon.name;
				}
			}
			catch (JsonReaderException)
			{
				backendStatuses.Clear();
				backendStatuses.Add(NetworkBackendStatus.ErrorParsingResponse);
			}
			finally
			{
				_dispatcher.InvokeAsync(delegate
				{
					if (!string.IsNullOrEmpty(id))
					{
						UserId = id;
					}
					if (!string.IsNullOrEmpty(name))
					{
						UserNickname = name;
					}
					Logger.Debug("Me(): id: " + id);
					Logger.Debug("Me(): name: " + name);
					Logger.Debug("Me(): backendStatuses: ");
					foreach (NetworkBackendStatus item in backendStatuses)
					{
						Logger.Debug("    " + item);
					}
					meCompleted(backendStatuses, id);
				});
			}
		});
	}

	public Task RegisterDeviceForPushNotification(string deviceId, string platform, Action<IEnumerable<NetworkBackendStatus>> registerCompleted)
	{
		return Task.Run(delegate
		{
			List<NetworkBackendStatus> backendStatuses = new List<NetworkBackendStatus>();
			try
			{
				IRestRequest restRequest = _restRequestFactory.CreateRestRequest("/1.0.0/devices", Method.POST);
				restRequest.AddParameter("token", deviceId, ParameterType.GetOrPost);
				restRequest.AddParameter("platform", platform, ParameterType.GetOrPost);
				IRestResponse response = _restClient.Execute(restRequest);
				backendStatuses = NetworkBackendClientBase.CheckForCommonStatuses(response);
			}
			catch (JsonReaderException)
			{
				backendStatuses.Clear();
				backendStatuses.Add(NetworkBackendStatus.ErrorParsingResponse);
			}
			finally
			{
				_dispatcher.InvokeAsync(delegate
				{
					Logger.Debug("RegisterDeviceForPushNotification(): backendStatuses: ");
					foreach (NetworkBackendStatus item in backendStatuses)
					{
						Logger.Debug("    " + item);
					}
					registerCompleted(backendStatuses);
				});
			}
		});
	}

	public Task UnregisterDeviceForPushNotification(string deviceId, string platform, Action<IEnumerable<NetworkBackendStatus>> unregisterCompleted)
	{
		return Task.Run(delegate
		{
			List<NetworkBackendStatus> backendStatuses = new List<NetworkBackendStatus>();
			try
			{
				string data = "{ \"token\": " + deviceId + ", \"platform\": " + platform + "}";
				IRestRequest request = _restRequestFactory.CreateRestRequest("/1.0.0/devices", Method.DELETE, data);
				IRestResponse response = _restClient.Execute(request);
				backendStatuses = NetworkBackendClientBase.CheckForCommonStatuses(response);
			}
			catch (JsonReaderException)
			{
				backendStatuses.Clear();
				backendStatuses.Add(NetworkBackendStatus.ErrorParsingResponse);
			}
			finally
			{
				_dispatcher.InvokeAsync(delegate
				{
					Logger.Debug("UnregisterDeviceForPushNotification(): backendStatuses: ");
					foreach (NetworkBackendStatus item in backendStatuses)
					{
						Logger.Debug("    " + item);
					}
					unregisterCompleted(backendStatuses);
				});
			}
		});
	}
}
