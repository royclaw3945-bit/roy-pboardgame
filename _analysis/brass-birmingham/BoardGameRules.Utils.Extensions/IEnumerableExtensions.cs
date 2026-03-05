using System;
using System.Collections.Generic;
using System.Linq;

namespace BoardGameRules.Utils.Extensions;

public static class IEnumerableExtensions
{
	public static IEnumerable<T> TakeWhileIncluding<T>(this IEnumerable<T> data, Func<T, bool> predicate)
	{
		foreach (T item in data)
		{
			yield return item;
			if (!predicate(item))
			{
				break;
			}
		}
	}

	public static IEnumerable<T> DropLast<T>(this IEnumerable<T> enumerable)
	{
		return enumerable.Take(enumerable.Count() - 1);
	}
}
