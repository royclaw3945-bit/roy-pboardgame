using System.Collections.Generic;
using BoardGameRules.Entities.Players;
using BrassApplication.UseCases.Game.EndOfEra;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.EndOfEra;

public class EndOfEraController : IController
{
	private readonly IEndOfEraInput _useCase;

	public EndOfEraController(IEndOfEraInput useCase)
	{
		_useCase = useCase;
	}

	public void OnViewCreated()
	{
	}

	public void OnViewDestroyed()
	{
	}

	public Dictionary<PlayerColor, PlayerAvatar> GetPlayerAvatars()
	{
		return _useCase.GetPlayersAvatars();
	}

	public Dictionary<PlayerColor, string> GetPlayerNames()
	{
		return _useCase.GetPlayersNames();
	}

	public string GetLocalizationScoringIndustries()
	{
		return _useCase.GetLocalizationScoringIndustries();
	}

	public string GetLocalizationScoringLinks()
	{
		return _useCase.GetLocalizationScoringLinks();
	}

	public string GetLocalizationRemoveIndustries()
	{
		return _useCase.GetLocalizationRemoveIndustries();
	}

	public string GetLocalizationScoringMoney()
	{
		return _useCase.GetLocalizationScoringMoney();
	}

	public string GetLocalizationScoringIncome()
	{
		return _useCase.GetLocalizationScoringIncome();
	}

	public string GetLocalizationScoringTwoPlusIndustries()
	{
		return _useCase.GetLocalizationScoringTwoPlusIndustries();
	}

	public string GetLocalizationEndOfCanalEra()
	{
		return _useCase.GetLocalizationEndOfCanalEra();
	}

	public string GetLocalizationEndOfRailroadEra()
	{
		return _useCase.GetLocalizationEndOfRailroadEra();
	}

	public string GetLocalizationEndOfIntroGame()
	{
		return _useCase.GetLocalizationEndOfIntroGame();
	}

	public string GetLocalizationSkipToResults()
	{
		return _useCase.GetLocalizationSkipToResults();
	}

	public string GetLocalizationOk()
	{
		return _useCase.GetLocalizationOk();
	}
}
