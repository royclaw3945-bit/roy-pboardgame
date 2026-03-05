using System;
using BoardGameRules.Entities.EventSourcing.Aggregates;
using BoardGameRules.Entities.NumericalResources.Events;
using BoardGameRules.Entities.Players;

namespace BoardGameRules.Entities.NumericalResources;

public class Bank
{
	private IAggregateRoot _AggregateRoot { get; }

	private PlayersTrack _PlayersTrack { get; }

	public Bank(IAggregateRoot aggregateRoot, PlayersTrack playersTrack)
	{
		_AggregateRoot = aggregateRoot;
		_PlayersTrack = playersTrack;
		aggregateRoot.RegisterApplyEvent<IncomeMoneyPaid>(ApplyEvent);
		aggregateRoot.RegisterApplyEvent<MoneyLoaned>(ApplyEvent);
	}

	private void ApplyEvent(IncomeMoneyPaid incomeMoneyPaid)
	{
		foreach (Player player in _PlayersTrack.Players)
		{
			player.Money = Math.Max(0, player.Money + IncomeTrack.GetCurrentIncomeLevel(player.IncomeMarkerPosition));
		}
	}

	private void ApplyEvent(MoneyLoaned moneyLoaned)
	{
		_PlayersTrack.GetPlayerByColor(moneyLoaned.Player).Money += moneyLoaned.Amount;
	}
}
