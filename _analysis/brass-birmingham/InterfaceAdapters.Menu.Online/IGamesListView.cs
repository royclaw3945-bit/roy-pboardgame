using System.Collections.Generic;
using InterfaceAdapters.Menu.OnlineGameMenu;

namespace InterfaceAdapters.Menu.Online;

public interface IGamesListView
{
	void UpdateOpenGamesList(List<NetworkGameViewModel> gamesList);

	void UpdatePlayerGamesList(List<NetworkGameViewModel> gamesList);

	void RemoveAllGames();
}
