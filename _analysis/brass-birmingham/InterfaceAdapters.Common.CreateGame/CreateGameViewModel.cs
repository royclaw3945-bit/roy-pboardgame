using System.Collections.Generic;
using BrassApplication.Game;

namespace InterfaceAdapters.Common.CreateGame;

public class CreateGameViewModel
{
	public int NumberOfPlayers { get; set; }

	public List<PlayerCreateGameViewModel> PlayersViewModels { get; set; }

	public BrassApplicationGame Game { get; set; }

	public bool StartGameButtonIsActive { get; set; }

	public string LocalGameTranslation { get; set; }

	public string CreateGameTranslation { get; set; }

	public string EraTranslation { get; set; }

	public string RoundTranslation { get; set; }

	public string IntroductoryGameTranslation { get; set; }
}
