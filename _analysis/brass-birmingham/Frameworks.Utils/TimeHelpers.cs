namespace Frameworks.Utils;

public class TimeHelpers
{
	public const int MIN = 60;

	public const int HOUR = 3600;

	public const int DAY = 86400;

	public static string SecondsToTimeString(int seconds)
	{
		return seconds switch
		{
			300 => "5m", 
			600 => "10m", 
			900 => "15m", 
			1800 => "30m", 
			3600 => "1h", 
			7200 => "2h", 
			10800 => "3h", 
			21600 => "6h", 
			43200 => "12h", 
			86400 => "1d", 
			172800 => "2d", 
			432000 => "5d", 
			604800 => "7d", 
			1209600 => "14d", 
			2592000 => "30d", 
			_ => "Error", 
		};
	}
}
