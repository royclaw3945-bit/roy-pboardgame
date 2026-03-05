using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.Players;
using BrassApplication.AI.AIHelpers;
using BrassApplication.Game;

namespace BrassApplication.AI.RuleBasedSystem.Rules;

internal class ForceDevelopForIronOverbuild : BaseRule
{
	public ForceDevelopForIronOverbuild()
	{
		base.Id = RuleId.ForceDevelopForIronOverbuild;
		base.AILevel = AIType.Hard;
	}

	public override int GetPriority(BrassApplicationGame brassApplicationGame)
	{
		return 800;
	}

	public override bool IsApplicable(BrassApplicationGame brassApplicationGame)
	{
		int remainingActionsCount = brassApplicationGame.Game.TurnFlow.GetRemainingActionsCount();
		int sellingPrice = brassApplicationGame.Game.Board.IronMarket.GetSellingPrice();
		Player player = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
		if (!brassApplicationGame.Game.TurnFlow.DevelopAction.CanDevelopGivenTypeOfIndustry(BuildingType.IronWorks, player))
		{
			return false;
		}
		BaseIndustry firstBuildingOfType = player.Mat.GetFirstBuildingOfType(BuildingType.IronWorks);
		int moneyCost = firstBuildingOfType.MoneyCost;
		if (brassApplicationGame.Game.Board.GetAmountOfIronOnBoard() > 0)
		{
			return false;
		}
		IEnumerable<BaseIndustry> enumerable = from i in brassApplicationGame.Game.Board.Regions.SelectMany((Region r) => r.BuildingSlots.Select((BuildingSlot s) => s.BuildedIndustry))
			where i is IronWorks && i.Color != player.Color
			select i;
		int num = 10;
		foreach (BaseIndustry item in enumerable)
		{
			num = Math.Min(num, item.Level);
		}
		bool flag = firstBuildingOfType.Level == num;
		return remainingActionsCount == 2 && sellingPrice >= 5 && player.Money >= 8 + moneyCost && flag;
	}

	public override Action MakeAction(BrassApplicationGame brassApplicationGame, bool printOnly)
	{
		return delegate
		{
			Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
			DevelopAction developAction = brassApplicationGame.Game.TurnFlow.StartDevelopAction();
			BaseCard baseCard = BoardHelper.LowestValueCard(currentPlayer.Hand, brassApplicationGame);
			BaseRule.Logger.Info(BaseRule.LOG_PREFIX + GetPriority(brassApplicationGame) + ": " + base.Id.ToString() + BaseRule.LOG_NEWLINE + "Develop Iron Works with card: " + baseCard);
			if (!printOnly)
			{
				DevelopActionAIHelper.DevelopIndustry(developAction, baseCard, BuildingType.IronWorks, BuildingType.Undefined, brassApplicationGame.Game.Board, currentPlayer);
			}
		};
	}
}
