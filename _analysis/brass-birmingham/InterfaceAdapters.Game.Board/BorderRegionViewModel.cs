using System;
using System.Collections.Generic;
using BoardGameRules.Entities.Board.Regions;

namespace InterfaceAdapters.Game.Board;

public class BorderRegionViewModel
{
	public Action<RegionName, float, float> OnHover = delegate
	{
	};

	public Action OnHoverExit = delegate
	{
	};

	public RegionName Name { get; set; }

	public IList<MerchantSlotViewModel> MerchantSlotsViewModels { get; set; }

	public BonusType BonusType { get; set; }
}
