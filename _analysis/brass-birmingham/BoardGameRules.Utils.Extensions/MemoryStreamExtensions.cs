using System.IO;

namespace BoardGameRules.Utils.Extensions;

public static class MemoryStreamExtensions
{
	public static string ConvertToString(this MemoryStream stream)
	{
		using MemoryStream stream2 = new MemoryStream(stream.ToArray());
		using StreamReader streamReader = new StreamReader(stream2);
		return streamReader.ReadToEnd();
	}
}
