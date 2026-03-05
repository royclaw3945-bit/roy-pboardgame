using System.ComponentModel;

namespace BoardGameRules.Entities.Board.BuildingSlots;

public enum BuildingType
{
	Undefined,
	Brewery,
	[Description("Coal Mine")]
	CoalMine,
	[Description("Iron Works")]
	IronWorks,
	[Description("Cotton Mill")]
	CottonMill,
	Manufacturer,
	Pottery
}
