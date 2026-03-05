using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoardGameRules.Entities.Board.BuildingSlots;
using BoardGameRules.Entities.Board.Links;
using BoardGameRules.Entities.Board.Regions;
using BoardGameRules.Entities.Cards;
using BoardGameRules.Entities.Common;
using BoardGameRules.Entities.Players;
using BrassApplication.ApplicationSettings;
using BrassApplication.Game;
using BrassApplication.GameHistory;
using BrassApplication.UseCases.Game.GameScene;
using BrassApplication.Utils;

namespace BrassApplication.UseCases.Tutorials.Tutorial;

public class TutorialGameSceneUseCase : GameSceneUseCase, ITutorialGameSceneInput, IGameSceneInput
{
	private readonly ITutorialGameSceneOutput _presenter;

	protected readonly IGameHistory _gameHistory;

	private readonly IAnalytics _analytics;

	private const string TutorialId = "TutorialId";

	private int _currentStage;

	public TutorialGameSceneUseCase(ITutorialGameSceneOutput presenter, IGameSettings gameSettings, IGameHistory gameHistory, IAnalytics analytics)
		: base(presenter, gameSettings)
	{
		_presenter = presenter;
		_gameHistory = gameHistory;
		_analytics = analytics;
	}

	public override async void StartGameScene()
	{
		_presenter.ShowLoadingView();
		_presenter.SetupTutorial(MoveToTheNextStage);
		_presenter.ShowBoard();
		_presenter.ScrollToTutorialStartingPoint();
		_presenter.HideAllMerchantRegions();
		_presenter.ShowGameProgressSummary();
		_presenter.ShowGamePanel();
		_presenter.PlayGame();
		_presenter.SetAutoSkipReplay(useInstantReplay: true);
		_presenter.SetManualReplay(manualReplay: false);
		while (_presenter.IsReplayInProgress())
		{
			await Task.Delay(50);
		}
		_presenter.PassAction(_gameHistory.CurrentGame.Game.GameProgressInformation.CurrentPlayer.Hand.Last());
		_presenter.SetActionsToPerform(new Action[4] { _presenter.TurnOffPlayerActionsViewInteractions, _presenter.MoveGameSceneViewOnTop, _presenter.EndTurn, MoveToTheNextStage });
		WaitForEndTurnPossibilityAndPerformActions();
		_analytics.TutorialStart("TutorialId");
	}

	public void MoveToTheNextStage()
	{
		BrassApplicationGame currentGame = _gameHistory.CurrentGame;
		Player currentPlayer = currentGame.Game.GameProgressInformation.CurrentPlayer;
		currentGame.Metadata.GetPlayerMetadataByColor(currentPlayer.Color);
		_presenter.MoveToTheNextStage(_currentStage++);
		if (_currentStage % 2 == 0)
		{
			_analytics.TutorialStep("TutorialId", _currentStage);
		}
		StageSpecificAction(_currentStage);
	}

	public async void WaitAndMoveToTheNextStage(int waitTimeMiliseconds)
	{
		_presenter.HideStage(_currentStage);
		await Task.Delay(waitTimeMiliseconds);
		MoveToTheNextStage();
	}

	public void TriggerPerfomActions()
	{
		_presenter.PerformActions();
	}

	public void HideCurrentStageArrows()
	{
		_presenter.HideCurrentStageArrows(_currentStage);
	}

	private bool CanEndTurn()
	{
		return _gameHistory.CurrentGame.Game.GameFlow.CanEndTurn();
	}

	private async void WaitForEndTurnPossibilityAndPerformActions(int delay = 200)
	{
		while (!CanEndTurn())
		{
			await Task.Delay(200);
		}
		await Task.Delay(delay);
		_presenter.PerformActions();
	}

	private void StageSpecificAction(int stage)
	{
		switch (stage)
		{
		case 1:
			_presenter.SetActionsToPerform(new Action[3] { _presenter.HidePlayerActions, _presenter.MoveGameSceneViewOnTop, _presenter.TurnOffLoadSceneView });
			_presenter.WaitForPlayerActionsViewAndPerformActions();
			break;
		case 3:
			_presenter.ShowTutorialPlayersSummary();
			break;
		case 8:
			_presenter.ShrinkTutorialPlayersSummaryToNormal();
			_presenter.ShowIndustriesSummary();
			break;
		case 11:
			_presenter.ShowCards();
			_presenter.SlipCardsOut();
			break;
		case 12:
			_presenter.SlipCardsIn();
			_presenter.ShowPlayerActions();
			break;
		case 16:
			_presenter.ShowCardOverview();
			break;
		case 17:
			_presenter.CardOverviewHighlightLocations();
			break;
		case 18:
			_presenter.CardOverviewHighlightBoth();
			break;
		case 19:
			_presenter.HideCardOverview();
			break;
		case 20:
		{
			List<BaseCard> list3 = _gameHistory.CurrentGame.Game.GameProgressInformation.CurrentPlayer.Hand.FindAll((BaseCard card) => card is LocationCard locationCard && locationCard.Region == RegionName.StokeOnTrent);
			list3.AddRange(_gameHistory.CurrentGame.Game.GameProgressInformation.CurrentPlayer.Hand.FindAll((BaseCard card) => card is IndustryCard industryCard && industryCard.BuildingTypes.Contains(BuildingType.CottonMill)));
			_presenter.SetCardsThatCanBeSelected(list3);
			_presenter.SetAllowedIndustries(new List<BuildingType> { BuildingType.CottonMill });
			_presenter.StartBuildAction();
			Region regionByName7 = _gameHistory.CurrentGame.Game.Board.GetRegionByName(RegionName.StokeOnTrent);
			_presenter.HighlightOnlySpecificSlot(regionByName7.BuildingSlots[2].UniqueId);
			_presenter.SetActionsToPerform(new Action[1] { MoveToTheNextStage });
			_presenter.WaitForActionEndAndPerformActions(2200);
			break;
		}
		case 21:
			_presenter.SetActionsToPerform(new Action[2] { _presenter.TurnOffPlayerActionsViewInteractions, _presenter.MoveGameSceneViewOnTop });
			_presenter.WaitForPlayerActionsViewAndPerformActions();
			MoveToTheNextStage();
			break;
		case 22:
			MoveToTheNextStage();
			break;
		case 25:
			_presenter.StartNetworkActionWithSpecifiedLinks(new List<Link> { _gameHistory.CurrentGame.Game.Board.GetRegionByName(RegionName.StokeOnTrent).Links[1] });
			break;
		case 26:
			_presenter.SetCardsThatCanBeSelected(_gameHistory.CurrentGame.Game.GameProgressInformation.CurrentPlayer.Hand);
			MoveToTheNextStage();
			break;
		case 27:
			_presenter.SetActionsToPerform(new Action[1] { MoveToTheNextStage });
			_presenter.WaitForActionEndAndPerformActions(1000);
			break;
		case 28:
			_presenter.SetActionsToPerform(new Action[3] { _presenter.MoveGameSceneViewOnTop, EnableEndTurnButton3, MoveToTheNextStage });
			WaitForEndTurnPossibilityAndPerformActions();
			break;
		case 29:
			_presenter.SetActionsToPerform(new Action[3] { _presenter.TurnOffPlayerActionsViewInteractions, _presenter.MoveGameSceneViewOnTop, MoveToTheNextStage });
			_presenter.WaitForEndTurnButtonToDisappearAndPerformActions();
			break;
		case 30:
			_presenter.EnableEndTurnButton(enableEndTurn: false);
			_presenter.TurnOffPlayerActionsViewInteractions();
			_presenter.MoveGameSceneViewOnTop();
			break;
		case 31:
		{
			List<BaseCard> list2 = _gameHistory.CurrentGame.Game.GameProgressInformation.CurrentPlayer.Hand.FindAll((BaseCard card) => card is LocationCard locationCard && locationCard.Region == RegionName.Stone);
			list2.AddRange(_gameHistory.CurrentGame.Game.GameProgressInformation.CurrentPlayer.Hand.FindAll((BaseCard card) => card is IndustryCard industryCard && industryCard.BuildingTypes.Contains(BuildingType.CottonMill)));
			_presenter.SetCardsThatCanBeSelected(list2);
			_presenter.StartBuildAction();
			Region regionByName2 = _gameHistory.CurrentGame.Game.Board.GetRegionByName(RegionName.Stone);
			_presenter.HighlightOnlySpecificSlot(regionByName2.BuildingSlots[0].UniqueId);
			_presenter.MoveArrowOverBuildingSlot(stage, regionByName2.BuildingSlots[0].UniqueId, RegionName.Stone);
			_presenter.SetActionsToPerform(new Action[1] { MoveToTheNextStage });
			_presenter.WaitForActionEndAndPerformActions(2200);
			break;
		}
		case 32:
			_presenter.SetActionsToPerform(new Action[2] { _presenter.TurnOffPlayerActionsViewInteractions, _presenter.MoveGameSceneViewOnTop });
			_presenter.WaitForPlayerActionsViewAndPerformActions();
			MoveToTheNextStage();
			break;
		case 33:
			MoveToTheNextStage();
			break;
		case 34:
			_presenter.ShowMarket();
			_presenter.AllowBoardScrolling();
			break;
		case 36:
		{
			List<BaseCard> list = _gameHistory.CurrentGame.Game.GameProgressInformation.CurrentPlayer.Hand.FindAll((BaseCard card) => card is LocationCard locationCard && locationCard.Region == RegionName.Stafford);
			list.AddRange(_gameHistory.CurrentGame.Game.GameProgressInformation.CurrentPlayer.Hand.FindAll((BaseCard card) => card is IndustryCard industryCard && industryCard.BuildingTypes.Contains(BuildingType.Brewery)));
			_presenter.SetCardsThatCanBeSelected(list);
			_presenter.SetAllowedIndustries(new List<BuildingType> { BuildingType.Brewery });
			_presenter.StartBuildAction();
			Region regionByName = _gameHistory.CurrentGame.Game.Board.GetRegionByName(RegionName.Stafford);
			_presenter.HighlightOnlySpecificSlot(regionByName.BuildingSlots[0].UniqueId);
			_presenter.SetActionsToPerform(new Action[1] { MoveToTheNextStage });
			_presenter.WaitForFadeViewAndPerformActions(0);
			break;
		}
		case 37:
			_presenter.MoveGameSceneViewOnTop();
			break;
		case 39:
			_presenter.SetActionsToPerform(new Action[1] { MoveToTheNextStage });
			_presenter.WaitForActionEndAndPerformActions(2200);
			break;
		case 40:
			_presenter.SetActionsToPerform(new Action[3] { _presenter.MoveGameSceneViewOnTop, EnableEndTurnButton7, MoveToTheNextStage });
			WaitForEndTurnPossibilityAndPerformActions();
			break;
		case 41:
			_presenter.SetActionsToPerform(new Action[3] { _presenter.TurnOffPlayerActionsViewInteractions, _presenter.MoveGameSceneViewOnTop, MoveToTheNextStage });
			_presenter.WaitForEndTurnButtonToDisappearAndPerformActions();
			break;
		case 42:
			_presenter.EnableEndTurnButton(enableEndTurn: false);
			_presenter.ScrollToWarrington();
			_presenter.ShowRegion(RegionName.Warrington, 1f);
			MoveToTheNextStage();
			break;
		case 43:
			_presenter.SetActionsToPerform(new Action[2] { _presenter.TurnOffPlayerActionsViewInteractions, _presenter.MoveGameSceneViewOnTop });
			_presenter.WaitForPlayerActionsViewAndPerformActions(2200);
			break;
		case 47:
			_presenter.StartNetworkActionWithSpecifiedLinks(new List<Link> { _gameHistory.CurrentGame.Game.Board.GetRegionByName(RegionName.Warrington).Links[1] });
			_presenter.SetCardsThatCanBeSelected(_gameHistory.CurrentGame.Game.GameProgressInformation.CurrentPlayer.Hand);
			_presenter.SetActionsToPerform(new Action[1] { MoveToTheNextStage });
			_presenter.WaitForActionEndAndPerformActions(2200);
			break;
		case 48:
			_presenter.SetActionsToPerform(new Action[2] { _presenter.TurnOffPlayerActionsViewInteractions, _presenter.MoveGameSceneViewOnTop });
			_presenter.WaitForPlayerActionsViewAndPerformActions();
			break;
		case 50:
		{
			IBeerSource beerSource = (IBeerSource)_gameHistory.CurrentGame.Game.Board.GetRegionByName(RegionName.Stafford).BuildingSlots[0].BuildedIndustry;
			_presenter.SetPossibleBeerSourcesForSell(new IBeerSource[2] { beerSource, null });
			_presenter.StartSellAction();
			break;
		}
		case 51:
			MoveToTheNextStage();
			break;
		case 52:
			_presenter.MoveGameSceneViewOnTop();
			break;
		case 59:
			_presenter.ShowGameProgressDetails();
			_presenter.MoveGameSceneViewOnTop();
			break;
		case 63:
			_presenter.HideGameProgressDetails();
			_presenter.SetPossibleBeerSourcesForSell(_gameHistory.CurrentGame.Game.Board.GetMerchantsWithBeer().Cast<IBeerSource>());
			_presenter.InitiateNextSell();
			break;
		case 64:
			MoveToTheNextStage();
			break;
		case 68:
			_presenter.SetActionsToPerform(new Action[4] { _presenter.TurnOnPlayerActionsViewInteractions, _presenter.MoveGameSceneViewOnTop, Stage69Action, EnableEndTurnButton6 });
			WaitForEndTurnPossibilityAndPerformActions();
			break;
		case 69:
			_presenter.SetActionsToPerform(new Action[3] { _presenter.TurnOffPlayerActionsViewInteractions, _presenter.MoveGameSceneViewOnTop, Stage69View });
			_presenter.WaitForEndTurnButtonToDisappearAndPerformActions(400);
			break;
		case 70:
			_presenter.EnableEndTurnButton(enableEndTurn: false);
			_presenter.MoveGameSceneViewOnTop();
			_presenter.TurnOffPlayerActionsViewInteractions();
			break;
		case 71:
		{
			List<BaseCard> cardsThatCanBeSelected3 = _gameHistory.CurrentGame.Game.GameProgressInformation.CurrentPlayer.Hand.FindAll((BaseCard card) => card is LocationCard locationCard && locationCard.Region == RegionName.Belper);
			_presenter.SetCardsThatCanBeSelected(cardsThatCanBeSelected3);
			_presenter.SetAllowedIndustries(new List<BuildingType> { BuildingType.CoalMine });
			_presenter.StartBuildAction();
			Region regionByName6 = _gameHistory.CurrentGame.Game.Board.GetRegionByName(RegionName.Belper);
			_presenter.HighlightOnlySpecificSlot(regionByName6.BuildingSlots[1].UniqueId);
			_presenter.SetActionsToPerform(new Action[1] { MoveToTheNextStage });
			_presenter.WaitForActionEndAndPerformActions(1800);
			break;
		}
		case 73:
			_presenter.MoveGameSceneViewOnTop();
			_presenter.TurnOffPlayerActionsViewInteractions();
			break;
		case 74:
			_presenter.MoveGameSceneViewOnTop();
			_presenter.StartNetworkActionWithSpecifiedLinks(new List<Link> { _gameHistory.CurrentGame.Game.Board.GetRegionByName(RegionName.Belper).Links[2] });
			_presenter.SetCardsThatCanBeSelected(_gameHistory.CurrentGame.Game.GameProgressInformation.CurrentPlayer.Hand);
			_presenter.SetActionsToPerform(new Action[5] { _presenter.TurnOnPlayerActionsViewInteractions, _presenter.MoveGameSceneViewOnTop, _presenter.LockSkipButton, MoveToTheNextStage, EnableEndTurnButton5 });
			WaitForEndTurnPossibilityAndPerformActions();
			break;
		case 75:
			_presenter.SetAutoSkipReplay(useInstantReplay: false);
			_presenter.SetManualReplay(manualReplay: true);
			_presenter.SetActionsToPerform(new Action[3] { _presenter.TurnOffPlayerActionsViewInteractions, _presenter.MoveGameSceneViewOnTop, MoveToTheNextStage });
			_presenter.WaitForReplayToAwaitManualInteractionAndPerformActions(400);
			break;
		case 76:
			_presenter.EnableEndTurnButton(enableEndTurn: false);
			break;
		case 78:
			MoveToTheNextStage();
			break;
		case 79:
			_presenter.ReplayEndTurnFirstEvents();
			_presenter.ShowAllPlayers();
			_presenter.SetActionsToPerform(new Action[2] { _presenter.MoveGameSceneViewOnTop, MoveToTheNextStage });
			_presenter.WaitForReplayToAwaitManualInteractionAndPerformActions();
			break;
		case 80:
			_presenter.RestoreTutorialPlayersSummaryToStandard();
			break;
		case 81:
			_presenter.ReplayNextAction();
			_presenter.SetActionsToPerform(new Action[2] { _presenter.MoveGameSceneViewOnTop, MoveToTheNextStage });
			_presenter.WaitForReplayToAwaitManualInteractionAndPerformActions();
			break;
		case 83:
			_presenter.ReplayRemaining();
			_presenter.SetActionsToPerform(new Action[3] { _presenter.UpdateGameHistoryAndFinishReplay, _presenter.MoveGameSceneViewOnTop, MoveToTheNextStage });
			_presenter.WaitForReplayToAwaitManualInteractionAndPerformActions();
			break;
		case 88:
			_presenter.TurnOffPlayerActionsViewInteractions();
			_presenter.MoveGameSceneViewOnTop();
			break;
		case 89:
		{
			_presenter.SetManualReplay(manualReplay: false);
			List<BaseCard> cardsThatCanBeSelected2 = _gameHistory.CurrentGame.Game.GameProgressInformation.CurrentPlayer.Hand.FindAll((BaseCard card) => card is IndustryCard industryCard && industryCard.BuildingTypes.Contains(BuildingType.IronWorks));
			_presenter.SetCardsThatCanBeSelected(cardsThatCanBeSelected2);
			_presenter.SetAllowedIndustries(new List<BuildingType> { BuildingType.IronWorks });
			_presenter.StartBuildAction();
			Region regionByName5 = _gameHistory.CurrentGame.Game.Board.GetRegionByName(RegionName.Derby);
			_presenter.HighlightOnlySpecificSlot(regionByName5.BuildingSlots[2].UniqueId);
			_presenter.SetActionsToPerform(new Action[3] { MoveToTheNextStage, _presenter.TurnOffPlayerActionsViewInteractions, _presenter.MoveGameSceneViewOnTop });
			_presenter.WaitForReplayToAwaitManualInteractionAndPerformActions(800);
			break;
		}
		case 92:
			_presenter.ReplayManualEvent();
			_presenter.SetActionsToPerform(new Action[3] { MoveToTheNextStage, _presenter.TurnOffPlayerActionsViewInteractions, _presenter.MoveGameSceneViewOnTop });
			_presenter.WaitForActionEndAndPerformActions(1000);
			break;
		case 95:
			_presenter.TurnOffPlayerActionsViewInteractions();
			break;
		case 96:
		{
			_presenter.MoveGameSceneViewOnTop();
			List<BaseCard> cardsThatCanBeSelected = _gameHistory.CurrentGame.Game.GameProgressInformation.CurrentPlayer.Hand.FindAll((BaseCard card) => card is LocationCard locationCard && locationCard.Region == RegionName.BurtonOnTrent);
			_presenter.SetCardsThatCanBeSelected(cardsThatCanBeSelected);
			_presenter.SetAllowedIndustries(new List<BuildingType> { BuildingType.Manufacturer });
			_presenter.StartBuildAction();
			Region regionByName4 = _gameHistory.CurrentGame.Game.Board.GetRegionByName(RegionName.BurtonOnTrent);
			_presenter.HighlightOnlySpecificSlot(regionByName4.BuildingSlots[0].UniqueId);
			_presenter.SetActionsToPerform(new Action[2] { _presenter.TurnOffPlayerActionsViewInteractions, MoveToTheNextStage });
			_presenter.WaitForActionEndAndPerformActions(1000);
			break;
		}
		case 97:
			_presenter.ScrollToDerby();
			break;
		case 99:
			_presenter.SetActionsToPerform(new Action[6] { SetManualReplay, EnableEndTurnButton4, _presenter.TurnOnPlayerActionsViewInteractions, _presenter.MoveGameSceneViewOnTop, _presenter.LockSkipButton, MoveToTheNextStage });
			WaitForEndTurnPossibilityAndPerformActions();
			break;
		case 100:
			_presenter.SetActionsToPerform(new Action[2] { _presenter.MoveGameSceneViewOnTop, MoveToTheNextStage });
			_presenter.WaitForReplayToAwaitManualInteractionAndPerformActions();
			break;
		case 101:
			_presenter.EnableEndTurnButton(enableEndTurn: false);
			_presenter.ReplayNextAction();
			_presenter.SetMoveToNextStageAsActionToPerform();
			_presenter.WaitForReplayToAwaitManualInteractionAndPerformActions();
			break;
		case 104:
			_presenter.ScrollToDerby();
			_presenter.ShowRegion(RegionName.Nottingham, 1f);
			MoveToTheNextStage();
			break;
		case 106:
			_presenter.ReplayRemaining();
			_presenter.SetActionsToPerform(new Action[3] { _presenter.UpdateGameHistoryAndFinishReplay, _presenter.MoveGameSceneViewOnTop, MoveToTheNextStage });
			_presenter.WaitForReplayToAwaitManualInteractionAndPerformActions();
			break;
		case 108:
			_presenter.SetManualReplay(manualReplay: false);
			_presenter.MoveGameSceneViewOnTop();
			_presenter.TurnOffPlayerActionsViewInteractions();
			break;
		case 109:
		{
			List<BaseCard> hand2 = _gameHistory.CurrentGame.Game.GameProgressInformation.CurrentPlayer.Hand;
			_presenter.SetCardsThatCanBeSelected(hand2);
			_presenter.StartDevelopAction();
			_presenter.SetMoveToNextStageAsActionToPerform();
			_presenter.WaitForIndustriesDetailsViewAndPerformActions();
			break;
		}
		case 113:
			_presenter.DevelopHighlightAll();
			break;
		case 115:
			_presenter.DevelopHighlightNone();
			break;
		case 117:
			_presenter.DevelopHighlightCottonMill();
			break;
		case 118:
			_presenter.SetMoveToNextStageAsActionToPerform();
			_presenter.WaitForIndustriesDetailsViewToHideAndPerformActions();
			break;
		case 119:
			MoveToTheNextStage();
			break;
		case 120:
			_presenter.SetMoveToNextStageAsActionToPerform();
			_presenter.WaitForActionEndAndPerformActions();
			break;
		case 123:
			_presenter.TurnOffPlayerActionsViewInteractions();
			_presenter.MoveGameSceneViewOnTop();
			break;
		case 124:
			_presenter.MoveGameSceneViewOnTop();
			_presenter.StartNetworkActionWithSpecifiedLinks(new List<Link> { _gameHistory.CurrentGame.Game.Board.GetRegionByName(RegionName.BurtonOnTrent).Links.First((Link link) => link is Canal && (link.Region1.Name == RegionName.Tamworth || link.Region2.Name == RegionName.Tamworth)) });
			_presenter.SetCardsThatCanBeSelected(_gameHistory.CurrentGame.Game.GameProgressInformation.CurrentPlayer.Hand);
			_presenter.SetActionsToPerform(new Action[4] { _presenter.TurnOnPlayerActionsViewInteractions, _presenter.MoveGameSceneViewOnTop, MoveToTheNextStage, EnableEndTurnButton2 });
			WaitForEndTurnPossibilityAndPerformActions();
			break;
		case 125:
			_presenter.SetActionsToPerform(new Action[3] { _presenter.TurnOffPlayerActionsViewInteractions, _presenter.MoveGameSceneViewOnTop, MoveToTheNextStage });
			_presenter.WaitForEndTurnButtonToDisappearAndPerformActions();
			break;
		case 126:
			_presenter.EnableEndTurnButton(enableEndTurn: false);
			break;
		case 127:
			_presenter.TurnOffPlayerActionsViewInteractions();
			_presenter.MoveGameSceneViewOnTop();
			break;
		case 128:
		{
			_presenter.MoveGameSceneViewOnTop();
			List<BaseCard> first = _gameHistory.CurrentGame.Game.GameProgressInformation.CurrentPlayer.Hand.FindAll((BaseCard card) => card is LocationCard locationCard && locationCard.Region == RegionName.Tamworth);
			first = first.Concat(_gameHistory.CurrentGame.Game.GameProgressInformation.CurrentPlayer.Hand.FindAll((BaseCard card) => card is IndustryCard industryCard && industryCard.BuildingTypes.Contains(BuildingType.CottonMill))).ToList();
			_presenter.SetCardsThatCanBeSelected(first);
			_presenter.SetAllowedIndustries(new List<BuildingType> { BuildingType.CottonMill });
			_presenter.StartBuildAction();
			Region regionByName3 = _gameHistory.CurrentGame.Game.Board.GetRegionByName(RegionName.Tamworth);
			_presenter.HighlightOnlySpecificSlot(regionByName3.BuildingSlots[0].UniqueId);
			_presenter.SetActionsToPerform(new Action[1] { MoveToTheNextStage });
			_presenter.WaitForActionEndAndPerformActions(2200);
			break;
		}
		case 130:
			_presenter.TurnOffPlayerActionsViewInteractions();
			_presenter.MoveGameSceneViewOnTop();
			break;
		case 131:
		{
			List<BaseCard> hand = _gameHistory.CurrentGame.Game.GameProgressInformation.CurrentPlayer.Hand;
			_presenter.SetCardsThatCanBeSelected(hand);
			_presenter.StartSellAction();
			break;
		}
		case 132:
			_presenter.SetMoveToNextStageAsActionToPerform();
			_presenter.WaitForActionEndAndPerformActions(2000);
			break;
		case 134:
			_presenter.SetManualReplay(manualReplay: true);
			_presenter.SetActionsToPerform(new Action[4] { _presenter.TurnOnPlayerActionsViewInteractions, _presenter.MoveGameSceneViewOnTop, EnableEndTurnButton, MoveToTheNextStage });
			WaitForEndTurnPossibilityAndPerformActions();
			break;
		case 135:
			_presenter.SetActionsToPerform(new Action[2] { _presenter.MoveGameSceneViewOnTop, MoveToTheNextStage });
			_presenter.WaitForReplayToAwaitManualInteractionAndPerformActions();
			break;
		case 136:
			_presenter.EnableEndTurnButton(enableEndTurn: false);
			_presenter.ReplayNextAction();
			_presenter.SetMoveToNextStageAsActionToPerform();
			_presenter.WaitForReplayToAwaitManualInteractionAndPerformActions();
			break;
		case 137:
			_presenter.ReplayNextAction();
			_presenter.WaitForReplayToAwaitManualInteractionAndPerformActions();
			break;
		case 139:
			_presenter.ReplayEndOfEraStart();
			_presenter.WaitForReplayToAwaitManualInteractionAndPerformActions();
			break;
		case 142:
			_presenter.ReplayEndOfEraScoringLinks();
			_presenter.WaitForReplayToAwaitManualInteractionAndPerformActions();
			break;
		case 144:
			_presenter.ReplayEndOfEraScoringIndustries();
			_presenter.WaitForReplayToAwaitManualInteractionAndPerformActions();
			break;
		case 146:
			_presenter.ReplayEndOfEndOfEra();
			_presenter.WaitForReplayToAwaitManualInteractionAndPerformActions();
			break;
		case 147:
			_presenter.ReplayStartOfRailEra();
			_presenter.WaitForReplayToAwaitManualInteractionAndPerformActions();
			break;
		case 149:
			_presenter.ShowWholeMap();
			_presenter.ShowRegion(RegionName.Oxford);
			_presenter.ShowRegion(RegionName.Gloucester);
			_presenter.ShowRegion(RegionName.Shrewsbury);
			_presenter.ReplayRemaining();
			_presenter.SetActionsToPerform(new Action[3] { _presenter.UpdateGameHistoryAndFinishReplay, _presenter.MoveGameSceneViewOnTop, MoveToTheNextStage });
			_presenter.WaitForReplayToAwaitManualInteractionAndPerformActions();
			break;
		case 151:
			_presenter.SetManualReplay(manualReplay: false);
			_presenter.RestoreTutorialActionsUseCasesToStandard();
			_presenter.RestoreHandOfCardsViewToStandard(_gameHistory.CurrentGame.Metadata.PlayerMetadata.First((PlayerMetadata pm) => pm.PlayerStartConfiguration.Color == PlayerColor.Yellow));
			_presenter.TurnOffPlayerActionsViewInteractions();
			_presenter.MoveGameSceneViewOnTop();
			break;
		case 152:
			_presenter.MoveGameSceneViewOnTop();
			_presenter.StartBuildAction();
			_presenter.SetActionsToPerform(new Action[1] { MoveToTheNextStage });
			_presenter.WaitForActionEndAndPerformActions(2200);
			break;
		case 154:
			_presenter.TurnOffPlayerActionsViewInteractions();
			_presenter.MoveGameSceneViewOnTop();
			break;
		case 155:
			_presenter.MoveGameSceneViewOnTop();
			_presenter.StartNetworkAction();
			_presenter.SetActionsToPerform(new Action[3] { _presenter.TurnOffPlayerActionsViewInteractions, _presenter.MoveGameSceneViewOnTop, MoveToTheNextStage });
			_presenter.WaitForNetworkView();
			break;
		case 156:
			_presenter.SetActionsToPerform(new Action[1] { MoveToTheNextStage });
			_presenter.WaitForActionEndAndPerformActions(2200);
			break;
		case 157:
			_presenter.AllowCancellingOfActions();
			_presenter.TurnOnGameProgressSummaryViewInteractions();
			_presenter.UnlockSkipButton();
			_presenter.EnableEndTurnButton(enableEndTurn: true);
			_presenter.RestoreAllPlayerActions();
			break;
		case 158:
			_presenter.TurnOnPlayerActionsViewInteractions();
			_presenter.SetActionsToPerform(new Action[1] { MoveToTheNextStage });
			_presenter.WaitForEndOfGameViewAndPerformActions();
			break;
		case 159:
		{
			_presenter.MoveGameSceneViewOnTop();
			_analytics.TutorialComplete("TutorialId");
			Dictionary<PlayerColor, int> dictionary2 = _gameHistory.CurrentGame.Game.PlayersTrack.Players.ToDictionary((Player p) => p.Color, (Player p) => p.VP);
			if (dictionary2[PlayerColor.Yellow] <= dictionary2[PlayerColor.Red])
			{
				MoveToTheNextStage();
			}
			break;
		}
		case 160:
		{
			_presenter.MoveGameSceneViewOnTop();
			_analytics.TutorialComplete("TutorialId");
			Dictionary<PlayerColor, int> dictionary = _gameHistory.CurrentGame.Game.PlayersTrack.Players.ToDictionary((Player p) => p.Color, (Player p) => p.VP);
			if (dictionary[PlayerColor.Yellow] > dictionary[PlayerColor.Red])
			{
				MoveToTheNextStage();
			}
			break;
		}
		case 2:
		case 4:
		case 5:
		case 6:
		case 7:
		case 9:
		case 10:
		case 13:
		case 14:
		case 15:
		case 23:
		case 24:
		case 35:
		case 38:
		case 44:
		case 45:
		case 46:
		case 49:
		case 53:
		case 54:
		case 55:
		case 56:
		case 57:
		case 58:
		case 60:
		case 61:
		case 62:
		case 65:
		case 66:
		case 67:
		case 72:
		case 77:
		case 82:
		case 84:
		case 85:
		case 86:
		case 87:
		case 90:
		case 91:
		case 93:
		case 94:
		case 98:
		case 102:
		case 103:
		case 105:
		case 107:
		case 110:
		case 111:
		case 112:
		case 114:
		case 116:
		case 121:
		case 122:
		case 129:
		case 133:
		case 138:
		case 140:
		case 141:
		case 143:
		case 145:
		case 148:
		case 150:
		case 153:
			break;
		}
		void EnableEndTurnButton()
		{
			_presenter.EnableEndTurnButton(enableEndTurn: true);
		}
		void EnableEndTurnButton2()
		{
			_presenter.EnableEndTurnButton(enableEndTurn: true);
		}
		void EnableEndTurnButton3()
		{
			_presenter.EnableEndTurnButton(enableEndTurn: true);
		}
		void EnableEndTurnButton4()
		{
			_presenter.EnableEndTurnButton(enableEndTurn: true);
		}
		void EnableEndTurnButton5()
		{
			_presenter.EnableEndTurnButton(enableEndTurn: true);
		}
		void EnableEndTurnButton6()
		{
			_presenter.EnableEndTurnButton(enableEndTurn: true);
		}
		void EnableEndTurnButton7()
		{
			_presenter.EnableEndTurnButton(enableEndTurn: true);
		}
		void SetManualReplay()
		{
			_presenter.SetManualReplay(manualReplay: true);
		}
		void Stage69Action()
		{
			StageSpecificAction(69);
		}
		void Stage69View()
		{
			_presenter.MoveToTheNextStage(_currentStage++);
		}
	}
}
