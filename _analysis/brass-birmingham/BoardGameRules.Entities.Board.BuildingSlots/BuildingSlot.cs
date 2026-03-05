using System;
using System.Collections.Generic;
using BoardGameRules.Entities.Industries;

namespace BoardGameRules.Entities.Board.BuildingSlots;

public class BuildingSlot
{
	public string UniqueId { get; }

	public List<BuildingType> BuildingTypes { get; }

	public BaseIndustry BuildedIndustry { get; set; }

	public BuildingSlot()
	{
	}

	public BuildingSlot(string uniqueId, BuildingType buildingType)
	{
		UniqueId = uniqueId;
		BuildingTypes = new List<BuildingType> { buildingType };
	}

	public BuildingSlot(string uniqueId, BuildingType firstType, BuildingType secondType)
	{
		UniqueId = uniqueId;
		BuildingTypes = new List<BuildingType> { firstType, secondType };
	}

	public bool CanBuildIndustryType(BuildingType type)
	{
		return BuildingTypes.Contains(type);
	}

	public void BuildIndustry(BaseIndustry industry)
	{
		if (!CanBuildIndustryType(industry.BuildingType))
		{
			throw new InvalidOperationException();
		}
		BuildedIndustry = industry;
	}

	public string GetCurrentImageName()
	{
		object obj;
		if (BuildedIndustry == null)
		{
			obj = BuildedIndustry?.BuildingType.ToString();
			if (obj == null)
			{
				return GetBackgroundImageName();
			}
		}
		else
		{
			obj = string.Concat(BuildedIndustry.Color, BuildedIndustry?.BuildingType, BuildedIndustry.IsFlipped ? "Flipped" : "");
		}
		return (string)obj;
	}

	public string GetBackgroundImageName()
	{
		return string.Concat(BuildingTypes[0], (BuildingTypes.Count > 1) ? BuildingTypes[1].ToString() : "", "Type");
	}

	public override bool Equals(object obj)
	{
		if (obj == null || GetType() != obj.GetType())
		{
			return false;
		}
		BuildingSlot buildingSlot = (BuildingSlot)obj;
		return UniqueId == buildingSlot.UniqueId;
	}

	public override int GetHashCode()
	{
		return UniqueId.GetHashCode();
	}

	public override string ToString()
	{
		string text = string.Empty;
		foreach (BuildingType buildingType in BuildingTypes)
		{
			if (text != string.Empty)
			{
				text += "/";
			}
			text += buildingType;
		}
		return text;
	}
}
