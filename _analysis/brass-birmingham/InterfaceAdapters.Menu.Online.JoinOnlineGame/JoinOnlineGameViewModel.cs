using System.Collections.Generic;
using BrassApplication.Game;
using InterfaceAdapters.Menu.Online.CreateOnlineGame;

namespace InterfaceAdapters.Menu.Online.JoinOnlineGame;

public class JoinOnlineGameViewModel
{
	public int NumberOfPlayers { get; set; }

	public List<PlayerOnlineGameViewModel> PlayersViewModels { get; set; }

	public BrassApplicationGame Game { get; set; }

	public string Name { get; set; }

	public string Password { get; set; }

	public string TimePerTurn { get; set; }
}
