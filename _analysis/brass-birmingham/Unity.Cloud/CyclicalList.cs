using System;
using System.Collections;
using System.Collections.Generic;

namespace Unity.Cloud;

public class CyclicalList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
{
	private struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
	{
		private int currentIndex;

		private CyclicalList<T> list;

		public T Current
		{
			get
			{
				if (currentIndex < 0 || currentIndex >= list.Count)
				{
					return default(T);
				}
				return list[currentIndex];
			}
		}

		object IEnumerator.Current => Current;

		public Enumerator(CyclicalList<T> list)
		{
			this.list = list;
			currentIndex = -1;
		}

		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			currentIndex++;
			return currentIndex < list.count;
		}

		public void Reset()
		{
			currentIndex = 0;
		}
	}

	private int count;

	private T[] items;

	private int nextPointer;

	public int Capacity => items.Length;

	public int Count => count;

	public bool IsReadOnly => false;

	public T this[int index]
	{
		get
		{
			if (index < 0 || index >= count)
			{
				throw new IndexOutOfRangeException();
			}
			return items[GetPointer(index)];
		}
		set
		{
			if (index < 0 || index >= count)
			{
				throw new IndexOutOfRangeException();
			}
			items[GetPointer(index)] = value;
		}
	}

	public CyclicalList(int capacity)
	{
		items = new T[capacity];
	}

	public void Add(T item)
	{
		items[nextPointer] = item;
		count++;
		if (count > items.Length)
		{
			count = items.Length;
		}
		nextPointer++;
		if (nextPointer >= items.Length)
		{
			nextPointer = 0;
		}
	}

	public void Clear()
	{
		count = 0;
		nextPointer = 0;
	}

	public bool Contains(T item)
	{
		using (IEnumerator<T> enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Equals(item))
				{
					return true;
				}
			}
		}
		return false;
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		int num = 0;
		using IEnumerator<T> enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			int num2 = arrayIndex + num;
			if (num2 >= array.Length)
			{
				break;
			}
			array[num2] = current;
			num++;
		}
	}

	public IEnumerator<T> GetEnumerator()
	{
		return new Enumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public T GetNextEviction()
	{
		return items[nextPointer];
	}

	private int GetPointer(int index)
	{
		if (index < 0 || index >= count)
		{
			throw new IndexOutOfRangeException();
		}
		if (count < items.Length)
		{
			return index;
		}
		return (nextPointer + index) % count;
	}

	public int IndexOf(T item)
	{
		int num = 0;
		using (IEnumerator<T> enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Equals(item))
				{
					return num;
				}
				num++;
			}
		}
		return -1;
	}

	public void Insert(int index, T item)
	{
		if (index < 0 || index >= count)
		{
			throw new IndexOutOfRangeException();
		}
	}

	public bool Remove(T item)
	{
		return false;
	}

	public void RemoveAt(int index)
	{
		if (index < 0 || index >= count)
		{
			throw new IndexOutOfRangeException();
		}
	}
}
