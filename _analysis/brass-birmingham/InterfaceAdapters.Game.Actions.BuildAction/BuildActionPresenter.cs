using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Actions;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.Industries;
using BoardGameRules.Entities.Players;
using BrassApplication.Game;
using BrassApplication.UseCases.Game.Actions;
using BrassApplication.UseCases.Game.Actions.ActionInProgress;
using BrassApplication.UseCases.Game.Actions.BaseAction;
using BrassApplication.UseCases.Game.Actions.BuildAction;
using BrassApplication.UseCases.Game.Actions.CoalDelivery;
using BrassApplication.UseCases.Game.Actions.IronDelivery;
using BrassApplication.UseCases.Game.HandOfCards;
using BrassApplication.UseCases.Game.RegionDetails;
using InterfaceAdapters.Game.ActionInProgressView;
using InterfaceAdapters.Game.Actions.BaseAction;
using InterfaceAdapters.Game.Board;
using InterfaceAdapters.Game.HandOfCards;
using InterfaceAdapters.Game.IndustriesSummary;
using InterfaceAdapters.Game.PlayerActions;
using InterfaceAdapters.Game.RegionDetails;
using InterfaceAdapters.Game.Replay;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Utils;

namespace InterfaceAdapters.Game.Actions.BuildAction;

public class BuildActionPresenter : BaseActionPresenter, IBuildActionOutput, IBaseActionOutput
{
	public BuildActionPresenter(IRouter router, ILocalization localization)
		: base(router, localization)
	{
	}

	public void HighlightValidBuildingSlots(BoardGameRules.Entities.Board.Board board, List<BuildingSlot> buildingSlotsWhereBuildIsPossible, ISelectBuildingDelegate selectBuildingDelegate, Action<RegionName> actionOnClick)
	{
		BoardViewModel boardViewModel = PrepareBoardViewModel(board, buildingSlotsWhereBuildIsPossible, actionOnClick);
		_router.GetView<IBoardView>().HighlightBuildingSlots(boardViewModel);
		_router.UseCaseBuilder.GetUseCase<RegionDetailsUseCase>().SetBuildingSelectedDelegate(selectBuildingDelegate);
	}

	public void HighlightValidBuildingSlotsInRegion(BoardGameRules.Entities.Board.Board board, List<BuildingSlot> buildingSlotsWhereBuildIsPossible, ISelectBuildingDelegate selectBuildingDelegate, Action<RegionName> actionOnClick, RegionName regionName)
	{
		BoardViewModel boardViewModel = PrepareBoardViewModel(board, buildingSlotsWhereBuildIsPossible, actionOnClick);
		foreach (KeyValuePair<RegionName, RegionViewModel> regionsViewModel in boardViewModel.RegionsViewModels)
		{
			if (regionsViewModel.Key == regionName)
			{
				continue;
			}
			foreach (BuildingSlotViewModel buildingSlotsViewModel in regionsViewModel.Value.BuildingSlotsViewModels)
			{
				buildingSlotsViewModel.Highlighted = false;
			}
		}
		_router.GetView<IBoardView>().HighlightBuildingSlots(boardViewModel);
		_router.UseCaseBuilder.GetUseCase<RegionDetailsUseCase>().SetBuildingSelectedDelegate(selectBuildingDelegate);
	}

	public void TurnOffSlotsHighlightning(BoardGameRules.Entities.Board.Board board)
	{
		BoardViewModel boardViewModel = PrepareBoardViewModel(board, new List<BuildingSlot>(), delegate
		{
		});
		_router.GetView<IBoardView>().HighlightBuildingSlots(boardViewModel);
	}

	public void ShowPlayerInfoToSelectBuildingSlot(IActionInProgressCancelDelegate cancelActionDelegate, int currentActionNumber, int actionsMaxCount)
	{
		_router.HideView<IPlayerActionsView>();
		_router.GetView<IActionInProgressView>().SetActionInfoText(_localization.GetTranslation("UI/SELECT_BUILDING_SLOT"));
		_router.GetView<IActionInProgressView>().UpdateActionCounter(currentActionNumber, actionsMaxCount);
		_router.ShowView<IActionInProgressView>();
		_router.UseCaseBuilder.GetUseCase<ActionInProgressUseCase>().SetCancelActionDelegate(cancelActionDelegate);
	}

	public void ShowRegionDetailsView(RegionName regionName, BrassApplicationGame game, IDictionary<Tuple<BuildingSlot, BuildingType>, IList<CantUseBuildActionReason>> cantBuildReasons)
	{
		_router.ShowView<IRegionDetailsView>();
		Region regionByName = game.Game.Board.GetRegionByName(regionName);
		RegionDetailsViewModel regionDetailsViewModel = new RegionDetailsViewModel
		{
			Region = _router.GetView<IBoardView>().GetRegionViewForGivenRegionName(regionName),
			Slots = new List<List<IndustryViewModel>>(),
			ShowBackButton = false
		};
		List<BuildingSlot> buildingSlots = regionByName.BuildingSlots;
		Player currentPlayer = game.Game.GameProgressInformation.CurrentPlayer;
		PlayerMat mat = game.Game.PlayersTrack.Players.Find((Player p) => p.Color == currentPlayer.Color).Mat;
		foreach (BuildingSlot item in buildingSlots)
		{
			List<IndustryViewModel> list = new List<IndustryViewModel>();
			foreach (BuildingType buildingType in item.BuildingTypes)
			{
				BaseIndustry firstBuildingOfType = mat.GetFirstBuildingOfType(buildingType);
				IList<CantUseBuildActionReason> source = cantBuildReasons[new Tuple<BuildingSlot, BuildingType>(item, buildingType)];
				list.Add(new IndustryViewModel
				{
					BuildingType = buildingType,
					BuildingSlot = item,
					Active = !source.Any(),
					IndustryAvailable = (firstBuildingOfType != null),
					CantBuildReason = _localization.GetTranslation("ActionReasons/" + (source.Any() ? source.OrderBy((CantUseBuildActionReason r) => (int)r).First() : CantUseBuildActionReason.NO_REASON)),
					CanBeDeveloped = (firstBuildingOfType?.CanBeDeveloped ?? false),
					OnlyCanalEra = (firstBuildingOfType?.IsForCanalEra ?? false),
					OnlyRailEra = (firstBuildingOfType?.IsForRailEra ?? false),
					Level = (firstBuildingOfType?.Level ?? 0),
					MoneyCost = (firstBuildingOfType?.MoneyCost ?? 0),
					VPForBuilding = (firstBuildingOfType?.VPForBuilding ?? 0),
					IncomeMarkerIncrease = (firstBuildingOfType?.IncomeIncrease ?? 0),
					VpPerLink = (firstBuildingOfType?.VPPerLink ?? 0),
					CoalCost = (firstBuildingOfType?.CoalCost ?? 0),
					IronCost = (firstBuildingOfType?.IronCost ?? 0),
					BeerCost = ((firstBuildingOfType is BaseSellableIndustry baseSellableIndustry) ? baseSellableIndustry.CostOfBeerBarrelsToSellIndustry : 0),
					BeerSource = ((firstBuildingOfType is Brewery brewery) ? brewery._numberOfBeersFunc() : 0),
					CoalSource = ((firstBuildingOfType is CoalMine coalMine) ? coalMine.AvailableCoal : 0),
					IronSource = ((firstBuildingOfType is IronWorks ironWorks) ? ironWorks.AvailableIron : 0),
					Color = (firstBuildingOfType?.Color ?? PlayerColor.Red),
					Name = _localization.GetTranslation("UI/" + firstBuildingOfType?.BuildingType)
				});
			}
			regionDetailsViewModel.Slots.Add(list);
		}
		_router.GetView<IRegionDetailsView>().UpdateRegionDetailsView(regionDetailsViewModel);
		_router.MoveOnTop<IActionInProgressView>();
	}

	public void HideRegionDetailsView()
	{
		_router.HideView<IRegionDetailsView>();
	}

	public new void HideIronDelivery()
	{
		_router.UseCaseBuilder.GetUseCase<IronDeliveryUseCase>().HideIronDelivery();
	}

	public void HideCoalDelivery()
	{
		_router.UseCaseBuilder.GetUseCase<CoalDeliveryUseCase>().HideCoalDelivery();
	}

	public void ShowValidCardsToBuild(List<BaseCard> validCardList, ISelectCardDelegate selectCardDelegate)
	{
		_router.UseCaseBuilder.GetUseCase<HandOfCardsUseCase>().SetOnCardClickActionDelegate(selectCardDelegate);
		_router.GetView<IHandOfCardsView>().HighlightCards(validCardList);
	}

	public void HideCards()
	{
		_router.UseCaseBuilder.GetUseCase<HandOfCardsUseCase>().SetOnCardClickActionDelegate(null);
		_router.GetView<IHandOfCardsView>().HideCards();
	}

	public void SetFastForwardButtonActive(bool isEnabled)
	{
		_router.GetView<IReplayView>().SetFastForwardActive(isEnabled);
		if (!isEnabled)
		{
			_router.GetView<IHandOfCardsView>().TurnOffInteractions();
		}
	}

	public void ShowPlayerInfoToSelectIndustry()
	{
		_router.GetView<IActionInProgressView>().SetActionInfoText(_localization.GetTranslation("UI/SELECT_INDUSTRY"));
	}

	public void ShowPlayerInfoToSelectCardToDiscard()
	{
		_router.GetView<IActionInProgressView>().SetActionInfoText(_localization.GetTranslation("UI/SELECT_CARD_TO_DISCARD"));
	}

	public void StartIronDeliveryUseCase(int ironToDeliver, IIronDeliveryResultDelegate ironDeliveryResultDelegate)
	{
		_router.UseCaseBuilder.GetUseCase<IronDeliveryUseCase>().StartIronDelivery(ironToDeliver, ironDeliveryResultDelegate);
	}

	public void StartCoalDeliveryUseCase(int coalToDeliver, IEnumerable<CoalMine> possibleCoalMinesSources, ICoalDeliveryResultDelegate coalDeliveryResultDelegate)
	{
		_router.UseCaseBuilder.GetUseCase<CoalDeliveryUseCase>().StartCoalDelivery(coalToDeliver, possibleCoalMinesSources, coalDeliveryResultDelegate);
	}

	private BoardViewModel PrepareBoardViewModel(BoardGameRules.Entities.Board.Board board, List<BuildingSlot> buildingSlotsWhereBuildIsPossible, Action<RegionName> actionOnClick)
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
					Highlighted = buildingSlotsWhereBuildIsPossible.Contains(buildingSlot),
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
				FriendlyName = _localization.GetTranslation("Regions/" + region.Name)
			};
			boardViewModel.RegionsViewModels.Add(region.Name, value);
		}
		return boardViewModel;
	}
}
