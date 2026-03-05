using System;
using System.Collections.Generic;
using BoardGameRules.Entities.Players;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.EndOfGame;

public interface IEndOfGameView : IBaseView
{
	void UpdateEndOfGameView(List<(PlayerColor color, PlayerAvatar avatar, string playerName, int vpFromCanalEra, int vpFromLinksInLastEra, int vpFromIndustriesInLastEra, int vpFromBonusesInLastEra, int totalVP, int place)> endOfGamePlayersScore);

	void UpdateEndOfIntroductoryGameView(List<(PlayerColor color, PlayerAvatar avatar, string playerName, int vpFromLinksInLastEra, int vpFromIndustriesInLastEra, int vpFromBonusesInLastEra, int totalVP, int place, int vpFromMoney, int vpFromIncome, int vpFromHigherIndustries)> endOfGamePlayersScore);

	void SetOnReviewButtonClicked(Action onReviewButtonClicked);

	void SetOnExitGameButtonClicked(Action onReviewButtonClicked);
}
