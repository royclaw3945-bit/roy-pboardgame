using System;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Common;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.Industries;

public class Brewery : BaseIndustry, IBeerSource
{
	public readonly Func<int> _numberOfBeersFunc;

	public override BuildingType BuildingType { get; } = BuildingType.Brewery;

	public bool HasBeer => AvailableBeer > 0;

	public int AvailableBeer { get; set; }

	public Brewery(string uniqueId, PlayerColor color, int level, int vpForBuilding, int vpPerLink, int incomeIncrease, bool canBeDeveloped, bool isForCanalEra, bool isForRailEra, int moneyCost, int coalCost, int ironCost, Func<int> numberOfBeersFunc)
		: base(uniqueId, color, level, vpForBuilding, vpPerLink, incomeIncrease, canBeDeveloped, isForCanalEra, isForRailEra, moneyCost, coalCost, ironCost)
	{
		_numberOfBeersFunc = numberOfBeersFunc;
	}

	public int BeerPrice(int amount = 1)
	{
		return 0;
	}

	public void ConsumeBeer(int amount = 1)
	{
		if (AvailableBeer < amount)
		{
			throw new InvalidOperationException("Not enough Beer to consume.");
		}
		AvailableBeer -= amount;
	}

	public override void Build()
	{
		AvailableBeer = _numberOfBeersFunc();
	}

	public override int GetResource()
	{
		return AvailableBeer;
	}
}
