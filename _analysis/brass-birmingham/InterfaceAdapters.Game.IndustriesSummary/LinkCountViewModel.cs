using BoardGameRules.Entities.GamePhases;
using BoardGameRules.Entities.Players;

namespace InterfaceAdapters.Game.IndustriesSummary;

public class LinkCountViewModel
{
	public int LinkCount { get; set; }

	public PlayerColor Color { get; set; }

	public GameEra GameEra { get; set; }
}
