using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.Markets;
using BoardGameRules.Entities.Players;
using BrassApplication.Game;
using BrassApplication.GameHistory;
using BrassApplication.UseCases.Common;

namespace BrassApplication.UseCases.Game.Actions.IronDelivery;

public class IronDeliveryUseCase : IUseCase, IIronDeliveryInput, ISelectIndustryDelegate, ISelectMarketDelegate
{
	private readonly IIronDeliveryOutput _presenter;

	private readonly IGameHistory _gameHistory;

	private IIronDeliveryResultDelegate _ironDeliveryResultDelegate;

	private Player _currentPlayer;

	private int _ironToDeliver;

	private IronDeliveryUseCaseState _ironDeliveryUseCaseState;

	public IronDeliveryUseCase(IIronDeliveryOutput presenter, IGameHistory gameHistory)
	{
		_presenter = presenter;
		_gameHistory = gameHistory;
	}

	public void StartIronDelivery(int ironToDeliver, IIronDeliveryResultDelegate ironDeliveryResultDelegate)
	{
		BrassApplicationGame game = _gameHistory.CurrentGame;
		_ironToDeliver = ironToDeliver;
		_ironDeliveryResultDelegate = ironDeliveryResultDelegate;
		_currentPlayer = game.Game.GameProgressInformation.CurrentPlayer;
		List<IronWorks> list = game.Game.Board.GetUnflippedIronWorks().ToList();
		if (!list.Any())
		{
			_presenter.ShowPlayerInfoToSelectIronMarket();
			_presenter.HighlightMarket(game.Game.Board.IronMarket, this, ironToDeliver);
			_ironDeliveryUseCaseState = IronDeliveryUseCaseState.HighlightedMarkets;
			return;
		}
		if (list.Count == 1)
		{
			int num = Math.Min(list.First().AvailableIron, ironToDeliver);
			_ironToDeliver -= num;
			_ironDeliveryResultDelegate.SelectIronSource(list.First(), num);
			return;
		}
		List<BuildingSlot> buildingSlotsToHighlight = list.Select((IronWorks iw) => game.Game.Board.GetBuildingSlotContainingGivenIndustry(iw)).ToList();
		_presenter.HighlightIronWorks(game.Game.Board, buildingSlotsToHighlight, this);
		_presenter.ShowPlayerInfoToSelectIronWorks();
		_ironDeliveryUseCaseState = IronDeliveryUseCaseState.HighlightedSlots;
	}

	public void SelectIndustrySlot(BuildingSlot buildingSlot)
	{
		_ironToDeliver--;
		_ironDeliveryResultDelegate.SelectIronSource((IronWorks)buildingSlot.BuildedIndustry);
	}

	public void SelectMarket(BaseMarket market)
	{
		_presenter.TakeMarketResource();
		_ironDeliveryResultDelegate.SelectIronSource((IronMarket)market, _ironToDeliver);
		_ironToDeliver = 0;
	}

	public void HideIronDelivery()
	{
		if (_ironDeliveryUseCaseState == IronDeliveryUseCaseState.HighlightedMarkets)
		{
			_presenter.HideMarketHighlights();
		}
		if (_ironDeliveryUseCaseState == IronDeliveryUseCaseState.HighlightedSlots)
		{
			_presenter.HighlightIronWorks(_gameHistory.CurrentGame.Game.Board, new List<BuildingSlot>(), this);
		}
	}

	public void DeliverIronAndHide()
	{
		if (_ironDeliveryUseCaseState == IronDeliveryUseCaseState.HighlightedMarkets)
		{
			_presenter.TakeMarketResource();
		}
	}

	private void ShowBuildRewards()
	{
		_presenter.ReplayEvents(_gameHistory.CurrentGame.Game.GetUncommittedEvents());
	}
}
