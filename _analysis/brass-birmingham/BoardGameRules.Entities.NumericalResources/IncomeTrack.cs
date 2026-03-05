using System;
using BoardGameRules.Entities.EventSourcing.Aggregates;
using BoardGameRules.Entities.NumericalResources.Events;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.NumericalResources;

public class IncomeTrack : AggregateRootClient
{
	private PlayersTrack _playersTrack { get; }

	public IncomeTrack(IAggregateRoot aggregateRoot, PlayersTrack playersTrack)
		: base(aggregateRoot)
	{
		_playersTrack = playersTrack;
		aggregateRoot.RegisterApplyEvent<IncomeMarkerPositionChanged>(ApplyEvent);
		aggregateRoot.RegisterApplyEvent<IncomeLevelChanged>(ApplyEvent);
	}

	public static int GetCurrentIncomeLevel(int marker)
	{
		if (marker < 11)
		{
			return marker - 10;
		}
		if (marker < 31)
		{
			return (int)Math.Ceiling((float)(marker - 10) / 2f);
		}
		if (marker < 61)
		{
			return (int)Math.Ceiling((float)marker / 3f);
		}
		return Math.Min(30, (int)Math.Ceiling((float)(marker + 20) / 4f));
	}

	public static int GetIncomeThreshold(int marker)
	{
		if (marker < 11)
		{
			return marker;
		}
		if (marker < 31)
		{
			return marker + (marker - 10) % 2;
		}
		if (marker < 61)
		{
			return marker - 1 + (3 - (marker - 1) % 3);
		}
		return Math.Min(99, marker - 1 + (4 - (marker - 1) % 4));
	}

	public static int ChangeIncomeMarkerPositionBy(int marker, int numberOfPoints)
	{
		return Math.Min(99, Math.Max(marker + numberOfPoints, 0));
	}

	public static int DecreaseIncomeLevelBy(int marker, int numberOfLevels)
	{
		int currentIncomeLevel = GetCurrentIncomeLevel(marker);
		if (currentIncomeLevel - numberOfLevels < -10)
		{
			throw new InvalidOperationException();
		}
		while (GetCurrentIncomeLevel(marker) != currentIncomeLevel - numberOfLevels)
		{
			marker--;
		}
		return marker;
	}

	private void ApplyEvent(IncomeMarkerPositionChanged incomeMarkerPositionChanged)
	{
		Player playerByColor = _playersTrack.GetPlayerByColor(incomeMarkerPositionChanged.Player);
		playerByColor.IncomeMarkerPosition = ChangeIncomeMarkerPositionBy(playerByColor.IncomeMarkerPosition, incomeMarkerPositionChanged.NumberOfPoints);
	}

	private void ApplyEvent(IncomeLevelChanged incomeLevelChanged)
	{
		Player playerByColor = _playersTrack.GetPlayerByColor(incomeLevelChanged.Player);
		playerByColor.IncomeMarkerPosition = DecreaseIncomeLevelBy(playerByColor.IncomeMarkerPosition, incomeLevelChanged.NumberOfLevels);
	}
}
