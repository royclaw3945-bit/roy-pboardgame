using System;
using BoardGameRules.Utils.Logging;
using BrassApplication.Game;

namespace BrassApplication.AI.RuleBasedSystem.Rules;

public abstract class BaseRule : IRule
{
	protected static readonly ILog Logger = LogFactory.BuildLogger(typeof(BaseRule));

	protected static readonly string LOG_NEWLINE = "\r\n";

	public static readonly string LOG_PREFIX = LOG_NEWLINE + " ★ ";

	public IRule DecoratedRule { get; set; }

	public RuleId Id { get; protected set; }

	public AIType AILevel { get; set; }

	public abstract int GetPriority(BrassApplicationGame brassApplicationGame);

	public abstract bool IsApplicable(BrassApplicationGame brassApplicationGame);

	public abstract Action MakeAction(BrassApplicationGame brassApplicationGame, bool printOnly);

	public virtual void PrintAction(BrassApplicationGame brassApplicationGame)
	{
		Logger.Debug(LOG_PREFIX + GetPriority(brassApplicationGame) + ": " + Id.ToString());
	}
}
