using Unity.Cloud.UserReporting.Plugin.SimpleJson;

namespace Unity.Cloud.UserReporting.Plugin;

public static class UnityUserReportParser
{
	public static UserReport ParseUserReport(string json)
	{
		return Unity.Cloud.UserReporting.Plugin.SimpleJson.SimpleJson.DeserializeObject<UserReport>(json);
	}

	public static UserReportList ParseUserReportList(string json)
	{
		return Unity.Cloud.UserReporting.Plugin.SimpleJson.SimpleJson.DeserializeObject<UserReportList>(json);
	}
}
