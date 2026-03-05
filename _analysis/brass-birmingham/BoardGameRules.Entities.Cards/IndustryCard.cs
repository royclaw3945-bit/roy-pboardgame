using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Common;

namespace BoardGameRules.Entities.Cards;

public class IndustryCard : BaseCard
{
	public List<BuildingType> BuildingTypes { get; }

	public int ImageTypeIndex { get; }

	public IndustryCard(string uniqueId, NumberOfPlayers minNumberOfPlayers, List<BuildingType> buildingTypes, int imageTypeIndex = 0)
		: base(uniqueId, minNumberOfPlayers)
	{
		BuildingTypes = buildingTypes;
		ImageTypeIndex = imageTypeIndex;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (!(obj is IndustryCard))
		{
			return false;
		}
		if (ImageTypeIndex == ((IndustryCard)obj).ImageTypeIndex && BuildingTypes.SequenceEqual(((IndustryCard)obj).BuildingTypes) && base.MinNumberOfPlayers == ((IndustryCard)obj).MinNumberOfPlayers)
		{
			return base.UniqueId == ((IndustryCard)obj).UniqueId;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (((BuildingTypes != null) ? BuildingTypes.GetHashCode() : 0) * 397) ^ ImageTypeIndex;
	}

	public override string ToString()
	{
		string text = "Industry[";
		foreach (BuildingType buildingType in BuildingTypes)
		{
			text = text + buildingType.ToString() + " ";
		}
		return text + "]";
	}
}
