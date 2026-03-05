using System.Collections.Generic;
using BoardGameRules.Entities.Players;
using BrassApplication.Game;
using BrassApplication.UseCases.Game.Replay;

namespace BrassApplication.UseCases.Game.FinishedGameReview;

public interface IFinishedGameReviewOutput
{
	void ReplayUnseenActions(PlayerMetadata currentPlayerMetadata, IReplayDelegate replayDelegate);

	void ShowSkipToMyNextTurnView();

	void HidePlayerActionView();

	void ShowGameSummaryTable(List<(PlayerColor color, PlayerAvatar avatar, string playerName, int vpFromCanalEra, int vpFromLinksInLastEra, int vpFromIndustriesInLastEra, int vpFromBonusesInLastEra, int totalVP, int place)> endOfGamePlayersScore);

	void ShowIntroductoryGameSummaryTable(List<(PlayerColor color, PlayerAvatar avatar, string playerName, int vpFromLinksInLastEra, int vpFromIndustriesInLastEra, int vpFromBonusesInLastEra, int totalVP, int place, int vpFromMoney, int vpFromIncome, int vpFromHigherIndustries)> endOfIntroductoryGamePlayersScore);

	void ShowButtonToLaunchGameSummaryTable();
}
