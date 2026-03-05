using System;
using System.Collections.ObjectModel;
using BrassApplication.Game;

namespace BrassApplication.UseCases.Menu.OnlineGame.ListPlayerOnlineGamesMenu;

public interface IOnlineGameMenuOutput
{
	void UpdatePlayerGamesView(ReadOnlyCollection<GameMetadata> playerGames, string userNickname, string userId, Action<string> launchGameClicked, Action<string> leaveGameClicked);

	void UpdateOpenGamesView(ReadOnlyCollection<GameMetadata> openGames, Action<string> joinGameClicked);

	void HideView();

	void ShowLogOutPopup();

	void ShowCreateOnlineGameView();

	void ShowConnectionView();

	void HideConnectionView();

	void ShowInfoViewWithError(string errorKey);

	void LaunchGame(BrassApplicationGame game);

	void ShowJoinOnlineGameView(GameMetadata gameMetadata);

	void UpdateFiltersVisuals(bool show2Players, bool show3Players, bool show4Players, bool showPrivate);
}
