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

internal class BuildCoalForStock : BaseRule
{
	public BuildCoalForStock()
	{
		base.Id = RuleId.BuildCoalForStock;
		base.AILevel = AIType.Hard;
	}

	public override int GetPriority(BrassApplicationGame brassApplicationGame)
	{
		return 1240;
	}

	public override bool IsApplicable(BrassApplicationGame brassApplicationGame)
	{
		Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
		BaseIndustry firstBuildingOfType = currentPlayer.Mat.GetFirstBuildingOfType(BuildingType.CoalMine);
		if (firstBuildingOfType == null)
		{
			return false;
		}
		if (brassApplicationGame.Game.GameProgressInformation.CurrentGameEra != GameEra.CanalEra)
		{
			return false;
		}
		if (currentPlayer.Money < 15)
		{
			return false;
		}
		if ((firstBuildingOfType.IsForCanalEra && brassApplicationGame.Game.GameProgressInformation.CurrentGameEra != GameEra.CanalEra) || (firstBuildingOfType.IsForRailEra && brassApplicationGame.Game.GameProgressInformation.CurrentGameEra != GameEra.RailEra))
		{
			return false;
		}
		return ApplicableSlot(brassApplicationGame) != null;
	}

	public BuildingSlot ApplicableSlot(BrassApplicationGame brassApplicationGame)
	{
		Board board = brassApplicationGame.Game.Board;
		Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
		HashSet<Region> hashSet = new HashSet<Region>();
		foreach (BorderRegion borderRegion in board.GetBorderRegions((BorderRegion t) => true))
		{
			HashSet<Region> other = BoardHelper.ConnectedRegions(brassApplicationGame, borderRegion);
			hashSet.UnionWith(other);
		}
		foreach (BuildingSlot item in BoardHelper.PossibleBuildingSlots[BuildingType.CoalMine])
		{
			if (item.BuildedIndustry != null && item.BuildedIndustry.Color != currentPlayer.Color && hashSet.Contains(board.GetRegionContainingGivenSlot(item)))
			{
				return item;
			}
		}
		foreach (BuildingSlot item2 in BoardHelper.PossibleBuildingSlots[BuildingType.CoalMine])
		{
			if (item2.BuildedIndustry == null && board.GetRegionContainingGivenSlot(item2).Color == RegionColor.Violet)
			{
				return item2;
			}
		}
		foreach (BuildingSlot item3 in BoardHelper.PossibleBuildingSlots[BuildingType.CoalMine])
		{
			if (item3.BuildedIndustry == null && hashSet.Contains(board.GetRegionContainingGivenSlot(item3)))
			{
				return item3;
			}
		}
		if (BoardHelper.PossibleBuildingSlots[BuildingType.CoalMine].Count > 0)
		{
			return BoardHelper.PossibleBuildingSlots[BuildingType.CoalMine].First();
		}
		return null;
	}

	public override Action MakeAction(BrassApplicationGame brassApplicationGame, bool printOnly)
	{
		return delegate
		{
			Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
			BuildAction buildAction = brassApplicationGame.Game.TurnFlow.StartBuildAction();
			BuildingSlot buildingSlot = ApplicableSlot(brassApplicationGame);
			BaseCard baseCard = BoardHelper.LowestValueCard(buildAction.GetValidCardsForBuildingInSlot(buildingSlot, BuildingType.CoalMine, currentPlayer, brassApplicationGame.Game.Board), brassApplicationGame);
			BaseRule.Logger.Info(string.Concat(BaseRule.LOG_PREFIX, GetPriority(brassApplicationGame), ": ", base.Id.ToString(), BaseRule.LOG_NEWLINE, "Build Coal Mine in ", brassApplicationGame.Game.Board.GetRegionContainingGivenSlot(buildingSlot), " with card: ", baseCard));
			if (!printOnly)
			{
				BuildActionAIHelper.BuildIndustry(buildAction, baseCard, buildingSlot, BuildingType.CoalMine, brassApplicationGame.Game.Board, currentPlayer);
			}
		};
	}

	public override void PrintAction(BrassApplicationGame brassApplicationGame)
	{
		MakeAction(brassApplicationGame, printOnly: true)();
	}
}
