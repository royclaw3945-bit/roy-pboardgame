using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.EventSourcing.Events;

namespace BoardGameRules.Entities.Actions.Events;

public class IndustryFlipped : BaseEvent
{
	public string IndustryUniqueId { get; }

	public RegionName RegionName { get; }

	public IndustryFlipped(string industryUniqueId, RegionName regionName)
	{
		IndustryUniqueId = industryUniqueId;
		RegionName = regionName;
	}
}
