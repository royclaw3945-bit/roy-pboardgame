using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.Common;
using BoardGameRules.Entities.GamePhases;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.Players;
using BrassApplication.AI.AIHelpers;
using BrassApplication.Game;

namespace BrassApplication.AI.RuleBasedSystem.Rules;

internal class DevelopCanalEraBrewery : BaseRule
{
	public DevelopCanalEraBrewery()
	{
		base.Id = RuleId.DevelopCanalEraBrewery;
		base.AILevel = AIType.Medium;
	}

	public bool isLateCanalGame(BrassApplicationGame brassApplicationGame)
	{
		if (brassApplicationGame.Game.GameProgressInformation.CurrentGameEra == GameEra.CanalEra)
		{
			return brassApplicationGame.Game.GameProgressInformation.CurrentRoundNumber >= 6;
		}
		return false;
	}

	public override int GetPriority(BrassApplicationGame brassApplicationGame)
	{
		return 1430;
	}

	public override bool IsApplicable(BrassApplicationGame brassApplicationGame)
	{
		Board board = brassApplicationGame.Game.Board;
		IIronSource ironSource = board.GetUnflippedIronWorks().FirstOrDefault();
		IIronSource ironSource2 = ironSource ?? board.IronMarket;
		Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
		if (ApplicableIndustries(brassApplicationGame).Contains(BuildingType.Brewery) && (isLateCanalGame(brassApplicationGame) || brassApplicationGame.Game.GameProgressInformation.CurrentGameEra == GameEra.RailEra))
		{
			return ironSource2.IronPrice() <= currentPlayer.Money;
		}
		return false;
	}

	public List<BuildingType> ApplicableIndustries(BrassApplicationGame brassApplicationGame)
	{
		List<BuildingType> list = new List<BuildingType>();
		Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
		BaseIndustry firstBuildingOfType = currentPlayer.Mat.GetFirstBuildingOfType(BuildingType.CoalMine);
		BaseIndustry firstBuildingOfType2 = currentPlayer.Mat.GetFirstBuildingOfType(BuildingType.IronWorks);
		BaseIndustry firstBuildingOfType3 = currentPlayer.Mat.GetFirstBuildingOfType(BuildingType.Manufacturer);
		BaseIndustry firstBuildingOfType4 = currentPlayer.Mat.GetFirstBuildingOfType(BuildingType.Brewery);
		BaseIndustry firstBuildingOfType5 = currentPlayer.Mat.GetFirstBuildingOfType(BuildingType.CottonMill);
		if (firstBuildingOfType != null && firstBuildingOfType.IsForCanalEra && !firstBuildingOfType.IsForRailEra)
		{
			list.Add(BuildingType.CoalMine);
		}
		if (firstBuildingOfType2 != null && firstBuildingOfType2.IsForCanalEra && !firstBuildingOfType2.IsForRailEra)
		{
			list.Add(BuildingType.IronWorks);
		}
		if (firstBuildingOfType3 != null && firstBuildingOfType3.IsForCanalEra && !firstBuildingOfType3.IsForRailEra)
		{
			list.Add(BuildingType.Manufacturer);
		}
		if (firstBuildingOfType4 != null && firstBuildingOfType4.IsForCanalEra && !firstBuildingOfType4.IsForRailEra)
		{
			list.Add(BuildingType.Brewery);
		}
		if (firstBuildingOfType5 != null && firstBuildingOfType5.IsForCanalEra && !firstBuildingOfType5.IsForRailEra)
		{
			list.Add(BuildingType.CottonMill);
		}
		return list;
	}

	public override Action MakeAction(BrassApplicationGame brassApplicationGame, bool printOnly)
	{
		return delegate
		{
			Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
			DevelopAction developAction = brassApplicationGame.Game.TurnFlow.StartDevelopAction();
			BaseCard baseCard = BoardHelper.LowestValueCard(currentPlayer.Hand, brassApplicationGame);
			List<BuildingType> list = ApplicableIndustries(brassApplicationGame);
			list.Remove(BuildingType.Brewery);
			BuildingType buildingType = BuildingType.Brewery;
			BuildingType buildingType2 = BuildingType.Undefined;
			if (list.Count > 0)
			{
				buildingType2 = list[0];
			}
			BaseRule.Logger.Info(string.Concat(BaseRule.LOG_PREFIX, GetPriority(brassApplicationGame), ": ", base.Id.ToString(), BaseRule.LOG_NEWLINE, "Develop Industries: ", buildingType, ", ", buildingType2, " with card: ", baseCard));
			if (!printOnly)
			{
				DevelopActionAIHelper.DevelopIndustry(developAction, baseCard, buildingType, buildingType2, brassApplicationGame.Game.Board, currentPlayer);
			}
		};
	}

	public override void PrintAction(BrassApplicationGame brassApplicationGame)
	{
		MakeAction(brassApplicationGame, printOnly: true)();
	}
}
