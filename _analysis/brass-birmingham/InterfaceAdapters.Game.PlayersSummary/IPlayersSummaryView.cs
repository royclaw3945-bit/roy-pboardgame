using System.Collections.Generic;
using BoardGameRules.Entities.GamePhases.Events;
using BoardGameRules.Entities.Players;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.PlayersSummary;

public interface IPlayersSummaryView : IBaseView
{
	void UpdatePlayersSummaryView(List<PlayerViewModel> playerViewModels);

	IAnimation CreateShowCurrentPlayerAnimation(PlayerColor curPlayerColor);

	IAnimation CreateShuffleNewPlayersOrderAnimation(RoundStarted roundStartedEvent);

	IAnimation CreateEndOfRoundAnimation(RoundEnded roundEndedEvent);

	IAnimation CreateChangeMoneyAnimation(string newAmount, PlayerColor playerColor);

	IAnimation CreateChangeMoneySpentAnimation(string newAmount, PlayerColor playerColor, string newMoneySpent);

	IAnimation CreateChangeVPAnimation(string newAmount, PlayerColor playerColor);

	IAnimation CreateChangeIncomeAnimation(string newIncomeLevel, string newIncomeMarker, PlayerColor playerColor);

	IAnimation CreateVictoryPointsForMoneyAnimation(int amount, PlayerColor playerColor);

	IAnimation CreateVictoryPointsForIncomeAnimation(int amount, PlayerColor playerColor);
}
