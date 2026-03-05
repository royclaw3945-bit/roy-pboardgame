using System;
using BoardGameRules.Entities.Board.Merchants;

namespace InterfaceAdapters.Game.Board;

public class MerchantSlotViewModel
{
	public MerchantSlot MerchantSlot { get; set; }

	public string MerchantTileImageName { get; set; }

	public bool HasBeer { get; set; }

	public bool Highlighted { get; set; }

	public bool HighlightedBeer { get; set; }

	public bool IsInteractive { get; set; }

	public Action<MerchantSlot> ActionOnClick { get; set; } = delegate
	{
	};

	public Action<MerchantSlot> ActionOnHover { get; set; } = delegate
	{
	};
}
