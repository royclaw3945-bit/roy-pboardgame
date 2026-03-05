using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Players;
using BrassApplication.Game;
using BrassApplication.GameHistory;
using BrassApplication.UseCases.Common;

namespace BrassApplication.UseCases.Game.EndOfEra;

public class EndOfEraUseCase : IUseCase, IEndOfEraInput
{
	private readonly IEndOfEraOutput _presenter;

	private IGameHistory _gameHistory;

	public EndOfEraUseCase(IEndOfEraOutput presenter, IGameHistory gameHistory)
	{
		_presenter = presenter;
		_gameHistory = gameHistory;
	}

	public string GetLocalizationScoringIndustries()
	{
		return _presenter.GetLocalizationScoringIndustries();
	}

	public string GetLocalizationScoringLinks()
	{
		return _presenter.GetLocalizationScoringLinks();
	}

	public string GetLocalizationRemoveIndustries()
	{
		return _presenter.GetLocalizationRemoveIndustries();
	}

	public string GetLocalizationScoringMoney()
	{
		return _presenter.GetLocalizationScoringMoney();
	}

	public string GetLocalizationScoringIncome()
	{
		return _presenter.GetLocalizationScoringIncome();
	}

	public string GetLocalizationScoringTwoPlusIndustries()
	{
		return _presenter.GetLocalizationScoringTwoPlusIndustries();
	}

	public string GetLocalizationEndOfCanalEra()
	{
		return _presenter.GetLocalizationEndOfCanalEra();
	}

	public string GetLocalizationEndOfRailroadEra()
	{
		return _presenter.GetLocalizationEndOfRailroadEra();
	}

	public string GetLocalizationEndOfIntroGame()
	{
		return _presenter.GetLocalizationEndOfIntroGame();
	}

	public string GetLocalizationSkipToResults()
	{
		return _presenter.GetLocalizationSkipToResults();
	}

	public string GetLocalizationOk()
	{
		return _presenter.GetLocalizationOk();
	}

	public Dictionary<PlayerColor, PlayerAvatar> GetPlayersAvatars()
	{
		return _gameHistory.CurrentGame.Metadata.PlayerMetadata.ToDictionary((PlayerMetadata p) => p.PlayerStartConfiguration.Color, (PlayerMetadata p) => p.PlayerAvatar);
	}

	public Dictionary<PlayerColor, string> GetPlayersNames()
	{
		return _gameHistory.CurrentGame.Metadata.PlayerMetadata.ToDictionary((PlayerMetadata p) => p.PlayerStartConfiguration.Color, (PlayerMetadata p) => p.NickName);
	}
}
