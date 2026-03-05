using System;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Common;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.Industries;

public class IronWorks : BaseIndustry, IIronSource
{
	private int _maxIron;

	public override BuildingType BuildingType { get; } = BuildingType.IronWorks;

	public bool HasIron => AvailableIron > 0;

	public int AvailableIron { get; set; }

	public IronWorks(string uniqueId, PlayerColor color, int level, int vpForBuilding, int vpPerLink, int incomeIncrease, bool canBeDeveloped, bool isForCanalEra, bool isForRailEra, int moneyCost, int coalCost, int ironCost, int numberOfIron)
		: base(uniqueId, color, level, vpForBuilding, vpPerLink, incomeIncrease, canBeDeveloped, isForCanalEra, isForRailEra, moneyCost, coalCost, ironCost)
	{
		AvailableIron = numberOfIron;
		_maxIron = numberOfIron;
	}

	public int IronPrice(int amount = 1)
	{
		return 0;
	}

	public void ConsumeIron(int amount)
	{
		if (AvailableIron < amount)
		{
			throw new InvalidOperationException("Not enough iron to consume.");
		}
		AvailableIron -= amount;
	}

	public override int GetResource()
	{
		return AvailableIron;
	}

	public override int GetMaxResource()
	{
		return _maxIron;
	}
}
