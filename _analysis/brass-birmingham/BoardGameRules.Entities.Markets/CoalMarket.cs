using BoardGameRules.Entities.Common;

namespace BoardGameRules.Entities.Markets;

public class CoalMarket : BaseMarket, ICoalSource
{
	public string UniqueId => "CoalMarket";

	protected override int PriceOverLimit => 8;

	protected override int StartAmount => 13;

	protected override int MaxAmount => 14;

	public bool HasCoal => true;

	public int AvailableCoal => Amount;

	public int CoalPrice(int amount)
	{
		return GetBuyingPrice(amount);
	}

	public void ConsumeCoal(int amount)
	{
		Buy(amount);
	}
}
