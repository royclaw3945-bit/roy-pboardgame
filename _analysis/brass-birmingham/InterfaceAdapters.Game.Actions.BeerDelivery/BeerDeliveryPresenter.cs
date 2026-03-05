using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Merchants;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.EventSourcing.Events;
using BrassApplication.UseCases.Game.Actions;
using BrassApplication.UseCases.Game.Actions.BeerDelivery;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.ActionInProgressView;
using InterfaceAdapters.Game.Board;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Utils;

namespace InterfaceAdapters.Game.Actions.BeerDelivery;

public class BeerDeliveryPresenter : IPresenter, IBeerDeliveryOutput
{
	private readonly IRouter _router;

	protected ILocalization _localization;

	public BeerDeliveryPresenter(IRouter router, ILocalization localization)
	{
		_router = router;
		_localization = localization;
	}

	public void HighlightBreweries(BoardGameRules.Entities.Board.Board board, List<BuildingSlot> buildingSlotsToHighlight, ISelectIndustryDelegate selectIndustrySlotDelegate)
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
		_router.MoveOnTop<IActionInProgressView>();
		_router.GetView<IBoardView>().HighlightBuildingSlots(boardViewModel);
	}

	public void HighlightMerchantSlots(BoardGameRules.Entities.Board.Board board, IEnumerable<MerchantSlot> merchantSlotsToHighlight, ISelectMerchantSlotDelegate selectMerchantSlotDelegate)
	{
		Dictionary<RegionName, BorderRegionViewModel> dictionary = new Dictionary<RegionName, BorderRegionViewModel>();
		foreach (BorderRegion borderRegion in board.GetBorderRegions((BorderRegion r) => true))
		{
			List<MerchantSlotViewModel> list = new List<MerchantSlotViewModel>();
			foreach (MerchantSlot merchantSlot in borderRegion.MerchantSlots)
			{
				MerchantSlotViewModel item = new MerchantSlotViewModel
				{
					MerchantSlot = merchantSlot,
					MerchantTileImageName = ((merchantSlot.MerchantTile != null && merchantSlot.MerchantTile.BuildingTypes.Any()) ? ((merchantSlot.MerchantTile.BuildingTypes.Count > 1) ? "UndefinedType" : string.Concat(merchantSlot.MerchantTile.BuildingTypes[0], "Type")) : "Blank"),
					HasBeer = merchantSlot.HasBeer,
					Highlighted = merchantSlotsToHighlight.Contains(merchantSlot),
					HighlightedBeer = merchantSlotsToHighlight.Contains(merchantSlot),
					IsInteractive = merchantSlotsToHighlight.Contains(merchantSlot),
					ActionOnClick = selectMerchantSlotDelegate.SelectMerchantSlot
				};
				list.Add(item);
			}
			BorderRegionViewModel value = new BorderRegionViewModel
			{
				Name = borderRegion.Name,
				MerchantSlotsViewModels = list,
				BonusType = borderRegion.MerchantBeerBonusType
			};
			dictionary.Add(borderRegion.Name, value);
		}
		_router.GetView<IBoardView>().HighlightMerchantSlots(dictionary);
	}

	public void ShowPlayerInfoToSelectBrewery()
	{
		_router.GetView<IActionInProgressView>().SetActionInfoText(_localization.GetTranslation("UI/SELECT_BREWERY"));
	}

	public void ReplayEvents(ReadOnlyCollection<IEvent> events)
	{
	}
}
