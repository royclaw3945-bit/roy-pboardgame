using BrassApplication.AI.RuleBasedSystem.Rules;
using BrassApplication.Game;

namespace BrassApplication.AI.RuleBasedSystem.RuleSelectors;

public class FirstApplicableSelector : RuleSelector
{
	public FirstApplicableSelector(RuleSet ruleSet)
		: base(ruleSet)
	{
	}

	public override IRule Select(BrassApplicationGame brassApplicationGame)
	{
		foreach (IRule item in _ruleSet)
		{
			if (item.IsApplicable(brassApplicationGame))
			{
				return item;
			}
		}
		return null;
	}
}
