using System;

namespace Unity.Cloud.UserReporting;

public struct UserReportEvent
{
	public SerializableException Exception { get; set; }

	public int FrameNumber { get; set; }

	public string FullMessage => $"{Message}{Environment.NewLine}{StackTrace}";

	public UserReportEventLevel Level { get; set; }

	public string Message { get; set; }

	public string StackTrace { get; set; }

	public DateTime Timestamp { get; set; }
}
