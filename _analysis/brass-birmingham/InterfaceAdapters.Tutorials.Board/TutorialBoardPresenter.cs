using System;
using System.Collections.Generic;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.GamePhases;
using BrassApplication.Game;
using BrassApplication.UseCases.Common.LoadScene;
using BrassApplication.UseCases.Game.Board;
using BrassApplication.UseCases.Tutorials.Board;
using InterfaceAdapters.Game.Board;
using InterfaceAdapters.Game.IndustriesSummary;
using InterfaceAdapters.Game.RegionDetails;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Utils;

namespace InterfaceAdapters.Tutorials.Board;

public class TutorialBoardPresenter : BoardPresenter, ITutorialBoardOutput, IBoardOutput
{
	private List<BuildingType> _allowedIndustries = new List<BuildingType>();

	public TutorialBoardPresenter(IRouter router, ILocalization localization)
		: base(router, localization)
	{
	}

	public override void UpdateBoardView(BoardGameRules.Entities.Board.Board board, GameEra currentEra, Action<RegionName> actionOnClick)
	{
		BoardViewModel boardViewModel = PrepareBoardViewModel(board, currentEra, actionOnClick);
		foreach (KeyValuePair<RegionName, RegionViewModel> regionsViewModel in boardViewModel.RegionsViewModels)
		{
			regionsViewModel.Value.RegionsInteractive = false;
		}
		_router.GetView<IBoardView>().UpdateBoardView(boardViewModel);
		_router.UseCaseBuilder.GetUseCase<LoadSceneUseCase>().SetIsBoardUpdated(isBoardUpdated: true, isTutorial: true);
	}

	public void HighlightOnlySpecificSlot(BoardGameRules.Entities.Board.Board board, GameEra currentEra, Region specificRegion, Action<RegionName> actionOnClick, string slotUniqueId)
	{
		BoardViewModel boardViewModel = PrepareBoardViewModel(board, currentEra, actionOnClick);
		foreach (KeyValuePair<RegionName, RegionViewModel> regionsViewModel in boardViewModel.RegionsViewModels)
		{
			regionsViewModel.Value.RegionsInteractive = false;
			regionsViewModel.Value.ActionOnClick = delegate
			{
			};
		}
		List<BuildingSlotViewModel> list = new List<BuildingSlotViewModel>();
		foreach (BuildingSlot buildingSlot in specificRegion.BuildingSlots)
		{
			BuildingSlotViewModel item = new BuildingSlotViewModel
			{
				BuildingSlot = buildingSlot,
				ImageName = buildingSlot.GetCurrentImageName(),
				BackgroundImageName = buildingSlot.GetBackgroundImageName(),
				Build = (buildingSlot.BuildedIndustry != null),
				Highlighted = (buildingSlot.UniqueId == slotUniqueId),
				ResourceCount = (buildingSlot.BuildedIndustry?.GetResource() ?? 0),
				IsInteractive = false
			};
			list.Add(item);
		}
		RegionViewModel regionViewModel = new RegionViewModel
		{
			Name = specificRegion.Name,
			Color = specificRegion.Color,
			RegionsInteractive = true,
			BuildingSlotsViewModels = list,
			ActionOnClick = actionOnClick,
			FriendlyName = _localization.GetTranslation("Regions/" + specificRegion.Name)
		};
		boardViewModel.RegionsViewModels[regionViewModel.Name] = regionViewModel;
		_router.GetView<ITutorialBoardView>().HighlightBuildingSlots(boardViewModel);
	}

	public void SetAllowedIndustries(List<BuildingType> allowedIndustries)
	{
		_allowedIndustries = allowedIndustries;
	}

	public override void ShowRegionDetailsView(RegionName regionName, BrassApplicationGame game, IDictionary<Tuple<BuildingSlot, BuildingType>, IList<CantUseBuildActionReason>> cantBuildReasons)
	{
		_router.ShowView<IRegionDetailsView>();
		RegionDetailsViewModel regionDetailsViewModel = PrepareRegionDetailsViewModel(regionName, game, cantBuildReasons);
		foreach (List<IndustryViewModel> slot in regionDetailsViewModel.Slots)
		{
			foreach (IndustryViewModel item in slot)
			{
				item.Active = item.CantBuildReason == "";
				if (item.Active)
				{
					item.Active = _allowedIndustries.Contains(item.BuildingType);
					if (!item.Active)
					{
						item.CantBuildReason = _localization.GetTranslation("ActionReasons/LOCKED_FOR_TUTORIAL");
					}
				}
			}
		}
		_router.GetView<IRegionDetailsView>().UpdateRegionDetailsView(regionDetailsViewModel);
	}
}
