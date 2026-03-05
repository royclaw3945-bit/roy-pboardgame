using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.Game;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.Players;
using BrassApplication.AI.AIHelpers;
using BrassApplication.Game;

namespace BrassApplication.AI.RuleBasedSystem.Rules;

internal class BuildIndustry : BaseRule
{
	public BuildIndustry()
	{
		base.Id = RuleId.BuildIndustry;
		base.AILevel = AIType.Medium;
	}

	public override int GetPriority(BrassApplicationGame brassApplicationGame)
	{
		return 400;
	}

	public override bool IsApplicable(BrassApplicationGame brassApplicationGame)
	{
		Board board = brassApplicationGame.Game.Board;
		GameProgressInformation gameProgressInformation = brassApplicationGame.Game.GameProgressInformation;
		Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
		BuildAction buildAction = brassApplicationGame.Game.TurnFlow.BuildAction;
		List<BuildingType> list = Enum.GetValues(typeof(BuildingType)).Cast<BuildingType>().ToList();
		list.Remove(BuildingType.Undefined);
		List<BaseIndustry> list2 = new List<BaseIndustry>();
		foreach (BuildingType item in list)
		{
			BaseIndustry firstBuildingOfType = currentPlayer.Mat.GetFirstBuildingOfType(item);
			if (firstBuildingOfType != null && buildAction.CanUseBuildActionForGivenIndustry(firstBuildingOfType, board, gameProgressInformation))
			{
				list2.Add(firstBuildingOfType);
			}
		}
		list2 = list2.OrderByDescending((BaseIndustry i) => i.VPForBuilding + 2 * i.VPPerLink).ToList();
		if (list2.Count == 0)
		{
			return false;
		}
		return BoardHelper.PossibleBuildingSlots.Any();
	}

	public override Action MakeAction(BrassApplicationGame brassApplicationGame, bool printOnly)
	{
		return delegate
		{
			Board board = brassApplicationGame.Game.Board;
			GameProgressInformation gameProgressInformation = brassApplicationGame.Game.GameProgressInformation;
			Player currentPlayer = gameProgressInformation.CurrentPlayer;
			BuildAction buildAction = brassApplicationGame.Game.TurnFlow.StartBuildAction();
			List<BuildingSlot> list = BoardHelper.PossibleBuildingSlots[BuildingType.Undefined].ToList();
			List<BuildingType> list2 = Enum.GetValues(typeof(BuildingType)).Cast<BuildingType>().ToList();
			list2.Remove(BuildingType.Undefined);
			List<BaseIndustry> list3 = new List<BaseIndustry>();
			foreach (BuildingType item in list2)
			{
				BaseIndustry firstBuildingOfType = currentPlayer.Mat.GetFirstBuildingOfType(item);
				if (firstBuildingOfType != null && !buildAction.GetCantUseBuildActionReasonForGivenIndustry(firstBuildingOfType, board, gameProgressInformation).Any())
				{
					list3.Add(firstBuildingOfType);
				}
			}
			list3 = list3.OrderByDescending((BaseIndustry i) => i.VPForBuilding + 2 * i.VPPerLink).ToList();
			BuildingSlot buildingSlot = null;
			BaseIndustry baseIndustry = null;
			foreach (BaseIndustry item2 in list3)
			{
				if (buildingSlot != null)
				{
					break;
				}
				for (int num = 0; num < list.Count(); num++)
				{
					BuildingSlot buildingSlot2 = list[num];
					List<CantUseBuildActionReason> cantUseBuildActionReasonForGivenSlot = buildAction.GetCantUseBuildActionReasonForGivenSlot(buildingSlot2, item2, currentPlayer, board, gameProgressInformation);
					bool flag = buildAction.IsGivenSlotTheBestOneInGivenRegionForGivenType(buildingSlot2, item2.BuildingType, board);
					if (!cantUseBuildActionReasonForGivenSlot.Any() && flag)
					{
						buildingSlot = buildingSlot2;
						baseIndustry = item2;
						break;
					}
				}
			}
			if (baseIndustry == null)
			{
				throw new InvalidOperationException("Industry to build is null");
			}
			BaseCard baseCard = BoardHelper.LowestValueCard(buildAction.GetValidCardsForBuildingInSlot(buildingSlot, baseIndustry.BuildingType, currentPlayer, board), brassApplicationGame);
			Region regionContainingGivenSlot = board.GetRegionContainingGivenSlot(buildingSlot);
			BaseRule.Logger.Info(string.Concat(BaseRule.LOG_PREFIX, GetPriority(brassApplicationGame), ": ", base.Id.ToString(), BaseRule.LOG_NEWLINE, "Build ", baseIndustry.BuildingType, " in ", regionContainingGivenSlot.Name, " with card: ", baseCard));
			if (!printOnly)
			{
				BuildActionAIHelper.BuildIndustry(buildAction, baseCard, buildingSlot, baseIndustry.BuildingType, board, currentPlayer);
			}
		};
	}

	public override void PrintAction(BrassApplicationGame brassApplicationGame)
	{
		MakeAction(brassApplicationGame, printOnly: true)();
	}
}
