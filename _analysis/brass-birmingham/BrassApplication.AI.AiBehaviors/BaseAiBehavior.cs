using System;
using System.Linq;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.Game;
using BoardGameRules.Entities.Players;
using BoardGameRules.Entities.TurnPhases;
using BoardGameRules.Utils.Logging;
using BrassApplication.Game;

namespace BrassApplication.AI.AiBehaviors;

public abstract class BaseAiBehavior : IAiBehavior
{
	protected string PlayerId;

	protected BaseAiBehavior(string playerId)
	{
		PlayerId = playerId;
	}

	public static Action PassFirstCard(GameProgressInformation gameProgressInformation, TurnFlow turnFlow)
	{
		Player currentPlayer = gameProgressInformation.CurrentPlayer;
		BaseCard card = currentPlayer.Hand.First();
		return delegate
		{
			turnFlow.StartPassAction().Pass(card, currentPlayer);
		};
	}

	public abstract Action GetNextAIMove(BrassApplicationGame brassApplicationGame);

	public abstract void PrintNextAIMove(BrassApplicationGame brassApplicationGame, ILog logger);
}
