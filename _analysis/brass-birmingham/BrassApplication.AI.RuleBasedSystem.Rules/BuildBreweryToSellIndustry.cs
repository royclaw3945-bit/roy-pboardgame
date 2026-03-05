using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.Players;
using BrassApplication.AI.AIHelpers;
using BrassApplication.Game;

namespace BrassApplication.AI.RuleBasedSystem.Rules;

internal class BuildBreweryToSellIndustry : BaseRule
{
	public BuildBreweryToSellIndustry()
	{
		base.Id = RuleId.BuildBreweryToSellIndustry;
		base.AILevel = AIType.Easy;
	}

	public override int GetPriority(BrassApplicationGame brassApplicationGame)
	{
		return 2120;
	}

	public override bool IsApplicable(BrassApplicationGame brassApplicationGame)
	{
		return ApplicableBrewerySlots(brassApplicationGame).Count > 0;
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
			BaseRule.Logger.Info(string.Concat(BaseRule.LOG_PREFIX, GetPriority(brassApplicationGame), ": ", base.Id.ToString(), BaseRule.LOG_NEWLINE, "Build ", buildingType.ToString(), " in ", regionContainingGivenSlot.Name, " (", buildingSlot, ") with card: ", baseCard));
			if (!printOnly)
			{
				BuildActionAIHelper.BuildIndustry(buildAction, baseCard, buildingSlot, buildingType, board, currentPlayer);
			}
		};
	}

	private List<BuildingSlot> ApplicableBrewerySlots(BrassApplicationGame brassApplicationGame)
	{
		Board board = brassApplicationGame.Game.Board;
		Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
		List<BuildingSlot> list = new List<BuildingSlot>();
		foreach (BuildingSlot item in board.Regions.SelectMany((Region r) => r.BuildingSlots.Where((BuildingSlot s) => s.BuildedIndustry is BaseSellableIndustry)))
		{
			if (item.BuildedIndustry.Color == currentPlayer.Color && !item.BuildedIndustry.IsFlipped && BoardHelper.CanIndustryBeSoldButLackingBeer(brassApplicationGame, item.BuildedIndustry, currentPlayer, board))
			{
				list.Add(item);
			}
		}
		if (list.Count > 0)
		{
			return BoardHelper.PossibleBuildingSlots[BuildingType.Brewery].ToList();
		}
		return new List<BuildingSlot>();
	}

	public override void PrintAction(BrassApplicationGame brassApplicationGame)
	{
		MakeAction(brassApplicationGame, printOnly: true)();
	}
}
