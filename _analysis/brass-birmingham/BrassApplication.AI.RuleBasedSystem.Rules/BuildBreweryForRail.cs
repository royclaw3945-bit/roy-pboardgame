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

internal class BuildBreweryForRail : BaseRule
{
	public BuildBreweryForRail()
	{
		base.Id = RuleId.BuildBreweryForRail;
		base.AILevel = AIType.Hard;
	}

	public override int GetPriority(BrassApplicationGame brassApplicationGame)
	{
		int sellingPrice = brassApplicationGame.Game.Board.CoalMarket.GetSellingPrice();
		if (brassApplicationGame.Game.TurnFlow.GetRemainingActionsCount() == 2 && sellingPrice <= 5)
		{
			return 2210;
		}
		return 1620;
	}

	public override bool IsApplicable(BrassApplicationGame brassApplicationGame)
	{
		Board board = brassApplicationGame.Game.Board;
		Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
		if (brassApplicationGame.Game.Board.CoalMarket.GetSellingPrice() >= 5 && currentPlayer.Money <= 36)
		{
			return false;
		}
		bool flag = false;
		bool flag2 = false;
		foreach (BaseIndustry item in board.GetIndustriesOnBoardOfGivenPlayer(currentPlayer.Color))
		{
			if (item.BuildingType == BuildingType.Brewery && !item.IsFlipped)
			{
				flag = true;
			}
		}
		foreach (BaseIndustry item2 in from s in board.Regions.SelectMany((Region r) => r.BuildingSlots)
			where s.BuildedIndustry != null
			select s.BuildedIndustry)
		{
			if (item2.BuildingType == BuildingType.CoalMine && !item2.IsFlipped)
			{
				flag2 = true;
			}
		}
		if (!flag && flag2 && currentPlayer.Money >= 32 && brassApplicationGame.Game.GameProgressInformation.CurrentGameEra == GameEra.RailEra)
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
			BaseRule.Logger.Info(string.Concat(BaseRule.LOG_PREFIX, GetPriority(brassApplicationGame), ": ", base.Id.ToString(), BaseRule.LOG_NEWLINE, "Build ", buildingType.ToString(), " in ", regionContainingGivenSlot.Name, " ", buildingSlot, " with card: ", baseCard));
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
