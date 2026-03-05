namespace BrassApplication.AI.RuleBasedSystem.RuleSelectors;

public static class RuleSelectorFactory
{
	public static RuleSelector CreatePrioritySelector(RuleSet ruleSet)
	{
		return new PrioritySelector(ruleSet);
	}

	public static RuleSelector CreateFirstApplicableSelector(RuleSet ruleSet)
	{
		return new FirstApplicableSelector(ruleSet);
	}
}
