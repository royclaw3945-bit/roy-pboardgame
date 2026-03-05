using System;
using System.Collections.Generic;
using BoardGameRules.Entities.Common;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.Game;

public class GameStartConfiguration
{
	public bool IsIntroductoryGame { get; set; }

	public bool IsTutorialGame { get; set; }

	public int Seed { get; set; }

	public IList<PlayerStartConfiguration> Players { get; set; }

	public NumberOfPlayers NumberOfPlayers => (NumberOfPlayers)Players.Count;

	public int NumberOfPlayersAsInt => (int)NumberOfPlayers;

	public GameStartConfiguration(NumberOfPlayers numberOfPlayers)
	{
		Seed = new Random().Next();
		IsIntroductoryGame = false;
		IsTutorialGame = false;
		Players = new List<PlayerStartConfiguration>();
		for (int i = 0; i < (int)numberOfPlayers; i++)
		{
			Players.Add(new PlayerStartConfiguration((PlayerColor)i, i));
		}
	}
}
