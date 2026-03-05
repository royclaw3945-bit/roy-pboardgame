using System;
using BoardGameRules.Utils.Logging;
using BrassApplication.AI.AIHelpers;
using BrassApplication.AI.RuleBasedSystem;
using BrassApplication.AI.RuleBasedSystem.RuleSelectors;
using BrassApplication.AI.RuleBasedSystem.Rules;
using BrassApplication.Game;

namespace BrassApplication.AI.AiBehaviors;

public class NormalAiBehavior : EasyAiBehavior
{
	private readonly RuleSelector _actionRuleSelector;

	private static readonly ILog Logger = LogFactory.BuildLogger(typeof(NormalAiBehavior));

	public NormalAiBehavior(string playerId)
		: base(playerId)
	{
		RuleSet ruleSet = RuleSetFactory.CreateBasicRuleSet(AIType.Medium);
		_actionRuleSelector = RuleSelectorFactory.CreatePrioritySelector(ruleSet);
	}

	public override Action GetNextAIMove(BrassApplicationGame brassApplicationGame)
	{
		BoardHelper.Init(brassApplicationGame, AIType.Medium);
		IRule rule = _actionRuleSelector.Select(brassApplicationGame);
		Logger.Debug("RuleSelector.GetNextAIMove() - rule = " + rule);
		if (rule != null)
		{
			return rule.MakeAction(brassApplicationGame);
		}
		return delegate
		{
		};
	}

	public override void PrintNextAIMove(BrassApplicationGame brassApplicationGame, ILog logger)
	{
		_actionRuleSelector.Select(brassApplicationGame).PrintAction(brassApplicationGame);
	}
}
