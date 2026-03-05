using System;
using System.Linq;
using BrassApplication.Game;

namespace BrassApplication.AI.RuleBasedSystem.Rules;

internal class PassRule : BaseRule
{
	public PassRule()
	{
		base.Id = RuleId.PassRule;
		base.AILevel = AIType.Easy;
	}

	public override int GetPriority(BrassApplicationGame brassApplicationGame)
	{
		return -100;
	}

	public override bool IsApplicable(BrassApplicationGame brassApplicationGame)
	{
		return true;
	}

	public override Action MakeAction(BrassApplicationGame brassApplicationGame, bool printOnly)
	{
		return delegate
		{
			BaseRule.Logger.Info(BaseRule.LOG_PREFIX + GetPriority(brassApplicationGame) + ": " + base.Id.ToString() + BaseRule.LOG_NEWLINE + "Pass action with card: " + brassApplicationGame.Game.GameProgressInformation.CurrentPlayer.Hand.First());
			if (!printOnly)
			{
				brassApplicationGame.Game.TurnFlow.StartPassAction().Pass(brassApplicationGame.Game.GameProgressInformation.CurrentPlayer.Hand.First(), brassApplicationGame.Game.GameProgressInformation.CurrentPlayer);
			}
		};
	}
}
