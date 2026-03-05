using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Common;
using BoardGameRules.Utils;

namespace BoardGameRules.Entities.Board.Merchants;

public static class MerchantTilesFactory
{
	public static List<MerchantTile> GenerateMerchantTilesForGivenNumberOfPlayers(NumberOfPlayers numberOfPlayers, IRandomer randomer)
	{
		return (from t in GenerateMerchantTiles()
			where t.MinNumberOfPlayers <= numberOfPlayers
			orderby randomer.Next()
			select t).ToList();
	}

	private static List<MerchantTile> GenerateMerchantTiles()
	{
		return new List<MerchantTile>
		{
			new MerchantTile(NumberOfPlayers.Two, new List<BuildingType> { BuildingType.Manufacturer }),
			new MerchantTile(NumberOfPlayers.Two, new List<BuildingType> { BuildingType.CottonMill }),
			new MerchantTile(NumberOfPlayers.Two, new List<BuildingType>()),
			new MerchantTile(NumberOfPlayers.Two, new List<BuildingType>()),
			new MerchantTile(NumberOfPlayers.Two, new List<BuildingType>
			{
				BuildingType.CottonMill,
				BuildingType.Pottery,
				BuildingType.Manufacturer
			}),
			new MerchantTile(NumberOfPlayers.Three, new List<BuildingType>()),
			new MerchantTile(NumberOfPlayers.Three, new List<BuildingType> { BuildingType.Pottery }),
			new MerchantTile(NumberOfPlayers.Four, new List<BuildingType> { BuildingType.Manufacturer }),
			new MerchantTile(NumberOfPlayers.Four, new List<BuildingType> { BuildingType.CottonMill })
		};
	}
}
