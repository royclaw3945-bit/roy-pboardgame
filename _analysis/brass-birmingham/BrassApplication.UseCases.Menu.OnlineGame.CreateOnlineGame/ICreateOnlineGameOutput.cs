using System;
using System.Collections.Generic;
using BoardGameRules.Entities.Game;
using BoardGameRules.Entities.Players;
using BrassApplication.Game;

namespace BrassApplication.UseCases.Menu.OnlineGame.CreateOnlineGame;

public interface ICreateOnlineGameOutput
{
	void PresentGameConfiguration(IList<PlayerStartConfiguration> playerStartConfigurations, IList<PlayerMetadata> playersMetadata, int numberOfPlayers, string name, string password, int timePerTurn, Action<PlayerColor, PlayerAvatar> editPortraitAndColor, Action increaseNumberOfPlayers, Action decreaseNumberOfPlayers);

	void ShowChangeTimeView(List<Action> onClickActions);

	void ShowChangePasswordView(Action<string> actionOnConfirm, string password);

	void HideView();

	void HideInputView();

	void ShowConnectionView();

	void HideConnectionView();

	void ShowInfoViewGameCreateSuccessful();

	void ShowInfoViewWithError(string errorKey);

	void ShowChoosePortraitAndColorView(PlayerColor playerColor, PlayerAvatar playerAvatar);

	void LockPasswordIcon(bool isPrivate);

	void UpdateColorInPortraitAndColorView(PlayerColor color);

	void UpdatePortraitInPortraitAndColorView(PlayerAvatar avatar);

	string GetDefaultGameNameTranslation();
}
