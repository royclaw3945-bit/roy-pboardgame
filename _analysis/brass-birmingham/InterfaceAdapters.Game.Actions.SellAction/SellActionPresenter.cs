using System.Collections.Generic;
using BoardGameRules.Entities.Board;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Common;
using BoardGameRules.Entities.Players;
using BrassApplication.Game;
using BrassApplication.UseCases.Game.Actions;
using BrassApplication.UseCases.Game.Actions.ActionInProgress;
using BrassApplication.UseCases.Game.Actions.BaseAction;
using BrassApplication.UseCases.Game.Actions.BeerDelivery;
using BrassApplication.UseCases.Game.Actions.SellAction;
using BrassApplication.UseCases.Game.HandOfCards;
using BrassApplication.UseCases.Game.IndustriesDetails;
using InterfaceAdapters.Game.ActionInProgressView;
using InterfaceAdapters.Game.Actions.BaseAction;
using InterfaceAdapters.Game.Board;
using InterfaceAdapters.Game.HandOfCards;
using InterfaceAdapters.Game.IndustriesDetails;
using InterfaceAdapters.Game.PlayerActions;
using InterfaceAdapters.Game.Replay;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Utils;

namespace InterfaceAdapters.Game.Actions.SellAction;

public class SellActionPresenter : BaseActionPresenter, ISellActionOutput, IBaseActionOutput
{
	public SellActionPresenter(IRouter router, ILocalization localization)
		: base(router, localization)
	{
	}

	public void ShowPlayerInfoToSelectCardToDiscard(IActionInProgressCancelDelegate cancelActionDelegate, int currentActionNumber, int actionsMaxCount)
	{
		_router.HideView<IPlayerActionsView>();
		_router.GetView<IActionInProgressView>().SetActionInfoText(_localization.GetTranslation("UI/SELECT_CARD_TO_DISCARD"));
		_router.GetView<IActionInProgressView>().UpdateActionCounter(currentActionNumber, actionsMaxCount);
		_router.ShowView<IActionInProgressView>();
		_router.UseCaseBuilder.GetUseCase<ActionInProgressUseCase>().SetCancelActionDelegate(cancelActionDelegate);
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

	public void HighlightValidBuildingSlots(BoardGameRules.Entities.Board.Board board, List<BuildingSlot> buildingSlotsWithIndustriesToSell, ISelectIndustryDelegate selectIndustryDelegate)
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
					Highlighted = buildingSlotsWithIndustriesToSell.Contains(buildingSlot),
					IsInteractive = buildingSlotsWithIndustriesToSell.Contains(buildingSlot),
					ActionOnClick = selectIndustryDelegate.SelectIndustrySlot,
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

	public void ShowPlayerInfoToSelectIndustry(IActionInProgressCancelDelegate cancelActionDelegate)
	{
		_router.GetView<IActionInProgressView>().SetActionInfoText(_localization.GetTranslation("UI/SELECT_INDUSTRY"));
		_router.MoveOnTop<IActionInProgressView>();
		_router.UseCaseBuilder.GetUseCase<ActionInProgressUseCase>().SetCancelActionDelegate(cancelActionDelegate);
	}

	public void ShowNextSellChoicePanel(List<BuildingSlot> buildingSlotsWithIndustriesToSell)
	{
		List<string> list = new List<string>();
		foreach (BuildingSlot item in buildingSlotsWithIndustriesToSell)
		{
			list.Add(item.GetCurrentImageName());
		}
		_router.ShowView<ISellActionView>();
		_router.GetView<ISellActionView>().SetSellActionView(list);
	}

	public virtual void HideNextSellChoicePanel()
	{
		_router.HideView<ISellActionView>();
	}

	public void StartBeerDeliveryUseCase(int beerToDeliver, IEnumerable<IBeerSource> possibleBeerSources, IBeerDeliveryResultDelegate beerDeliveryResultDelegate)
	{
		_router.UseCaseBuilder.GetUseCase<BeerDeliveryUseCase>().StartBeerDelivery(beerToDeliver, possibleBeerSources, beerDeliveryResultDelegate);
	}

	public void ShowMerchantDevelopBonus(BrassApplicationGame game, ISelectIndustryOfTypeDelegate selectIndustryOfActionDelegate)
	{
		_router.UseCaseBuilder.GetUseCase<IndustriesDetailsUseCase>().SetIndustryActionDelegate(selectIndustryOfActionDelegate);
		Player currentPlayer = game.Game.GameProgressInformation.CurrentPlayer;
		PlayerMat mat = game.Game.PlayersTrack.Players.Find((Player p) => p.Color == currentPlayer.Color).Mat;
		IndustriesDetailsViewModel industriesDetailsViewModel = new IndustriesDetailsViewModel
		{
			CoalMineHighestLevelIndex = (mat.CanIndustryBeDeveloped(BuildingType.CoalMine) ? (mat.GetFirstBuildingOfType(BuildingType.CoalMine).Level - 1) : 10),
			BreweryHighestLevelIndex = (mat.CanIndustryBeDeveloped(BuildingType.Brewery) ? (mat.GetFirstBuildingOfType(BuildingType.Brewery).Level - 1) : 10),
			IronWorksHighestLevelIndex = (mat.CanIndustryBeDeveloped(BuildingType.IronWorks) ? (mat.GetFirstBuildingOfType(BuildingType.IronWorks).Level - 1) : 10),
			ManufactureHighestLevelIndex = (mat.CanIndustryBeDeveloped(BuildingType.Manufacturer) ? (mat.GetFirstBuildingOfType(BuildingType.Manufacturer).Level - 1) : 10),
			CottonMillHighestLevelIndex = (mat.CanIndustryBeDeveloped(BuildingType.CottonMill) ? (mat.GetFirstBuildingOfType(BuildingType.CottonMill).Level - 1) : 10),
			PotteryHighestLevelIndex = (mat.CanIndustryBeDeveloped(BuildingType.Pottery) ? (mat.GetFirstBuildingOfType(BuildingType.Pottery).Level - 1) : 10)
		};
		_router.GetView<IIndustriesDetailsView>().HighlightIndustriesToDevelop(industriesDetailsViewModel);
		_router.GetView<IIndustriesDetailsView>().SetAsMerchantBonus();
		_router.ShowView<IIndustriesDetailsView>();
	}

	public void HideIndustriesDetailsView()
	{
		_router.UseCaseBuilder.GetUseCase<IndustriesDetailsUseCase>().SetIndustryActionDelegate(null);
		_router.HideView<IIndustriesDetailsView>();
	}

	public void SetFastForwardButtonActive(bool isEnabled)
	{
		_router.GetView<IReplayView>().SetFastForwardActive(isEnabled);
		if (!isEnabled)
		{
			_router.GetView<IHandOfCardsView>().TurnOffInteractions();
		}
	}
}
