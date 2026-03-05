using System.Collections.Generic;
using InterfaceAdapters.Game.IndustriesSummary;

namespace InterfaceAdapters.Game.RegionDetails;

public class RegionDetailsViewModel
{
	public object Region { get; set; }

	public List<List<IndustryViewModel>> Slots { get; set; }

	public bool ShowBackButton { get; set; }
}
