using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Industries;
using BrassApplication.Game;
using BrassApplication.GameHistory;
using BrassApplication.UseCases.Game.Actions.DevelopAction;

namespace BrassApplication.UseCases.Tutorials.Actions.Develop;

public class TutorialDevelopActionUseCase : DevelopActionUseCase
{
	private ITutorialDevelopActionOutput _presenter;

	public TutorialDevelopActionUseCase(ITutorialDevelopActionOutput presenter, IGameHistory gameHistory)
		: base(presenter, gameHistory)
	{
		_presenter = presenter;
	}

	public override void ReplayCompleted()
	{
		DevelopActionUseCase.Logger.Debug("DevelopActionUseCase.ReplayCompleted");
		if (_developActionUseCaseState == DevelopActionUseCaseState.SelectingCard)
		{
			_presenter.HideCards();
			_presenter.ShowIndustriesDetailsView(_gameHistory.CurrentGame, this);
			_presenter.ShowPlayerInfoToSelectIndustry();
			_developActionUseCaseState = DevelopActionUseCaseState.SelectingIndustry;
		}
		else if (_developActionUseCaseState == DevelopActionUseCaseState.SelectingIndustry)
		{
			FinishAction();
		}
		else
		{
			base.ReplayCompleted();
		}
	}

	public override void FinishAction()
	{
		_presenter.HideFinishButton();
		_presenter.HideIndustriesDetailsView();
		StartTutorialDevelopIronDelivery();
		_developActionUseCaseState = DevelopActionUseCaseState.IronDelivery;
		ReplayCompleted();
	}

	public void HighlightCottonMillOnly()
	{
		_presenter.HighlightCottonMillOnly(_gameHistory.CurrentGame);
	}

	public void HighlightAllPosiible()
	{
		_presenter.HighlightAllPosiible(_gameHistory.CurrentGame);
	}

	public void HighlightNone()
	{
		_presenter.HighlightNone();
	}

	public void SelectIronSource(BuildingSlot buildingSlot)
	{
		SelectIronSource((IronWorks)buildingSlot.BuildedIndustry);
	}

	public void StartTutorialDevelopIronDelivery()
	{
		BrassApplicationGame game = _gameHistory.CurrentGame;
		_currentPlayer = game.Game.GameProgressInformation.CurrentPlayer;
		List<BuildingSlot> slotsToHighlight = (from iw in game.Game.Board.GetUnflippedIronWorks().ToList()
			select game.Game.Board.GetBuildingSlotContainingGivenIndustry(iw)).ToList();
		_presenter.HighlightIronWorks(game.Game.Board, slotsToHighlight);
		_presenter.ShowPlayerInfoToSelectIronWorks();
	}
}
