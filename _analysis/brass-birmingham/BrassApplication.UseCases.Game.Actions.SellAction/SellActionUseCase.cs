using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Merchants;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.Common;
using BoardGameRules.Entities.Game;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.Players;
using BrassApplication.Game;
using BrassApplication.GameHistory;
using BrassApplication.UseCases.Game.Actions.BaseAction;
using BrassApplication.UseCases.Game.Actions.BeerDelivery;
using BrassApplication.UseCases.Game.Replay;

namespace BrassApplication.UseCases.Game.Actions.SellAction;

public class SellActionUseCase : BaseActionUseCase, ISellActionInput, IBaseActionInput, IActionInProgressCancelDelegate, IReplayDelegate, ISelectCardDelegate, ISelectIndustryDelegate, ISelectIndustryOfTypeDelegate, IBeerDeliveryResultDelegate
{
	private readonly ISellActionOutput _presenter;

	protected Player _currentPlayer;

	protected BuildingSlot _buildingSlot;

	protected BaseIndustry _baseIndustry;

	protected MerchantSlot _usedMerchantSlot;

	protected BoardGameRules.Entities.Actions.SellAction _sellAction;

	protected SellActionUseCaseState _sellActionUseCaseState;

	public SellActionUseCase(ISellActionOutput presenter, IGameHistory gameHistory)
		: base(presenter, gameHistory)
	{
		_presenter = presenter;
	}

	public void StartSellAction()
	{
		BrassApplicationGame currentGame = _gameHistory.CurrentGame;
		_currentPlayer = currentGame.Game.GameProgressInformation.CurrentPlayer;
		_usedMerchantSlot = null;
		_sellAction = currentGame.Game.TurnFlow.StartSellAction();
		_presenter.ShowCardsToDiscard(this);
		_presenter.ShowPlayerInfoToSelectCardToDiscard(this, currentGame.Game.TurnFlow.GetCurrentActionNumber(), currentGame.Game.TurnFlow.GetMaxActionsCount());
		_sellActionUseCaseState = SellActionUseCaseState.SelectingCard;
	}

	public void SelectCard(BaseCard card)
	{
		_sellAction.DiscardCard(card, _currentPlayer);
		_presenter.HideCards();
		PresentEvents();
	}

	public void SelectIndustrySlot(BuildingSlot buildingSlot)
	{
		_sellActionUseCaseState = SellActionUseCaseState.SelectingIndustry;
		_buildingSlot = buildingSlot;
		_baseIndustry = buildingSlot.BuildedIndustry;
		_sellAction.StartSellProcedure(buildingSlot.BuildedIndustry);
		PresentEvents();
	}

	protected void StartBeerDelivery()
	{
		BoardGameRulesGame game = _gameHistory.CurrentGame.Game;
		BoardGameRules.Entities.Board.Board board = game.Board;
		Region targetRegion = game.Board.GetRegionContainingGivenSlot(_buildingSlot);
		IEnumerable<Brewery> source = from b in game.Board.GetUnflippedBreweries()
			where _sellAction.GeneralAction.ConsumingBeer.CanBeerBeSuppliedToSlotFromSource(b, targetRegion, _currentPlayer)
			select b;
		IEnumerable<MerchantSlot> second = new List<MerchantSlot>();
		if (_usedMerchantSlot == null)
		{
			second = from m in game.Board.GetMerchantsWithBeer()
				where m.MerchantTile.BuildingTypes.Contains(_baseIndustry.BuildingType) && board.DoesRouteExists(board.GetRegionOfGivenMerchantTile(m), board.GetRegionContainingGivenIndustry(_baseIndustry))
				select m;
		}
		IEnumerable<IBeerSource> possibleBeerSources = source.Cast<IBeerSource>().Concat(second);
		_presenter.StartBeerDeliveryUseCase(_sellAction.NeededBeer, possibleBeerSources, this);
		_sellActionUseCaseState = SellActionUseCaseState.SelectingBeer;
	}

	public void SelectBeerSource(IBeerSource beerSource, int amount = 1)
	{
		_sellAction.SupplyBeer(beerSource, amount);
		_usedMerchantSlot = beerSource as MerchantSlot;
		PresentEvents();
	}

	public void SelectIndustryOfType(BuildingType buildingType)
	{
		_gameHistory.CurrentGame.Game.DevelopMerchantBonus.Develop(_currentPlayer, buildingType);
		PresentEvents();
	}

	public override void ReplayCompleted()
	{
		if (_sellActionUseCaseState == SellActionUseCaseState.SelectingCard)
		{
			BrassApplicationGame currentGame = _gameHistory.CurrentGame;
			List<BuildingSlot> validBuildingSlotsForSellingIndustries = _sellAction.GetValidBuildingSlotsForSellingIndustries(currentGame.Game.Board, _currentPlayer);
			_presenter.HighlightValidBuildingSlots(currentGame.Game.Board, validBuildingSlotsForSellingIndustries, this);
			_presenter.ShowPlayerInfoToSelectIndustry(this);
			_sellActionUseCaseState = SellActionUseCaseState.SelectingIndustry;
		}
		else if (_sellActionUseCaseState == SellActionUseCaseState.SelectingIndustry)
		{
			if (_sellAction.IsBeerNeeded())
			{
				StartBeerDelivery();
			}
			else
			{
				TryNextSellingOrEndAction();
			}
		}
		else if (_sellActionUseCaseState == SellActionUseCaseState.SelectingBeer)
		{
			if (_sellAction.IsBeerNeeded())
			{
				StartBeerDelivery();
			}
			else if (DevelopMerchantBonus.Instance.MerchantDevelopmentNeeded)
			{
				_sellActionUseCaseState = SellActionUseCaseState.DevelopBonus;
				_presenter.ShowMerchantDevelopBonus(_gameHistory.CurrentGame, this);
			}
			else
			{
				_sellAction.SellIndustry();
				_sellActionUseCaseState = SellActionUseCaseState.BeerSelectedAnimation;
				PresentEvents();
			}
		}
		else if (_sellActionUseCaseState == SellActionUseCaseState.BeerSelectedAnimation)
		{
			TryNextSellingOrEndAction();
		}
		else if (_sellActionUseCaseState == SellActionUseCaseState.DevelopBonus)
		{
			_sellActionUseCaseState = SellActionUseCaseState.SellAnimation;
			_presenter.HideIndustriesDetailsView();
			_sellAction.SellIndustry();
			PresentEvents();
		}
		else
		{
			if (_sellActionUseCaseState != SellActionUseCaseState.SellAnimation)
			{
				base.ReplayCompleted();
				_presenter.SetFastForwardButtonActive(isEnabled: true);
				return;
			}
			TryNextSellingOrEndAction();
		}
		_presenter.SetFastForwardButtonActive(isEnabled: false);
	}

	protected void TryNextSellingOrEndAction()
	{
		_usedMerchantSlot = null;
		_sellActionUseCaseState = SellActionUseCaseState.Choice;
		List<BuildingSlot> validBuildingSlotsForSellingIndustries = _sellAction.GetValidBuildingSlotsForSellingIndustries(_gameHistory.CurrentGame.Game.Board, _currentPlayer);
		if (validBuildingSlotsForSellingIndustries.Any())
		{
			_presenter.ShowNextSellChoicePanel(validBuildingSlotsForSellingIndustries);
		}
		else
		{
			EndSellAction();
		}
	}

	public virtual void EndSellAction()
	{
		_presenter.HideNextSellChoicePanel();
		ConfirmAction();
	}

	public virtual void SelectNextIndustryToSell()
	{
		BrassApplicationGame currentGame = _gameHistory.CurrentGame;
		List<BuildingSlot> validBuildingSlotsForSellingIndustries = _sellAction.GetValidBuildingSlotsForSellingIndustries(currentGame.Game.Board, _currentPlayer);
		_presenter.HighlightValidBuildingSlots(currentGame.Game.Board, validBuildingSlotsForSellingIndustries, this);
		_presenter.ShowPlayerInfoToSelectIndustry(this);
		_presenter.HideNextSellChoicePanel();
	}
}
