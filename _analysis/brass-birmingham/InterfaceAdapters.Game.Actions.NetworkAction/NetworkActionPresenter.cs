using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Board.Links;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Entities.GamePhases;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.Players;
using BrassApplication.Game;
using BrassApplication.UseCases.Game.Actions;
using BrassApplication.UseCases.Game.Actions.ActionInProgress;
using BrassApplication.UseCases.Game.Actions.BaseAction;
using BrassApplication.UseCases.Game.Actions.BeerDelivery;
using BrassApplication.UseCases.Game.Actions.CoalDelivery;
using BrassApplication.UseCases.Game.Actions.NetworkAction;
using BrassApplication.UseCases.Game.Board;
using BrassApplication.UseCases.Game.HandOfCards;
using InterfaceAdapters.Game.ActionInProgressView;
using InterfaceAdapters.Game.Actions.BaseAction;
using InterfaceAdapters.Game.Board;
using InterfaceAdapters.Game.HandOfCards;
using InterfaceAdapters.Game.PlayerActions;
using InterfaceAdapters.Game.Replay;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Utils;

namespace InterfaceAdapters.Game.Actions.NetworkAction;

public class NetworkActionPresenter : BaseActionPresenter, INetworkActionOutput, IBaseActionOutput
{
	public NetworkActionPresenter(IRouter router, ILocalization localization)
		: base(router, localization)
	{
	}

	public void HighlightValidConnections(BrassApplicationGame game, List<Link> linksThatCanBePlaced, ILinkSelectedDelegate linkSelectedDelegate)
	{
		_router.UseCaseBuilder.GetUseCase<BoardUseCase>().SetLinkSelectedDelegate(linkSelectedDelegate);
		Player currentPlayer = game.Game.GameProgressInformation.CurrentPlayer;
		GameEra currentGameEra = game.Game.GameProgressInformation.CurrentGameEra;
		Dictionary<string, LinkViewModel> dictionary = new Dictionary<string, LinkViewModel>();
		bool flag = currentGameEra == GameEra.CanalEra;
		foreach (Link item in game.Game.Board.Regions.SelectMany((Region r) => r.Links).Distinct())
		{
			string key = string.Concat(item.Region1.Name, item.Region2.Name);
			if (((currentGameEra == GameEra.CanalEra) ? (item is Canal) : (item is Rail)) && dictionary.ContainsKey(key))
			{
				LinkViewModel linkViewModel = dictionary[key];
				linkViewModel.Link = item;
				linkViewModel.IsRiverVisible |= item is Canal;
				linkViewModel.IsRailVisible |= item is Rail;
				linkViewModel.IsTokenVisible |= item.HasOwner;
				linkViewModel.IsOutlineVisible = linksThatCanBePlaced.Contains(item);
				linkViewModel.IsInteractive = linkViewModel.IsOutlineVisible;
				linkViewModel.CurrentEra = currentGameEra;
				linkViewModel.TokenColor = item.Owner?.Color ?? currentPlayer.Color;
			}
			else if (!dictionary.ContainsKey(key))
			{
				dictionary.Add(key, new LinkViewModel
				{
					Link = item,
					IsRiverVisible = (flag && item is Canal),
					IsRailVisible = (item is Rail),
					IsTokenVisible = item.HasOwner,
					IsOutlineVisible = linksThatCanBePlaced.Contains(item),
					IsInteractive = linksThatCanBePlaced.Contains(item),
					CurrentEra = currentGameEra,
					TokenColor = (item.Owner?.Color ?? currentPlayer.Color)
				});
			}
			else if (dictionary.ContainsKey(key) && item is Rail)
			{
				dictionary[key].IsRailVisible = true;
			}
		}
		_router.GetView<IBoardView>().HighlightLinks(dictionary, linkSelectedDelegate);
		_router.GetView<IBoardView>().TurnOffRegionsActionOnClick();
	}

	public void ShowAnotherTrackSelectionPanel(List<CantUseLinkActionReason> reasons)
	{
		_router.ShowView<INetworkActionView>();
		INetworkActionView view = _router.GetView<INetworkActionView>();
		view.EnableBuildingAnotherLink(!reasons.Any());
		if (reasons.Any())
		{
			CantUseLinkActionReason cantUseLinkActionReason = reasons.First();
			string cantBuildMessage = "ERROR";
			switch (cantUseLinkActionReason)
			{
			case CantUseLinkActionReason.NO_PLACE_TO_BUILD_CONNECTION:
				cantBuildMessage = _localization.GetTranslation("ActionReasons/NO_PLACE_TO_BUILD_CONNECTION");
				break;
			case CantUseLinkActionReason.NO_MORE_CONNECTIONS_TO_BUILD:
				cantBuildMessage = _localization.GetTranslation("ActionReasons/NO_MORE_CONNECTIONS_TO_BUILD");
				break;
			case CantUseLinkActionReason.NO_MONEY:
				cantBuildMessage = _localization.GetTranslation("ActionReasons/NO_MONEY");
				break;
			case CantUseLinkActionReason.NO_COAL:
				cantBuildMessage = _localization.GetTranslation("ActionReasons/NO_COAL");
				break;
			case CantUseLinkActionReason.NO_BEER:
				cantBuildMessage = _localization.GetTranslation("ActionReasons/NO_BEER");
				break;
			case CantUseLinkActionReason.NO_MONEY_COAL:
				cantBuildMessage = _localization.GetTranslation("ActionReasons/NO_MONEY_COAL");
				break;
			}
			view.SetCantBuildMessage(cantBuildMessage);
		}
	}

	public void HideNumberOfTracksToBuildSelectionPanel()
	{
		_router.HideView<INetworkActionView>();
	}

	public void SetFastForwardButtonActive(bool isEnabled)
	{
		_router.GetView<IReplayView>().SetFastForwardActive(isEnabled);
		if (!isEnabled)
		{
			_router.GetView<IHandOfCardsView>().TurnOffInteractions();
		}
	}

	public void ShowPlayerInfoToSelectLinks()
	{
		_router.GetView<IActionInProgressView>().SetActionInfoText(_localization.GetTranslation("UI/SELECT_LINK"));
		_router.MoveOnTop<IActionInProgressView>();
	}

	public void ShowPlayerInfoToSelectCardToDiscard()
	{
		_router.GetView<IActionInProgressView>().SetActionInfoText(_localization.GetTranslation("UI/SELECT_CARD_TO_DISCARD"));
		_router.MoveOnTop<IActionInProgressView>();
	}

	public void ShowCardsToDiscard(ISelectCardDelegate selectCardDelegate)
	{
		_router.UseCaseBuilder.GetUseCase<HandOfCardsUseCase>().SetOnCardClickActionDelegate(selectCardDelegate);
		_router.GetView<IHandOfCardsView>().ShowCards();
	}

	public void HideCards()
	{
		_router.UseCaseBuilder.GetUseCase<HandOfCardsUseCase>().SetOnCardClickActionDelegate(null);
		_router.GetView<IHandOfCardsView>().HideCards();
	}

	public void HideCoalDelivery()
	{
		_router.UseCaseBuilder.GetUseCase<CoalDeliveryUseCase>().HideCoalDelivery();
	}

	public void HideBeerDelivery()
	{
		_router.UseCaseBuilder.GetUseCase<BeerDeliveryUseCase>().HideBeerDelivery();
	}

	public void AllowRegionInteractions(bool isRaycastTarget)
	{
		_router.GetView<IBoardView>().AllowRegionInteractions(isRaycastTarget);
	}

	public void ShowActionInProgressView(IActionInProgressCancelDelegate cancelActionDelegate, int currentActionNumber, int actionsMaxCount)
	{
		_router.HideView<IPlayerActionsView>();
		_router.UseCaseBuilder.GetUseCase<ActionInProgressUseCase>().SetCancelActionDelegate(cancelActionDelegate);
		_router.GetView<IActionInProgressView>().UpdateActionCounter(currentActionNumber, actionsMaxCount);
		_router.ShowView<IActionInProgressView>();
	}

	public void ShowPlayerInfoToSelectIfAnotherTrackShouldBePlaced()
	{
		_router.GetView<IActionInProgressView>().SetActionInfoText(_localization.GetTranslation("UI/SELECT_NUMBER_OF_TRACKS"));
		_router.MoveOnTop<IActionInProgressView>();
	}

	public void StartBeerDeliveryUseCase(int beerToDeliver, IEnumerable<Brewery> possibleBrewerySources, IBeerDeliveryResultDelegate beerDeliveryResultDelegate)
	{
		_router.UseCaseBuilder.GetUseCase<BeerDeliveryUseCase>().StartBeerDelivery(beerToDeliver, possibleBrewerySources, beerDeliveryResultDelegate);
	}

	public void StartCoalDeliveryUseCase(int coalToDeliver, IEnumerable<CoalMine> possibleCoalMinesSources, ICoalDeliveryResultDelegate coalDeliveryResultDelegate)
	{
		_router.UseCaseBuilder.GetUseCase<CoalDeliveryUseCase>().StartCoalDelivery(coalToDeliver, possibleCoalMinesSources, coalDeliveryResultDelegate);
	}

	public void ReplayEvents(ReadOnlyCollection<IEvent> events)
	{
	}
}
