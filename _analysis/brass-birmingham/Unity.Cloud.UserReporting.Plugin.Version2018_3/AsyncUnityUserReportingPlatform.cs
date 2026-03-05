using System;
using System.Collections.Generic;
using System.Globalization;
using Unity.Cloud.UserReporting.Client;
using Unity.Cloud.UserReporting.Plugin.SimpleJson;
using Unity.Screenshots;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Networking;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;

namespace Unity.Cloud.UserReporting.Plugin.Version2018_3;

public class AsyncUnityUserReportingPlatform : IUserReportingPlatform
{
	private struct LogMessage
	{
		public string LogString;

		public LogType LogType;

		public string StackTrace;
	}

	private class PostOperation
	{
		public Action<bool, byte[]> Callback { get; set; }

		public Action<float, float> ProgressCallback { get; set; }

		public UnityWebRequest WebRequest { get; set; }
	}

	private struct ProfilerSampler
	{
		public string Name;

		public Recorder Recorder;

		public double GetValue()
		{
			if (Recorder == null)
			{
				return 0.0;
			}
			return (double)Recorder.elapsedNanoseconds / 1000000.0;
		}
	}

	private List<LogMessage> logMessages;

	private List<PostOperation> postOperations;

	private List<ProfilerSampler> profilerSamplers;

	private ScreenshotManager screenshotManager;

	private List<PostOperation> taskOperations;

	public AsyncUnityUserReportingPlatform()
	{
		logMessages = new List<LogMessage>();
		postOperations = new List<PostOperation>();
		screenshotManager = new ScreenshotManager();
		profilerSamplers = new List<ProfilerSampler>();
		foreach (KeyValuePair<string, string> samplerName in GetSamplerNames())
		{
			Sampler sampler = Sampler.Get(samplerName.Key);
			if (sampler.isValid)
			{
				Recorder recorder = sampler.GetRecorder();
				recorder.enabled = true;
				ProfilerSampler item = new ProfilerSampler
				{
					Name = samplerName.Value,
					Recorder = recorder
				};
				profilerSamplers.Add(item);
			}
		}
		Application.logMessageReceivedThreaded += delegate(string logString, string stackTrace, LogType logType)
		{
			lock (logMessages)
			{
				LogMessage item2 = new LogMessage
				{
					LogString = logString,
					StackTrace = stackTrace,
					LogType = logType
				};
				logMessages.Add(item2);
			}
		};
	}

	public T DeserializeJson<T>(string json)
	{
		return Unity.Cloud.UserReporting.Plugin.SimpleJson.SimpleJson.DeserializeObject<T>(json);
	}

	public void OnEndOfFrame(UserReportingClient client)
	{
		screenshotManager.OnEndOfFrame();
	}

	public void Post(string endpoint, string contentType, byte[] content, Action<float, float> progressCallback, Action<bool, byte[]> callback)
	{
		UnityWebRequest unityWebRequest = new UnityWebRequest(endpoint, "POST");
		unityWebRequest.uploadHandler = new UploadHandlerRaw(content);
		unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
		unityWebRequest.SetRequestHeader("Content-Type", contentType);
		unityWebRequest.SendWebRequest();
		PostOperation postOperation = new PostOperation();
		postOperation.WebRequest = unityWebRequest;
		postOperation.Callback = callback;
		postOperation.ProgressCallback = progressCallback;
		postOperations.Add(postOperation);
	}

	public void RunTask(Func<object> task, Action<object> callback)
	{
		callback(task());
	}

	public void SendAnalyticsEvent(string eventName, Dictionary<string, object> eventData)
	{
		Analytics.CustomEvent(eventName, eventData);
	}

	public string SerializeJson(object instance)
	{
		return Unity.Cloud.UserReporting.Plugin.SimpleJson.SimpleJson.SerializeObject(instance);
	}

	public void TakeScreenshot(int frameNumber, int maximumWidth, int maximumHeight, object source, Action<int, byte[]> callback)
	{
		screenshotManager.TakeScreenshot(source, frameNumber, maximumWidth, maximumHeight, callback);
	}

	public void Update(UserReportingClient client)
	{
		lock (logMessages)
		{
			foreach (LogMessage logMessage in logMessages)
			{
				UserReportEventLevel level = UserReportEventLevel.Info;
				if (logMessage.LogType == LogType.Warning)
				{
					level = UserReportEventLevel.Warning;
				}
				else if (logMessage.LogType == LogType.Error)
				{
					level = UserReportEventLevel.Error;
				}
				else if (logMessage.LogType == LogType.Exception)
				{
					level = UserReportEventLevel.Error;
				}
				else if (logMessage.LogType == LogType.Assert)
				{
					level = UserReportEventLevel.Error;
				}
				if (client.IsConnectedToLogger)
				{
					client.LogEvent(level, logMessage.LogString, logMessage.StackTrace);
				}
			}
			logMessages.Clear();
		}
		if (client.Configuration.MetricsGatheringMode == MetricsGatheringMode.Automatic)
		{
			SampleAutomaticMetrics(client);
			foreach (ProfilerSampler profilerSampler in profilerSamplers)
			{
				client.SampleMetric(profilerSampler.Name, profilerSampler.GetValue());
			}
		}
		int num = 0;
		while (num < postOperations.Count)
		{
			PostOperation postOperation = postOperations[num];
			if (postOperation.WebRequest.isDone)
			{
				bool flag = postOperation.WebRequest.error != null && postOperation.WebRequest.responseCode != 200;
				if (flag)
				{
					string message = $"UnityUserReportingPlatform.Post: {postOperation.WebRequest.responseCode} {postOperation.WebRequest.error}";
					Debug.Log(message);
					client.LogEvent(UserReportEventLevel.Error, message);
				}
				postOperation.ProgressCallback(1f, 1f);
				postOperation.Callback(!flag, postOperation.WebRequest.downloadHandler.data);
				postOperations.Remove(postOperation);
			}
			else
			{
				postOperation.ProgressCallback(postOperation.WebRequest.uploadProgress, postOperation.WebRequest.downloadProgress);
				num++;
			}
		}
	}

	public virtual IDictionary<string, string> GetDeviceMetadata()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("BuildGUID", Application.buildGUID);
		dictionary.Add("DeviceModel", SystemInfo.deviceModel);
		dictionary.Add("DeviceType", SystemInfo.deviceType.ToString());
		dictionary.Add("DeviceUniqueIdentifier", SystemInfo.deviceUniqueIdentifier);
		dictionary.Add("DPI", Screen.dpi.ToString(CultureInfo.InvariantCulture));
		dictionary.Add("GraphicsDeviceName", SystemInfo.graphicsDeviceName);
		dictionary.Add("GraphicsDeviceType", SystemInfo.graphicsDeviceType.ToString());
		dictionary.Add("GraphicsDeviceVendor", SystemInfo.graphicsDeviceVendor);
		dictionary.Add("GraphicsDeviceVersion", SystemInfo.graphicsDeviceVersion);
		dictionary.Add("GraphicsMemorySize", SystemInfo.graphicsMemorySize.ToString());
		dictionary.Add("InstallerName", Application.installerName);
		dictionary.Add("InstallMode", Application.installMode.ToString());
		dictionary.Add("IsEditor", Application.isEditor.ToString());
		dictionary.Add("IsFullScreen", Screen.fullScreen.ToString());
		dictionary.Add("OperatingSystem", SystemInfo.operatingSystem);
		dictionary.Add("OperatingSystemFamily", SystemInfo.operatingSystemFamily.ToString());
		dictionary.Add("Orientation", Screen.orientation.ToString());
		dictionary.Add("Platform", Application.platform.ToString());
		try
		{
			dictionary.Add("QualityLevel", QualitySettings.names[QualitySettings.GetQualityLevel()]);
		}
		catch
		{
		}
		dictionary.Add("ResolutionWidth", Screen.currentResolution.width.ToString());
		dictionary.Add("ResolutionHeight", Screen.currentResolution.height.ToString());
		dictionary.Add("ResolutionRefreshRate", Screen.currentResolution.refreshRate.ToString());
		dictionary.Add("SystemLanguage", Application.systemLanguage.ToString());
		dictionary.Add("SystemMemorySize", SystemInfo.systemMemorySize.ToString());
		dictionary.Add("TargetFrameRate", Application.targetFrameRate.ToString());
		dictionary.Add("UnityVersion", Application.unityVersion);
		dictionary.Add("Version", Application.version);
		dictionary.Add("Source", "Unity");
		Type type = GetType();
		dictionary.Add("IUserReportingPlatform", type.Name);
		return dictionary;
	}

	public virtual Dictionary<string, string> GetSamplerNames()
	{
		return new Dictionary<string, string>
		{
			{ "AudioManager.FixedUpdate", "AudioManager.FixedUpdateInMilliseconds" },
			{ "AudioManager.Update", "AudioManager.UpdateInMilliseconds" },
			{ "LateBehaviourUpdate", "Behaviors.LateUpdateInMilliseconds" },
			{ "BehaviourUpdate", "Behaviors.UpdateInMilliseconds" },
			{ "Camera.Render", "Camera.RenderInMilliseconds" },
			{ "Overhead", "Engine.OverheadInMilliseconds" },
			{ "WaitForRenderJobs", "Engine.WaitForRenderJobsInMilliseconds" },
			{ "WaitForTargetFPS", "Engine.WaitForTargetFPSInMilliseconds" },
			{ "GUI.Repaint", "GUI.RepaintInMilliseconds" },
			{ "Network.Update", "Network.UpdateInMilliseconds" },
			{ "ParticleSystem.EndUpdateAll", "ParticleSystem.EndUpdateAllInMilliseconds" },
			{ "ParticleSystem.Update", "ParticleSystem.UpdateInMilliseconds" },
			{ "Physics.FetchResults", "Physics.FetchResultsInMilliseconds" },
			{ "Physics.Processing", "Physics.ProcessingInMilliseconds" },
			{ "Physics.ProcessReports", "Physics.ProcessReportsInMilliseconds" },
			{ "Physics.Simulate", "Physics.SimulateInMilliseconds" },
			{ "Physics.UpdateBodies", "Physics.UpdateBodiesInMilliseconds" },
			{ "Physics.Interpolation", "Physics.InterpolationInMilliseconds" },
			{ "Physics2D.DynamicUpdate", "Physics2D.DynamicUpdateInMilliseconds" },
			{ "Physics2D.FixedUpdate", "Physics2D.FixedUpdateInMilliseconds" }
		};
	}

	public virtual void ModifyUserReport(UserReport userReport)
	{
		Scene activeScene = SceneManager.GetActiveScene();
		userReport.DeviceMetadata.Add(new UserReportNamedValue("ActiveSceneName", activeScene.name));
		Camera main = Camera.main;
		if (main != null)
		{
			userReport.DeviceMetadata.Add(new UserReportNamedValue("MainCameraName", main.name));
			userReport.DeviceMetadata.Add(new UserReportNamedValue("MainCameraPosition", main.transform.position.ToString()));
			userReport.DeviceMetadata.Add(new UserReportNamedValue("MainCameraForward", main.transform.forward.ToString()));
			if (Physics.Raycast(main.transform.position, main.transform.forward, out var hitInfo))
			{
				GameObject gameObject = hitInfo.transform.gameObject;
				userReport.DeviceMetadata.Add(new UserReportNamedValue("LookingAt", hitInfo.point.ToString()));
				userReport.DeviceMetadata.Add(new UserReportNamedValue("LookingAtGameObject", gameObject.name));
				userReport.DeviceMetadata.Add(new UserReportNamedValue("LookingAtGameObjectPosition", gameObject.transform.position.ToString()));
			}
		}
	}

	public virtual void SampleAutomaticMetrics(UserReportingClient client)
	{
		client.SampleMetric("Graphics.FramesPerSecond", 1f / Time.deltaTime);
		client.SampleMetric("Memory.MonoUsedSizeInBytes", Profiler.GetMonoUsedSizeLong());
		client.SampleMetric("Memory.TotalAllocatedMemoryInBytes", Profiler.GetTotalAllocatedMemoryLong());
		client.SampleMetric("Memory.TotalReservedMemoryInBytes", Profiler.GetTotalReservedMemoryLong());
		client.SampleMetric("Memory.TotalUnusedReservedMemoryInBytes", Profiler.GetTotalUnusedReservedMemoryLong());
		client.SampleMetric("Battery.BatteryLevelInPercent", SystemInfo.batteryLevel);
	}
}
