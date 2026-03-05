using System.Collections.Generic;
using System.Linq;
using BrassApplication.Network;
using BrassApplication.UseCases.Common;

namespace BrassApplication.UseCases.Menu.OnlineGame.Register;

public class RegisterUseCase : IUseCase, IRegisterInput
{
	private readonly IRegisterOutput _presenter;

	private readonly INetworkBackendClient _networkBackendClient;

	public RegisterUseCase(IRegisterOutput presenter, INetworkBackendClient networkBackendClient)
	{
		_presenter = presenter;
		_networkBackendClient = networkBackendClient;
	}

	public void RegisterClicked(string email, string nickname, string password, string confirmPassword)
	{
		if (password.Equals(confirmPassword))
		{
			_presenter.ShowConnectionView();
			_networkBackendClient.UserManagement.Register(email, nickname, password, delegate(IEnumerable<NetworkBackendStatus> statusList)
			{
				_presenter.HideConnectionView();
				if (statusList.Contains(NetworkBackendStatus.Success))
				{
					_presenter.ShowInfoViewRegisterSuccessful();
				}
				else
				{
					_presenter.ShowInfoViewWithError(statusList.First().ToString());
				}
			});
		}
		else
		{
			_presenter.ShowInfoViewWithError("PASSWORDS_DO_NOT_MATCH");
		}
	}

	public void BackClicked()
	{
		_presenter.HideView();
	}
}
