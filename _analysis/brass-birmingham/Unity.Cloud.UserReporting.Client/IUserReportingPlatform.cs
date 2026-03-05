using System;
using System.Collections.Generic;

namespace Unity.Cloud.UserReporting.Client;

public interface IUserReportingPlatform
{
	T DeserializeJson<T>(string json);

	IDictionary<string, string> GetDeviceMetadata();

	void ModifyUserReport(UserReport userReport);

	void OnEndOfFrame(UserReportingClient client);

	void Post(string endpoint, string contentType, byte[] content, Action<float, float> progressCallback, Action<bool, byte[]> callback);

	void RunTask(Func<object> task, Action<object> callback);

	void SendAnalyticsEvent(string eventName, Dictionary<string, object> eventData);

	string SerializeJson(object instance);

	void TakeScreenshot(int frameNumber, int maximumWidth, int maximumHeight, object source, Action<int, byte[]> callback);

	void Update(UserReportingClient client);
}
