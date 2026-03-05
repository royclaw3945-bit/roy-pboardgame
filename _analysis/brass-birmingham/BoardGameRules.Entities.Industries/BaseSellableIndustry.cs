using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.Industries;

public abstract class BaseSellableIndustry : BaseIndustry
{
	public int CostOfBeerBarrelsToSellIndustry { get; }

	public BaseSellableIndustry(string uniqueId, PlayerColor color, int level, int vpForBuilding, int vpPerLink, int incomeIncrease, bool canBeDeveloped, bool isForCanalEra, bool isForRailEra, int moneyCost, int coalCost, int ironCost, int costOfBeerBarrelsToSellIndustry)
		: base(uniqueId, color, level, vpForBuilding, vpPerLink, incomeIncrease, canBeDeveloped, isForCanalEra, isForRailEra, moneyCost, coalCost, ironCost)
	{
		CostOfBeerBarrelsToSellIndustry = costOfBeerBarrelsToSellIndustry;
	}

	public override int GetBeerNeeded()
	{
		return CostOfBeerBarrelsToSellIndustry;
	}
}
