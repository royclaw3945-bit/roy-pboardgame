using System;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.Consumables;
using BoardGameRules.Entities.EventSourcing.Aggregates;
using BoardGameRules.Entities.Game;
using BoardGameRules.Entities.GamePhases;
using BoardGameRules.Entities.NumericalResources;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.TurnPhases;

public class TurnFlow : AggregateRootClient
{
	private int RemainingActionsCount;

	public IGameAction CurrentAction { get; private set; }

	private PlayersTrack PlayersTrack { get; }

	private CardDealer CardDealer { get; }

	private GameProgressInformation GameProgressInformation { get; }

	private Player CurPlayer { get; set; }

	public GeneralAction GeneralAction { get; private set; }

	public BuildAction BuildAction { get; private set; }

	public PassAction PassAction { get; private set; }

	public LoanAction LoanAction { get; private set; }

	public ScoutAction ScoutAction { get; private set; }

	public DevelopAction DevelopAction { get; private set; }

	public NetworkAction NetworkAction { get; private set; }

	public SellAction SellAction { get; private set; }

	private ConsumingCoal ConsumingCoal { get; }

	private ConsumingIron ConsumingIron { get; }

	private ConsumingBeer ConsumingBeer { get; }

	private int MaxActionsCount
	{
		get
		{
			if (GameProgressInformation.CurrentRoundNumber != 1 || GameProgressInformation.CurrentGameEra != GameEra.CanalEra)
			{
				return 2;
			}
			return 1;
		}
	}

	public TurnFlow(IAggregateRoot aggregateRoot, PlayersTrack playersTrack, BoardGameRules.Entities.Board.Board board, CardDealer cardDealer, GameProgressInformation gameProgressInformation)
		: base(aggregateRoot)
	{
		PlayersTrack = playersTrack;
		CardDealer = cardDealer;
		GameProgressInformation = gameProgressInformation;
		ConsumingCoal = new ConsumingCoal(_aggregateRoot, board, playersTrack);
		ConsumingIron = new ConsumingIron(_aggregateRoot, board, playersTrack);
		ConsumingBeer = new ConsumingBeer(_aggregateRoot, board, playersTrack);
		GeneralAction = new GeneralAction(_aggregateRoot, PlayersTrack, board, DecreaseRemainingActionsCount, ConsumingCoal, ConsumingIron, ConsumingBeer);
		BuildAction = new BuildAction(_aggregateRoot, PlayersTrack, board, GeneralAction, gameProgressInformation);
		PassAction = new PassAction(_aggregateRoot, PlayersTrack, board, GeneralAction);
		LoanAction = new LoanAction(_aggregateRoot, PlayersTrack, board, GeneralAction);
		ScoutAction = new ScoutAction(_aggregateRoot, PlayersTrack, board, GeneralAction);
		DevelopAction = new DevelopAction(_aggregateRoot, PlayersTrack, board, GeneralAction);
		NetworkAction = new NetworkAction(_aggregateRoot, PlayersTrack, board, GeneralAction, gameProgressInformation);
		SellAction = new SellAction(_aggregateRoot, PlayersTrack, board, GeneralAction);
	}

	public void StartTurn(Player currentPlayer, int currentRound, GameEra era)
	{
		CurPlayer = currentPlayer;
		RemainingActionsCount = MaxActionsCount;
	}

	public void EndTurn()
	{
		if (DevelopMerchantBonus.Instance.MerchantDevelopmentNeeded)
		{
			throw new InvalidOperationException("You can't end turn before finishing develop merchant bonus");
		}
		CardDealer.DealCardsForEndOfTurn(GameProgressInformation.CurrentPlayer);
		int num = PlayersTrack.Players.IndexOf(GameProgressInformation.CurrentPlayer);
		if (num != -1)
		{
			GameProgressInformation.CurrentPlayer = PlayersTrack.Players[++num % PlayersTrack.Players.Count];
		}
	}

	public BuildAction StartBuildAction()
	{
		CurrentAction = BuildAction;
		return BuildAction;
	}

	public PassAction StartPassAction()
	{
		CurrentAction = PassAction;
		return PassAction;
	}

	public LoanAction StartLoanAction()
	{
		CurrentAction = LoanAction;
		return LoanAction;
	}

	public ScoutAction StartScoutAction()
	{
		CurrentAction = ScoutAction;
		return ScoutAction;
	}

	public DevelopAction StartDevelopAction()
	{
		CurrentAction = DevelopAction;
		return DevelopAction;
	}

	public NetworkAction StartNetworkAction()
	{
		CurrentAction = NetworkAction;
		return NetworkAction;
	}

	public SellAction StartSellAction()
	{
		CurrentAction = SellAction;
		return SellAction;
	}

	public void CancelCurrentAction()
	{
		CurrentAction?.ResetAction();
		CurrentAction = null;
	}

	public void ConfirmCurrentAction()
	{
		CurrentAction?.ResetAction();
		CurrentAction = null;
		GeneralAction.DecreaseRemainingActionsCount();
	}

	public int GetRemainingActionsCount()
	{
		return RemainingActionsCount;
	}

	public int GetCurrentActionNumber()
	{
		return MaxActionsCount - RemainingActionsCount + 1;
	}

	public int GetMaxActionsCount()
	{
		return MaxActionsCount;
	}

	public void DecreaseRemainingActionsCount()
	{
		if (RemainingActionsCount <= 0)
		{
			throw new InvalidOperationException();
		}
		RemainingActionsCount--;
	}
}
