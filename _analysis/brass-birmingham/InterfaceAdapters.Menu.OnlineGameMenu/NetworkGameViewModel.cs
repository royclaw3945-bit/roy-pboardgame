using System;
using System.Collections.Generic;
using BoardGameRules.Entities.Game;
using BoardGameRules.Entities.Players;
using BrassApplication.Game;

namespace InterfaceAdapters.Menu.OnlineGameMenu;

public class NetworkGameViewModel
{
	public string GameId { get; set; }

	public string Name { get; set; }

	public string PlayerCountText { get; set; }

	public int MaxPlayerCount { get; set; }

	public int CurrentPlayerCount { get; set; }

	public int TurnTime { get; set; }

	public bool IsPrivate { get; set; }

	public Action<string> JoinGameAction { get; set; } = delegate
	{
	};

	public Action<string> LeaveGameAction { get; set; } = delegate
	{
	};

	public Action<string> LaunchGameAction { get; set; } = delegate
	{
	};

	public IList<PlayerStartConfiguration> PlayersStartConfigurations { get; set; }

	public IList<PlayerMetadata> JoinedPlayersMetadata { get; set; }

	public bool IsLoggedPlayerTurn { get; set; }

	public NetworkGameProgressInformation.GameStatus GameStatus { get; set; }

	public string Era { get; set; }

	public string Round { get; set; }

	public PlayerColor? CurrentPlayerColor { get; set; }

	public int TimeLeft { get; set; }

	public string TimeLeftText { get; set; }
}
