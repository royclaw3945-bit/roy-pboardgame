using System;
using BoardGameRules.Utils.Logging;
using BrassApplication.Game;

namespace BrassApplication.AI.AiBehaviors;

public interface IAiBehavior
{
	Action GetNextAIMove(BrassApplicationGame brassApplicationGame);

	void PrintNextAIMove(BrassApplicationGame brassApplicationGame, ILog logger);
}
