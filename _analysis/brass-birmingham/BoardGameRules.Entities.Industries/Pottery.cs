using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.Industries;

public class Pottery : BaseSellableIndustry
{
	public override BuildingType BuildingType { get; } = BuildingType.Pottery;

	public int NumberOfBeerBarrelsToConsume { get; }

	public Pottery(string uniqueId, PlayerColor color, int level, int vpForBuilding, int vpPerLink, int incomeIncrease, bool canBeDeveloped, bool isForCanalEra, bool isForRailEra, int moneyCost, int coalCost, int ironCost, int costOfBeerBarrelsToSellIndustry)
		: base(uniqueId, color, level, vpForBuilding, vpPerLink, incomeIncrease, canBeDeveloped, isForCanalEra, isForRailEra, moneyCost, coalCost, ironCost, costOfBeerBarrelsToSellIndustry)
	{
	}
}
