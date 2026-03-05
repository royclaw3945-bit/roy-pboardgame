using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Unity.Cloud.UserReporting.Client;

public class UserReportingClient
{
	private Dictionary<string, UserReportMetric> clientMetrics;

	private Dictionary<string, string> currentMeasureMetadata;

	private Dictionary<string, UserReportMetric> currentMetrics;

	private List<Action> currentSynchronizedActions;

	private List<UserReportNamedValue> deviceMetadata;

	private CyclicalList<UserReportEvent> events;

	private int frameNumber;

	private bool isMeasureBoundary;

	private int measureFrames;

	private CyclicalList<UserReportMeasure> measures;

	private CyclicalList<UserReportScreenshot> screenshots;

	private int screenshotsSaved;

	private int screenshotsTaken;

	private List<Action> synchronizedActions;

	private Stopwatch updateStopwatch;

	public UserReportingClientConfiguration Configuration { get; private set; }

	public string Endpoint { get; private set; }

	public bool IsConnectedToLogger { get; set; }

	public bool IsSelfReporting { get; set; }

	public IUserReportingPlatform Platform { get; private set; }

	public string ProjectIdentifier { get; private set; }

	public bool SendEventsToAnalytics { get; set; }

	public UserReportingClient(string endpoint, string projectIdentifier, IUserReportingPlatform platform, UserReportingClientConfiguration configuration)
	{
		Endpoint = endpoint;
		ProjectIdentifier = projectIdentifier;
		Platform = platform;
		Configuration = configuration;
		Configuration.FramesPerMeasure = ((Configuration.FramesPerMeasure <= 0) ? 1 : Configuration.FramesPerMeasure);
		Configuration.MaximumEventCount = ((Configuration.MaximumEventCount <= 0) ? 1 : Configuration.MaximumEventCount);
		Configuration.MaximumMeasureCount = ((Configuration.MaximumMeasureCount <= 0) ? 1 : Configuration.MaximumMeasureCount);
		Configuration.MaximumScreenshotCount = ((Configuration.MaximumScreenshotCount <= 0) ? 1 : Configuration.MaximumScreenshotCount);
		clientMetrics = new Dictionary<string, UserReportMetric>();
		currentMeasureMetadata = new Dictionary<string, string>();
		currentMetrics = new Dictionary<string, UserReportMetric>();
		events = new CyclicalList<UserReportEvent>(configuration.MaximumEventCount);
		measures = new CyclicalList<UserReportMeasure>(configuration.MaximumMeasureCount);
		screenshots = new CyclicalList<UserReportScreenshot>(configuration.MaximumScreenshotCount);
		deviceMetadata = new List<UserReportNamedValue>();
		foreach (KeyValuePair<string, string> deviceMetadatum in Platform.GetDeviceMetadata())
		{
			AddDeviceMetadata(deviceMetadatum.Key, deviceMetadatum.Value);
		}
		AddDeviceMetadata("UserReportingClientVersion", "2.0");
		synchronizedActions = new List<Action>();
		currentSynchronizedActions = new List<Action>();
		updateStopwatch = new Stopwatch();
		IsConnectedToLogger = true;
	}

	public void AddDeviceMetadata(string name, string value)
	{
		lock (deviceMetadata)
		{
			UserReportNamedValue item = new UserReportNamedValue
			{
				Name = name,
				Value = value
			};
			deviceMetadata.Add(item);
		}
	}

	public void AddMeasureMetadata(string name, string value)
	{
		if (currentMeasureMetadata.ContainsKey(name))
		{
			currentMeasureMetadata[name] = value;
		}
		else
		{
			currentMeasureMetadata.Add(name, value);
		}
	}

	public void ClearScreenshots()
	{
		lock (screenshots)
		{
			screenshots.Clear();
		}
	}

	public void CreateUserReport(Action<UserReport> callback)
	{
		LogEvent(UserReportEventLevel.Info, "Creating user report.");
		WaitForPerforation(screenshotsTaken, delegate
		{
			Platform.RunTask(delegate
			{
				Stopwatch stopwatch = Stopwatch.StartNew();
				UserReport userReport = new UserReport
				{
					ProjectIdentifier = ProjectIdentifier
				};
				lock (deviceMetadata)
				{
					userReport.DeviceMetadata = deviceMetadata.ToList();
				}
				lock (events)
				{
					userReport.Events = events.ToList();
				}
				lock (measures)
				{
					userReport.Measures = measures.ToList();
				}
				lock (screenshots)
				{
					userReport.Screenshots = screenshots.ToList();
				}
				userReport.Complete();
				Platform.ModifyUserReport(userReport);
				stopwatch.Stop();
				SampleClientMetric("UserReportingClient.CreateUserReport.Task", stopwatch.ElapsedMilliseconds);
				foreach (KeyValuePair<string, UserReportMetric> clientMetric in clientMetrics)
				{
					userReport.ClientMetrics.Add(clientMetric.Value);
				}
				return userReport;
			}, delegate(object result)
			{
				callback(result as UserReport);
			});
		});
	}

	public string GetEndpoint()
	{
		if (Endpoint == null)
		{
			return "https://localhost";
		}
		return Endpoint.Trim();
	}

	public void LogEvent(UserReportEventLevel level, string message)
	{
		LogEvent(level, message, null, null);
	}

	public void LogEvent(UserReportEventLevel level, string message, string stackTrace)
	{
		LogEvent(level, message, stackTrace, null);
	}

	private void LogEvent(UserReportEventLevel level, string message, string stackTrace, Exception exception)
	{
		lock (events)
		{
			UserReportEvent item = new UserReportEvent
			{
				Level = level,
				Message = message,
				FrameNumber = frameNumber,
				StackTrace = stackTrace,
				Timestamp = DateTime.UtcNow
			};
			if (exception != null)
			{
				item.Exception = new SerializableException(exception);
			}
			events.Add(item);
		}
	}

	public void LogException(Exception exception)
	{
		LogEvent(UserReportEventLevel.Error, null, null, exception);
	}

	public void SampleClientMetric(string name, double value)
	{
		if (!double.IsInfinity(value) && !double.IsNaN(value))
		{
			if (!clientMetrics.ContainsKey(name))
			{
				UserReportMetric value2 = new UserReportMetric
				{
					Name = name
				};
				clientMetrics.Add(name, value2);
			}
			UserReportMetric value3 = clientMetrics[name];
			value3.Sample(value);
			clientMetrics[name] = value3;
			if (IsSelfReporting)
			{
				SampleMetric(name, value);
			}
		}
	}

	public void SampleMetric(string name, double value)
	{
		if (Configuration.MetricsGatheringMode != MetricsGatheringMode.Disabled && !double.IsInfinity(value) && !double.IsNaN(value))
		{
			if (!currentMetrics.ContainsKey(name))
			{
				UserReportMetric value2 = new UserReportMetric
				{
					Name = name
				};
				currentMetrics.Add(name, value2);
			}
			UserReportMetric value3 = currentMetrics[name];
			value3.Sample(value);
			currentMetrics[name] = value3;
		}
	}

	public void SaveUserReportToDisk(UserReport userReport)
	{
		LogEvent(UserReportEventLevel.Info, "Saving user report to disk.");
		string contents = Platform.SerializeJson(userReport);
		File.WriteAllText("UserReport.json", contents);
	}

	public void SendUserReport(UserReport userReport, Action<bool, UserReport> callback)
	{
		SendUserReport(userReport, null, callback);
	}

	public void SendUserReport(UserReport userReport, Action<float, float> progressCallback, Action<bool, UserReport> callback)
	{
		try
		{
			if (userReport == null)
			{
				return;
			}
			if (userReport.Identifier != null)
			{
				LogEvent(UserReportEventLevel.Warning, "Identifier cannot be set on the client side. The value provided was discarded.");
				return;
			}
			if (userReport.ContentLength != 0L)
			{
				LogEvent(UserReportEventLevel.Warning, "ContentLength cannot be set on the client side. The value provided was discarded.");
				return;
			}
			if (userReport.ReceivedOn != default(DateTime))
			{
				LogEvent(UserReportEventLevel.Warning, "ReceivedOn cannot be set on the client side. The value provided was discarded.");
				return;
			}
			if (userReport.ExpiresOn != default(DateTime))
			{
				LogEvent(UserReportEventLevel.Warning, "ExpiresOn cannot be set on the client side. The value provided was discarded.");
				return;
			}
			LogEvent(UserReportEventLevel.Info, "Sending user report.");
			string s = Platform.SerializeJson(userReport);
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			string endpoint = GetEndpoint();
			string endpoint2 = string.Format($"{endpoint}/api/userreporting");
			Platform.Post(endpoint2, "application/json", bytes, delegate(float uploadProgress, float downloadProgress)
			{
				if (progressCallback != null)
				{
					progressCallback(uploadProgress, downloadProgress);
				}
			}, delegate(bool success, byte[] result)
			{
				synchronizedActions.Add(delegate
				{
					if (success)
					{
						try
						{
							string json = Encoding.UTF8.GetString(result);
							UserReport userReport2 = Platform.DeserializeJson<UserReport>(json);
							if (userReport2 != null)
							{
								if (SendEventsToAnalytics)
								{
									Dictionary<string, object> eventData = new Dictionary<string, object> { { "UserReportIdentifier", userReport.Identifier } };
									Platform.SendAnalyticsEvent("UserReportingClient.SendUserReport", eventData);
								}
								callback(success, userReport2);
							}
							else
							{
								callback(arg1: false, null);
							}
							return;
						}
						catch (Exception ex2)
						{
							LogEvent(UserReportEventLevel.Error, $"Sending user report failed: {ex2.ToString()}");
							callback(arg1: false, null);
							return;
						}
					}
					LogEvent(UserReportEventLevel.Error, "Sending user report failed.");
					callback(arg1: false, null);
				});
			});
		}
		catch (Exception ex)
		{
			LogEvent(UserReportEventLevel.Error, $"Sending user report failed: {ex.ToString()}");
			callback(arg1: false, null);
		}
	}

	public void TakeScreenshot(int maximumWidth, int maximumHeight, Action<UserReportScreenshot> callback)
	{
		TakeScreenshotFromSource(maximumWidth, maximumHeight, null, callback);
	}

	public void TakeScreenshotFromSource(int maximumWidth, int maximumHeight, object source, Action<UserReportScreenshot> callback)
	{
		LogEvent(UserReportEventLevel.Info, "Taking screenshot.");
		screenshotsTaken++;
		Platform.TakeScreenshot(frameNumber, maximumWidth, maximumHeight, source, delegate(int passedFrameNumber, byte[] data)
		{
			synchronizedActions.Add(delegate
			{
				lock (screenshots)
				{
					UserReportScreenshot userReportScreenshot = new UserReportScreenshot
					{
						FrameNumber = passedFrameNumber,
						DataBase64 = Convert.ToBase64String(data)
					};
					screenshots.Add(userReportScreenshot);
					screenshotsSaved++;
					callback(userReportScreenshot);
				}
			});
		});
	}

	public void Update()
	{
		updateStopwatch.Reset();
		updateStopwatch.Start();
		Platform.Update(this);
		if (Configuration.MetricsGatheringMode != MetricsGatheringMode.Disabled)
		{
			isMeasureBoundary = false;
			int framesPerMeasure = Configuration.FramesPerMeasure;
			if (measureFrames >= framesPerMeasure)
			{
				lock (measures)
				{
					UserReportMeasure item = new UserReportMeasure
					{
						StartFrameNumber = frameNumber - framesPerMeasure,
						EndFrameNumber = frameNumber - 1
					};
					UserReportMeasure nextEviction = measures.GetNextEviction();
					if (nextEviction.Metrics != null)
					{
						item.Metadata = nextEviction.Metadata;
						item.Metrics = nextEviction.Metrics;
					}
					else
					{
						item.Metadata = new List<UserReportNamedValue>();
						item.Metrics = new List<UserReportMetric>();
					}
					item.Metadata.Clear();
					item.Metrics.Clear();
					foreach (KeyValuePair<string, string> currentMeasureMetadatum in currentMeasureMetadata)
					{
						UserReportNamedValue item2 = new UserReportNamedValue
						{
							Name = currentMeasureMetadatum.Key,
							Value = currentMeasureMetadatum.Value
						};
						item.Metadata.Add(item2);
					}
					foreach (KeyValuePair<string, UserReportMetric> currentMetric in currentMetrics)
					{
						item.Metrics.Add(currentMetric.Value);
					}
					currentMetrics.Clear();
					measures.Add(item);
					measureFrames = 0;
					isMeasureBoundary = true;
				}
			}
			measureFrames++;
		}
		else
		{
			isMeasureBoundary = true;
		}
		foreach (Action synchronizedAction in synchronizedActions)
		{
			currentSynchronizedActions.Add(synchronizedAction);
		}
		synchronizedActions.Clear();
		foreach (Action currentSynchronizedAction in currentSynchronizedActions)
		{
			currentSynchronizedAction();
		}
		currentSynchronizedActions.Clear();
		frameNumber++;
		updateStopwatch.Stop();
		SampleClientMetric("UserReportingClient.Update", updateStopwatch.ElapsedMilliseconds);
	}

	public void UpdateOnEndOfFrame()
	{
		updateStopwatch.Reset();
		updateStopwatch.Start();
		Platform.OnEndOfFrame(this);
		updateStopwatch.Stop();
		SampleClientMetric("UserReportingClient.UpdateOnEndOfFrame", updateStopwatch.ElapsedMilliseconds);
	}

	private void WaitForPerforation(int currentScreenshotsTaken, Action callback)
	{
		if (screenshotsSaved >= currentScreenshotsTaken && isMeasureBoundary)
		{
			callback();
			return;
		}
		synchronizedActions.Add(delegate
		{
			WaitForPerforation(currentScreenshotsTaken, callback);
		});
	}
}
