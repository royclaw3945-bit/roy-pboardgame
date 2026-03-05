using BoardGameRules.Entities.Common;

namespace BoardGameRules.Entities.Cards;

public class WildLocationCard : BaseCard
{
	public WildLocationCard(string uniqueId, NumberOfPlayers minNumberOfPlayers)
		: base(uniqueId, minNumberOfPlayers)
	{
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (!(obj is WildLocationCard))
		{
			return false;
		}
		if (base.MinNumberOfPlayers == ((WildLocationCard)obj).MinNumberOfPlayers)
		{
			return base.UniqueId == ((WildLocationCard)obj).UniqueId;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return base.UniqueId.GetHashCode();
	}

	public override string ToString()
	{
		return "WildLocationCard";
	}
}
