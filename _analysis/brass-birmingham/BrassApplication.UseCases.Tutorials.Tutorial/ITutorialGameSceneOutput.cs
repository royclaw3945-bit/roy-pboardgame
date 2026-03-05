using System;
using System.Collections.Generic;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Links;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.Common;
using BrassApplication.Game;
using BrassApplication.UseCases.Game.GameScene;

namespace BrassApplication.UseCases.Tutorials.Tutorial;

public interface ITutorialGameSceneOutput : IGameSceneOutput
{
	void HidePlayerActions();

	void SetupTutorial(Action onClickAction);

	void TurnOffLoadSceneView();

	void SetCardsThatCanBeSelected(List<BaseCard> cards);

	bool IsReplayInProgress();

	void SetAutoSkipReplay(bool useInstantReplay);

	void SetManualReplay(bool manualReplay);

	void ReplayEndTurnFirstEvents();

	void ReplayNextAction();

	void ReplayEndOfEraStart();

	void ReplayEndOfEraScoringLinks();

	void ReplayEndOfEraScoringIndustries();

	void ReplayEndOfEndOfEra();

	void ReplayStartOfRailEra();

	void MoveToTheNextStage(int stage);

	void HideStage(int stage);

	void ShowTutorialPlayersSummary();

	void ShrinkTutorialPlayersSummaryToNormal();

	void RestoreTutorialPlayersSummaryToStandard();

	void RestoreHandOfCardsViewToStandard(PlayerMetadata playerMetadata);

	void RestoreTutorialActionsUseCasesToStandard();

	void ScrollToTutorialStartingPoint();

	void ScrollToWarrington();

	void ScrollToDerby();

	void AllowBoardScrolling();

	void HighlightOnlySpecificSlot(string buildingSlotUniqueId);

	void HideAllMerchantRegions();

	void ShowRegion(RegionName regionName, float delay = 0f);

	void SetAllowedIndustries(List<BuildingType> allowedIndustries);

	void AllowCancellingOfActions();

	void RestoreAllPlayerActions();

	void EnableEndTurnButton(bool enableEndTurn);

	void StartBuildAction();

	void StartSellAction();

	void StartDevelopAction();

	void StartNetworkAction();

	void SetPossibleBeerSourcesForSell(IEnumerable<IBeerSource> beerSources);

	void StartNetworkActionWithSpecifiedLinks(List<Link> linksToHighlight);

	void ShowCardOverview();

	void PassAction(BaseCard card);

	void CardOverviewHighlightLocations();

	void CardOverviewHighlightBoth();

	void HideCardOverview();

	void SlipCardsOut();

	void SlipCardsIn();

	void TurnOffPlayerActionsViewInteractions();

	void SetActionsToPerform(Action[] actions);

	void SetMoveToNextStageAsActionToPerform();

	void WaitForPlayerActionsViewAndPerformActions(int delay = 200);

	void WaitForActionEndAndPerformActions(int delay = 200);

	void WaitForFadeViewAndPerformActions(int delay = 200);

	void WaitForIndustriesDetailsViewAndPerformActions(int delay = 200);

	void WaitForIndustriesDetailsViewToHideAndPerformActions(int delay = 200);

	void WaitForNetworkView(int delay = 200);

	void WaitForEndOfGameViewAndPerformActions(int delay = 200);

	void PerformActions();

	void MoveArrowOverCard(int stage, int cardNumber);

	void MoveArrowOverBuildingSlot(int stage, string buildingSlotUniqueId, RegionName regionName);

	void MoveGameSceneViewOnTop();

	void EndTurn();

	void HideCurrentStageArrows(int currentStage);

	void InitiateNextSell();

	void ShowGameProgressDetails();

	void HideGameProgressDetails();

	void WaitForReplayToAwaitManualInteractionAndPerformActions(int delay = 200);

	void UpdateGameHistoryAndFinishReplay();

	void ReplayRemaining();

	void ReplayManualEvent();

	void ShowAllPlayers();

	void LockSkipButton();

	void UnlockSkipButton();

	void DevelopHighlightAll();

	void DevelopHighlightNone();

	void DevelopHighlightCottonMill();

	void ShowWholeMap();

	void TurnOnPlayerActionsViewInteractions();

	void TurnOnGameProgressSummaryViewInteractions();

	void WaitForEndTurnButtonToDisappearAndPerformActions(int delay = 200);
}
