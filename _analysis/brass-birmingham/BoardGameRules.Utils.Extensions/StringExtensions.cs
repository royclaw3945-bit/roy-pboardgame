using System.IO;
using System.Text;

namespace BoardGameRules.Utils.Extensions;

public static class StringExtensions
{
	public static string ToOneWordPerLine(this string str)
	{
		return str.Replace(" ", "\n");
	}

	public static Stream ToStream(this string str)
	{
		MemoryStream memoryStream = new MemoryStream();
		StreamWriter streamWriter = new StreamWriter(memoryStream);
		streamWriter.Write(str);
		streamWriter.Flush();
		memoryStream.Position = 0L;
		return memoryStream;
	}

	public static Stream ToStreamUTF8(this string str)
	{
		return new MemoryStream(Encoding.UTF8.GetBytes(str));
	}
}
