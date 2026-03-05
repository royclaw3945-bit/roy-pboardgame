using System.Collections.Generic;
using System.Collections.ObjectModel;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Entities.Markets;
using BrassApplication.UseCases.Game.Actions;
using BrassApplication.UseCases.Game.Actions.CoalDelivery;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.ActionInProgressView;
using InterfaceAdapters.Game.Board;
using InterfaceAdapters.Game.GameProgressSummary;
using InterfaceAdapters.Game.GameScene;
using InterfaceAdapters.Game.HandOfCards;
using InterfaceAdapters.Game.IndustriesSummary;
using InterfaceAdapters.Game.Market;
using InterfaceAdapters.Game.PlayerActions;
using InterfaceAdapters.Game.PlayersSummary;
using InterfaceAdapters.Game.Replay;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Utils;

namespace InterfaceAdapters.Game.Actions.CoalDelivery;

public class CoalDeliveryPresenter : IPresenter, ICoalDeliveryOutput
{
	private readonly IRouter _router;

	protected ILocalization _localization;

	public CoalDeliveryPresenter(IRouter router, ILocalization localization)
	{
		_router = router;
		_localization = localization;
	}

	public void HighlightCoalMines(BoardGameRules.Entities.Board.Board board, List<BuildingSlot> buildingSlotsToHighlight, ISelectIndustryDelegate selectIndustrySlotDelegate)
	{
		BoardViewModel boardViewModel = new BoardViewModel
		{
			RegionsViewModels = new Dictionary<RegionName, RegionViewModel>(),
			Links = new Dictionary<string, LinkViewModel>()
		};
		foreach (Region region in board.Regions)
		{
			List<BuildingSlotViewModel> list = new List<BuildingSlotViewModel>();
			foreach (BuildingSlot buildingSlot in region.BuildingSlots)
			{
				BuildingSlotViewModel item = new BuildingSlotViewModel
				{
					BuildingSlot = buildingSlot,
					ImageName = buildingSlot.GetCurrentImageName(),
					BackgroundImageName = buildingSlot.GetBackgroundImageName(),
					Build = (buildingSlot.BuildedIndustry != null),
					Highlighted = buildingSlotsToHighlight.Contains(buildingSlot),
					HighlightResource = buildingSlotsToHighlight.Contains(buildingSlot),
					IsInteractive = buildingSlotsToHighlight.Contains(buildingSlot),
					ActionOnClick = selectIndustrySlotDelegate.SelectIndustrySlot,
					ResourceCount = (buildingSlot.BuildedIndustry?.GetResource() ?? 0)
				};
				list.Add(item);
			}
			RegionViewModel value = new RegionViewModel
			{
				Name = region.Name,
				Color = region.Color,
				RegionsInteractive = false,
				BuildingSlotsViewModels = list,
				FriendlyName = _localization.GetTranslation("Regions/" + region.Name)
			};
			boardViewModel.RegionsViewModels.Add(region.Name, value);
		}
		_router.GetView<IBoardView>().HighlightBuildingSlots(boardViewModel);
	}

	public void ShowPlayerInfoToSelectCoalMarket()
	{
		_router.GetView<IActionInProgressView>().SetActionInfoText(_localization.GetTranslation("UI/SELECT_COAL_MARKET"));
	}

	public void ShowPlayerInfoToSelectCoalMine()
	{
		_router.GetView<IActionInProgressView>().SetActionInfoText(_localization.GetTranslation("UI/SELECT_COAL_MINE"));
	}

	public void HideMarketHighlights()
	{
		_router.GetView<IMarketView>().StopCoalHighlight();
		_router.HideView<IFadeView>();
		_router.MoveOnTop<IHandOfCardsView>();
		_router.MoveOnTop<IMarketView>();
		_router.MoveOnTop<IPlayersSummaryView>();
		_router.MoveOnTop<IIndustriesSummaryView>();
		_router.MoveOnTop<IGameSceneView>();
		_router.MoveOnTop<IReplayView>();
		_router.MoveOnTop<IPlayerActionsView>();
	}

	public void TakeMarketResource()
	{
		_router.GetView<IMarketView>().TakeResource();
		_router.HideView<IFadeView>();
		_router.MoveOnTop<IHandOfCardsView>();
		_router.MoveOnTop<IMarketView>();
		_router.MoveOnTop<IPlayersSummaryView>();
		_router.MoveOnTop<IIndustriesSummaryView>();
		_router.MoveOnTop<IGameSceneView>();
		_router.MoveOnTop<IReplayView>();
		_router.MoveOnTop<IPlayerActionsView>();
	}

	public void HighlightMarket(CoalMarket coalMarket, ISelectMarketDelegate selectMarketDelegate, int coalTDeliver)
	{
		_router.ShowView<IFadeView>();
		_router.GetView<IFadeView>().FadeIn();
		_router.MoveOnTop<IGameProgressSummaryView>();
		_router.MoveOnTop<IActionInProgressView>();
		_router.MoveOnTop<IMarketView>();
		_router.GetView<IMarketView>().SetUpCoalMarketOnClickAction(coalTDeliver, coalMarket.AvailableCoal, delegate
		{
			selectMarketDelegate.SelectMarket(coalMarket);
		});
	}

	public void ReplayEvents(ReadOnlyCollection<IEvent> events)
	{
	}
}
