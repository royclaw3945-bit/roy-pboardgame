namespace BrassApplication.ApplicationSettings;

public interface INetworkFilterSettings
{
	bool GetTwoPlayersFilter();

	bool GetThreePlayersFilter();

	bool GetFourPlayersFilter();

	bool GetOnlyPrivateGamesFilter();

	void SetTwoPlayersFilter(bool isOn);

	void SetThreePlayersFilter(bool isOn);

	void SetFourPlayersFilter(bool isOn);

	void SetOnlyPrivateGamesFilter(bool isOn);
}
