using System;
using System.Collections.Generic;

namespace BoardGameRules.Utils.Extensions;

public static class DictionaryExtensions
{
	public static bool DictionaryEqual<TKey, TValue>(this IDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second)
	{
		return first.DictionaryEqual(second, null);
	}

	public static bool DictionaryEqual<TKey, TValue>(this IDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second, IEqualityComparer<TValue> valueComparer)
	{
		if (first == second)
		{
			return true;
		}
		if (first == null || second == null)
		{
			return false;
		}
		if (first.Count != second.Count)
		{
			return false;
		}
		valueComparer = valueComparer ?? EqualityComparer<TValue>.Default;
		foreach (KeyValuePair<TKey, TValue> item in first)
		{
			if (!second.TryGetValue(item.Key, out var value))
			{
				return false;
			}
			if (!valueComparer.Equals(item.Value, value))
			{
				return false;
			}
		}
		return true;
	}

	public static Dictionary<TKey, TValue> CloneDictionaryCloningValues<TKey, TValue>(this Dictionary<TKey, TValue> original) where TValue : ICloneable
	{
		Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>(original.Count, original.Comparer);
		foreach (KeyValuePair<TKey, TValue> item in original)
		{
			dictionary.Add(item.Key, (TValue)item.Value.Clone());
		}
		return dictionary;
	}
}
