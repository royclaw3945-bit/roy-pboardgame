using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.EventSourcing.Aggregates;
using BoardGameRules.Entities.GamePhases;
using BoardGameRules.Entities.GamePhases.Events;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.NumericalResources;
using BoardGameRules.Entities.SellIndustry;
using BoardGameRules.Entities.TurnPhases;
using BoardGameRules.Entities.Tutorial.Cards;
using BoardGameRules.Utils;

namespace BoardGameRules.Entities.Game;

public class BoardGameRulesGame : AggregateRoot
{
	public GameStartConfiguration GameStartConfiguration { get; private set; }

	public GameProgressInformation GameProgressInformation { get; private set; }

	public Randomer Randomer { get; private set; }

	public VictoryPointsTrack VictoryPointsTrack { get; private set; }

	public IncomeTrack IncomeTrack { get; private set; }

	public TurnFlow TurnFlow { get; private set; }

	public GameFlow GameFlow { get; private set; }

	public BoardGameRules.Entities.Board.Board Board { get; private set; }

	public Bank Bank { get; private set; }

	public PlayersTrack PlayersTrack { get; private set; }

	public CardDealer CardDealer { get; private set; }

	public SellIndustryFlow SellIndustryFlow { get; private set; }

	public DevelopMerchantBonus DevelopMerchantBonus { get; private set; }

	public BoardGameRulesGame(GameStartConfiguration gameStartConfiguration)
	{
		GameStartConfiguration = gameStartConfiguration;
		RegisterApplyEvent<GameStarted>(ApplyEventGameStarted);
	}

	public BoardGameRulesGame(BoardGameRulesGame boardGameRulesGame)
	{
	}

	public BaseIndustry GetIndustryByUniqueId(string uniqueId)
	{
		return Board.GetIndustryByUniqueId(uniqueId) ?? PlayersTrack.GetIndustryByUniqueId(uniqueId);
	}

	public void StartGame()
	{
		ApplyEvent(new GameStarted(GameStartConfiguration));
		GameFlow.StartGame();
	}

	private void ApplyEventGameStarted(GameStarted gameStarted)
	{
		Init(gameStarted.GameStartConfiguration);
		if (!gameStarted.GameStartConfiguration.IsTutorialGame)
		{
			GameFlow.SetupGame();
		}
		else
		{
			GameFlow.SetupTutorialGame();
		}
		GameProgressInformation.CurrentPlayer = PlayersTrack.GetFirstPlayer();
	}

	public void Init(GameStartConfiguration gameStartConfiguration)
	{
		Randomer = new Randomer(new RandomerData(gameStartConfiguration?.Seed ?? 0));
		GameProgressInformation gameProgressInformation = (GameProgressInformation = new GameProgressInformation());
		PlayersTrack = new PlayersTrack(this);
		PlayersTrack.SetPlayerStartConfigurations(gameStartConfiguration.Players);
		Board = new BoardGameRules.Entities.Board.Board(this, PlayersTrack, Randomer);
		if (!gameStartConfiguration.IsTutorialGame)
		{
			CardDealer = new CardDealer(Board.CardDeck);
		}
		else
		{
			CardDealer = new TutorialCardDealer(Board.CardDeck);
		}
		SellIndustryFlow = new SellIndustryFlow(this, Board, PlayersTrack);
		DevelopMerchantBonus = new DevelopMerchantBonus(this);
		TurnFlow = new TurnFlow(this, PlayersTrack, Board, CardDealer, gameProgressInformation);
		GameFlow = new GameFlow(this, Board, Randomer, gameStartConfiguration, gameProgressInformation, TurnFlow, PlayersTrack, CardDealer, SellIndustryFlow);
		IncomeTrack = new IncomeTrack(this, PlayersTrack);
		VictoryPointsTrack = new VictoryPointsTrack(this, PlayersTrack);
		Bank = new Bank(this, PlayersTrack);
	}
}
