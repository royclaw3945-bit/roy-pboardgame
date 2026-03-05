using BrassApplication.Network;
using UnityEngine;

namespace Frameworks.Network;

public class NetworkCredentialsStorage : INetworkCredentialsStorage
{
	private const string PlayerLogin = "PlayerLogin";

	private const string PlayerPassword = "PlayerPassword";

	private const string PlayerAuthToken = "PlayerAuthToken";

	private const string PlayerId = "PlayerId";

	private const string DeviceToken = "DeviceToken";

	private const string DeviceTokenId = "DeviceTokenId";

	public void SaveNetworkCredentials(string authToken)
	{
		PlayerPrefs.SetString("PlayerAuthToken", authToken);
		PlayerPrefs.Save();
	}

	public void DeleteNetworkCredentials()
	{
		PlayerPrefs.DeleteKey("PlayerAuthToken");
		PlayerPrefs.Save();
	}

	public string GetSavedAuthToken()
	{
		return PlayerPrefs.GetString("PlayerAuthToken");
	}
}
