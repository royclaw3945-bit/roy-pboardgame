using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Players;

namespace InterfaceAdapters.Game.IndustriesSummary;

public class IndustryViewModel
{
	public BuildingType BuildingType { get; set; }

	public BuildingSlot BuildingSlot { get; set; }

	public bool Active { get; set; }

	public bool IndustryAvailable { get; set; }

	public bool CanBeDeveloped { get; set; }

	public bool OnlyCanalEra { get; set; }

	public bool OnlyRailEra { get; set; }

	public int Level { get; set; }

	public int SameLevelCount { get; set; }

	public int MoneyCost { get; set; }

	public int VPForBuilding { get; set; }

	public int IncomeMarkerIncrease { get; set; }

	public int VpPerLink { get; set; }

	public int CoalCost { get; set; }

	public int IronCost { get; set; }

	public int BeerCost { get; set; }

	public int CoalSource { get; set; }

	public int IronSource { get; set; }

	public int BeerSource { get; set; }

	public bool HasCoal { get; set; }

	public bool HasIron { get; set; }

	public bool HasBeer { get; set; }

	public PlayerColor Color { get; set; }

	public string CantBuildReason { get; set; }

	public string Name { get; set; }
}
