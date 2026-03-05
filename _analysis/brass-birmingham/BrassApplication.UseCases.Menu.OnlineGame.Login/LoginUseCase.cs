using System.Collections.Generic;
using System.Linq;
using BrassApplication.Network;
using BrassApplication.UseCases.Common;

namespace BrassApplication.UseCases.Menu.OnlineGame.Login;

public class LoginUseCase : IUseCase, ILoginInput
{
	private readonly ILoginOutput _presenter;

	private readonly INetworkBackendClient _networkBackendClient;

	private readonly INetworkCredentialsStorage _networkCredentialsStorage;

	public LoginUseCase(ILoginOutput presenter, INetworkBackendClient networkBackendClient, INetworkCredentialsStorage networkCredentialsStorage)
	{
		_presenter = presenter;
		_networkBackendClient = networkBackendClient;
		_networkCredentialsStorage = networkCredentialsStorage;
	}

	public void LoginClicked(string email, string password)
	{
		_presenter.ShowConnectionView();
		_networkBackendClient.UserManagement.Login(email, password, delegate(IEnumerable<NetworkBackendStatus> loginStatusList, string accessToken)
		{
			loginStatusList = loginStatusList.ToList();
			if (loginStatusList.Contains(NetworkBackendStatus.Success))
			{
				_networkBackendClient.UserManagement.Me(delegate(IEnumerable<NetworkBackendStatus> meStatusList, string id)
				{
					meStatusList = meStatusList.ToList();
					_presenter.HideConnectionView();
					if (meStatusList.Contains(NetworkBackendStatus.Success))
					{
						_presenter.ShowOnlineGamesView();
						_networkCredentialsStorage.SaveNetworkCredentials(accessToken);
					}
					else
					{
						NetworkBackendStatus networkBackendStatus = meStatusList.First();
						_presenter.ShowInfoViewWithError(networkBackendStatus.ToString());
					}
				});
			}
			else
			{
				_presenter.HideConnectionView();
				if (loginStatusList.Contains(NetworkBackendStatus.UnauthorizedAndNeedToLoginAgain))
				{
					_presenter.ShowQuestionViewWithError(NetworkBackendStatus.UnauthorizedAndNeedToLoginAgain.ToString());
				}
				else
				{
					_presenter.ShowInfoViewWithError(loginStatusList.First().ToString());
				}
			}
		});
	}

	public void RegisterClicked()
	{
		_presenter.ShowRegisterView();
	}

	public void ForgotPasswordClicked()
	{
		_presenter.ShowForgotPasswordView();
	}

	public void BackClicked()
	{
		_presenter.HideView();
	}
}
