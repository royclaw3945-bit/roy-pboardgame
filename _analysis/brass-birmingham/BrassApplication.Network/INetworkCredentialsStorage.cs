namespace BrassApplication.Network;

public interface INetworkCredentialsStorage
{
	void SaveNetworkCredentials(string authToken);

	void DeleteNetworkCredentials();

	string GetSavedAuthToken();
}
