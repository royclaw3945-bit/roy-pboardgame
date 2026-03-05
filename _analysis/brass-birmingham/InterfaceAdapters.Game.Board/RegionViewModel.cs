using System;
using System.Collections.Generic;
using BoardGameRules.Entities.Board.Regions;

namespace InterfaceAdapters.Game.Board;

public class RegionViewModel
{
	public RegionName Name { get; set; }

	public RegionColor Color { get; set; }

	public IList<BuildingSlotViewModel> BuildingSlotsViewModels { get; set; }

	public bool RegionsInteractive { get; set; }

	public string FriendlyName { get; set; }

	public Action<RegionName> ActionOnClick { get; set; } = delegate
	{
	};
}
