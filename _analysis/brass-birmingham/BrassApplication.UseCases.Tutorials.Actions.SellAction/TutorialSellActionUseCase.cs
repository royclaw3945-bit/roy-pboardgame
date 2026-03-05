using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Common;
using BrassApplication.Game;
using BrassApplication.GameHistory;
using BrassApplication.UseCases.Game.Actions.SellAction;

namespace BrassApplication.UseCases.Tutorials.Actions.SellAction;

public class TutorialSellActionUseCase : SellActionUseCase
{
	private readonly ITutorialSellActionOutput _presenter;

	private IEnumerable<IBeerSource> _possibleBeerSources;

	private bool _firstSell = true;

	public TutorialSellActionUseCase(ITutorialSellActionOutput presenter, IGameHistory gameHistory)
		: base(presenter, gameHistory)
	{
		_presenter = presenter;
	}

	public override void ReplayCompleted()
	{
		if (_firstSell)
		{
			if (_sellActionUseCaseState == SellActionUseCaseState.SelectingIndustry && _sellAction.IsBeerNeeded())
			{
				_presenter.NextTutorialStage();
			}
			if (_sellActionUseCaseState == SellActionUseCaseState.SelectingCard)
			{
				BrassApplicationGame currentGame = _gameHistory.CurrentGame;
				List<BuildingSlot> validBuildingSlotsForSellingIndustries = _sellAction.GetValidBuildingSlotsForSellingIndustries(currentGame.Game.Board, _currentPlayer);
				_presenter.HighlightValidBuildingSlots(currentGame.Game.Board, validBuildingSlotsForSellingIndustries, this);
				_presenter.ShowPlayerInfoToSelectIndustry(this);
				_sellActionUseCaseState = SellActionUseCaseState.SelectingIndustry;
				_presenter.HideReplayView();
			}
			else if (_sellActionUseCaseState == SellActionUseCaseState.SelectingIndustry)
			{
				if (_sellAction.IsBeerNeeded())
				{
					TutorialStartBeerDelivery();
				}
				else
				{
					TryNextStageOrEndAction();
				}
				_presenter.HideReplayView();
			}
			else if (_sellActionUseCaseState == SellActionUseCaseState.SelectingBeer)
			{
				if (!_sellAction.IsBeerNeeded())
				{
					if (DevelopMerchantBonus.Instance.MerchantDevelopmentNeeded)
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
			}
			else if (_sellActionUseCaseState == SellActionUseCaseState.BeerSelectedAnimation)
			{
				TryNextStageOrEndAction();
				_presenter.HideReplayView();
			}
			else if (_sellActionUseCaseState == SellActionUseCaseState.DevelopBonus)
			{
				_sellActionUseCaseState = SellActionUseCaseState.SellAnimation;
				_presenter.HideIndustriesDetailsView();
				_sellAction.SellIndustry();
				PresentEvents();
			}
			else if (_sellActionUseCaseState == SellActionUseCaseState.SellAnimation)
			{
				TryNextStageOrEndAction();
				_presenter.HideReplayView();
			}
			else
			{
				_presenter.HideReplayView();
			}
		}
		else
		{
			if (_sellActionUseCaseState == SellActionUseCaseState.BeerSelectedAnimation)
			{
				TutorialTryNextSellingOrEndAction();
				base.ReplayCompleted();
			}
			base.ReplayCompleted();
		}
	}

	protected void TutorialStartBeerDelivery()
	{
		_presenter.StartBeerDeliveryUseCase(_sellAction.NeededBeer, _possibleBeerSources, this);
		_sellActionUseCaseState = SellActionUseCaseState.SelectingBeer;
	}

	public void SetPossibleBeerSources(IEnumerable<IBeerSource> beerSources)
	{
		_possibleBeerSources = beerSources;
	}

	public void InitiateNextSell()
	{
		_sellActionUseCaseState = SellActionUseCaseState.Choice;
		List<BuildingSlot> validBuildingSlotsForSellingIndustries = _sellAction.GetValidBuildingSlotsForSellingIndustries(_gameHistory.CurrentGame.Game.Board, _currentPlayer);
		_presenter.ShowTutorialNextSellChoicePanel(validBuildingSlotsForSellingIndustries);
	}

	public override void EndSellAction()
	{
		_presenter.HideNextSellChoicePanel();
		ConfirmAction();
	}

	public override void SelectNextIndustryToSell()
	{
		BrassApplicationGame currentGame = _gameHistory.CurrentGame;
		List<BuildingSlot> validBuildingSlotsForSellingIndustries = _sellAction.GetValidBuildingSlotsForSellingIndustries(currentGame.Game.Board, _currentPlayer);
		_presenter.HighlightValidBuildingSlots(currentGame.Game.Board, validBuildingSlotsForSellingIndustries, this);
		_presenter.ShowPlayerInfoToSelectIndustry(this);
		_presenter.HideNextSellChoicePanel();
	}

	protected void TryNextStageOrEndAction()
	{
		_sellActionUseCaseState = SellActionUseCaseState.Choice;
		if (_sellAction.GetValidBuildingSlotsForSellingIndustries(_gameHistory.CurrentGame.Game.Board, _currentPlayer).Any())
		{
			_presenter.NextTutorialStage();
			return;
		}
		_firstSell = false;
		EndSellAction();
		_presenter.NextTutorialStage();
	}

	protected void TutorialTryNextSellingOrEndAction()
	{
		_sellActionUseCaseState = SellActionUseCaseState.Choice;
		List<BuildingSlot> validBuildingSlotsForSellingIndustries = _sellAction.GetValidBuildingSlotsForSellingIndustries(_gameHistory.CurrentGame.Game.Board, _currentPlayer);
		if (validBuildingSlotsForSellingIndustries.Any())
		{
			_presenter.ShowTutorialNextSellChoicePanel(validBuildingSlotsForSellingIndustries);
			return;
		}
		EndSellAction();
		_presenter.NextTutorialStage();
	}
}
