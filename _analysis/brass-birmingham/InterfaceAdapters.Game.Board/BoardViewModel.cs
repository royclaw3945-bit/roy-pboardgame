using System.Collections.Generic;
using BoardGameRules.Entities.Board.Regions;

namespace InterfaceAdapters.Game.Board;

public class BoardViewModel
{
	public IDictionary<RegionName, RegionViewModel> RegionsViewModels { get; set; }

	public IDictionary<RegionName, BorderRegionViewModel> BorderRegionsViewModels { get; set; }

	public IDictionary<string, LinkViewModel> Links { get; set; }
}
