using System.Collections.Generic;
using BoardGameRules.Entities.Players;

namespace BrassApplication.UseCases.Game.EndOfEra;

public interface IEndOfEraInput
{
	string GetLocalizationScoringIndustries();

	string GetLocalizationScoringLinks();

	string GetLocalizationRemoveIndustries();

	string GetLocalizationScoringMoney();

	string GetLocalizationScoringIncome();

	string GetLocalizationScoringTwoPlusIndustries();

	Dictionary<PlayerColor, PlayerAvatar> GetPlayersAvatars();

	Dictionary<PlayerColor, string> GetPlayersNames();

	string GetLocalizationEndOfCanalEra();

	string GetLocalizationEndOfRailroadEra();

	string GetLocalizationEndOfIntroGame();

	string GetLocalizationSkipToResults();

	string GetLocalizationOk();
}
