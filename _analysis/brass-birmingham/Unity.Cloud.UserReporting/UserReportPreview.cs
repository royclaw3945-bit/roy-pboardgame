using System;
using System.Collections.Generic;
using Unity.Cloud.Authorization;

namespace Unity.Cloud.UserReporting;

public class UserReportPreview
{
	public List<UserReportMetric> AggregateMetrics { get; set; }

	public UserReportAppearanceHint AppearanceHint { get; set; }

	public long ContentLength { get; set; }

	public List<UserReportNamedValue> Dimensions { get; set; }

	public DateTime ExpiresOn { get; set; }

	public string GeoCountry { get; set; }

	public string Identifier { get; set; }

	public string IPAddress { get; set; }

	public bool IsHiddenWithoutDimension { get; set; }

	public bool IsSilent { get; set; }

	public bool IsTemporary { get; set; }

	public LicenseLevel LicenseLevel { get; set; }

	public string ProjectIdentifier { get; set; }

	public DateTime ReceivedOn { get; set; }

	public string Summary { get; set; }

	public UserReportScreenshot Thumbnail { get; set; }

	public UserReportPreview()
	{
		Dimensions = new List<UserReportNamedValue>();
	}
}
