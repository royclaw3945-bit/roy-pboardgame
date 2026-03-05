using BoardGameRules.Entities.EventSourcing.Events;

namespace BoardGameRules.Entities.GamePhases.Events;

public class RoundStarted : BaseEvent
{
	public int RoundNumber { get; }

	public int MaxRounds { get; }

	public GameEra GameEra { get; }

	public RoundStarted(int roundNumber, int maxRounds, GameEra gameEra)
	{
		RoundNumber = roundNumber;
		MaxRounds = maxRounds;
		GameEra = gameEra;
	}
}
