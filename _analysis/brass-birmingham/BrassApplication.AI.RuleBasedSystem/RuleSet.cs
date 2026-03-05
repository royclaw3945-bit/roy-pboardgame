using System.Collections.Generic;
using BrassApplication.AI.RuleBasedSystem.Rules;

namespace BrassApplication.AI.RuleBasedSystem;

public class RuleSet : List<IRule>
{
	public void Add(RuleSet ruleSet)
	{
		foreach (IRule rule in ruleSet)
		{
			IRule rule2 = Find((IRule el) => el.Id == rule.Id);
			if (rule2 != null)
			{
				int index = IndexOf(rule2);
				base[index] = rule;
				rule.DecoratedRule = rule2;
			}
			else
			{
				Add(rule);
			}
		}
	}
}
