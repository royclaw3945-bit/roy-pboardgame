using System;
using BrassApplication.Game;

namespace BrassApplication.AI.RuleBasedSystem.Rules;

public interface IRule
{
	RuleId Id { get; }

	IRule DecoratedRule { get; set; }

	AIType AILevel { get; set; }

	bool IsApplicable(BrassApplicationGame brassApplicationGame);

	int GetPriority(BrassApplicationGame brassApplicationGame);

	Action MakeAction(BrassApplicationGame brassApplicationGame, bool printOnly = false);

	void PrintAction(BrassApplicationGame brassApplicationGame);
}
