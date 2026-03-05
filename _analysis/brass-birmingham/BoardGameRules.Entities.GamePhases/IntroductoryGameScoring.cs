using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.EventSourcing.Aggregates;
using BoardGameRules.Entities.Game;
using BoardGameRules.Entities.GamePhases.Events;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.NumericalResources;
using BoardGameRules.Entities.NumericalResources.Events;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.GamePhases;

public class IntroductoryGameScoring : AggregateRootClient
{
	private readonly PlayersTrack _playersTrack;

	private readonly BoardGameRules.Entities.Board.Board _board;

	private readonly GameProgressInformation _gameProgressInformation;

	public IntroductoryGameScoring(IAggregateRoot aggregateRoot, PlayersTrack playersTrack, BoardGameRules.Entities.Board.Board board, GameProgressInformation gameProgressInformation)
		: base(aggregateRoot)
	{
		_playersTrack = playersTrack;
		_board = board;
		_gameProgressInformation = gameProgressInformation;
		_aggregateRoot.RegisterApplyEvent<ScoreAdditionalMoneyPhaseStarted>(ApplyEvent);
		_aggregateRoot.RegisterApplyEvent<ScoreAdditionalIncomePhaseStarted>(ApplyEvent);
		_aggregateRoot.RegisterApplyEvent<ScoreAdditionalIndustryPhaseStarted>(ApplyEvent);
	}

	public void CalculateAdditionalPoints()
	{
		_aggregateRoot.ApplyEvent(new ScoreAdditionalMoneyPhaseStarted());
		IDictionary<PlayerColor, int> dictionary = AddVPForMoney();
		_aggregateRoot.ApplyEvent(new ScoreAdditionalIncomePhaseStarted());
		AddVPForIncome();
		_aggregateRoot.ApplyEvent(new ScoreAdditionalIndustryPhaseStarted());
		IDictionary<PlayerColor, int> dictionary2 = AddVPForIndustries();
		Dictionary<PlayerColor, (int, int, int)> dictionary3 = new Dictionary<PlayerColor, (int, int, int)>();
		foreach (Player player in _playersTrack.Players)
		{
			dictionary3.Add(player.Color, (dictionary[player.Color], player.IncomeLevel, dictionary2[player.Color]));
		}
		_gameProgressInformation.IntroductoryPoints = dictionary3;
	}

	private IDictionary<PlayerColor, int> AddVPForMoney()
	{
		IDictionary<PlayerColor, int> dictionary = new Dictionary<PlayerColor, int>();
		foreach (Player player in _playersTrack.Players)
		{
			int num = Math.Min(15, player.Money / 4);
			dictionary.Add(player.Color, num);
			_aggregateRoot.ApplyEvent(new IntroductoryGameAdditionalMoneyVictoryPointsChanged(player.Color, num));
		}
		return dictionary;
	}

	private void AddVPForIncome()
	{
		foreach (Player player in _playersTrack.Players)
		{
			_aggregateRoot.ApplyEvent(new IntroductoryGameAdditionalIncomeVictoryPointsChanged(player.Color, player.IncomeLevel));
		}
	}

	private IDictionary<PlayerColor, int> AddVPForIndustries()
	{
		IEnumerable<BaseIndustry> enumerable = _board.Regions.SelectMany((Region r) => from s in r.BuildingSlots
			where s.BuildedIndustry != null && s.BuildedIndustry.Level >= 2
			select s.BuildedIndustry);
		IDictionary<PlayerColor, int> dictionary = new Dictionary<PlayerColor, int>();
		foreach (Player player in _playersTrack.Players)
		{
			dictionary.Add(player.Color, 0);
		}
		foreach (BaseIndustry item in enumerable)
		{
			dictionary[item.Color] += item.VPForBuilding;
		}
		foreach (Player player2 in _playersTrack.Players)
		{
			_aggregateRoot.ApplyEvent(new IntroductoryGameAdditionalIndustryVictoryPointsChanged(player2.Color, dictionary[player2.Color]));
		}
		return dictionary;
	}

	private void ApplyEvent(ScoreAdditionalMoneyPhaseStarted scoreAdditionalMoneyPhaseStarted)
	{
	}

	private void ApplyEvent(ScoreAdditionalIncomePhaseStarted scoreAdditionalIncomePhaseStarted)
	{
	}

	private void ApplyEvent(ScoreAdditionalIndustryPhaseStarted scoreAdditionalIncomePhaseStarted)
	{
	}
}
