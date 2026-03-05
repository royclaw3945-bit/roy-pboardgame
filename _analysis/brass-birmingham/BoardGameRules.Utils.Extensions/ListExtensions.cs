using System;
using System.Collections.Generic;
using System.Linq;

namespace BoardGameRules.Utils.Extensions;

public static class ListExtensions
{
	public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
	{
		return listToClone?.Select((T item) => (T)(item?.Clone())).ToList();
	}

	public static IList<T> GenerateRandomElements<T>(this IList<T> listToUse, int numberToGenerate)
	{
		Random random = new Random();
		List<T> list = new List<T>();
		List<T> list2 = new List<T>(listToUse);
		for (int i = 0; i < numberToGenerate; i++)
		{
			int index = random.Next(list2.Count);
			list.Add(list2[index]);
			list2.RemoveAt(index);
		}
		return list;
	}
}
