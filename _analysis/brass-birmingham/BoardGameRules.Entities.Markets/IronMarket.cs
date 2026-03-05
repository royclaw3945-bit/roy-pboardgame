using BoardGameRules.Entities.Common;

namespace BoardGameRules.Entities.Markets;

public class IronMarket : BaseMarket, IIronSource
{
	public string UniqueId => "IronMarket";

	protected override int PriceOverLimit => 6;

	protected override int StartAmount => 8;

	protected override int MaxAmount => 10;

	public bool HasIron => true;

	public int AvailableIron => Amount;

	public int IronPrice(int amount = 1)
	{
		return GetBuyingPrice(amount);
	}

	public void ConsumeIron(int amount)
	{
		Buy(amount);
	}
}
