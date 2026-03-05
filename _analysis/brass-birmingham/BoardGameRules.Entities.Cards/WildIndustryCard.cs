using BoardGameRules.Entities.Common;

namespace BoardGameRules.Entities.Cards;

public class WildIndustryCard : BaseCard
{
	public WildIndustryCard(string uniqueId, NumberOfPlayers minNumberOfPlayers)
		: base(uniqueId, minNumberOfPlayers)
	{
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (!(obj is WildIndustryCard))
		{
			return false;
		}
		if (base.MinNumberOfPlayers == ((WildIndustryCard)obj).MinNumberOfPlayers)
		{
			return base.UniqueId == ((WildIndustryCard)obj).UniqueId;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return base.UniqueId.GetHashCode();
	}

	public override string ToString()
	{
		return "WildIndustryCard";
	}
}
