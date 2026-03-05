using System.Collections.Generic;
using BoardGameRules.Entities.Players;

namespace InterfaceAdapters.Game.GameProgressDetails;

public class GameProgressDetailsViewModel
{
	public int DeckCount { get; set; }

	public int WildLocationCardsCount { get; set; }

	public int WildIndustryCardsCount { get; set; }

	public PlayerColor CurPlayerColor { get; set; }

	public Dictionary<PlayerColor, int> IncomePositions { get; set; }

	public Dictionary<PlayerColor, int> IncomeLevels { get; set; }

	public Dictionary<PlayerColor, PlayerAvatar> PlayerAvatars { get; set; }
}
