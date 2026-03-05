using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Links;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.Common;
using BrassApplication.Game;
using BrassApplication.UseCases.Common.LoadScene;
using BrassApplication.UseCases.Game.GameScene;
using BrassApplication.UseCases.Game.HandOfCards;
using BrassApplication.UseCases.Game.PlayGame;
using BrassApplication.UseCases.Game.PlayerActions;
using BrassApplication.UseCases.Game.PlayersSummary;
using BrassApplication.UseCases.Tutorials.Actions.Develop;
using BrassApplication.UseCases.Tutorials.Actions.Network;
using BrassApplication.UseCases.Tutorials.Actions.SellAction;
using BrassApplication.UseCases.Tutorials.Board;
using BrassApplication.UseCases.Tutorials.HandOfCards;
using BrassApplication.UseCases.Tutorials.Replay;
using BrassApplication.UseCases.Tutorials.Tutorial;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.ActionInProgressView;
using InterfaceAdapters.Game.Actions.DevelopAction;
using InterfaceAdapters.Game.Actions.NetworkAction;
using InterfaceAdapters.Game.Actions.SellAction;
using InterfaceAdapters.Game.EndOfEra;
using InterfaceAdapters.Game.EndOfGame;
using InterfaceAdapters.Game.GameProgressDetails;
using InterfaceAdapters.Game.GameProgressSummary;
using InterfaceAdapters.Game.GameScene;
using InterfaceAdapters.Game.HandOfCards;
using InterfaceAdapters.Game.IndustriesDetails;
using InterfaceAdapters.Game.IndustriesSummary;
using InterfaceAdapters.Game.PlayerActions;
using InterfaceAdapters.Game.PlayersSummary;
using InterfaceAdapters.Game.Replay;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Tutorials.Actions.ActionInProgress;
using InterfaceAdapters.Tutorials.Board;
using InterfaceAdapters.Tutorials.HandOfCards;
using InterfaceAdapters.Tutorials.PlayerActions;
using InterfaceAdapters.Tutorials.PlayersSummary;
using InterfaceAdapters.Utils;

namespace InterfaceAdapters.Tutorials.Tutorial;

public class TutorialGameScenePresenter : GameScenePresenter, ITutorialGameSceneOutput, IGameSceneOutput
{
	private Action[] actionsToPerform;

	public TutorialGameScenePresenter(IRouter router, ILocalization localization)
		: base(router, localization)
	{
	}

	public void SetupTutorial(Action onClickAction)
	{
		List<BaseCard> cardsThatCanBeSelected = new List<BaseCard>();
		_router.UseCaseBuilder.GetUseCase<TutorialHandOfCardsUseCase>().SetCardsThatCanBeSelected(cardsThatCanBeSelected);
		_router.GetView<ITutorialPlayersSummaryView>();
		_router.GetView<ITutorialHandOfCardsView>();
		_router.GetView<ITutorialBoardView>();
		_router.GetView<ITutorialActionInProgressView>();
		_router.GetView<ITutorialGameSceneView>().SetOnClickAction(onClickAction);
		_router.ShowView<ITutorialPlayerActionsView>();
	}

	public void TurnOffLoadSceneView()
	{
		_router.UseCaseBuilder.GetUseCase<LoadSceneUseCase>().SetIsTutorialReady(isTutorialReady: true);
	}

	public void MoveGameSceneViewOnTop()
	{
		_router.MoveOnTop<ITutorialGameSceneView>();
	}

	private void RestoreUseCaseToStandard(Type useCaseViewInterfaceType)
	{
		_router.UseCaseBuilder.RestoreUseCaseOfType(useCaseViewInterfaceType);
	}

	public void TurnOffPlayerActionsViewInteractions()
	{
		_router.GetView<IPlayerActionsView>().SetInteractable(isInteractable: false);
	}

	public void TurnOnPlayerActionsViewInteractions()
	{
		_router.GetView<IPlayerActionsView>().SetInteractable(isInteractable: true);
	}

	public void TurnOnGameProgressSummaryViewInteractions()
	{
		_router.GetView<IGameProgressSummaryView>().SetInteractable(isInteractable: true);
	}

	public bool IsReplayInProgress()
	{
		return _router.UseCaseBuilder.GetUseCase<TutorialReplayUseCase>().GetIsReplayInProgress();
	}

	public void SetAutoSkipReplay(bool autoSkipReplay)
	{
		_router.UseCaseBuilder.GetUseCase<TutorialReplayUseCase>().AutoSkipReplay = autoSkipReplay;
	}

	public void SetManualReplay(bool manualReplay)
	{
		_router.UseCaseBuilder.GetUseCase<TutorialReplayUseCase>().ManualReplay = manualReplay;
	}

	public void LockSkipButton()
	{
		_router.GetView<IReplayView>().SetSkipButtonInteractable(interactable: false);
	}

	public void UnlockSkipButton()
	{
		_router.GetView<IReplayView>().SetSkipButtonInteractable(interactable: true);
	}

	public void ReplayEndTurnFirstEvents()
	{
		_router.UseCaseBuilder.GetUseCase<TutorialReplayUseCase>().ShowEventsBeforeTurnStarted();
	}

	public void ReplayNextAction()
	{
		_router.UseCaseBuilder.GetUseCase<TutorialReplayUseCase>().ReplayNextAction();
	}

	public void ReplayEndOfEraStart()
	{
		LockSkipButton();
		_router.GetView<IEndOfEraView>().SetSkipButtonInteractable(skipButtonInteractable: false);
		_router.UseCaseBuilder.GetUseCase<TutorialReplayUseCase>().ReplayEndOfEraStart();
	}

	public void ReplayEndOfEraScoringLinks()
	{
		_router.UseCaseBuilder.GetUseCase<TutorialReplayUseCase>().ReplayEndOfEraScoringLinks();
	}

	public void ReplayEndOfEraScoringIndustries()
	{
		_router.UseCaseBuilder.GetUseCase<TutorialReplayUseCase>().ReplayEndOfEraScoringIndustries();
	}

	public void ReplayEndOfEndOfEra()
	{
		_router.UseCaseBuilder.GetUseCase<TutorialReplayUseCase>().ReplayEndOfEndOfEra();
	}

	public void ReplayStartOfRailEra()
	{
		_router.UseCaseBuilder.GetUseCase<TutorialReplayUseCase>().ReplayStartOfRailEra();
	}

	public void ReplayManualEvent()
	{
		_router.UseCaseBuilder.GetUseCase<TutorialReplayUseCase>().ManualReplayOfPreparedEvent();
	}

	public void ReplayRemaining()
	{
		_router.UseCaseBuilder.GetUseCase<TutorialReplayUseCase>().ReplayRemaining();
	}

	public void UpdateGameHistoryAndFinishReplay()
	{
		_router.UseCaseBuilder.GetUseCase<TutorialReplayUseCase>().UpdateGameHistoryAndFinishReplay();
	}

	public new virtual void ShowGamePanel()
	{
		_router.ShowView<ITutorialGameSceneView>();
	}

	public async void ShowTutorialPlayersSummary()
	{
		_router.ShowView<ITutorialPlayersSummaryView>();
		ITutorialPlayersSummaryView playersSummaryView = _router.GetView<ITutorialPlayersSummaryView>();
		playersSummaryView.HideOtherPlayers();
		await Task.Delay(300);
		playersSummaryView.GrowYellowPlayerPortrait();
		_router.GetView<ITutorialGameSceneView>().MoveToTop();
	}

	public new void ShowBoard()
	{
		_router.ShowView<ITutorialBoardView>();
		_router.GetView<ITutorialBoardView>().AllowRegionInteractions(isRaycastTarget: false);
		_router.GetView<ITutorialBoardView>().EnableScrolling(isEnabled: false);
	}

	public new void ShowCards()
	{
		_router.ShowView<ITutorialHandOfCardsView>();
	}

	public new async void ShowGameProgressSummary()
	{
		_router.ShowView<IGameProgressSummaryView>();
		await Task.Delay(300);
		_router.GetView<IGameProgressSummaryView>().SetInteractable(isInteractable: false);
	}

	public void ShowGameProgressDetails()
	{
		_router.ShowView<IGameProgressDetailsView>();
	}

	public new async void ShowPlayerActions()
	{
		base.ShowPlayerActions();
		await Task.Delay(300);
		_router.GetView<IPlayerActionsView>().SetInteractable(isInteractable: false);
		_router.MoveOnTop<ITutorialGameSceneView>();
	}

	public new async void ShowIndustriesSummary()
	{
		base.ShowIndustriesSummary();
		await Task.Delay(300);
		_router.GetView<IIndustriesSummaryView>().SetInteractable(isInteractable: false);
		_router.MoveOnTop<ITutorialGameSceneView>();
	}

	public void ShowCardOverview()
	{
		_router.ShowView<ITutorialCardOverviewView>();
		_router.MoveOnTop<ITutorialGameSceneView>();
	}

	public void HidePlayerActions()
	{
		_router.HideView<IPlayerActionsView>();
	}

	public void HideGameProgressDetails()
	{
		_router.HideView<IGameProgressDetailsView>();
	}

	public void ShrinkTutorialPlayersSummaryToNormal()
	{
		_router.GetView<ITutorialPlayersSummaryView>().ShrinkYellowPlayerPortrait();
	}

	public void ShowAllPlayers()
	{
		_router.GetView<ITutorialPlayersSummaryView>().ShowAllPlayersAfterNextEndTurnAnimation();
	}

	public async void RestoreTutorialPlayersSummaryToStandard()
	{
		_router.GetView<ITutorialPlayersSummaryView>().ShowAllSetPlayers();
		_router.GetView<ITutorialPlayersSummaryView>().DestroyWhenClosed = true;
		_router.HideView<ITutorialPlayersSummaryView>();
		while (_router.IsViewCached<ITutorialPlayersSummaryView>())
		{
			await Task.Delay(1);
		}
		RestoreUseCaseToStandard(typeof(IPlayersSummaryView));
		_router.ShowView<IPlayersSummaryView>();
		_router.UseCaseBuilder.GetUseCase<PlayersSummaryUseCase>().UpdatePlayers();
	}

	public void SetCardsThatCanBeSelected(List<BaseCard> cards)
	{
		_router.UseCaseBuilder.GetUseCase<TutorialHandOfCardsUseCase>().SetCardsThatCanBeSelected(cards);
	}

	public void SlipCardsOut()
	{
		_router.GetView<ITutorialHandOfCardsView>().SlipCardsOut();
	}

	public void SlipCardsIn()
	{
		_router.GetView<ITutorialHandOfCardsView>().HideCards();
	}

	public void RestoreHandOfCardsViewToStandard(PlayerMetadata playerMetadata)
	{
		_router.HideView<ITutorialHandOfCardsView>().OnComplete(delegate
		{
			RestoreUseCaseToStandard(typeof(IHandOfCardsView));
			_router.ShowView<IHandOfCardsView>();
			_router.UseCaseBuilder.GetUseCase<HandOfCardsUseCase>().UpdateCards(playerMetadata);
		});
	}

	public void RestoreTutorialActionsUseCasesToStandard()
	{
		RestoreUseCaseToStandard(typeof(INetworkActionView));
		RestoreUseCaseToStandard(typeof(ISellActionView));
		RestoreUseCaseToStandard(typeof(IDevelopActionView));
	}

	public void HighlightOnlySpecificSlot(string buildingSlotId)
	{
		_router.UseCaseBuilder.GetUseCase<TutorialBoardUseCase>().HighlightOnlySpecificSlot(buildingSlotId);
	}

	public void SetAllowedIndustries(List<BuildingType> allowedIndustries)
	{
		_router.UseCaseBuilder.GetUseCase<TutorialBoardUseCase>().SetAllowedIndustries(allowedIndustries);
	}

	public void ScrollToTutorialStartingPoint()
	{
		_router.GetView<ITutorialBoardView>().ScrollToPosition(4.96f, -412f, 0.054f);
	}

	public void ScrollToWarrington()
	{
		_router.GetView<ITutorialBoardView>().ScrollToPosition(615f, -940f, 0.08f);
	}

	public void ScrollToDerby()
	{
		_router.GetView<ITutorialBoardView>().ScrollToPosition(-465f, -700f, 0.075f);
	}

	public void ShowWholeMap()
	{
		_router.GetView<ITutorialBoardView>().ScrollToPosition(0f, 0f, 0.028f);
	}

	public void AllowBoardScrolling()
	{
		_router.GetView<ITutorialBoardView>().EnableScrolling(isEnabled: true);
	}

	public void HideAllMerchantRegions()
	{
		_router.GetView<ITutorialBoardView>().HideAllMerchantRegions();
	}

	public void ShowRegion(RegionName regionName, float delay = 0f)
	{
		_router.GetView<ITutorialBoardView>().ShowRegion(regionName, delay);
	}

	public void AllowCancellingOfActions()
	{
		_router.GetView<ITutorialActionInProgressView>().TurnOnCancelButton();
	}

	public void RestoreAllPlayerActions()
	{
		ITutorialPlayerActionsView view = _router.GetView<ITutorialPlayerActionsView>();
		view.AddActionButtonsListeners();
		view.EnableUndoButton(enableUndo: true);
	}

	public void EnableEndTurnButton(bool enableEndTurn)
	{
		_router.GetView<ITutorialPlayerActionsView>().EnableEndTurnButton(enableEndTurn);
	}

	public void StartBuildAction()
	{
		_router.UseCaseBuilder.GetUseCase<PlayerActionsUseCase>().StartBuildAction();
	}

	public void SetPossibleBeerSourcesForSell(IEnumerable<IBeerSource> beerSources)
	{
		_router.UseCaseBuilder.GetUseCase<TutorialSellActionUseCase>().SetPossibleBeerSources(beerSources);
	}

	public void StartSellAction()
	{
		_router.UseCaseBuilder.GetUseCase<PlayerActionsUseCase>().StartSellAction();
	}

	public void InitiateNextSell()
	{
		_router.UseCaseBuilder.GetUseCase<TutorialSellActionUseCase>().InitiateNextSell();
	}

	public void StartNetworkAction()
	{
		_router.UseCaseBuilder.GetUseCase<PlayerActionsUseCase>().StartNetworkAction();
	}

	public void StartNetworkActionWithSpecifiedLinks(List<Link> linksToHighlight)
	{
		_router.UseCaseBuilder.GetUseCase<TutorialNetworkActionUseCase>().StartNetworkActionWithSpecifiedLinks(linksToHighlight);
	}

	public void PassAction(BaseCard card)
	{
		_router.UseCaseBuilder.GetUseCase<PlayerActionsUseCase>().StartPassAction();
		_router.UseCaseBuilder.GetUseCase<HandOfCardsUseCase>().OnCardSelected(card);
	}

	public void StartDevelopAction()
	{
		_router.UseCaseBuilder.GetUseCase<TutorialDevelopActionUseCase>().StartDevelopAction();
	}

	public void DevelopHighlightAll()
	{
		_router.UseCaseBuilder.GetUseCase<TutorialDevelopActionUseCase>().HighlightAllPosiible();
	}

	public void DevelopHighlightCottonMill()
	{
		_router.UseCaseBuilder.GetUseCase<TutorialDevelopActionUseCase>().HighlightCottonMillOnly();
	}

	public void DevelopHighlightNone()
	{
		_router.UseCaseBuilder.GetUseCase<TutorialDevelopActionUseCase>().HighlightNone();
	}

	public void EndTurn()
	{
		_router.UseCaseBuilder.GetUseCase<PlayGameUseCase>().EndTurn();
	}

	public void SetActionsToPerform(Action[] actions)
	{
		actionsToPerform = actions;
	}

	public void SetMoveToNextStageAsActionToPerform()
	{
		actionsToPerform = new Action[3]
		{
			TurnOffPlayerActionsViewInteractions,
			MoveGameSceneViewOnTop,
			_router.UseCaseBuilder.GetUseCase<TutorialGameSceneUseCase>().MoveToTheNextStage
		};
	}

	public async void WaitForPlayerActionsViewAndPerformActions(int delay = 200)
	{
		while (true)
		{
			if (!_router.IsViewVisible<IPlayerActionsView>() || _router.IsViewVisible<IReplayView>())
			{
				await Task.Delay(200);
				continue;
			}
			await Task.Delay(delay);
			if (_router.IsViewVisible<IPlayerActionsView>() && !_router.IsViewVisible<IReplayView>())
			{
				break;
			}
		}
		PerformActions();
	}

	public async void WaitForEndTurnButtonToDisappearAndPerformActions(int delay = 200)
	{
		while (true)
		{
			if (_router.GetView<IPlayerActionsView>().IsEndTurnButtonVisible() || _router.IsViewVisible<IReplayView>())
			{
				await Task.Delay(200);
				continue;
			}
			await Task.Delay(delay);
			if (!_router.GetView<IPlayerActionsView>().IsEndTurnButtonVisible() && !_router.IsViewVisible<IReplayView>())
			{
				break;
			}
		}
		PerformActions();
	}

	public async void WaitForActionEndAndPerformActions(int delay = 200)
	{
		while (true)
		{
			if (_router.IsViewVisible<IActionInProgressView>())
			{
				await Task.Delay(200);
				continue;
			}
			await Task.Delay(delay);
			if (!_router.IsViewVisible<IActionInProgressView>())
			{
				break;
			}
		}
		PerformActions();
	}

	public async void WaitForFadeViewAndPerformActions(int delay = 200)
	{
		while (!_router.IsViewVisible<IFadeView>())
		{
			await Task.Delay(200);
		}
		await Task.Delay(delay);
		PerformActions();
	}

	public async void WaitForReplayToAwaitManualInteractionAndPerformActions(int delay = 200)
	{
		TutorialReplayUseCase replayUseCase = _router.UseCaseBuilder.GetUseCase<TutorialReplayUseCase>();
		while (true)
		{
			if (!replayUseCase.IsAwaitingManualInteraction())
			{
				await Task.Delay(200);
				continue;
			}
			await Task.Delay(delay);
			if (replayUseCase.IsAwaitingManualInteraction())
			{
				break;
			}
		}
		PerformActions();
	}

	public async void WaitForIndustriesDetailsViewAndPerformActions(int delay = 200)
	{
		while (true)
		{
			if (!_router.IsViewVisible<IIndustriesDetailsView>())
			{
				await Task.Delay(200);
				continue;
			}
			await Task.Delay(delay);
			if (_router.IsViewVisible<IIndustriesDetailsView>())
			{
				break;
			}
		}
		PerformActions();
	}

	public async void WaitForIndustriesDetailsViewToHideAndPerformActions(int delay = 200)
	{
		while (true)
		{
			if (_router.IsViewVisible<IIndustriesDetailsView>())
			{
				await Task.Delay(200);
				continue;
			}
			await Task.Delay(delay);
			if (!_router.IsViewVisible<IIndustriesDetailsView>())
			{
				break;
			}
		}
		PerformActions();
	}

	public async void WaitForNetworkView(int delay = 200)
	{
		while (true)
		{
			if (!_router.IsViewVisible<INetworkActionView>())
			{
				await Task.Delay(200);
				continue;
			}
			await Task.Delay(delay);
			if (_router.IsViewVisible<INetworkActionView>())
			{
				break;
			}
		}
		PerformActions();
	}

	public async void WaitForEndOfGameViewAndPerformActions(int delay = 200)
	{
		while (true)
		{
			if (!_router.IsViewVisible<IEndOfGameView>())
			{
				await Task.Delay(200);
				continue;
			}
			await Task.Delay(delay);
			if (_router.IsViewVisible<IEndOfGameView>())
			{
				break;
			}
		}
		PerformActions();
	}

	public void PerformActions()
	{
		Action[] array = actionsToPerform;
		for (int i = 0; i < array.Length; i++)
		{
			array[i]();
		}
	}

	public void MoveToTheNextStage(int stage)
	{
		_router.GetView<ITutorialGameSceneView>().MoveToTheNextStage(stage);
	}

	public void HideStage(int stage)
	{
		_router.GetView<ITutorialGameSceneView>().HideStage(stage);
	}

	public void CardOverviewHighlightLocations()
	{
		_router.GetView<ITutorialCardOverviewView>().HighlightLocations();
	}

	public void CardOverviewHighlightBoth()
	{
		_router.GetView<ITutorialCardOverviewView>().HighlightBoth();
	}

	public void HideCardOverview()
	{
		_router.HideView<ITutorialCardOverviewView>();
	}

	public void MoveArrowOverCard(int stage, int cardNumber)
	{
		(float, float, float) cardPosition = _router.GetView<IHandOfCardsView>().GetCardPosition(cardNumber);
		_router.GetView<ITutorialGameSceneView>().MoveArrowFromStageToPosition(stage, cardPosition.Item1, null, -3f);
	}

	public void MoveArrowOverBuildingSlot(int stage, string buildingSlotUniqueId, RegionName regionName)
	{
		(float, float) buildingSlotPosition = _router.GetView<ITutorialBoardView>().GetBuildingSlotPosition(buildingSlotUniqueId, regionName);
		_router.GetView<ITutorialGameSceneView>().MoveArrowFromStageToPosition(stage, buildingSlotPosition.Item1, buildingSlotPosition.Item2, -5f, -10f);
	}

	public void HideCurrentStageArrows(int stage)
	{
		_router.GetView<ITutorialGameSceneView>().HideArrowFromStage(stage);
	}
}
