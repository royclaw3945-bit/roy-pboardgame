using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.Players;
using BrassApplication.Game;
using BrassApplication.GameHistory;
using BrassApplication.UseCases.Common;

namespace BrassApplication.UseCases.Game.IndustriesSummary;

public class IndustriesSummaryUseCase : IUseCase, IIndustriesSummaryInput
{
	private readonly IIndustriesSummaryOutput _presenter;

	private readonly IGameHistory _gameHistory;

	public IndustriesSummaryUseCase(IIndustriesSummaryOutput presenter, IGameHistory gameHistory)
	{
		_presenter = presenter;
		_gameHistory = gameHistory;
	}

	public void UpdateIndustries()
	{
		Player currentPlayer = _gameHistory.CurrentGame.Game.GameProgressInformation.CurrentPlayer;
		UpdateIndustries(currentPlayer.Color);
	}

	public void UpdateIndustries(PlayerColor playerColor)
	{
		BrassApplicationGame currentGame = _gameHistory.CurrentGame;
		BuildAction buildAction = currentGame.Game.TurnFlow.BuildAction;
		Player player = currentGame.Game.PlayersTrack.Players.Find((Player p) => p.Color == playerColor);
		PlayerMat mat = player.Mat;
		Dictionary<BuildingType, IList<CantUseBuildActionReason>> dictionary = new Dictionary<BuildingType, IList<CantUseBuildActionReason>>();
		List<BuildingType> list = Enum.GetValues(typeof(BuildingType)).Cast<BuildingType>().ToList();
		list.Remove(BuildingType.Undefined);
		foreach (BuildingType item in list)
		{
			BaseIndustry firstBuildingOfType = mat.GetFirstBuildingOfType(item);
			List<CantUseBuildActionReason> cantUseBuildActionReasonForGivenIndustry = buildAction.GetCantUseBuildActionReasonForGivenIndustry(firstBuildingOfType, currentGame.Game.Board, currentGame.Game.GameProgressInformation);
			dictionary.Add(item, cantUseBuildActionReasonForGivenIndustry);
		}
		_presenter.UpdateIndustriesSummary(currentGame, player.Color, dictionary);
	}

	public void ShowIndustryDetails()
	{
		_presenter.ShowIndustryDetails();
	}

	public void ShowTooltip(string element, float x, float y)
	{
		int industryLevel = -1;
		int resources = -1;
		Player currentPlayer = _gameHistory.CurrentGame.Game.GameProgressInformation.CurrentPlayer;
		PlayerMat mat = _gameHistory.CurrentGame.Game.PlayersTrack.Players.Find((Player p) => p.Color == currentPlayer.Color).Mat;
		switch (element)
		{
		case "Brewery":
		{
			BaseIndustry firstBuildingOfType = mat.GetFirstBuildingOfType(BuildingType.Brewery);
			industryLevel = firstBuildingOfType?.Level ?? 0;
			resources = ((Brewery)firstBuildingOfType)?._numberOfBeersFunc() ?? 0;
			break;
		}
		case "CoalMine":
		{
			BaseIndustry firstBuildingOfType3 = mat.GetFirstBuildingOfType(BuildingType.CoalMine);
			industryLevel = firstBuildingOfType3?.Level ?? 0;
			resources = firstBuildingOfType3?.GetResource() ?? 0;
			break;
		}
		case "CottonMill":
			industryLevel = mat.GetFirstBuildingOfType(BuildingType.CottonMill)?.Level ?? 0;
			break;
		case "IronWorks":
		{
			BaseIndustry firstBuildingOfType2 = mat.GetFirstBuildingOfType(BuildingType.IronWorks);
			industryLevel = firstBuildingOfType2?.Level ?? 0;
			resources = firstBuildingOfType2?.GetResource() ?? 0;
			break;
		}
		case "Manufacture":
			industryLevel = mat.GetFirstBuildingOfType(BuildingType.Manufacturer)?.Level ?? 0;
			break;
		case "Pottery":
			industryLevel = mat.GetFirstBuildingOfType(BuildingType.Pottery)?.Level ?? 0;
			break;
		}
		_presenter.ShowTooltip(element, x, y, industryLevel, resources);
	}

	public void HideTooltip()
	{
		_presenter.HideTooltip();
	}
}
