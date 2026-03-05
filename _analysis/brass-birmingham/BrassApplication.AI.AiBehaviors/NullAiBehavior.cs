using System;
using BoardGameRules.Utils.Logging;
using BrassApplication.Game;

namespace BrassApplication.AI.AiBehaviors;

public class NullAiBehavior : BaseAiBehavior
{
	public NullAiBehavior(string playerId)
		: base(playerId)
	{
	}

	public override Action GetNextAIMove(BrassApplicationGame brassApplicationGame)
	{
		return delegate
		{
		};
	}

	public override void PrintNextAIMove(BrassApplicationGame brassApplicationGame, ILog logger)
	{
		logger.Debug("\r\n ★ NULL");
	}
}
