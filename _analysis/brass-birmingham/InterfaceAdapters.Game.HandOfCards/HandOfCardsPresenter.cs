using System;
using System.Collections.Generic;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Cards;
using BrassApplication.UseCases.Game.Board;
using BrassApplication.UseCases.Game.HandOfCards;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.Board;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Utils;

namespace InterfaceAdapters.Game.HandOfCards;

public class HandOfCardsPresenter : IPresenter, IHandOfCardsOutput
{
	protected readonly IRouter _router;

	protected readonly ILocalization _localization;

	public HandOfCardsPresenter(IRouter router, ILocalization localization)
	{
		_router = router;
		_localization = localization;
	}

	public void OnLocationCardClicked(RegionName regionClicked, List<BuildingSlot> buildingSlotsWhereBuildIsPossible, BoardGameRules.Entities.Board.Board board)
	{
		_router.GetView<IBoardView>().ScrollToRegion(regionClicked);
		Action<RegionName> actionOnClick = _router.UseCaseBuilder.GetUseCase<BoardUseCase>().RegionSelected;
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
					Highlighted = (region.Name == regionClicked && buildingSlotsWhereBuildIsPossible.Contains(buildingSlot)),
					ResourceCount = (buildingSlot.BuildedIndustry?.GetResource() ?? 0),
					IsInteractive = false
				};
				list.Add(item);
			}
			RegionViewModel value = new RegionViewModel
			{
				Name = region.Name,
				Color = region.Color,
				RegionsInteractive = true,
				BuildingSlotsViewModels = list,
				ActionOnClick = actionOnClick,
				FriendlyName = GetRegionFriendlyName(region.Name.ToString())
			};
			boardViewModel.RegionsViewModels.Add(region.Name, value);
		}
		_router.GetView<IBoardView>().HighlightBuildingSlots(boardViewModel);
	}

	public void UpdateHandCardsView(List<BaseCard> cards, bool shouldHideCards)
	{
		_router.GetView<IHandOfCardsView>().UpdateHandCardsView(cards, shouldHideCards);
	}

	public string GetRegionFriendlyName(string regionName)
	{
		return _localization.GetTranslation("Regions/" + regionName);
	}
}
