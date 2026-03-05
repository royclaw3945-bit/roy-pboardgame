using System;
using Unity.Cloud.UserReporting;
using Unity.Cloud.UserReporting.Plugin;
using UnityEngine;

public class UserReportingMonitor : MonoBehaviour
{
	public bool IsEnabled;

	public bool IsEnabledAfterTrigger;

	public bool IsHiddenWithoutDimension;

	public string MonitorName;

	public string Summary;

	public UserReportingMonitor()
	{
		IsEnabled = true;
		IsHiddenWithoutDimension = true;
		Type type = GetType();
		MonitorName = type.Name;
	}

	private void Start()
	{
		if (UnityUserReporting.CurrentClient == null)
		{
			UnityUserReporting.Configure();
		}
	}

	public void Trigger()
	{
		if (!IsEnabledAfterTrigger)
		{
			IsEnabled = false;
		}
		UnityUserReporting.CurrentClient.TakeScreenshot(2048, 2048, delegate
		{
		});
		UnityUserReporting.CurrentClient.TakeScreenshot(512, 512, delegate
		{
		});
		UnityUserReporting.CurrentClient.CreateUserReport(delegate(UserReport br)
		{
			if (string.IsNullOrEmpty(br.ProjectIdentifier))
			{
				Debug.LogWarning("The user report's project identifier is not set. Please setup cloud services using the Services tab or manually specify a project identifier when calling UnityUserReporting.Configure().");
			}
			br.Summary = Summary;
			br.DeviceMetadata.Add(new UserReportNamedValue("Monitor", MonitorName));
			string arg = "Unknown";
			string arg2 = "0.0";
			foreach (UserReportNamedValue deviceMetadatum in br.DeviceMetadata)
			{
				if (deviceMetadatum.Name == "Platform")
				{
					arg = deviceMetadatum.Value;
				}
				if (deviceMetadatum.Name == "Version")
				{
					arg2 = deviceMetadatum.Value;
				}
			}
			br.Dimensions.Add(new UserReportNamedValue("Monitor.Platform.Version", $"{MonitorName}.{arg}.{arg2}"));
			br.Dimensions.Add(new UserReportNamedValue("Monitor", MonitorName));
			br.IsHiddenWithoutDimension = IsHiddenWithoutDimension;
			UnityUserReporting.CurrentClient.SendUserReport(br, delegate
			{
				Triggered();
			});
		});
	}

	protected virtual void Triggered()
	{
	}
}
