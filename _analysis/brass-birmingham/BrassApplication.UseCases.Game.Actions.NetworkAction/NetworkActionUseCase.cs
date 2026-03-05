using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Board.Links;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.Common;
using BoardGameRules.Entities.GamePhases;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.Players;
using BoardGameRules.Utils.Logging;
using BrassApplication.Game;
using BrassApplication.GameHistory;
using BrassApplication.UseCases.Game.Actions.BaseAction;
using BrassApplication.UseCases.Game.Actions.BeerDelivery;
using BrassApplication.UseCases.Game.Actions.CoalDelivery;
using BrassApplication.UseCases.Game.Replay;

namespace BrassApplication.UseCases.Game.Actions.NetworkAction;

public class NetworkActionUseCase : BaseActionUseCase, INetworkActionInput, IBaseActionInput, IActionInProgressCancelDelegate, IReplayDelegate, IBeerDeliveryResultDelegate, ICoalDeliveryResultDelegate, ILinkSelectedDelegate, ISelectCardDelegate
{
	protected static readonly ILog Logger = LogFactory.BuildLogger(typeof(NetworkActionUseCase));

	protected readonly INetworkActionOutput _presenter;

	private Player _currentPlayer;

	private BoardGameRules.Entities.Actions.NetworkAction _networkAction;

	private int _remainingLinksToPlace;

	private Link _link;

	protected NetworkActionUseCaseState _networkActionUseCaseState;

	public NetworkActionUseCase(INetworkActionOutput presenter, IGameHistory gameHistory)
		: base(presenter, gameHistory)
	{
		_presenter = presenter;
	}

	public void StartNetworkAction()
	{
		BrassApplicationGame currentGame = _gameHistory.CurrentGame;
		_currentPlayer = currentGame.Game.GameProgressInformation.CurrentPlayer;
		_networkAction = currentGame.Game.TurnFlow.StartNetworkAction();
		_presenter.AllowRegionInteractions(isRaycastTarget: false);
		_presenter.ShowActionInProgressView(this, currentGame.Game.TurnFlow.GetCurrentActionNumber(), currentGame.Game.TurnFlow.GetMaxActionsCount());
		_remainingLinksToPlace = 2;
		_presenter.ShowCardsToDiscard(this);
		_presenter.ShowPlayerInfoToSelectCardToDiscard();
		_networkActionUseCaseState = NetworkActionUseCaseState.SelectingCard;
	}

	public void SelectCard(BaseCard card)
	{
		_networkAction.DiscardCard(card, _currentPlayer);
		_presenter.HideCards();
		PresentEvents();
	}

	public override void ReplayCompleted()
	{
		Logger.Debug("NetworkActionUseCase.ReplayCompleted");
		if (_networkActionUseCaseState == NetworkActionUseCaseState.SelectingCard)
		{
			_presenter.ShowPlayerInfoToSelectLinks();
			BrassApplicationGame currentGame = _gameHistory.CurrentGame;
			List<Link> validConnectionSlots = _networkAction.GetValidConnectionSlots(currentGame.Game.Board, currentGame.Game.GameProgressInformation);
			_presenter.HighlightValidConnections(currentGame, validConnectionSlots, this);
			_networkActionUseCaseState = NetworkActionUseCaseState.SelectLink;
		}
		else if (_networkActionUseCaseState == NetworkActionUseCaseState.SelectLink)
		{
			if (_networkAction.IsCoalNeeded())
			{
				_presenter.AllowRegionInteractions(isRaycastTarget: true);
				StartCoalDelivery();
			}
			else if (_networkAction.IsBeerNeeded())
			{
				StartBeerDelivery();
			}
			else if (_remainingLinksToPlace != 0)
			{
				_presenter.ShowPlayerInfoToSelectIfAnotherTrackShouldBePlaced();
				_presenter.ShowAnotherTrackSelectionPanel(_networkAction.GetNetworkActionNotPossibleReasons(_gameHistory.CurrentGame.Game.Board, _gameHistory.CurrentGame.Game.GameProgressInformation));
				_networkActionUseCaseState = NetworkActionUseCaseState.ChoosePlaceAnother;
			}
		}
		else if (_networkActionUseCaseState == NetworkActionUseCaseState.CoalDelivery)
		{
			if (_remainingLinksToPlace > 0)
			{
				_presenter.ShowPlayerInfoToSelectIfAnotherTrackShouldBePlaced();
				_presenter.ShowAnotherTrackSelectionPanel(_networkAction.GetNetworkActionNotPossibleReasons(_gameHistory.CurrentGame.Game.Board, _gameHistory.CurrentGame.Game.GameProgressInformation));
				_networkActionUseCaseState = NetworkActionUseCaseState.ChoosePlaceAnother;
			}
			else if (_networkAction.IsBeerNeeded())
			{
				StartBeerDelivery();
			}
		}
		else if (_networkActionUseCaseState == NetworkActionUseCaseState.Finish)
		{
			base.ReplayCompleted();
			_presenter.SetFastForwardButtonActive(isEnabled: true);
			return;
		}
		_presenter.SetFastForwardButtonActive(isEnabled: false);
	}

	protected void BaseReplayCompleted()
	{
		base.ReplayCompleted();
	}

	public void LinkSelected(Link link)
	{
		_link = link;
		_networkAction.PlaceLink(link);
		_remainingLinksToPlace--;
		if (_gameHistory.CurrentGame.Game.GameProgressInformation.CurrentGameEra == GameEra.CanalEra)
		{
			_networkActionUseCaseState = NetworkActionUseCaseState.Finish;
			_presenter.AllowRegionInteractions(isRaycastTarget: true);
			ConfirmAction();
		}
		else if (_networkAction.IsCoalNeeded() || _networkAction.IsBeerNeeded() || _remainingLinksToPlace != 0)
		{
			PresentEvents();
		}
		else
		{
			_networkActionUseCaseState = NetworkActionUseCaseState.Finish;
			_presenter.AllowRegionInteractions(isRaycastTarget: true);
			ConfirmAction();
		}
	}

	public void SelectCoalSource(ICoalSource coalSource, int amount = 1)
	{
		_networkAction.SupplyCoal(coalSource, amount);
		_presenter.HideCoalDelivery();
		if (_remainingLinksToPlace > 0)
		{
			PresentEvents();
			return;
		}
		if (_networkAction.IsBeerNeeded())
		{
			PresentEvents();
			return;
		}
		_networkActionUseCaseState = NetworkActionUseCaseState.Finish;
		_presenter.AllowRegionInteractions(isRaycastTarget: true);
		ConfirmAction();
	}

	private void StartCoalDelivery()
	{
		_networkActionUseCaseState = NetworkActionUseCaseState.CoalDelivery;
		List<Region> targetRegions = new List<Region> { _link.Region1, _link.Region2, _link.Region3 };
		targetRegions.RemoveAll((Region region) => region == null);
		IEnumerable<CoalMine> possibleCoalMinesSources = from c in _gameHistory.CurrentGame.Game.Board.GetUnflippedCoalMines()
			where targetRegions.Any((Region r) => _networkAction.GeneralAction.ConsumingCoal.CanCoalBeSuppliedToRegionFromSource(c, r))
			select c;
		_presenter.StartCoalDeliveryUseCase(_networkAction.NeededCoal, possibleCoalMinesSources, this);
	}

	private void StartBeerDelivery()
	{
		_networkActionUseCaseState = NetworkActionUseCaseState.BeerDelivery;
		List<Region> targetRegions = new List<Region> { _link.Region1, _link.Region2, _link.Region3 };
		targetRegions.RemoveAll((Region region) => region == null);
		IEnumerable<Brewery> possibleBrewerySources = from b in _gameHistory.CurrentGame.Game.Board.GetUnflippedBreweries()
			where targetRegions.Any((Region r) => _networkAction.GeneralAction.ConsumingBeer.CanBeerBeSuppliedToSlotFromSource(b, r, _currentPlayer))
			select b;
		_presenter.StartBeerDeliveryUseCase(_networkAction.NeededBeer, possibleBrewerySources, this);
	}

	public void SelectBeerSource(IBeerSource beerSource, int amount = 1)
	{
		_networkAction.SupplyBeer(beerSource, amount);
		_presenter.HideBeerDelivery();
		_networkActionUseCaseState = NetworkActionUseCaseState.Finish;
		ConfirmAction();
	}

	public override void CancelAction()
	{
		if (_networkActionUseCaseState == NetworkActionUseCaseState.ChoosePlaceAnother)
		{
			_presenter.HideNumberOfTracksToBuildSelectionPanel();
		}
		_presenter.AllowRegionInteractions(isRaycastTarget: true);
		_presenter.HideCoalDelivery();
		base.CancelAction();
	}

	public void EndNetworkAction()
	{
		_presenter.HideNumberOfTracksToBuildSelectionPanel();
		_networkActionUseCaseState = NetworkActionUseCaseState.Finish;
		ConfirmAction();
	}

	public void SelectSecondLink()
	{
		_presenter.HideNumberOfTracksToBuildSelectionPanel();
		_presenter.ShowPlayerInfoToSelectLinks();
		BrassApplicationGame currentGame = _gameHistory.CurrentGame;
		List<Link> validConnectionSlots = _networkAction.GetValidConnectionSlots(currentGame.Game.Board, currentGame.Game.GameProgressInformation);
		_presenter.HighlightValidConnections(currentGame, validConnectionSlots, this);
		_networkActionUseCaseState = NetworkActionUseCaseState.SelectLink;
	}
}
