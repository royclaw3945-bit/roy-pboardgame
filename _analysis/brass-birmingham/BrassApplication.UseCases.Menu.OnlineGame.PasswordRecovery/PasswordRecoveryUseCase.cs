using System.Collections.Generic;
using System.Linq;
using BrassApplication.Network;
using BrassApplication.UseCases.Common;

namespace BrassApplication.UseCases.Menu.OnlineGame.PasswordRecovery;

public class PasswordRecoveryUseCase : IUseCase, IPasswordRecoveryInput
{
	private readonly IPasswordRecoveryOutput _presenter;

	private readonly INetworkBackendClient _networkBackendClient;

	public PasswordRecoveryUseCase(IPasswordRecoveryOutput presenter, INetworkBackendClient networkBackendClient)
	{
		_presenter = presenter;
		_networkBackendClient = networkBackendClient;
	}

	public void SendEmailClicked(string email)
	{
		_presenter.ShowConnectionView();
		_networkBackendClient.UserManagement.ResetPassword(email, delegate(IEnumerable<NetworkBackendStatus> resetStatusList)
		{
			_presenter.HideConnectionView();
			if (resetStatusList.Contains(NetworkBackendStatus.Success))
			{
				_presenter.ShowInfoViewEmailSendSuccessfully();
			}
			else
			{
				_presenter.ShowInfoViewWithError(resetStatusList.First().ToString());
			}
		});
	}

	public void BackClicked()
	{
		_presenter.HideView();
	}
}
