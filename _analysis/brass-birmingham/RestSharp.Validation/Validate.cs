using System;

namespace RestSharp.Validation;

public class Validate
{
	public static void IsBetween(int value, int min, int max)
	{
		if (value < min || value > max)
		{
			throw new ArgumentException($"Value ({value}) is not between {min} and {max}.");
		}
	}

	public static void IsValidLength(string value, int maxSize)
	{
		if (value == null || value.Length <= maxSize)
		{
			return;
		}
		throw new ArgumentException($"String is longer than max allowed size ({maxSize}).");
	}
}
