using System.Collections.Generic;
using System.Linq;
using BrassApplication.AI.RuleBasedSystem.Rules;
using BrassApplication.Game;

namespace BrassApplication.AI.RuleBasedSystem.RuleSelectors;

public class PrioritySelector : RuleSelector
{
	public PrioritySelector(RuleSet ruleSet)
		: base(ruleSet)
	{
	}

	public override IRule Select(BrassApplicationGame brassApplicationGame)
	{
		SortedDictionary<int, BaseRule> sortedDictionary = new SortedDictionary<int, BaseRule>();
		foreach (BaseRule item in _ruleSet)
		{
			sortedDictionary.Add(item.GetPriority(brassApplicationGame), item);
		}
		foreach (int item2 in sortedDictionary.Keys.Reverse())
		{
			if (sortedDictionary[item2].IsApplicable(brassApplicationGame))
			{
				return sortedDictionary[item2];
			}
		}
		return null;
	}
}
