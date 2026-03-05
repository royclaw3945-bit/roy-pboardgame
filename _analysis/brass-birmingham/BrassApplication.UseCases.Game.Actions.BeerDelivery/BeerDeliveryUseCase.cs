using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Merchants;
using BoardGameRules.Entities.Common;
using BoardGameRules.Entities.Industries;
using BrassApplication.Game;
using BrassApplication.GameHistory;
using BrassApplication.UseCases.Common;

namespace BrassApplication.UseCases.Game.Actions.BeerDelivery;

public class BeerDeliveryUseCase : IUseCase, IBeerDeliveryInput, ISelectIndustryDelegate, ISelectMerchantSlotDelegate
{
	private readonly IBeerDeliveryOutput _presenter;

	private readonly IGameHistory _gameHistory;

	private IBeerDeliveryResultDelegate _beerDeliveryResultDelegate;

	private int _beerToDeliver;

	public BeerDeliveryUseCase(IBeerDeliveryOutput presenter, IGameHistory gameHistory)
	{
		_presenter = presenter;
		_gameHistory = gameHistory;
	}

	public void StartBeerDelivery(int beerToDeliver, IEnumerable<IBeerSource> possibleBeerSources, IBeerDeliveryResultDelegate beerDeliveryResultDelegate)
	{
		BrassApplicationGame game = _gameHistory.CurrentGame;
		_beerToDeliver = beerToDeliver;
		_beerDeliveryResultDelegate = beerDeliveryResultDelegate;
		if (possibleBeerSources.Count() == 1)
		{
			int num = Math.Min(possibleBeerSources.First().AvailableBeer, beerToDeliver);
			_beerToDeliver -= num;
			_beerDeliveryResultDelegate.SelectBeerSource(possibleBeerSources.First(), num);
			return;
		}
		List<BuildingSlot> buildingSlotsToHighlight = (from brewery in possibleBeerSources.OfType<Brewery>()
			select game.Game.Board.GetBuildingSlotContainingGivenIndustry(brewery)).ToList();
		_presenter.HighlightBreweries(game.Game.Board, buildingSlotsToHighlight, this);
		IEnumerable<MerchantSlot> merchantSlotsToHighlight = possibleBeerSources.OfType<MerchantSlot>();
		_presenter.HighlightMerchantSlots(game.Game.Board, merchantSlotsToHighlight, this);
		_presenter.ShowPlayerInfoToSelectBrewery();
	}

	public void SelectIndustrySlot(BuildingSlot buildingSlot)
	{
		_beerToDeliver--;
		_beerDeliveryResultDelegate.SelectBeerSource((Brewery)buildingSlot.BuildedIndustry);
	}

	public void SelectMerchantSlot(MerchantSlot merchantSlot)
	{
		_beerToDeliver--;
		_beerDeliveryResultDelegate.SelectBeerSource(merchantSlot);
	}

	private void ShowBuildRewards()
	{
		_presenter.ReplayEvents(_gameHistory.CurrentGame.Game.GetUncommittedEvents());
	}

	public void HideBeerDelivery()
	{
		_presenter.HighlightBreweries(_gameHistory.CurrentGame.Game.Board, new List<BuildingSlot>(), this);
	}
}
