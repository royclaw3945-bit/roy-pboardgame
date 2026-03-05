using System.Collections.Generic;

namespace InterfaceAdapters.Game.EndOfEra;

public class EndOfEraViewModel
{
	public List<EndOfEraPlayerViewModel> PlayersStats { get; set; }

	public string TitleText { get; set; }
}
