using BoardGameRules.Utils.Logging;
using BrassApplication.AI.RuleBasedSystem.Rules;
using BrassApplication.Game;

namespace BrassApplication.AI.RuleBasedSystem.RuleSelectors;

public abstract class RuleSelector
{
	protected static ILog Logger = LogFactory.BuildLogger(typeof(RuleSelector));

	protected readonly RuleSet _ruleSet;

	protected RuleSelector(RuleSet ruleSet)
	{
		_ruleSet = ruleSet;
	}

	public abstract IRule Select(BrassApplicationGame brassApplicationGame);
}
