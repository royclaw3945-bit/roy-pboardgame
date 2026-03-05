using Unity.Cloud.UserReporting.Client;
using UnityEngine;

namespace Unity.Cloud.UserReporting.Plugin;

public static class UnityUserReporting
{
	private static UserReportingClient currentClient;

	public static UserReportingClient CurrentClient
	{
		get
		{
			if (currentClient == null)
			{
				Configure();
			}
			return currentClient;
		}
		private set
		{
			currentClient = value;
		}
	}

	public static void Configure(string endpoint, string projectIdentifier, IUserReportingPlatform platform, UserReportingClientConfiguration configuration)
	{
		CurrentClient = new UserReportingClient(endpoint, projectIdentifier, platform, configuration);
	}

	public static void Configure(string endpoint, string projectIdentifier, UserReportingClientConfiguration configuration)
	{
		CurrentClient = new UserReportingClient(endpoint, projectIdentifier, GetPlatform(), configuration);
	}

	public static void Configure(string projectIdentifier, UserReportingClientConfiguration configuration)
	{
		Configure("https://userreporting.cloud.unity3d.com", projectIdentifier, GetPlatform(), configuration);
	}

	public static void Configure(string projectIdentifier)
	{
		Configure("https://userreporting.cloud.unity3d.com", projectIdentifier, GetPlatform(), new UserReportingClientConfiguration());
	}

	public static void Configure()
	{
		Configure("https://userreporting.cloud.unity3d.com", Application.cloudProjectId, GetPlatform(), new UserReportingClientConfiguration());
	}

	public static void Configure(UserReportingClientConfiguration configuration)
	{
		Configure("https://userreporting.cloud.unity3d.com", Application.cloudProjectId, GetPlatform(), configuration);
	}

	public static void Configure(string projectIdentifier, IUserReportingPlatform platform, UserReportingClientConfiguration configuration)
	{
		Configure("https://userreporting.cloud.unity3d.com", projectIdentifier, platform, configuration);
	}

	public static void Configure(IUserReportingPlatform platform, UserReportingClientConfiguration configuration)
	{
		Configure("https://userreporting.cloud.unity3d.com", Application.cloudProjectId, platform, configuration);
	}

	public static void Configure(IUserReportingPlatform platform)
	{
		Configure("https://userreporting.cloud.unity3d.com", Application.cloudProjectId, platform, new UserReportingClientConfiguration());
	}

	private static IUserReportingPlatform GetPlatform()
	{
		return new UnityUserReportingPlatform();
	}

	public static void Use(UserReportingClient client)
	{
		if (client != null)
		{
			CurrentClient = client;
		}
	}
}
