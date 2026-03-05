using System;
using System.Collections.Generic;
using BoardGameRules.Entities.Players;
using BrassApplication.Game;

namespace BrassApplication.UseCases.Menu.OnlineGame.JoinOnlineGame;

public interface IJoinOnlineGameOutput
{
	void PresentGameConfiguration(GameMetadata gameMetadata, Action<int> actionOnClick);

	void HideView();

	void ShowConnectionView();

	void HideConnectionView();

	void ShowInfoViewGameJoinSuccessful();

	void ShowInfoViewWithError(string errorKey);

	void ShowPasswordView(Action<int, string> joinGame, int slot);

	void ShowChoosePortraitAndJoinView(Action<int, PlayerAvatar> joinGameClicked, IEnumerable<PlayerAvatar> availableAvatars, int slot, PlayerColor color);
}
