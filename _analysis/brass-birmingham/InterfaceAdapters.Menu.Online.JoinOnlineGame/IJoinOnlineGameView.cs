using BrassApplication.Game;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Menu.Online.JoinOnlineGame;

public interface IJoinOnlineGameView : IBaseView
{
	void SetGameToJoin(GameMetadata gameMetadata);

	void ShowCurrentGameState(JoinOnlineGameViewModel viewModel);
}
