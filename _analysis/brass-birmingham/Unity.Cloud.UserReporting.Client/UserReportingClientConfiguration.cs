namespace Unity.Cloud.UserReporting.Client;

public class UserReportingClientConfiguration
{
	public int FramesPerMeasure { get; internal set; }

	public int MaximumEventCount { get; internal set; }

	public int MaximumMeasureCount { get; internal set; }

	public int MaximumScreenshotCount { get; internal set; }

	public MetricsGatheringMode MetricsGatheringMode { get; internal set; }

	public UserReportingClientConfiguration()
	{
		MaximumEventCount = 100;
		MaximumMeasureCount = 300;
		FramesPerMeasure = 60;
		MaximumScreenshotCount = 10;
	}

	public UserReportingClientConfiguration(int maximumEventCount, int maximumMeasureCount, int framesPerMeasure, int maximumScreenshotCount)
	{
		MaximumEventCount = maximumEventCount;
		MaximumMeasureCount = maximumMeasureCount;
		FramesPerMeasure = framesPerMeasure;
		MaximumScreenshotCount = maximumScreenshotCount;
	}

	public UserReportingClientConfiguration(int maximumEventCount, MetricsGatheringMode metricsGatheringMode, int maximumMeasureCount, int framesPerMeasure, int maximumScreenshotCount)
	{
		MaximumEventCount = maximumEventCount;
		MetricsGatheringMode = metricsGatheringMode;
		MaximumMeasureCount = maximumMeasureCount;
		FramesPerMeasure = framesPerMeasure;
		MaximumScreenshotCount = maximumScreenshotCount;
	}
}
