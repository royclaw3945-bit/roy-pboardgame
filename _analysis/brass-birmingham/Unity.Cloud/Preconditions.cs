using System;

namespace Unity.Cloud;

public static class Preconditions
{
	public static void ArgumentIsLessThanOrEqualToLength(object value, int length, string argumentName)
	{
		if (value is string text && text.Length > length)
		{
			throw new ArgumentException(argumentName);
		}
	}

	public static void ArgumentNotNullOrWhitespace(object value, string argumentName)
	{
		if (value == null)
		{
			throw new ArgumentNullException(argumentName);
		}
		if (value is string text && text.Trim() == string.Empty)
		{
			throw new ArgumentNullException(argumentName);
		}
	}
}
