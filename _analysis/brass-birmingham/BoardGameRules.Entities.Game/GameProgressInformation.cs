using System;
using System.Collections.Generic;
using BoardGameRules.Entities.GamePhases;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.Game;

public class GameProgressInformation : ICloneable
{
	public GameEra CurrentGameEra { get; set; }

	public Player CurrentPlayer { get; set; }

	public bool IsGameOver { get; set; }

	public int CurrentTurnNumber { get; set; }

	public int CurrentRoundNumber { get; set; }

	public List<PlayerColor> EndGameRanking { get; set; }

	public Dictionary<PlayerColor, (int vpFromLinksInLastEra, int vpFromIndustriesInLastEra, int vpFromBonusesInLastEra, int totalVP, int place)> EndGamePoints { get; set; }

	public Dictionary<PlayerColor, (int vpFromMoney, int vpFromIncome, int vpFromHigherIndustries)> IntroductoryPoints { get; set; }

	public object Clone()
	{
		return new GameProgressInformation
		{
			CurrentGameEra = CurrentGameEra,
			CurrentPlayer = (CurrentPlayer?.Clone() as Player),
			IsGameOver = IsGameOver,
			CurrentTurnNumber = CurrentTurnNumber,
			CurrentRoundNumber = CurrentRoundNumber,
			EndGameRanking = EndGameRanking,
			EndGamePoints = EndGamePoints,
			IntroductoryPoints = IntroductoryPoints
		};
	}
}
