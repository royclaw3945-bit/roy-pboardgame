using System.Collections.Generic;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Regions;
using BrassApplication.ApplicationSettings;
using BrassApplication.GameHistory;
using BrassApplication.UseCases.Game.Actions;
using BrassApplication.UseCases.Game.Board;

namespace BrassApplication.UseCases.Tutorials.Board;

public class TutorialBoardUseCase : BoardUseCase, ITutorialBoardInput, IBoardInput, IActionInProgressCancelDelegate
{
	private ITutorialBoardOutput _presenter;

	public TutorialBoardUseCase(ITutorialBoardOutput presenter, IGameHistory gameHistory, IGameSettings gameSettings)
		: base(presenter, gameHistory, gameSettings)
	{
		_presenter = presenter;
	}

	public void HighlightOnlySpecificSlot(string buildingSlotId)
	{
		BuildingSlot buildingSlotByUniqueId = _gameHistory.CurrentGame.Game.Board.GetBuildingSlotByUniqueId(buildingSlotId);
		Region regionContainingGivenSlot = _gameHistory.CurrentGame.Game.Board.GetRegionContainingGivenSlot(buildingSlotByUniqueId);
		_presenter.HighlightOnlySpecificSlot(_gameHistory.CurrentGame.Game.Board, _gameHistory.CurrentGame.Game.GameProgressInformation.CurrentGameEra, regionContainingGivenSlot, base.ShowRegionDetails, buildingSlotId);
	}

	public void SetAllowedIndustries(List<BuildingType> allowedIndustries)
	{
		_presenter.SetAllowedIndustries(allowedIndustries);
	}
}
