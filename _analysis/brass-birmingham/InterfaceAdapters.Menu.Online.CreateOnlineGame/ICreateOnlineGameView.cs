using System;
using System.Collections.Generic;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Menu.Online.CreateOnlineGame;

public interface ICreateOnlineGameView : IBaseView
{
	void ShowCurrentGameState(CreateOnlineGameViewModel viewModel);

	void LockPasswordIcon(bool isPrivate);

	void UpdateTimePanel(List<Action> onClickActions);
}
