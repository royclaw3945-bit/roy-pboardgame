using System.Collections.Generic;

namespace Unity.Cloud.UserReporting;

public struct UserReportMeasure
{
	public int EndFrameNumber { get; set; }

	public List<UserReportNamedValue> Metadata { get; set; }

	public List<UserReportMetric> Metrics { get; set; }

	public int StartFrameNumber { get; set; }
}
