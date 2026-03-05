using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.Common;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.Players;
using BrassApplication.Game;
using BrassApplication.GameHistory;
using BrassApplication.UseCases.Game.Actions.BaseAction;
using BrassApplication.UseCases.Game.Actions.CoalDelivery;
using BrassApplication.UseCases.Game.Actions.IronDelivery;
using BrassApplication.UseCases.Game.Replay;

namespace BrassApplication.UseCases.Game.Actions.BuildAction;

public class BuildActionUseCase : BaseActionUseCase, IBuildActionInput, IBaseActionInput, IActionInProgressCancelDelegate, IReplayDelegate, IIronDeliveryResultDelegate, ICoalDeliveryResultDelegate, ISelectBuildingDelegate, ISelectCardDelegate
{
	private readonly IBuildActionOutput _presenter;

	private Player _currentPlayer;

	private BoardGameRules.Entities.Actions.BuildAction _buildAction;

	private BuildingSlot _buildingSlot;

	private BuildingType _buildingType;

	private List<BaseCard> _validCards;

	private BuildActionUseCaseState _buildActionUseCaseState;

	public BuildActionUseCase(IBuildActionOutput presenter, IGameHistory gameHistory)
		: base(presenter, gameHistory)
	{
		_presenter = presenter;
	}

	public void StartBuildAction()
	{
		BrassApplicationGame currentGame = _gameHistory.CurrentGame;
		_currentPlayer = currentGame.Game.GameProgressInformation.CurrentPlayer;
		_buildAction = currentGame.Game.TurnFlow.StartBuildAction();
		List<BuildingSlot> buildingSlotsWhereBuildIsPossible = _buildAction.GetBuildingSlotsWhereBuildingIsPossible(currentGame.Game.Board, currentGame.Game.GameProgressInformation, (BuildingType t) => true).ToList();
		_presenter.HighlightValidBuildingSlots(currentGame.Game.Board, buildingSlotsWhereBuildIsPossible, this, ShowRegionDetails);
		_presenter.ShowPlayerInfoToSelectBuildingSlot(this, currentGame.Game.TurnFlow.GetCurrentActionNumber(), currentGame.Game.TurnFlow.GetMaxActionsCount());
		_buildActionUseCaseState = BuildActionUseCaseState.SelectingRegion;
	}

	public void StartBuildActionFromBoard(RegionName regionName)
	{
		BrassApplicationGame currentGame = _gameHistory.CurrentGame;
		if (currentGame.Game.TurnFlow.CurrentAction == null)
		{
			_currentPlayer = currentGame.Game.GameProgressInformation.CurrentPlayer;
			_buildAction = currentGame.Game.TurnFlow.StartBuildAction();
			List<BuildingSlot> buildingSlotsWhereBuildIsPossible = _buildAction.GetBuildingSlotsWhereBuildingIsPossible(currentGame.Game.Board, currentGame.Game.GameProgressInformation, (BuildingType t) => true).ToList();
			_presenter.HighlightValidBuildingSlotsInRegion(currentGame.Game.Board, buildingSlotsWhereBuildIsPossible, this, ShowRegionDetails, regionName);
			_presenter.ShowPlayerInfoToSelectBuildingSlot(this, currentGame.Game.TurnFlow.GetCurrentActionNumber(), currentGame.Game.TurnFlow.GetMaxActionsCount());
			ShowRegionDetails(regionName);
		}
	}

	public void ShowRegionDetails(RegionName regionName)
	{
		BrassApplicationGame currentGame = _gameHistory.CurrentGame;
		Dictionary<Tuple<BuildingSlot, BuildingType>, IList<CantUseBuildActionReason>> dictionary = new Dictionary<Tuple<BuildingSlot, BuildingType>, IList<CantUseBuildActionReason>>();
		List<BuildingSlot> buildingSlots = currentGame.Game.Board.GetRegionByName(regionName).BuildingSlots;
		Player currentPlayer = currentGame.Game.GameProgressInformation.CurrentPlayer;
		PlayerMat mat = currentGame.Game.PlayersTrack.Players.Find((Player p) => p.Color == currentPlayer.Color).Mat;
		foreach (BuildingSlot item in buildingSlots)
		{
			foreach (BuildingType buildingType in item.BuildingTypes)
			{
				BaseIndustry firstBuildingOfType = mat.GetFirstBuildingOfType(buildingType);
				List<CantUseBuildActionReason> cantUseBuildActionReasonForGivenSlot = _buildAction.GetCantUseBuildActionReasonForGivenSlot(item, firstBuildingOfType, currentPlayer, currentGame.Game.Board, currentGame.Game.GameProgressInformation);
				dictionary.Add(new Tuple<BuildingSlot, BuildingType>(item, buildingType), cantUseBuildActionReasonForGivenSlot);
			}
		}
		_presenter.ShowRegionDetailsView(regionName, currentGame, dictionary);
		_presenter.ShowPlayerInfoToSelectIndustry();
		_buildActionUseCaseState = BuildActionUseCaseState.SelectingSlot;
	}

	public void SelectBuilding(BuildingSlot buildingSlot, BuildingType buildingType)
	{
		BrassApplicationGame currentGame = _gameHistory.CurrentGame;
		_buildingSlot = buildingSlot;
		_buildingType = buildingType;
		_validCards = _buildAction.GetValidCardsForBuildingInSlot(buildingSlot, buildingType, _currentPlayer, currentGame.Game.Board).ToList();
		_presenter.HideRegionDetailsView();
		_presenter.TurnOffSlotsHighlightning(currentGame.Game.Board);
		_presenter.ShowValidCardsToBuild(_validCards, this);
		_presenter.ShowPlayerInfoToSelectCardToDiscard();
		_buildActionUseCaseState = BuildActionUseCaseState.SelectingCard;
	}

	public void SelectCard(BaseCard card)
	{
		if (_validCards.Contains(card))
		{
			_buildAction.DiscardCard(card, _currentPlayer);
			_buildAction.Build(_buildingSlot, _buildingType);
			_presenter.HideCards();
			if (_buildAction.IsIronNeeded() || _buildAction.IsCoalNeeded())
			{
				PresentEvents();
				return;
			}
			_buildActionUseCaseState = BuildActionUseCaseState.Finish;
			ConfirmAction();
		}
	}

	public void SelectIronSource(IIronSource ironSource, int amount = 1)
	{
		_buildAction.SupplyIron(ironSource, amount);
		_presenter.HideIronDelivery();
		if (_buildAction.IsIronNeeded() || _buildAction.IsCoalNeeded())
		{
			PresentEvents();
			return;
		}
		_buildActionUseCaseState = BuildActionUseCaseState.Finish;
		ConfirmAction();
	}

	private void StartCoalDelivery()
	{
		BrassApplicationGame currentGame = _gameHistory.CurrentGame;
		_buildActionUseCaseState = BuildActionUseCaseState.CoalDelivery;
		Region targetRegion = currentGame.Game.Board.GetRegionContainingGivenSlot(_buildingSlot);
		IEnumerable<CoalMine> possibleCoalMinesSources = from c in currentGame.Game.Board.GetUnflippedCoalMines()
			where _buildAction.GeneralAction.ConsumingCoal.CanCoalBeSuppliedToRegionFromSource(c, targetRegion)
			select c;
		_presenter.StartCoalDeliveryUseCase(_buildAction.NeededCoal, possibleCoalMinesSources, this);
	}

	public void SelectCoalSource(ICoalSource coalSource, int amount = 1)
	{
		_buildAction.SupplyCoal(coalSource, amount);
		_presenter.HideCoalDelivery();
		if (_buildAction.IsCoalNeeded())
		{
			PresentEvents();
			return;
		}
		_buildActionUseCaseState = BuildActionUseCaseState.Finish;
		ConfirmAction();
	}

	public override void ReplayCompleted()
	{
		if (_buildActionUseCaseState == BuildActionUseCaseState.Finish)
		{
			base.ReplayCompleted();
			_presenter.SetFastForwardButtonActive(isEnabled: true);
			return;
		}
		if (_buildAction.IsIronNeeded())
		{
			_buildActionUseCaseState = BuildActionUseCaseState.IronDelivery;
			_presenter.StartIronDeliveryUseCase(_buildAction.NeededIron, this);
		}
		else if (_buildAction.IsCoalNeeded())
		{
			_buildActionUseCaseState = BuildActionUseCaseState.CoalDelivery;
			StartCoalDelivery();
		}
		_presenter.SetFastForwardButtonActive(isEnabled: false);
	}

	public override void CancelAction()
	{
		if (_buildActionUseCaseState == BuildActionUseCaseState.SelectingSlot)
		{
			_presenter.HideRegionDetailsView();
		}
		if (_buildActionUseCaseState == BuildActionUseCaseState.IronDelivery)
		{
			_presenter.HideIronDelivery();
		}
		if (_buildActionUseCaseState == BuildActionUseCaseState.CoalDelivery)
		{
			_presenter.HideCoalDelivery();
		}
		base.CancelAction();
	}
}
