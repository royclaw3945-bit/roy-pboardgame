using System.Collections.Generic;
using BrassApplication.Game;

namespace InterfaceAdapters.Menu.Online.CreateOnlineGame;

public class CreateOnlineGameViewModel
{
	public int NumberOfPlayers { get; set; }

	public List<PlayerOnlineGameViewModel> PlayersViewModels { get; set; }

	public BrassApplicationGame Game { get; set; }

	public string Name { get; set; }

	public string Password { get; set; }

	public string TimePerTurn { get; set; }
}
