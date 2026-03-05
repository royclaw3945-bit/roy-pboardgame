using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Common;

namespace BoardGameRules.Entities.Cards;

public class LocationCard : BaseCard
{
	public RegionName Region { get; }

	public RegionColor Color { get; }

	public LocationCard(string uniqueId, NumberOfPlayers minNumberOfPlayers, RegionName region, RegionColor color)
		: base(uniqueId, minNumberOfPlayers)
	{
		Region = region;
		Color = color;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (!(obj is LocationCard))
		{
			return false;
		}
		if (Region == ((LocationCard)obj).Region && Color == ((LocationCard)obj).Color && base.MinNumberOfPlayers == ((LocationCard)obj).MinNumberOfPlayers)
		{
			return base.UniqueId == ((LocationCard)obj).UniqueId;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return ((int)Region * 397) ^ (int)Color;
	}

	public override string ToString()
	{
		return string.Concat(Region, " (", Color, ")");
	}
}
