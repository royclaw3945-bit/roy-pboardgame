using System.Collections.Generic;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Links;

namespace BoardGameRules.Entities.Board.Regions;

public class Region
{
	public List<Link> Links { get; } = new List<Link>();

	public List<BuildingSlot> BuildingSlots { get; }

	public RegionName Name { get; }

	public RegionColor Color { get; }

	public Region(RegionName name, RegionColor color, List<BuildingSlot> buildingSlots)
	{
		Name = name;
		Color = color;
		BuildingSlots = buildingSlots;
	}

	public override bool Equals(object obj)
	{
		if (obj == null || GetType() != obj.GetType())
		{
			return false;
		}
		Region region = (Region)obj;
		return Name == region.Name;
	}

	public override int GetHashCode()
	{
		return Name.GetHashCode();
	}

	public override string ToString()
	{
		return string.Concat(Name, "(", Color, ")");
	}
}
