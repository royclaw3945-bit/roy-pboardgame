using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassApplication.Network;

public interface INetworkBackendUserManagement
{
	string UserId { get; }

	string UserNickname { get; }

	Task Register(string email, string nickname, string password, Action<IEnumerable<NetworkBackendStatus>> registerCompleted);

	Task Login(string email, string password, Action<IEnumerable<NetworkBackendStatus>, string> loginCompleted);

	Task Logout();

	Task ResetPassword(string email, Action<IEnumerable<NetworkBackendStatus>> resetPasswordCompleted);

	Task Me(Action<IEnumerable<NetworkBackendStatus>, string> meCompleted);

	Task RegisterDeviceForPushNotification(string deviceId, string platform, Action<IEnumerable<NetworkBackendStatus>> registerCompleted);

	Task UnregisterDeviceForPushNotification(string deviceId, string platform, Action<IEnumerable<NetworkBackendStatus>> unregisterCompleted);
}
