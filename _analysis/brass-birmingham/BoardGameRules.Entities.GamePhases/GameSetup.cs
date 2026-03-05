using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.Common;
using BoardGameRules.Entities.EventSourcing.Aggregates;
using BoardGameRules.Entities.Game;
using BoardGameRules.Entities.NumericalResources;
using BoardGameRules.Entities.Players;
using BoardGameRules.Utils;

namespace BoardGameRules.Entities.GamePhases;

public class GameSetup : AggregateRootClient
{
	private readonly BoardGameRules.Entities.Board.Board _board;

	private readonly IRandomer _randomer;

	private readonly GameStartConfiguration _gameStartConfiguration;

	private readonly GameProgressInformation _gameProgressInformation;

	private readonly PlayersTrack _playersTrack;

	private readonly CardDealer _cardDealer;

	public GameSetup(IAggregateRoot aggregateRoot, BoardGameRules.Entities.Board.Board board, IRandomer randomer, GameStartConfiguration gameStartConfiguration, PlayersTrack playersTrack, CardDealer cardDealer, GameProgressInformation gameProgressInformation)
		: base(aggregateRoot)
	{
		_board = board;
		_randomer = randomer;
		_gameStartConfiguration = gameStartConfiguration;
		_playersTrack = playersTrack;
		_cardDealer = cardDealer;
		_gameProgressInformation = gameProgressInformation;
	}

	public void SetupGame()
	{
		SetupBoard();
		SetupPlayers();
	}

	public void SetupTutorialGame()
	{
		SetupTutorialBoard();
		SetupPlayers();
	}

	private void SetupBoard()
	{
		NumberOfPlayers numberOfPlayers = _gameStartConfiguration.NumberOfPlayers;
		_board.PrepareCardDecks(numberOfPlayers, _randomer);
		_board.PrepareMerchantTiles(numberOfPlayers, _randomer);
	}

	private void SetupTutorialBoard()
	{
		_board.PrepareCardDecks(NumberOfPlayers.Four, _randomer);
		_board.PrepareMerchantTiles(NumberOfPlayers.Four, _randomer);
	}

	private void SetupPlayers()
	{
		_playersTrack.CreatePlayers(_aggregateRoot, _gameStartConfiguration.NumberOfPlayers);
		foreach (Player player in _playersTrack.Players)
		{
			player.Money = 17;
			player.IncomeMarkerPosition = 10;
			player.Mat = PlayerMat.PrepareMat(player, _randomer, _gameProgressInformation);
		}
		_cardDealer.DealCardsForStartOfEra(_playersTrack.Players);
		_cardDealer.TakeOneCardFromDeckAtGameStartAndDiscardIt(_playersTrack.Players);
		_playersTrack.RandomizePlayersOrder(_randomer);
	}
}
