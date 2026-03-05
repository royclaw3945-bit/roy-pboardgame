using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Game;
using BoardGameRules.Entities.Players;
using BoardGameRules.Utils.Extensions;

namespace BrassApplication.Game;

public class GameMetadata : ICloneable
{
	private string _gameId;

	public GameType GameType { get; set; }

	public GameStartConfiguration GameStartConfiguration { get; set; }

	public GameProgressInformation GameProgressInformation { get; set; }

	public List<PlayerMetadata> PlayerMetadata { get; set; }

	public NetworkGameStartConfiguration NetworkGameStartConfiguration { get; set; }

	public NetworkGameProgressInformation NetworkGameProgressInformation { get; set; }

	public string GameId
	{
		get
		{
			return _gameId;
		}
		set
		{
			_gameId = value;
			PlayerMetadata.ForEach(delegate(PlayerMetadata pm)
			{
				if (pm != null)
				{
					pm.GameId = _gameId;
				}
			});
		}
	}

	public GameMetadata(GameStartConfiguration gameStartConfiguration, GameType gameType)
	{
		PlayerMetadata = new List<PlayerMetadata>();
		GameProgressInformation = new GameProgressInformation();
		NetworkGameStartConfiguration = new NetworkGameStartConfiguration();
		NetworkGameProgressInformation = new NetworkGameProgressInformation();
		GameStartConfiguration = gameStartConfiguration;
		GameType = gameType;
	}

	public void JoinGame(PlayerMetadata playerMetadata)
	{
		playerMetadata.GameId = GameId;
		PlayerMetadata.Add(playerMetadata);
	}

	public void LeaveGame(string playerId)
	{
		int num = PlayerMetadata.FindIndex((PlayerMetadata p) => p?.PlayerNetworkId.Equals(playerId) ?? false);
		if (num == -1)
		{
			throw new ArgumentException("Player with id = " + playerId + ", does not exists.");
		}
		PlayerMetadata.RemoveAt(num);
	}

	public PlayerMetadata GetPlayerMetadataByColor(PlayerColor color)
	{
		return PlayerMetadata.Find((PlayerMetadata pm) => pm != null && pm.PlayerStartConfiguration != null && color.Equals(pm.PlayerStartConfiguration.Color));
	}

	public PlayerMetadata GetPlayerMetadataByAvatar(PlayerAvatar avatar)
	{
		return PlayerMetadata.Find((PlayerMetadata pm) => pm != null && avatar.Equals(pm.PlayerAvatar));
	}

	public PlayerMetadata GetPlayerMetadataByNetworkPlayerId(string loggedInPlayerId)
	{
		return PlayerMetadata.Find((PlayerMetadata pm) => pm != null && loggedInPlayerId.Equals(pm.PlayerNetworkId));
	}

	public override bool Equals(object obj)
	{
		if (obj is GameMetadata gameMetadata)
		{
			return Equals(gameMetadata);
		}
		return false;
	}

	public bool Equals(GameMetadata gameMetadata)
	{
		if (gameMetadata == null)
		{
			return false;
		}
		if (GameId.Equals(gameMetadata.GameId))
		{
			return PlayerMetadata.SequenceEqual(gameMetadata.PlayerMetadata);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return GameId.GetHashCode() ^ GameStartConfiguration.GetHashCode() ^ GameProgressInformation.GetHashCode() ^ PlayerMetadata.Count;
	}

	public object Clone()
	{
		GameMetadata gameMetadata = new GameMetadata(GameStartConfiguration, GameType)
		{
			GameProgressInformation = (GameProgressInformation?.Clone() as GameProgressInformation),
			NetworkGameProgressInformation = (NetworkGameProgressInformation.Clone() as NetworkGameProgressInformation),
			NetworkGameStartConfiguration = (NetworkGameStartConfiguration.Clone() as NetworkGameStartConfiguration)
		};
		if (PlayerMetadata != null && PlayerMetadata.Count > 0)
		{
			gameMetadata.PlayerMetadata = PlayerMetadata.Clone() as List<PlayerMetadata>;
		}
		gameMetadata.GameId = GameId;
		return gameMetadata;
	}
}
