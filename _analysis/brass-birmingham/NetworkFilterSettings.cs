using BrassApplication.ApplicationSettings;
using UnityEngine;

public class NetworkFilterSettings : INetworkFilterSettings
{
	public const string TWO_PLAYERS_FILTER_PLAYERPREF_KEY = "TwoPlayersFilter";

	public const string THREE_PLAYERS_FILTER_PLAYERPREF_KEY = "ThreePlayersFilter";

	public const string FOUR_PLAYERS_FILTER_PLAYERPREF_KEY = "FourPlayersFilter";

	public const string ONLY_PRIVATE_GAMES_FILTER_PLAYERPREF_KEY = "OnlyPrivateGamesFilter";

	public bool GetTwoPlayersFilter()
	{
		if (PlayerPrefs.HasKey("TwoPlayersFilter"))
		{
			return PlayerPrefs.GetInt("TwoPlayersFilter") == 1;
		}
		return true;
	}

	public void SetTwoPlayersFilter(bool isOn)
	{
		PlayerPrefs.SetInt("TwoPlayersFilter", isOn ? 1 : 0);
	}

	public bool GetThreePlayersFilter()
	{
		if (PlayerPrefs.HasKey("ThreePlayersFilter"))
		{
			return PlayerPrefs.GetInt("ThreePlayersFilter") == 1;
		}
		return true;
	}

	public void SetThreePlayersFilter(bool isOn)
	{
		PlayerPrefs.SetInt("ThreePlayersFilter", isOn ? 1 : 0);
	}

	public bool GetFourPlayersFilter()
	{
		if (PlayerPrefs.HasKey("FourPlayersFilter"))
		{
			return PlayerPrefs.GetInt("FourPlayersFilter") == 1;
		}
		return true;
	}

	public void SetFourPlayersFilter(bool isOn)
	{
		PlayerPrefs.SetInt("FourPlayersFilter", isOn ? 1 : 0);
	}

	public bool GetOnlyPrivateGamesFilter()
	{
		if (PlayerPrefs.HasKey("OnlyPrivateGamesFilter"))
		{
			return PlayerPrefs.GetInt("OnlyPrivateGamesFilter") == 1;
		}
		return false;
	}

	public void SetOnlyPrivateGamesFilter(bool isOn)
	{
		PlayerPrefs.SetInt("OnlyPrivateGamesFilter", isOn ? 1 : 0);
	}
}
