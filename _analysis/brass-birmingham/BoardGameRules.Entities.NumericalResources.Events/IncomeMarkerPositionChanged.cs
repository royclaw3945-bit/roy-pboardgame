using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.NumericalResources.Events;

public class IncomeMarkerPositionChanged : BaseEvent
{
	public PlayerColor Player { get; }

	public int NumberOfPoints { get; }

	public string SourceUniqueId { get; }

	public IncomeMarkerPositionChanged(PlayerColor player, int numberOfPoints, string sourceUniqueId)
	{
		Player = player;
		NumberOfPoints = numberOfPoints;
		SourceUniqueId = sourceUniqueId;
	}
}
