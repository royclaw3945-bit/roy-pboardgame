namespace BrassApplication.UseCases.Game.EndOfEra;

public interface IEndOfEraOutput
{
	string GetLocalizationScoringIndustries();

	string GetLocalizationScoringLinks();

	string GetLocalizationRemoveIndustries();

	string GetLocalizationScoringMoney();

	string GetLocalizationScoringIncome();

	string GetLocalizationScoringTwoPlusIndustries();

	string GetLocalizationEndOfCanalEra();

	string GetLocalizationEndOfRailroadEra();

	string GetLocalizationEndOfIntroGame();

	string GetLocalizationSkipToResults();

	string GetLocalizationOk();
}
