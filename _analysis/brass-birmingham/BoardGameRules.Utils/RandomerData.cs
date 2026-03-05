namespace BoardGameRules.Utils;

public class RandomerData
{
	public int FastForwardCount { get; set; }

	public int Seed { get; set; }

	public RandomerData(int seed, int fastForwardCount = 0)
	{
		Seed = seed;
		FastForwardCount = fastForwardCount;
	}
}
