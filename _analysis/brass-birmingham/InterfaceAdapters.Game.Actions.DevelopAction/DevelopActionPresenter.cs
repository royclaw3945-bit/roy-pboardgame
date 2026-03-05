using System.Collections.ObjectModel;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.EventSourcing.Events;
using BoardGameRules.Entities.Players;
using BrassApplication.Game;
using BrassApplication.UseCases.Game.Actions;
using BrassApplication.UseCases.Game.Actions.ActionInProgress;
using BrassApplication.UseCases.Game.Actions.BaseAction;
using BrassApplication.UseCases.Game.Actions.DevelopAction;
using BrassApplication.UseCases.Game.Actions.IronDelivery;
using BrassApplication.UseCases.Game.HandOfCards;
using BrassApplication.UseCases.Game.IndustriesDetails;
using InterfaceAdapters.Game.ActionInProgressView;
using InterfaceAdapters.Game.Actions.BaseAction;
using InterfaceAdapters.Game.HandOfCards;
using InterfaceAdapters.Game.IndustriesDetails;
using InterfaceAdapters.Game.PlayerActions;
using InterfaceAdapters.Game.Replay;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Utils;

namespace InterfaceAdapters.Game.Actions.DevelopAction;

public class DevelopActionPresenter : BaseActionPresenter, IDevelopActionOutput, IBaseActionOutput
{
	public DevelopActionPresenter(IRouter router, ILocalization localization)
		: base(router, localization)
	{
	}

	public void ShowPlayerInfoToSelectCardToDiscard(IActionInProgressCancelDelegate cancelActionDelegate, IActionInProgressFinishDelegate finishDelegate, int currentActionNumber, int actionsMaxCount)
	{
		_router.HideView<IPlayerActionsView>();
		_router.GetView<IActionInProgressView>().SetActionInfoText(_localization.GetTranslation("UI/SELECT_CARD_TO_DISCARD"));
		_router.GetView<IActionInProgressView>().UpdateActionCounter(currentActionNumber, actionsMaxCount);
		_router.ShowView<IActionInProgressView>();
		ActionInProgressUseCase useCase = _router.UseCaseBuilder.GetUseCase<ActionInProgressUseCase>();
		useCase.SetCancelActionDelegate(cancelActionDelegate);
		useCase.SetFinishActionDelegate(finishDelegate);
	}

	public void ShowPlayerInfoToSelectIndustry()
	{
		_router.GetView<IActionInProgressView>().SetActionInfoText(_localization.GetTranslation("UI/SELECT_INDUSTRY"));
	}

	public void ShowCardsToDiscard(ISelectCardDelegate selectCardDelegate)
	{
		_router.UseCaseBuilder.GetUseCase<HandOfCardsUseCase>().SetOnCardClickActionDelegate(selectCardDelegate);
		_router.GetView<IHandOfCardsView>().ShowCards();
	}

	public void HideCards()
	{
		_router.GetView<IHandOfCardsView>().HideCards();
	}

	public void ShowIndustriesDetailsView(BrassApplicationGame game, ISelectIndustryOfTypeDelegate selectIndustryOfActionDelegate)
	{
		_router.UseCaseBuilder.GetUseCase<IndustriesDetailsUseCase>().SetIndustryActionDelegate(selectIndustryOfActionDelegate);
		UpdateIndustriesDetailsView(game);
		_router.ShowView<IIndustriesDetailsView>();
		_router.GetView<IActionInProgressView>().ShowFinishButton();
		_router.MoveOnTop<IActionInProgressView>();
	}

	public virtual void UpdateIndustriesDetailsView(BrassApplicationGame game)
	{
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
		_router.UseCaseBuilder.GetUseCase<IndustriesDetailsUseCase>().LoadIndustries();
		_router.GetView<IIndustriesDetailsView>().HighlightIndustriesToDevelop(industriesDetailsViewModel);
	}

	public void HideIndustriesDetailsView()
	{
		_router.UseCaseBuilder.GetUseCase<IndustriesDetailsUseCase>().SetIndustryActionDelegate(null);
		_router.HideView<IIndustriesDetailsView>();
	}

	public void HideIronMarketHighlight()
	{
		_router.UseCaseBuilder.GetUseCase<IronDeliveryUseCase>().HideIronDelivery();
	}

	public void DeliverIronAndHide()
	{
		_router.UseCaseBuilder.GetUseCase<IronDeliveryUseCase>().DeliverIronAndHide();
	}

	public void StartIronDeliveryUseCase(int ironToDeliver, IIronDeliveryResultDelegate ironDeliveryResultDelegate)
	{
		_router.UseCaseBuilder.GetUseCase<IronDeliveryUseCase>().StartIronDelivery(ironToDeliver, ironDeliveryResultDelegate);
	}

	public void ReplayEvents(ReadOnlyCollection<IEvent> events)
	{
	}

	public void ActivateFinishButton()
	{
		_router.GetView<IActionInProgressView>().ActivateFinishButton();
		_router.HideView<IReplayView>();
	}

	public void HideFinishButton()
	{
		_router.GetView<IActionInProgressView>().HideFinishButton();
	}

	public void HideNotEnoughResourcesForSecondDevelopPopup()
	{
		_router.HideView<IDevelopActionView>();
	}

	public void ShowNotEnoughResourcesForSecondDevelopPopup()
	{
		_router.ShowView<IDevelopActionView>();
	}
}
