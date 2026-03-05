using System;
using System.Collections.Generic;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.EventSourcing.Aggregates;
using BoardGameRules.Entities.Game;
using BoardGameRules.Entities.NumericalResources;
using BoardGameRules.Entities.NumericalResources.Events;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.Actions;

public class LoanAction : BaseAction
{
	private const int IncomeLevelTooLowForLoan = -8;

	public const int LevelsToTakeWithLoan = 3;

	public LoanAction(IAggregateRoot aggregateRoot, PlayersTrack playersTrack, BoardGameRules.Entities.Board.Board board, GeneralAction generalAction)
		: base(aggregateRoot, playersTrack, board, generalAction)
	{
	}

	public void Loan(PlayerColor playerColor)
	{
		if (base.PlayersTrack.GetPlayerByColor(playerColor).IncomeLevel <= -8)
		{
			throw new InvalidOperationException("Can't take loan because player's income level will fall below -10.");
		}
		base.AggregateRoot.ApplyEvent(new MoneyLoaned(playerColor, 30));
		base.PlayersTrack.GetPlayerByColor(playerColor).DecreaseIncomeLevelBy(3);
	}

	public List<CantUseLoanActionReason> GetLoanActionNotPossibleReasons(GameProgressInformation gameProgressInformation)
	{
		List<CantUseLoanActionReason> list = new List<CantUseLoanActionReason>();
		if (gameProgressInformation.CurrentPlayer.IncomeLevel <= -7)
		{
			list.Add(CantUseLoanActionReason.INCOME_TOO_LOW);
		}
		return list;
	}
}
