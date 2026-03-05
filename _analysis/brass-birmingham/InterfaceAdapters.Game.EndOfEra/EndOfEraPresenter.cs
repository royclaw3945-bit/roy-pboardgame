using BrassApplication.UseCases.Game.EndOfEra;
using InterfaceAdapters.Common;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Utils;

namespace InterfaceAdapters.Game.EndOfEra;

public class EndOfEraPresenter : IPresenter, IEndOfEraOutput
{
	protected readonly IRouter _router;

	protected readonly ILocalization _localization;

	public EndOfEraPresenter(IRouter router, ILocalization localization)
	{
		_router = router;
		_localization = localization;
	}

	public string GetLocalizationScoringIndustries()
	{
		return _localization.GetTranslation("UI/END_OF_ERA_SCORING_INDUSTRIES");
	}

	public string GetLocalizationScoringLinks()
	{
		return _localization.GetTranslation("UI/END_OF_ERA_SCORING_LINKS");
	}

	public string GetLocalizationRemoveIndustries()
	{
		return _localization.GetTranslation("UI/END_OF_ERA_REMOVE_INDUSTRIES");
	}

	public string GetLocalizationScoringMoney()
	{
		return _localization.GetTranslation("UI/END_OF_INTRO_GAME_SCORE_MONEY");
	}

	public string GetLocalizationScoringIncome()
	{
		return _localization.GetTranslation("UI/END_OF_INTRO_GAME_SCORE_INCOME");
	}

	public string GetLocalizationScoringTwoPlusIndustries()
	{
		return _localization.GetTranslation("UI/END_OF_INTRO_GAME_SCORE_LVL_TWO_PLUS_INDUSTRIES");
	}

	public string GetLocalizationEndOfCanalEra()
	{
		return _localization.GetTranslation("UI/END_OF_CANAL_ERA");
	}

	public string GetLocalizationEndOfRailroadEra()
	{
		return _localization.GetTranslation("UI/END_OF_RAIL_ERA");
	}

	public string GetLocalizationEndOfIntroGame()
	{
		return _localization.GetTranslation("UI/END_OF_INTRO_GAME");
	}

	public string GetLocalizationSkipToResults()
	{
		return _localization.GetTranslation("UI/SKIP_TO_RESULTS");
	}

	public string GetLocalizationOk()
	{
		return _localization.GetTranslation("UI/OK");
	}
}
