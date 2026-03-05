using BoardGameRules.Entities.Common;

namespace BoardGameRules.Entities.Cards;

public abstract class BaseCard
{
	public string UniqueId { get; }

	public NumberOfPlayers MinNumberOfPlayers { get; }

	protected BaseCard(string uniqueId, NumberOfPlayers minNumberOfPlayers)
	{
		UniqueId = uniqueId;
		MinNumberOfPlayers = minNumberOfPlayers;
	}

	public override string ToString()
	{
		return base.ToString();
	}
}
