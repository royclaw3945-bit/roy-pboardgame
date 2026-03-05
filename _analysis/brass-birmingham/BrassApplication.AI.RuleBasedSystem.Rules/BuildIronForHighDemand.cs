using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.GamePhases;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.Players;
using BrassApplication.AI.AIHelpers;
using BrassApplication.Game;

namespace BrassApplication.AI.RuleBasedSystem.Rules;

internal class BuildIronForHighDemand : BaseRule
{
	public BuildIronForHighDemand()
	{
		base.Id = RuleId.BuildIronForHighDemand;
		base.AILevel = AIType.Easy;
	}

	public override int GetPriority(BrassApplicationGame brassApplicationGame)
	{
		return GetPriorityStatic(brassApplicationGame);
	}

	public static int GetPriorityStatic(BrassApplicationGame brassApplicationGame)
	{
		Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
		BaseIndustry firstBuildingOfType = currentPlayer.Mat.GetFirstBuildingOfType(BuildingType.IronWorks);
		brassApplicationGame.Game.TurnFlow.GetRemainingActionsCount();
		int sellingPrice = brassApplicationGame.Game.Board.IronMarket.GetSellingPrice();
		if (firstBuildingOfType == null)
		{
			return -1300;
		}
		if (sellingPrice >= 3)
		{
			return 2500;
		}
		if (firstBuildingOfType.IsForCanalEra && !firstBuildingOfType.IsForRailEra && brassApplicationGame.Game.GameProgressInformation.CurrentGameEra == GameEra.CanalEra)
		{
			if (sellingPrice >= 2)
			{
				return 2500;
			}
			Board board = brassApplicationGame.Game.Board;
			int num = 0;
			foreach (CoalMine unflippedCoalMine in board.GetUnflippedCoalMines())
			{
				if (unflippedCoalMine.Color == currentPlayer.Color)
				{
					num++;
				}
			}
			if (sellingPrice >= 1 && num > 0)
			{
				return 2500;
			}
		}
		if (sellingPrice >= 2)
		{
			return 1300;
		}
		return -1300;
	}

	public static BuildingSlot BestSlot(BrassApplicationGame brassApplicationGame)
	{
		List<BuildingSlot> list = BoardHelper.PossibleBuildingSlots[BuildingType.IronWorks].ToList();
		Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
		if (list == null || list.Count == 0)
		{
			return null;
		}
		foreach (BuildingSlot item in list)
		{
			if (item.BuildedIndustry != null && item.BuildedIndustry.Color != currentPlayer.Color)
			{
				return item;
			}
		}
		foreach (BuildingSlot item2 in list)
		{
			if (item2.BuildedIndustry == null)
			{
				return item2;
			}
		}
		if (brassApplicationGame.Game.TurnFlow.GetRemainingActionsCount() == 2 && currentPlayer.Money >= 20)
		{
			return null;
		}
		return list[0];
	}

	public override bool IsApplicable(BrassApplicationGame brassApplicationGame)
	{
		if (BestSlot(brassApplicationGame) != null)
		{
			return GetPriority(brassApplicationGame) > 0;
		}
		return false;
	}

	public override Action MakeAction(BrassApplicationGame brassApplicationGame, bool printOnly)
	{
		return delegate
		{
			Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
			BuildAction buildAction = brassApplicationGame.Game.TurnFlow.StartBuildAction();
			BuildingSlot buildingSlot = BestSlot(brassApplicationGame);
			BaseCard baseCard = BoardHelper.BuildActionBestCard(buildAction.GetValidCardsForBuildingInSlot(buildingSlot, BuildingType.IronWorks, currentPlayer, brassApplicationGame.Game.Board), brassApplicationGame);
			BaseRule.Logger.Info(string.Concat(BaseRule.LOG_PREFIX, GetPriority(brassApplicationGame), ": ", base.Id.ToString(), BaseRule.LOG_NEWLINE, "Build Iron Works in ", brassApplicationGame.Game.Board.GetRegionContainingGivenSlot(buildingSlot), " with card: ", baseCard));
			if (!printOnly)
			{
				BuildActionAIHelper.BuildIndustry(buildAction, baseCard, buildingSlot, BuildingType.IronWorks, brassApplicationGame.Game.Board, currentPlayer);
			}
		};
	}

	public override void PrintAction(BrassApplicationGame brassApplicationGame)
	{
		MakeAction(brassApplicationGame, printOnly: true)();
	}
}
