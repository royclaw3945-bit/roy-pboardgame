using System.Collections.Generic;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Common;

namespace BoardGameRules.Entities.Board.Merchants;

public class MerchantTile
{
	public NumberOfPlayers MinNumberOfPlayers { get; }

	public List<BuildingType> BuildingTypes { get; }

	public MerchantTile(NumberOfPlayers minNumberOfPlayers, List<BuildingType> buildingTypes)
	{
		MinNumberOfPlayers = minNumberOfPlayers;
		BuildingTypes = buildingTypes;
	}
}
