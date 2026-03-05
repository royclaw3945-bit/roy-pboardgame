using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.Markets;
using BrassApplication.Game;
using BrassApplication.GameHistory;
using BrassApplication.UseCases.Common;

namespace BrassApplication.UseCases.Game.Actions.CoalDelivery;

public class CoalDeliveryUseCase : IUseCase, ICoalDeliveryInput, ISelectIndustryDelegate, ISelectMarketDelegate
{
	private readonly ICoalDeliveryOutput _presenter;

	private readonly IGameHistory _gameHistory;

	private ICoalDeliveryResultDelegate _coalDeliveryResultDelegate;

	private int _coalToDeliver;

	public CoalDeliveryUseCase(ICoalDeliveryOutput presenter, IGameHistory gameHistory)
	{
		_presenter = presenter;
		_gameHistory = gameHistory;
	}

	public void StartCoalDelivery(int coalToDeliver, IEnumerable<CoalMine> possibleCoalMinesSources, ICoalDeliveryResultDelegate coalDeliveryResultDelegate)
	{
		BrassApplicationGame game = _gameHistory.CurrentGame;
		_coalToDeliver = coalToDeliver;
		_coalDeliveryResultDelegate = coalDeliveryResultDelegate;
		if (possibleCoalMinesSources.Count() == 1)
		{
			int num = Math.Min(possibleCoalMinesSources.First().AvailableCoal, coalToDeliver);
			_coalToDeliver -= num;
			_coalDeliveryResultDelegate.SelectCoalSource(possibleCoalMinesSources.First(), num);
		}
		else if (possibleCoalMinesSources.Count() > 1)
		{
			List<BuildingSlot> buildingSlotsToHighlight = possibleCoalMinesSources.Select((CoalMine coalMine) => game.Game.Board.GetBuildingSlotContainingGivenIndustry(coalMine)).ToList();
			_presenter.HighlightCoalMines(game.Game.Board, buildingSlotsToHighlight, this);
			_presenter.ShowPlayerInfoToSelectCoalMine();
		}
		else
		{
			_presenter.ShowPlayerInfoToSelectCoalMarket();
			_presenter.HighlightMarket(game.Game.Board.CoalMarket, this, coalToDeliver);
		}
	}

	public void SelectIndustrySlot(BuildingSlot buildingSlot)
	{
		_coalToDeliver--;
		_coalDeliveryResultDelegate.SelectCoalSource((CoalMine)buildingSlot.BuildedIndustry);
	}

	public void SelectMarket(BaseMarket market)
	{
		_presenter.TakeMarketResource();
		_coalDeliveryResultDelegate.SelectCoalSource((CoalMarket)market, _coalToDeliver);
		_coalToDeliver = 0;
	}

	private void ShowBuildRewards()
	{
		_presenter.ReplayEvents(_gameHistory.CurrentGame.Game.GetUncommittedEvents());
	}

	public void HideCoalDelivery()
	{
		_presenter.HideMarketHighlights();
		_presenter.HighlightCoalMines(_gameHistory.CurrentGame.Game.Board, new List<BuildingSlot>(), this);
	}
}
