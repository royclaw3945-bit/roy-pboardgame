using System;

namespace Unity.Cloud.UserReporting;

public static class PngHelper
{
	public static int GetPngHeightFromBase64Data(string data)
	{
		if (data == null || data.Length < 32)
		{
			return 0;
		}
		byte[] array = Slice(Convert.FromBase64String(data.Substring(0, 32)), 20, 24);
		Array.Reverse((Array)array);
		return BitConverter.ToInt32(array, 0);
	}

	public static int GetPngWidthFromBase64Data(string data)
	{
		if (data == null || data.Length < 32)
		{
			return 0;
		}
		byte[] array = Slice(Convert.FromBase64String(data.Substring(0, 32)), 16, 20);
		Array.Reverse((Array)array);
		return BitConverter.ToInt32(array, 0);
	}

	private static byte[] Slice(byte[] source, int start, int end)
	{
		if (end < 0)
		{
			end = source.Length + end;
		}
		int num = end - start;
		byte[] array = new byte[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = source[i + start];
		}
		return array;
	}
}
