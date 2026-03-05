using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.Common;
using BoardGameRules.Entities.EventSourcing.Aggregates;
using BoardGameRules.Entities.Game;
using BoardGameRules.Entities.GamePhases.Events;
using BoardGameRules.Entities.NumericalResources;
using BoardGameRules.Entities.NumericalResources.Events;
using BoardGameRules.Entities.Players;
using BoardGameRules.Entities.SellIndustry;
using BoardGameRules.Entities.TurnPhases;
using BoardGameRules.Utils;

namespace BoardGameRules.Entities.GamePhases;

public class GameFlow : AggregateRootClient
{
	private readonly TurnFlow _turnFlow;

	private readonly SellIndustryFlow _sellIndustryFlow;

	private readonly GameSetup _gameSetup;

	private readonly EndOfGame _endOfGame;

	private readonly CanalEra _canalEra;

	private readonly RailEra _railEra;

	private bool _isSellIndustryNecessary;

	private bool _shouldEndRoundBeCalled;

	private GameStartConfiguration GameStartConfiguration { get; }

	private GameProgressInformation GameProgressInformation { get; }

	private PlayersTrack PlayersTrack { get; }

	public int GetNumberOfRounds(NumberOfPlayers numberOfPlayers)
	{
		if (GameStartConfiguration.IsTutorialGame)
		{
			return 8;
		}
		return numberOfPlayers switch
		{
			NumberOfPlayers.Two => 10, 
			NumberOfPlayers.Three => 9, 
			NumberOfPlayers.Four => 8, 
			_ => 10, 
		};
	}

	public GameFlow(IAggregateRoot aggregateRoot, BoardGameRules.Entities.Board.Board board, IRandomer randomer, GameStartConfiguration gameStartConfiguration, GameProgressInformation gameProgressInformation, TurnFlow turnFlow, PlayersTrack playersTrack, CardDealer cardDealer, SellIndustryFlow sellIndustryFlow)
		: base(aggregateRoot)
	{
		GameStartConfiguration = gameStartConfiguration;
		GameProgressInformation = gameProgressInformation;
		PlayersTrack = playersTrack;
		_canalEra = new CanalEra(_aggregateRoot, playersTrack, board, randomer, GameProgressInformation);
		_railEra = new RailEra(_aggregateRoot, gameProgressInformation, playersTrack, board, cardDealer, randomer);
		_gameSetup = new GameSetup(_aggregateRoot, board, randomer, gameStartConfiguration, playersTrack, cardDealer, GameProgressInformation);
		_endOfGame = new EndOfGame(_aggregateRoot, gameProgressInformation, playersTrack, randomer);
		_sellIndustryFlow = sellIndustryFlow;
		_turnFlow = turnFlow;
		_aggregateRoot.RegisterApplyEvent<RoundStarted>(ApplyEvent);
		_aggregateRoot.RegisterApplyEvent<RoundEnded>(ApplyEvent);
		_aggregateRoot.RegisterApplyEvent<TurnStarted>(ApplyEvent);
		_aggregateRoot.RegisterApplyEvent<TurnEnded>(ApplyEvent);
		_aggregateRoot.RegisterApplyEvent<ScoreLinksPhaseStarted>(ApplyEvent);
		_aggregateRoot.RegisterApplyEvent<ScoreFlippedIndustryTilesPhaseStarted>(ApplyEvent);
	}

	public void StartGame()
	{
		_canalEra.StartCanalEra();
		StartRound();
	}

	public void SetupGame()
	{
		_gameSetup.SetupGame();
	}

	public void SetupTutorialGame()
	{
		_gameSetup.SetupTutorialGame();
	}

	private void EndGame(Dictionary<PlayerColor, int> industriesVP, Dictionary<PlayerColor, int> linksVP, Dictionary<PlayerColor, int> lastEraBonusesVPs)
	{
		_endOfGame.EndGame(industriesVP, linksVP, lastEraBonusesVPs);
	}

	private void StartRound()
	{
		_aggregateRoot.ApplyEvent(new RoundStarted(GameProgressInformation.CurrentRoundNumber + 1, GetNumberOfRounds(GameStartConfiguration.NumberOfPlayers), GameProgressInformation.CurrentGameEra));
		StartTurn();
	}

	public bool IsLastTurnOver()
	{
		return _shouldEndRoundBeCalled;
	}

	public bool IsSellIndustryNecessary()
	{
		return _isSellIndustryNecessary;
	}

	private void BeginEndRound()
	{
		_shouldEndRoundBeCalled = true;
		_isSellIndustryNecessary = PlayersTrack.Players.Any((Player p) => p.Money + p.IncomeLevel < 0);
		if (_isSellIndustryNecessary)
		{
			_sellIndustryFlow.StartIndustrySelling();
		}
	}

	public void EndRound()
	{
		_shouldEndRoundBeCalled = false;
		if (_isSellIndustryNecessary)
		{
			_sellIndustryFlow.EndIndustrySelling();
			_isSellIndustryNecessary = false;
		}
		_aggregateRoot.ApplyEvent(new RoundEnded(GameProgressInformation.CurrentGameEra, PlayersTrack.GetNewPlayersOrder()));
		if (IsEndOfCanalEra())
		{
			_canalEra.EndCanalEra(GameStartConfiguration.IsIntroductoryGame);
			_aggregateRoot.ApplyEvent(new NewTurnOrderDetermined());
			_aggregateRoot.ApplyEvent(new IncomeMoneyPaid());
			if (!GameStartConfiguration.IsIntroductoryGame)
			{
				_railEra.StartRailEra();
				StartRound();
			}
			else
			{
				EndGame(_canalEra.GetEndOfEraIndustriesVPs(), _canalEra.GetEndOfEraLinksVPs(), _canalEra.GetCurrentEraBonusesVPs());
			}
		}
		else if (IsEndOfRailEra())
		{
			_railEra.EndRailEra();
			EndGame(_railEra.GetEndOfEraIndustriesVPs(), _railEra.GetEndOfEraLinksVPs(), _railEra.GetCurrentEraBonusesVPs());
		}
		else
		{
			_aggregateRoot.ApplyEvent(new NewTurnOrderDetermined());
			_aggregateRoot.ApplyEvent(new IncomeMoneyPaid());
			StartRound();
		}
	}

	private void StartTurn()
	{
		_aggregateRoot.ApplyEvent(new TurnStarted(GameProgressInformation.CurrentPlayer.Color));
	}

	public bool CanEndTurn()
	{
		if (_turnFlow.GetRemainingActionsCount() != 0)
		{
			return false;
		}
		return true;
	}

	public void EndTurn()
	{
		if (_turnFlow.GetRemainingActionsCount() != 0)
		{
			throw new InvalidOperationException("Can't end turn with remaining player actions.");
		}
		_aggregateRoot.ApplyEvent(new TurnEnded());
		if (GameProgressInformation.CurrentTurnNumber == (int)GameStartConfiguration.NumberOfPlayers)
		{
			BeginEndRound();
		}
		else
		{
			StartTurn();
		}
	}

	private void ApplyEvent(RoundStarted roundEnded)
	{
		GameProgressInformation.CurrentRoundNumber++;
		GameProgressInformation.CurrentTurnNumber = 0;
		GameProgressInformation.CurrentPlayer = PlayersTrack.GetFirstPlayer();
	}

	private void ApplyEvent(RoundEnded roundEnded)
	{
	}

	private void ApplyEvent(TurnStarted turnStarted)
	{
		GameProgressInformation.CurrentTurnNumber++;
		_turnFlow.StartTurn(GameProgressInformation.CurrentPlayer, GameProgressInformation.CurrentRoundNumber, GameProgressInformation.CurrentGameEra);
	}

	private void ApplyEvent(TurnEnded turnEnded)
	{
		_turnFlow.EndTurn();
	}

	private void ApplyEvent(ScoreLinksPhaseStarted scoreLinksPhaseStarted)
	{
	}

	private void ApplyEvent(ScoreFlippedIndustryTilesPhaseStarted scoreFlippedIndustryTilesPhaseStarted)
	{
	}

	private bool IsEndOfCanalEra()
	{
		if (GameProgressInformation.CurrentGameEra == GameEra.CanalEra)
		{
			return GameProgressInformation.CurrentRoundNumber == GetNumberOfRounds(GameStartConfiguration.NumberOfPlayers);
		}
		return false;
	}

	private bool IsEndOfRailEra()
	{
		if (GameProgressInformation.CurrentGameEra == GameEra.RailEra)
		{
			return GameProgressInformation.CurrentRoundNumber == GetNumberOfRounds(GameStartConfiguration.NumberOfPlayers);
		}
		return false;
	}
}
