using System.Collections.Generic;
using System.Linq;
using BrassApplication.Network;
using BrassApplication.UseCases.Common;

namespace BrassApplication.UseCases.Menu.MainMenu;

public class MainMenuUseCase : IUseCase, IMainMenuInput
{
	private readonly IMainMenuOutput _presenter;

	private readonly INetworkBackendClient _networkBackendClient;

	private readonly INetworkCredentialsStorage _networkCredentialsStorage;

	public MainMenuUseCase(IMainMenuOutput presenter, INetworkBackendClient networkBackendClient, INetworkCredentialsStorage networkCredentialsStorage)
	{
		_presenter = presenter;
		_networkBackendClient = networkBackendClient;
		_networkCredentialsStorage = networkCredentialsStorage;
	}

	public void ShowOfflineGameView()
	{
		_presenter.ShowOfflineGameView();
	}

	public void StartTutorial()
	{
		_presenter.StartTutorial();
	}

	public void ShowOnlineGamesView()
	{
		_presenter.ShowConnectionView();
		string savedAuthToken = _networkCredentialsStorage.GetSavedAuthToken();
		if (string.IsNullOrEmpty(savedAuthToken))
		{
			_presenter.HideConnectionView();
			_presenter.ShowLoginView();
			return;
		}
		_networkBackendClient.AuthToken = savedAuthToken;
		_networkBackendClient.UserManagement.Me(delegate(IEnumerable<NetworkBackendStatus> meStatusList, string id)
		{
			_presenter.HideConnectionView();
			if (meStatusList.Contains(NetworkBackendStatus.Success))
			{
				_presenter.ShowOnlineGamesView();
			}
			else
			{
				_presenter.ShowLoginView();
			}
		});
	}

	public void ShowTestGamesView()
	{
		_presenter.ShowTestGamesView();
	}

	public void ShowRulebookView()
	{
		_presenter.ShowRulebookView();
	}

	public void ShowSettingsView()
	{
		_presenter.ShowSettingsView();
	}

	public void ShowCreditsView()
	{
		_presenter.ShowCreditsView();
	}

	public void ShowQuitGameQuestion()
	{
		_presenter.ShowQuitGameQuestion();
	}
}
