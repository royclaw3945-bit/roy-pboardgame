using System;
using BoardGameRules.Entities.EventSourcing.Aggregates;
using BoardGameRules.Entities.NumericalResources.Events;

namespace BoardGameRules.Entities.NumericalResources;

public class VictoryPointsTrack : AggregateRootClient
{
	private PlayersTrack _playersTrack { get; }

	public VictoryPointsTrack(IAggregateRoot aggregateRoot, PlayersTrack playersTrack)
		: base(aggregateRoot)
	{
		_playersTrack = playersTrack;
		aggregateRoot.RegisterApplyEvent<VictoryPointsChanged>(ApplyEvent);
		aggregateRoot.RegisterApplyEvent<IntroductoryGameAdditionalIncomeVictoryPointsChanged>(ApplyEvent);
		aggregateRoot.RegisterApplyEvent<IntroductoryGameAdditionalMoneyVictoryPointsChanged>(ApplyEvent);
		aggregateRoot.RegisterApplyEvent<IntroductoryGameAdditionalIndustryVictoryPointsChanged>(ApplyEvent);
		aggregateRoot.RegisterApplyEvent<LinksVictoryPointsChanged>(ApplyEvent);
		aggregateRoot.RegisterApplyEvent<FlippedIndustriesVictoryPointsChanged>(ApplyEvent);
	}

	private void ApplyEvent(VictoryPointsChanged victoryPointsChanged)
	{
		_playersTrack.GetPlayerByColor(victoryPointsChanged.Player).VP += victoryPointsChanged.Amount;
	}

	private void ApplyEvent(IntroductoryGameAdditionalIncomeVictoryPointsChanged introductoryGameAdditionalIncomeVictoryPointsChanged)
	{
		_playersTrack.GetPlayerByColor(introductoryGameAdditionalIncomeVictoryPointsChanged.Player).VP += introductoryGameAdditionalIncomeVictoryPointsChanged.Amount;
	}

	private void ApplyEvent(IntroductoryGameAdditionalMoneyVictoryPointsChanged introductoryGameAdditionalMoneyVictoryPointsChanged)
	{
		_playersTrack.GetPlayerByColor(introductoryGameAdditionalMoneyVictoryPointsChanged.Player).VP = Math.Max(0, _playersTrack.GetPlayerByColor(introductoryGameAdditionalMoneyVictoryPointsChanged.Player).VP + introductoryGameAdditionalMoneyVictoryPointsChanged.Amount);
	}

	private void ApplyEvent(IntroductoryGameAdditionalIndustryVictoryPointsChanged introductoryGameAdditionalIndustryVictoryPointsChanged)
	{
		_playersTrack.GetPlayerByColor(introductoryGameAdditionalIndustryVictoryPointsChanged.Player).VP += introductoryGameAdditionalIndustryVictoryPointsChanged.Amount;
	}

	private void ApplyEvent(LinksVictoryPointsChanged linksVictoryPointsChanged)
	{
		_playersTrack.GetPlayerByColor(linksVictoryPointsChanged.Player).VP += linksVictoryPointsChanged.Amount;
	}

	private void ApplyEvent(FlippedIndustriesVictoryPointsChanged flippedIndustriesVictoryPointsChanged)
	{
		_playersTrack.GetPlayerByColor(flippedIndustriesVictoryPointsChanged.Player).VP += flippedIndustriesVictoryPointsChanged.Amount;
	}
}
