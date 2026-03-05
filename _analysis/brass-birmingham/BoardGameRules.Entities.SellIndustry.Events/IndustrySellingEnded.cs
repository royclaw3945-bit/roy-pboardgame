using System.Collections.Generic;
using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.SellIndustry.Events;

public class IndustrySellingEnded : BaseEvent
{
	public Dictionary<PlayerColor, List<BaseIndustry>> SoldInLastRound { get; set; }

	public Dictionary<PlayerColor, int> LostVpsInLastRound { get; set; }

	public IndustrySellingEnded(Dictionary<PlayerColor, List<BaseIndustry>> soldInLastRound, Dictionary<PlayerColor, int> lostVpsInLastRound)
	{
		SoldInLastRound = soldInLastRound;
		LostVpsInLastRound = lostVpsInLastRound;
	}
}
