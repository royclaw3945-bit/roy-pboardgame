using System;

namespace BoardGameRules.Entities.Markets;

public abstract class BaseMarket
{
	protected int Amount;

	protected abstract int PriceOverLimit { get; }

	protected abstract int StartAmount { get; }

	protected abstract int MaxAmount { get; }

	protected BaseMarket()
	{
		Amount = StartAmount;
	}

	public int Buy(int numberOfCubes)
	{
		int buyingPrice = GetBuyingPrice(numberOfCubes);
		Amount = Math.Max(Amount - numberOfCubes, 0);
		return buyingPrice;
	}

	public int Sell(int numberOfCubes)
	{
		int sellingPrice = GetSellingPrice(numberOfCubes);
		Amount += Math.Min(MaxAmount - Amount, numberOfCubes);
		return sellingPrice;
	}

	public int AmountOfResourcePossibleToSell()
	{
		return MaxAmount - Amount;
	}

	public int GetBuyingPrice(int amount = 1)
	{
		int num = 0;
		for (int i = 0; i < amount; i++)
		{
			num = ((Amount - i <= 0) ? (num + PriceOverLimit) : (num + ((MaxAmount - Amount + i) / 2 + 1)));
		}
		return num;
	}

	public int GetSellingPrice(int amount = 1)
	{
		int num = 0;
		for (int i = 0; i < amount && Amount + i < MaxAmount; i++)
		{
			num += (MaxAmount - Amount - 1 - i) / 2 + 1;
		}
		return num;
	}
}
