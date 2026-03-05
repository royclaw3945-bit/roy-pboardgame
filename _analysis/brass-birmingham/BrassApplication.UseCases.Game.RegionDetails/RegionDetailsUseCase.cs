using BoardGameRules.Entities.Board.BuildingSlots;
using BrassApplication.Game;
using BrassApplication.GameHistory;
using BrassApplication.UseCases.Common;
using BrassApplication.UseCases.Game.Actions;

namespace BrassApplication.UseCases.Game.RegionDetails;

public class RegionDetailsUseCase : IUseCase, IRegionDetailsInput
{
	private readonly IRegionDetailsOutput _presenter;

	private ISelectBuildingDelegate _selectBuildingDelegate;

	private IGameHistory _gameHistory;

	public RegionDetailsUseCase(IRegionDetailsOutput presenter, IGameHistory gameHistory)
	{
		_presenter = presenter;
		_gameHistory = gameHistory;
	}

	public void BuildClicked(BuildingSlot buildingSlot, BuildingType buildingType)
	{
		_selectBuildingDelegate?.SelectBuilding(buildingSlot, buildingType);
	}

	public void BackToBoard()
	{
		BrassApplicationGame currentGame = _gameHistory.CurrentGame;
		if (currentGame.Metadata.GameType != GameType.Tutorial)
		{
			if (currentGame.Game.TurnFlow.CurrentAction == currentGame.Game.TurnFlow.BuildAction)
			{
				_presenter.CancelAction();
			}
			_presenter.BackToBoard();
		}
	}

	public void SetBuildingSelectedDelegate(ISelectBuildingDelegate selectBuildingDelegate)
	{
		_selectBuildingDelegate = selectBuildingDelegate;
	}
}
