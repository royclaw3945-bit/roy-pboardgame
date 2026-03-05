using System.Collections.Generic;

namespace InterfaceAdapters.Menu.OnlineGameMenu;

public class ListPlayerOnlineGamesMenuViewModel
{
	public string Nickname { get; set; }

	public List<NetworkGameViewModel> PlayerGamesList { get; set; }

	public List<NetworkGameViewModel> OpenGamesList { get; set; }
}
