using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.GamePhases;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.Players;
using BrassApplication.AI.AIHelpers;
using BrassApplication.Game;

namespace BrassApplication.AI.RuleBasedSystem.Rules;

internal class BuildBreweryForCanalEra : BaseRule
{
	public BuildBreweryForCanalEra()
	{
		base.Id = RuleId.BuildBreweryForCanalEra;
		base.AILevel = AIType.Hard;
	}

	public override int GetPriority(BrassApplicationGame brassApplicationGame)
	{
		return 1230;
	}

	public override bool IsApplicable(BrassApplicationGame brassApplicationGame)
	{
		Board board = brassApplicationGame.Game.Board;
		Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
		BaseIndustry firstBuildingOfType = currentPlayer.Mat.GetFirstBuildingOfType(BuildingType.Brewery);
		bool flag = false;
		foreach (BaseIndustry item in board.GetIndustriesOnBoardOfGivenPlayer(currentPlayer.Color))
		{
			if (item.BuildingType == BuildingType.Brewery && !item.IsFlipped)
			{
				flag = true;
			}
		}
		if (!flag && brassApplicationGame.Game.GameProgressInformation.CurrentGameEra == GameEra.CanalEra && firstBuildingOfType.IsForCanalEra && !firstBuildingOfType.IsForRailEra && brassApplicationGame.Game.GameProgressInformation.CurrentRoundNumber <= 2)
		{
			return ApplicableBrewerySlots(brassApplicationGame).Count > 0;
		}
		return false;
	}

	public override Action MakeAction(BrassApplicationGame brassApplicationGame, bool printOnly)
	{
		return delegate
		{
			Board board = brassApplicationGame.Game.Board;
			Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
			BuildAction buildAction = brassApplicationGame.Game.TurnFlow.StartBuildAction();
			BuildingSlot buildingSlot = BoardHelper.BestBuildingSlot(ApplicableBrewerySlots(brassApplicationGame), brassApplicationGame);
			BuildingType buildingType = BuildingType.Brewery;
			BaseCard baseCard = BoardHelper.LowestValueCard(buildAction.GetValidCardsForBuildingInSlot(buildingSlot, buildingType, currentPlayer, board), brassApplicationGame);
			Region regionContainingGivenSlot = board.GetRegionContainingGivenSlot(buildingSlot);
			BaseRule.Logger.Info(string.Concat(BaseRule.LOG_PREFIX, GetPriority(brassApplicationGame), ": ", base.Id.ToString(), BaseRule.LOG_NEWLINE, "Build ", buildingType.ToString(), " in ", regionContainingGivenSlot.Name, " with card: ", baseCard));
			if (!printOnly)
			{
				BuildActionAIHelper.BuildIndustry(buildAction, baseCard, buildingSlot, buildingType, board, currentPlayer);
			}
		};
	}

	private List<BuildingSlot> ApplicableBrewerySlots(BrassApplicationGame brassApplicationGame)
	{
		return BoardHelper.PossibleBuildingSlots[BuildingType.Brewery].ToList();
	}

	public override void PrintAction(BrassApplicationGame brassApplicationGame)
	{
		MakeAction(brassApplicationGame, printOnly: true)();
	}
}
