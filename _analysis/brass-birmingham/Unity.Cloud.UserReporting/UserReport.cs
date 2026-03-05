using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unity.Cloud.UserReporting;

public class UserReport : UserReportPreview
{
	private class UserReportMetricSorter : IComparer<UserReportMetric>
	{
		public int Compare(UserReportMetric x, UserReportMetric y)
		{
			return string.Compare(x.Name, y.Name, StringComparison.Ordinal);
		}
	}

	public List<UserReportAttachment> Attachments { get; set; }

	public List<UserReportMetric> ClientMetrics { get; set; }

	public List<UserReportNamedValue> DeviceMetadata { get; set; }

	public List<UserReportEvent> Events { get; set; }

	public List<UserReportNamedValue> Fields { get; set; }

	public List<UserReportMeasure> Measures { get; set; }

	public List<UserReportScreenshot> Screenshots { get; set; }

	public UserReport()
	{
		base.AggregateMetrics = new List<UserReportMetric>();
		Attachments = new List<UserReportAttachment>();
		ClientMetrics = new List<UserReportMetric>();
		DeviceMetadata = new List<UserReportNamedValue>();
		Events = new List<UserReportEvent>();
		Fields = new List<UserReportNamedValue>();
		Measures = new List<UserReportMeasure>();
		Screenshots = new List<UserReportScreenshot>();
	}

	public UserReport Clone()
	{
		return new UserReport
		{
			AggregateMetrics = ((base.AggregateMetrics != null) ? base.AggregateMetrics.ToList() : null),
			Attachments = ((Attachments != null) ? Attachments.ToList() : null),
			ClientMetrics = ((ClientMetrics != null) ? ClientMetrics.ToList() : null),
			ContentLength = base.ContentLength,
			DeviceMetadata = ((DeviceMetadata != null) ? DeviceMetadata.ToList() : null),
			Dimensions = base.Dimensions.ToList(),
			Events = ((Events != null) ? Events.ToList() : null),
			ExpiresOn = base.ExpiresOn,
			Fields = ((Fields != null) ? Fields.ToList() : null),
			Identifier = base.Identifier,
			IPAddress = base.IPAddress,
			Measures = ((Measures != null) ? Measures.ToList() : null),
			ProjectIdentifier = base.ProjectIdentifier,
			ReceivedOn = base.ReceivedOn,
			Screenshots = ((Screenshots != null) ? Screenshots.ToList() : null),
			Summary = base.Summary,
			Thumbnail = base.Thumbnail
		};
	}

	public void Complete()
	{
		if (Screenshots.Count > 0)
		{
			base.Thumbnail = Screenshots[Screenshots.Count - 1];
		}
		Dictionary<string, UserReportMetric> dictionary = new Dictionary<string, UserReportMetric>();
		foreach (UserReportMeasure measure in Measures)
		{
			foreach (UserReportMetric metric in measure.Metrics)
			{
				if (!dictionary.ContainsKey(metric.Name))
				{
					dictionary.Add(value: new UserReportMetric
					{
						Name = metric.Name
					}, key: metric.Name);
				}
				UserReportMetric value = dictionary[metric.Name];
				value.Sample(metric.Average);
				dictionary[metric.Name] = value;
			}
		}
		if (base.AggregateMetrics == null)
		{
			base.AggregateMetrics = new List<UserReportMetric>();
		}
		foreach (KeyValuePair<string, UserReportMetric> item in dictionary)
		{
			base.AggregateMetrics.Add(item.Value);
		}
		base.AggregateMetrics.Sort(new UserReportMetricSorter());
	}

	public void Fix()
	{
		base.AggregateMetrics = base.AggregateMetrics ?? new List<UserReportMetric>();
		Attachments = Attachments ?? new List<UserReportAttachment>();
		ClientMetrics = ClientMetrics ?? new List<UserReportMetric>();
		DeviceMetadata = DeviceMetadata ?? new List<UserReportNamedValue>();
		base.Dimensions = base.Dimensions ?? new List<UserReportNamedValue>();
		Events = Events ?? new List<UserReportEvent>();
		Fields = Fields ?? new List<UserReportNamedValue>();
		Measures = Measures ?? new List<UserReportMeasure>();
		Screenshots = Screenshots ?? new List<UserReportScreenshot>();
	}

	public string GetDimensionsString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < base.Dimensions.Count; i++)
		{
			UserReportNamedValue userReportNamedValue = base.Dimensions[i];
			stringBuilder.Append(userReportNamedValue.Name);
			stringBuilder.Append(": ");
			stringBuilder.Append(userReportNamedValue.Value);
			if (i != base.Dimensions.Count - 1)
			{
				stringBuilder.Append(", ");
			}
		}
		return stringBuilder.ToString();
	}

	public void RemoveScreenshots(int maximumWidth, int maximumHeight, int totalBytes, int ignoreCount)
	{
		int num = 0;
		for (int num2 = Screenshots.Count; num2 > 0; num2--)
		{
			if (num2 >= ignoreCount)
			{
				UserReportScreenshot userReportScreenshot = Screenshots[num2];
				num += userReportScreenshot.DataBase64.Length;
				if (num > totalBytes)
				{
					break;
				}
				if (userReportScreenshot.Width > maximumWidth || userReportScreenshot.Height > maximumHeight)
				{
					Screenshots.RemoveAt(num2);
				}
			}
		}
	}

	public UserReportPreview ToPreview()
	{
		return new UserReportPreview
		{
			AggregateMetrics = ((base.AggregateMetrics != null) ? base.AggregateMetrics.ToList() : null),
			ContentLength = base.ContentLength,
			Dimensions = ((base.Dimensions != null) ? base.Dimensions.ToList() : null),
			ExpiresOn = base.ExpiresOn,
			Identifier = base.Identifier,
			IPAddress = base.IPAddress,
			ProjectIdentifier = base.ProjectIdentifier,
			ReceivedOn = base.ReceivedOn,
			Summary = base.Summary,
			Thumbnail = base.Thumbnail
		};
	}
}
