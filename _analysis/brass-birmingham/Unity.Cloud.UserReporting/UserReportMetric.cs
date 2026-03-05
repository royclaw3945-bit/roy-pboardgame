using System;

namespace Unity.Cloud.UserReporting;

public struct UserReportMetric
{
	public double Average => Sum / (double)Count;

	public int Count { get; set; }

	public double Maximum { get; set; }

	public double Minimum { get; set; }

	public string Name { get; set; }

	public double Sum { get; set; }

	public void Sample(double value)
	{
		if (Count == 0)
		{
			Minimum = double.MaxValue;
			Maximum = double.MinValue;
		}
		Count++;
		Sum += value;
		Minimum = Math.Min(Minimum, value);
		Maximum = Math.Max(Maximum, value);
	}
}
