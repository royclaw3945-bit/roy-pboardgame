using System;
using BoardGameRules.Entities.Players;
using BrassApplication.AI;
using BrassApplication.ApplicationSettings;
using BrassApplication.Game;
using UnityEngine;
using Utils;

namespace Frameworks.Settings;

public class GameSettings : IGameSettings
{
	public const string MAP_PLAYERPREF_KEY = "IsNightMap";

	public const string PLAYER_METADATA_PLAYERPREF_KEY = "PlayerMetadata";

	public const string NUMBER_OF_PLAYERS_PLAYERPREF_KEY = "NumberOfPlayers";

	public const string AITYPE_PLAYERPREF_KEY = "PlayerMetadataAIType";

	public const string PLAYER_AVATAR_PLAYERPREF_KEY = "PlayerMetadataPlayerAvatar";

	public const string PLAYER_COLOR_PLAYERPREF_KEY = "PlayerMetadataColor";

	public bool CameFromMenu()
	{
		return Global.Instance.CameFromMenu;
	}

	public bool GetIsNightMap()
	{
		return PlayerPrefs.GetInt("IsNightMap") == 1;
	}

	public void SetMap(bool isNightMap)
	{
		PlayerPrefs.SetInt("IsNightMap", isNightMap ? 1 : 0);
	}

	public void SetDefaultGameSettings(GameMetadata gameMetadata, int numberOfPLayers)
	{
		PlayerPrefs.SetInt("NumberOfPlayers", numberOfPLayers);
		for (int i = 0; i < gameMetadata.PlayerMetadata.Count; i++)
		{
			PlayerMetadata playerMetadata = gameMetadata.PlayerMetadata[i];
			PlayerPrefs.SetString("PlayerMetadataAIType" + i, playerMetadata.AiType.ToString());
			PlayerPrefs.SetString("PlayerMetadataPlayerAvatar" + i, playerMetadata.PlayerAvatar.ToString());
			if (playerMetadata.PlayerStartConfiguration != null)
			{
				PlayerPrefs.SetString("PlayerMetadataColor" + i, playerMetadata.PlayerStartConfiguration.Color.ToString());
			}
		}
		PlayerPrefs.Save();
	}

	public int GetNumberOfPlayers()
	{
		return PlayerPrefs.GetInt("NumberOfPlayers");
	}

	public bool IsSavedDefaultGameSettings()
	{
		return PlayerPrefs.HasKey("PlayerMetadataAIType" + 0);
	}

	public PlayerColor GetPlayerIndexSavedColor(int index)
	{
		return (PlayerColor)Enum.Parse(typeof(PlayerColor), PlayerPrefs.GetString("PlayerMetadataColor" + index), ignoreCase: true);
	}

	public PlayerAvatar GetPlayerIndexSavedAvatar(int index)
	{
		return (PlayerAvatar)Enum.Parse(typeof(PlayerAvatar), PlayerPrefs.GetString("PlayerMetadataPlayerAvatar" + index), ignoreCase: true);
	}

	public AIType GetPlayerIndexSavedAIType(int index)
	{
		return (AIType)Enum.Parse(typeof(AIType), PlayerPrefs.GetString("PlayerMetadataAIType" + index), ignoreCase: true);
	}

	private AIType GetAITypeByString(string aiTypeString)
	{
		return aiTypeString switch
		{
			"Human" => AIType.Human, 
			"Easy" => AIType.Easy, 
			"Medium" => AIType.Medium, 
			"Hard" => AIType.Hard, 
			"Tutorial" => AIType.Tutorial, 
			_ => AIType.Human, 
		};
	}

	private PlayerAvatar GetPlayerAvatarByString(string playerAvatarString)
	{
		return playerAvatarString switch
		{
			"Arkwright" => PlayerAvatar.Arkwright, 
			"Bessemer" => PlayerAvatar.Bessemer, 
			"Brunel" => PlayerAvatar.Brunel, 
			"Coade" => PlayerAvatar.Coade, 
			"Owen" => PlayerAvatar.Owen, 
			"Rothschild" => PlayerAvatar.Rothschild, 
			"Stephenson" => PlayerAvatar.Stephenson, 
			"Tinsley" => PlayerAvatar.Tinsley, 
			"Watt" => PlayerAvatar.Watt, 
			_ => PlayerAvatar.Arkwright, 
		};
	}
}
