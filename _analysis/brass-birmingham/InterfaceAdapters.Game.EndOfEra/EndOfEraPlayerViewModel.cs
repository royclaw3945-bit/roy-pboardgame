using BoardGameRules.Entities.Players;

namespace InterfaceAdapters.Game.EndOfEra;

public class EndOfEraPlayerViewModel
{
	public PlayerColor PlayerColor { get; set; }

	public PlayerAvatar PlayerAvatar { get; set; }

	public int LinksVP { get; set; }

	public int IndustriesVP { get; set; }

	public int BonusesVP { get; set; }

	public int SumVP { get; set; }

	public string PlayerName { get; set; }
}
