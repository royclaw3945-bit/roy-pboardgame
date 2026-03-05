using System;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Common;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.Industries;

public class CoalMine : BaseIndustry, ICoalSource
{
	private int maxCoal;

	public override BuildingType BuildingType { get; } = BuildingType.CoalMine;

	public bool HasCoal => AvailableCoal > 0;

	public int AvailableCoal { get; set; }

	public CoalMine(string uniqueId, PlayerColor color, int level, int vpForBuilding, int vpPerLink, int incomeIncrease, bool canBeDeveloped, bool isForCanalEra, bool isForRailEra, int moneyCost, int coalCost, int ironCost, int numberOfCoal)
		: base(uniqueId, color, level, vpForBuilding, vpPerLink, incomeIncrease, canBeDeveloped, isForCanalEra, isForRailEra, moneyCost, coalCost, ironCost)
	{
		AvailableCoal = numberOfCoal;
		maxCoal = numberOfCoal;
	}

	public int CoalPrice(int amount)
	{
		return 0;
	}

	public void ConsumeCoal(int amount)
	{
		if (AvailableCoal < amount)
		{
			throw new InvalidOperationException("Not enough coal to consume.");
		}
		AvailableCoal -= amount;
	}

	public override int GetResource()
	{
		return AvailableCoal;
	}

	public override int GetMaxResource()
	{
		return maxCoal;
	}
}
