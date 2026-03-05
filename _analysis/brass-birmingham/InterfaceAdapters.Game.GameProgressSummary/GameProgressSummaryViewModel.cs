using BoardGameRules.Entities.GamePhases;

namespace InterfaceAdapters.Game.GameProgressSummary;

public class GameProgressSummaryViewModel
{
	public GameEra CurrentEra { get; set; }

	public string CurrentEraTranslation { get; set; }

	public int RoundCount { get; set; }

	public int TurnCount { get; set; }

	public int MaxRound { get; set; }

	public int DeckSize { get; set; }

	public int DeckCount { get; set; }

	public int WildIndustryCardCount { get; set; }

	public int WildLocationCardCount { get; set; }
}
