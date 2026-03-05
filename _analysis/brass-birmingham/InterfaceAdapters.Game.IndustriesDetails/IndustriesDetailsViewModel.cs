using System.Collections.Generic;
using BoardGameRules.Entities.Players;
using InterfaceAdapters.Game.IndustriesSummary;

namespace InterfaceAdapters.Game.IndustriesDetails;

public class IndustriesDetailsViewModel
{
	public PlayerColor Color { get; set; }

	public List<IndustryViewModel> CoalMineList { get; set; }

	public List<IndustryViewModel> BreweryList { get; set; }

	public List<IndustryViewModel> PotteryList { get; set; }

	public List<IndustryViewModel> ManufactureList { get; set; }

	public List<IndustryViewModel> IronWorksList { get; set; }

	public List<IndustryViewModel> CottonMillList { get; set; }

	public int CoalMineHighestLevelIndex { get; set; }

	public int BreweryHighestLevelIndex { get; set; }

	public int IronWorksHighestLevelIndex { get; set; }

	public int ManufactureHighestLevelIndex { get; set; }

	public int CottonMillHighestLevelIndex { get; set; }

	public int PotteryHighestLevelIndex { get; set; }
}
