using System.Collections.Generic;

namespace Utils.Collections.Utils;

public class MultiMap<K, V>
{
	private readonly Dictionary<K, List<V>> _dictionary = new Dictionary<K, List<V>>();

	public IEnumerable<K> Keys => _dictionary.Keys;

	public List<V> this[K key]
	{
		get
		{
			if (!_dictionary.TryGetValue(key, out var value))
			{
				value = new List<V>();
				_dictionary[key] = value;
			}
			return value;
		}
	}

	public void Add(K key, V value)
	{
		if (_dictionary.TryGetValue(key, out var value2))
		{
			value2.Add(value);
			return;
		}
		value2 = new List<V>();
		value2.Add(value);
		_dictionary[key] = value2;
	}
}
