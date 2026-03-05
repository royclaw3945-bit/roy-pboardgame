using System.Collections.Generic;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Merchants;

namespace BoardGameRules.Entities.Board.Regions;

public class BorderRegion : Region
{
	public BonusType MerchantBeerBonusType { get; }

	public List<MerchantSlot> MerchantSlots { get; }

	public BorderRegion(RegionName name, BonusType merchantBeerBonusType, List<MerchantSlot> merchantSlots)
		: base(name, RegionColor.Gray, new List<BuildingSlot>())
	{
		MerchantBeerBonusType = merchantBeerBonusType;
		MerchantSlots = merchantSlots;
	}
}
