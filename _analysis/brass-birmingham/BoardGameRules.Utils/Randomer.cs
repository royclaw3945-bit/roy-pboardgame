using System;

namespace BoardGameRules.Utils;

public class Randomer : IRandomer
{
	private readonly Random _random;

	public RandomerData RandomerInitData { get; private set; }

	public Randomer(RandomerData randomerData)
	{
		RandomerInitData = randomerData;
		_random = new Random(RandomerInitData.Seed);
		FastForward(RandomerInitData.FastForwardCount);
	}

	private void FastForward(int count)
	{
		for (int i = 0; i < count; i++)
		{
			_random.Next();
		}
	}

	public int Next()
	{
		RandomerInitData.FastForwardCount++;
		return _random.Next();
	}
}
