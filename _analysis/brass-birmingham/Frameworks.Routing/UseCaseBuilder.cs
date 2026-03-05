using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Common;
using BoardGameRules.Entities.Game;
using BoardGameRules.Entities.Players;
using BoardGameRules.Utils.Extensions;
using BrassApplication.AI;
using BrassApplication.AI.AiBehaviors;
using BrassApplication.Game;
using BrassApplication.GameHistory;
using BrassApplication.Network;
using BrassApplication.Persistence.Repository;
using BrassApplication.Persistence.Serialization;
using BrassApplication.UseCases.Common;
using BrassApplication.UseCases.Common.LoadScene;
using BrassApplication.UseCases.Common.SaveGame;
using BrassApplication.UseCases.Game.Actions.ActionInProgress;
using BrassApplication.UseCases.Game.Actions.BeerDelivery;
using BrassApplication.UseCases.Game.Actions.BuildAction;
using BrassApplication.UseCases.Game.Actions.CoalDelivery;
using BrassApplication.UseCases.Game.Actions.DevelopAction;
using BrassApplication.UseCases.Game.Actions.IronDelivery;
using BrassApplication.UseCases.Game.Actions.LoanAction;
using BrassApplication.UseCases.Game.Actions.NetworkAction;
using BrassApplication.UseCases.Game.Actions.PassAction;
using BrassApplication.UseCases.Game.Actions.ScoutAction;
using BrassApplication.UseCases.Game.Actions.SellAction;
using BrassApplication.UseCases.Game.Board;
using BrassApplication.UseCases.Game.EndOfEra;
using BrassApplication.UseCases.Game.ExitToMenu;
using BrassApplication.UseCases.Game.FinishedGameReview;
using BrassApplication.UseCases.Game.GameLog;
using BrassApplication.UseCases.Game.GameProgressDetails;
using BrassApplication.UseCases.Game.GameProgressSummary;
using BrassApplication.UseCases.Game.GameScene;
using BrassApplication.UseCases.Game.HandOfCards;
using BrassApplication.UseCases.Game.IndustriesDetails;
using BrassApplication.UseCases.Game.IndustriesSummary;
using BrassApplication.UseCases.Game.Market;
using BrassApplication.UseCases.Game.PlayAiTurn;
using BrassApplication.UseCases.Game.PlayGame;
using BrassApplication.UseCases.Game.PlayHumanTurn;
using BrassApplication.UseCases.Game.PlaySpectatorTurn;
using BrassApplication.UseCases.Game.PlayerActions;
using BrassApplication.UseCases.Game.PlayersSummary;
using BrassApplication.UseCases.Game.RegionDetails;
using BrassApplication.UseCases.Game.Replay;
using BrassApplication.UseCases.Game.SellIndustryFlow;
using BrassApplication.UseCases.Menu.ChoosePortraitAndColor;
using BrassApplication.UseCases.Menu.Credits;
using BrassApplication.UseCases.Menu.MainMenu;
using BrassApplication.UseCases.Menu.OfflineGame.CreateOfflineGame;
using BrassApplication.UseCases.Menu.OnlineGame.ChoosePortraitAndJoin;
using BrassApplication.UseCases.Menu.OnlineGame.CreateOnlineGame;
using BrassApplication.UseCases.Menu.OnlineGame.JoinOnlineGame;
using BrassApplication.UseCases.Menu.OnlineGame.ListPlayerOnlineGamesMenu;
using BrassApplication.UseCases.Menu.OnlineGame.Login;
using BrassApplication.UseCases.Menu.OnlineGame.PasswordRecovery;
using BrassApplication.UseCases.Menu.OnlineGame.Register;
using BrassApplication.UseCases.Menu.Settings;
using BrassApplication.UseCases.Menu.SplashScreen;
using BrassApplication.UseCases.Menu.Tutorial;
using BrassApplication.UseCases.Tutorials.Actions.Develop;
using BrassApplication.UseCases.Tutorials.Actions.Network;
using BrassApplication.UseCases.Tutorials.Actions.SellAction;
using BrassApplication.UseCases.Tutorials.Board;
using BrassApplication.UseCases.Tutorials.HandOfCards;
using BrassApplication.UseCases.Tutorials.PlayerActions;
using BrassApplication.UseCases.Tutorials.PlayersSummary;
using BrassApplication.UseCases.Tutorials.Replay;
using BrassApplication.UseCases.Tutorials.Tutorial;
using Frameworks.Network;
using Frameworks.Persistence.Serialization;
using Frameworks.Settings;
using InterfaceAdapters.Common;
using InterfaceAdapters.Common.CreateGame;
using InterfaceAdapters.Common.LoadScene;
using InterfaceAdapters.Common.SaveGame;
using InterfaceAdapters.Game.ActionInProgressView;
using InterfaceAdapters.Game.Actions.BeerDelivery;
using InterfaceAdapters.Game.Actions.BuildAction;
using InterfaceAdapters.Game.Actions.CoalDelivery;
using InterfaceAdapters.Game.Actions.DevelopAction;
using InterfaceAdapters.Game.Actions.IronDelivery;
using InterfaceAdapters.Game.Actions.LoanAction;
using InterfaceAdapters.Game.Actions.NetworkAction;
using InterfaceAdapters.Game.Actions.PassAction;
using InterfaceAdapters.Game.Actions.ScoutAction;
using InterfaceAdapters.Game.Actions.SellAction;
using InterfaceAdapters.Game.Board;
using InterfaceAdapters.Game.EndOfEra;
using InterfaceAdapters.Game.ExitToMenu;
using InterfaceAdapters.Game.FinishedGameReview;
using InterfaceAdapters.Game.GameLogView;
using InterfaceAdapters.Game.GameProgressDetails;
using InterfaceAdapters.Game.GameProgressSummary;
using InterfaceAdapters.Game.GameScene;
using InterfaceAdapters.Game.HandOfCards;
using InterfaceAdapters.Game.IndustriesDetails;
using InterfaceAdapters.Game.IndustriesSummary;
using InterfaceAdapters.Game.Market;
using InterfaceAdapters.Game.PlayAiTurn;
using InterfaceAdapters.Game.PlayGame;
using InterfaceAdapters.Game.PlayHumanTurn;
using InterfaceAdapters.Game.PlaySpectatorTurn;
using InterfaceAdapters.Game.PlayerActions;
using InterfaceAdapters.Game.PlayersSummary;
using InterfaceAdapters.Game.RegionDetails;
using InterfaceAdapters.Game.Replay;
using InterfaceAdapters.Game.SellIndustryFlow;
using InterfaceAdapters.Menu.ChoosePortraitAndJoin;
using InterfaceAdapters.Menu.CreateGame;
using InterfaceAdapters.Menu.Credits;
using InterfaceAdapters.Menu.Login;
using InterfaceAdapters.Menu.MainMenu;
using InterfaceAdapters.Menu.Online.CreateOnlineGame;
using InterfaceAdapters.Menu.Online.JoinOnlineGame;
using InterfaceAdapters.Menu.OnlineGameMenu;
using InterfaceAdapters.Menu.PasswordRecovery;
using InterfaceAdapters.Menu.Register;
using InterfaceAdapters.Menu.Settings;
using InterfaceAdapters.Menu.Tutorial;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Tutorials.Actions.Develop;
using InterfaceAdapters.Tutorials.Actions.SellAction;
using InterfaceAdapters.Tutorials.Board;
using InterfaceAdapters.Tutorials.HandOfCards;
using InterfaceAdapters.Tutorials.PlayerActions;
using InterfaceAdapters.Tutorials.PlayersSummary;
using InterfaceAdapters.Tutorials.Tutorial;
using InterfaceAdapters.Utils;
using UnityEngine.SceneManagement;
using Utils;

namespace Frameworks.Routing;

public class UseCaseBuilder : IUseCaseBuilder
{
	private readonly IRouter _router;

	private readonly IDictionary<Type, PUCContainer> _pucContainers;

	private readonly IDictionary<Type, PUCContainer> _pucContainersArchive;

	private readonly INetworkBackendClient _networkBackendClient;

	private readonly IRepositoryFactory _repositoryFactory;

	private readonly ILocalization _localization;

	private readonly IGameHistory _gameHistory;

	private readonly ISerialize _serializer;

	public UseCaseBuilder(IRouter router, ILocalization localization, INetworkBackendClient networkBackendClient, IRepositoryFactory repositoryFactory, IGameHistory gameHistory)
	{
		_router = router;
		_gameHistory = gameHistory;
		_router.UseCaseBuilder = this;
		_localization = localization;
		_networkBackendClient = networkBackendClient;
		_repositoryFactory = repositoryFactory;
		_pucContainers = new Dictionary<Type, PUCContainer>();
		_pucContainersArchive = new Dictionary<Type, PUCContainer>();
		_serializer = new JsonSerializer();
	}

	public void BuildUseCases()
	{
		if (SceneManager.GetActiveScene().name == "MenuScene")
		{
			CreateCommonUseCases();
			CreateMenuUseCases().OnApplicationStarted();
		}
		else if (SceneManager.GetActiveScene().name == "GameScene")
		{
			SetupEditorGame();
			CreateCommonUseCases();
			GameSceneController gameSceneController = CreateGameUseCases();
			if (_gameHistory.CurrentGame != null && _gameHistory.CurrentGame.Metadata.GameType != GameType.Local && _gameHistory.CurrentGame.Metadata.GameType != GameType.Network)
			{
				gameSceneController = CreateTutorialUseCases();
			}
			gameSceneController.StartGameScene();
		}
	}

	private void CreateCommonUseCases()
	{
		PUCContainer value = new PUCContainer
		{
			Presenter = new LoadScenePresenter(_router, _localization)
		};
		value.UseCase = new LoadSceneUseCase(value.Presenter as ILoadSceneOutput);
		value.Controller = new LoadSceneController(value.UseCase as ILoadSceneInput);
		_pucContainers[typeof(ILoadSceneView)] = value;
	}

	private SplashScreenController CreateMenuUseCases()
	{
		PUCContainer value = new PUCContainer
		{
			Presenter = new SplashScreenPresenter(_router)
		};
		value.UseCase = new SplashScreenUseCase(value.Presenter as ISplashScreenOutput, SettingsFactory.CreateMenuSettings(), SettingsFactory.CreateLanguageSettings());
		value.Controller = new SplashScreenController(value.UseCase as ISplashScreenInput);
		_pucContainers[typeof(ISplashScreenView)] = value;
		value = new PUCContainer
		{
			Presenter = new MainMenuPresenter(_router, _localization)
		};
		value.UseCase = new MainMenuUseCase(value.Presenter as IMainMenuOutput, _networkBackendClient, NetworkFactory.CreateNetworkCredentialsStorage());
		value.Controller = new MainMenuController(value.UseCase as IMainMenuInput);
		_pucContainers[typeof(IMainMenuView)] = value;
		value = new PUCContainer
		{
			Presenter = new SettingsPresenter(_router)
		};
		value.UseCase = new SettingsUseCase(value.Presenter as ISettingsOutput, SettingsFactory.CreateAudioSettings(), SettingsFactory.CreateLanguageSettings(), SettingsFactory.CreateGameSettings(), Global.Instance.Analytics);
		value.Controller = new SettingsController(value.UseCase as ISettingsInput);
		_pucContainers[typeof(ISettingsView)] = value;
		value = new PUCContainer
		{
			Presenter = new CreditsPresenter(_router)
		};
		value.UseCase = new CreditsUseCase(value.Presenter as ICreditsOutput);
		value.Controller = new CreditsController(value.UseCase as ICreditsInput);
		_pucContainers[typeof(ICreditsView)] = value;
		value = new PUCContainer
		{
			Presenter = new CreateOfflineGamePresenter(_router, _localization)
		};
		value.UseCase = new CreateOfflineGameUseCase(value.Presenter as ICreateOfflineGameOutput, _repositoryFactory, _gameHistory, Global.Instance.Analytics, SettingsFactory.CreateGameSettings());
		value.Controller = new CreateOfflineGameController(value.UseCase as ICreateOfflineGameInput);
		_pucContainers[typeof(ICreateOfflineGameView)] = value;
		value = new PUCContainer
		{
			Presenter = new TutorialPresenter(_router)
		};
		value.UseCase = new TutorialUseCase(value.Presenter as ITutorialOutput, _repositoryFactory, _gameHistory);
		value.Controller = new TutorialController(value.UseCase as ITutorialInput);
		_pucContainers[typeof(ITutorialView)] = value;
		value = new PUCContainer
		{
			Presenter = new ListPlayerOnlineGamesMenuPresenter(_router, _localization, Global.Instance.MainThreadDispatcher)
		};
		value.UseCase = new ListPlayerOnlineGamesMenuUseCase(value.Presenter as IOnlineGameMenuOutput, _repositoryFactory, _gameHistory, _networkBackendClient, NetworkFactory.CreateNetworkCredentialsStorage(), SettingsFactory.CreateNetworkFilterSettings());
		value.Controller = new ListPlayerOnlineGamesMenuController(value.UseCase as IListPlayerOnlineGamesMenuInput);
		_pucContainers[typeof(IListPlayerOnlineGamesMenuView)] = value;
		value = new PUCContainer
		{
			Presenter = new CreateOnlineGamePresenter(_router, _localization, Global.Instance.MainThreadDispatcher)
		};
		value.UseCase = new CreateOnlineGameUseCase(value.Presenter as ICreateOnlineGameOutput, _networkBackendClient, _gameHistory);
		value.Controller = new CreateOnlineGameController(value.UseCase as ICreateOnlineGameInput);
		_pucContainers[typeof(ICreateOnlineGameView)] = value;
		value = new PUCContainer
		{
			Presenter = new JoinOnlineGamePresenter(_router, _localization, Global.Instance.MainThreadDispatcher)
		};
		value.UseCase = new JoinOnlineGameUseCase(value.Presenter as IJoinOnlineGameOutput, _networkBackendClient, _gameHistory);
		value.Controller = new JoinOnlineGameController(value.UseCase as IJoinOnlineGameInput);
		_pucContainers[typeof(IJoinOnlineGameView)] = value;
		value = new PUCContainer
		{
			Presenter = new LoginPresenter(_router, _localization)
		};
		value.UseCase = new LoginUseCase(value.Presenter as ILoginOutput, _networkBackendClient, NetworkFactory.CreateNetworkCredentialsStorage());
		value.Controller = new LoginController(value.UseCase as ILoginInput);
		_pucContainers[typeof(ILoginView)] = value;
		value = new PUCContainer
		{
			Presenter = new RegisterPresenter(_router, _localization)
		};
		value.UseCase = new RegisterUseCase(value.Presenter as IRegisterOutput, _networkBackendClient);
		value.Controller = new RegisterController(value.UseCase as IRegisterInput);
		_pucContainers[typeof(IRegisterView)] = value;
		value = new PUCContainer
		{
			Presenter = new PasswordRecoveryPresenter(_router, _localization)
		};
		value.UseCase = new PasswordRecoveryUseCase(value.Presenter as IPasswordRecoveryOutput, _networkBackendClient);
		value.Controller = new PasswordRecoveryController(value.UseCase as IPasswordRecoveryInput);
		_pucContainers[typeof(IPasswordRecoveryView)] = value;
		value = new PUCContainer
		{
			Presenter = new ChoosePortraitAndColorPresenter(_router)
		};
		value.UseCase = new ChoosePortraitAndColorUseCase(value.Presenter as IChoosePortraitAndColorOutput);
		value.Controller = new ChoosePortraitAndColorController(value.UseCase as IChoosePortraitAndColorInput);
		_pucContainers[typeof(IChoosePortraitAndColorView)] = value;
		value = new PUCContainer
		{
			Presenter = new ChoosePortraitAndJoinPresenter(_router)
		};
		value.UseCase = new ChoosePortraitAndJoinUseCase(value.Presenter as IChoosePortraitAndJoinOutput);
		value.Controller = new ChoosePortraitAndJoinController(value.UseCase as IChoosePortraitAndJoinInput);
		_pucContainers[typeof(IChoosePortraitAndJoinView)] = value;
		return _pucContainers[typeof(ISplashScreenView)].Controller as SplashScreenController;
	}

	private GameSceneController CreateGameUseCases()
	{
		PUCContainer value = new PUCContainer
		{
			Presenter = new GameScenePresenter(_router, _localization)
		};
		value.UseCase = new GameSceneUseCase(value.Presenter as IGameSceneOutput, SettingsFactory.CreateGameSettings());
		value.Controller = new GameSceneController(value.UseCase as IGameSceneInput);
		_pucContainers[typeof(IGameSceneView)] = value;
		value = new PUCContainer
		{
			Presenter = new SaveGamePresenter(_router, Global.Instance.MainThreadDispatcher)
		};
		value.UseCase = new SaveGameUseCase(value.Presenter as ISaveGameOutput, _gameHistory.Repository);
		value.Controller = new SaveGameController(value.UseCase as ISaveGameInput);
		_pucContainers[typeof(ISaveGameView)] = value;
		value = new PUCContainer
		{
			Presenter = new HandOfCardsPresenter(_router, _localization)
		};
		value.UseCase = new HandOfCardsUseCase(value.Presenter as IHandOfCardsOutput, _gameHistory);
		value.Controller = new HandOfCardsController(value.UseCase as IHandOfCardsInput);
		_pucContainers[typeof(IHandOfCardsView)] = value;
		value = new PUCContainer
		{
			Presenter = new PlayerActionsPresenter(_router, _localization)
		};
		value.UseCase = new PlayerActionsUseCase(value.Presenter as IPlayerActionsOutput, _gameHistory);
		value.Controller = new PlayerActionsController(value.UseCase as IPlayerActionsInput);
		_pucContainers[typeof(IPlayerActionsView)] = value;
		value = new PUCContainer
		{
			Presenter = new PlayersSummaryPresenter(_router, _localization)
		};
		value.UseCase = new PlayersSummaryUseCase(value.Presenter as IPlayersSummaryOutput, _gameHistory);
		value.Controller = new PlayersSummaryController(value.UseCase as IPlayersSummaryInput);
		_pucContainers[typeof(IPlayersSummaryView)] = value;
		value = new PUCContainer
		{
			Presenter = new IndustriesSummaryPresenter(_router, _localization)
		};
		value.UseCase = new IndustriesSummaryUseCase(value.Presenter as IIndustriesSummaryOutput, _gameHistory);
		value.Controller = new IndustriesSummaryController(value.UseCase as IIndustriesSummaryInput);
		_pucContainers[typeof(IIndustriesSummaryView)] = value;
		value = new PUCContainer
		{
			Presenter = new IndustriesDetailsPresenter(_router, _localization)
		};
		value.UseCase = new IndustriesDetailsUseCase(value.Presenter as IIndustriesDetailsOutput, _gameHistory);
		value.Controller = new IndustriesDetailsController(value.UseCase as IIndustriesDetailsInput);
		_pucContainers[typeof(IIndustriesDetailsView)] = value;
		value = new PUCContainer
		{
			Presenter = new GameProgressDetailsPresenter(_router, _localization)
		};
		value.UseCase = new GameProgressDetailsUseCase(value.Presenter as IGameProgressDetailsOutput, _gameHistory);
		value.Controller = new GameProgressDetailsController(value.UseCase as IGameProgressDetailsInput);
		_pucContainers[typeof(IGameProgressDetailsView)] = value;
		value = new PUCContainer
		{
			Presenter = new GameLogPresenter(_router, _localization, _gameHistory)
		};
		value.UseCase = new GameLogUseCase(value.Presenter as IGameLogOutput, _gameHistory.Repository, _gameHistory);
		value.Controller = new GameLogController(value.UseCase as IGameLogInput);
		_pucContainers[typeof(IGameLogView)] = value;
		value = new PUCContainer
		{
			Presenter = new MarketPresenter(_router, _localization)
		};
		value.UseCase = new MarketUseCase(value.Presenter as IMarketOutput, _gameHistory);
		value.Controller = new MarketController(value.UseCase as IMarketInput);
		_pucContainers[typeof(IMarketView)] = value;
		value = new PUCContainer
		{
			Presenter = new BoardPresenter(_router, _localization)
		};
		value.UseCase = new BoardUseCase(value.Presenter as IBoardOutput, _gameHistory, SettingsFactory.CreateGameSettings());
		value.Controller = new BoardController(value.UseCase as IBoardInput);
		_pucContainers[typeof(IBoardView)] = value;
		value = new PUCContainer
		{
			Presenter = new BuildActionPresenter(_router, _localization)
		};
		value.UseCase = new BuildActionUseCase(value.Presenter as IBuildActionOutput, _gameHistory);
		value.Controller = new BuildActionController(value.UseCase as IBuildActionInput);
		_pucContainers[typeof(IBuildActionView)] = value;
		value = new PUCContainer
		{
			Presenter = new PassActionPresenter(_router, _localization)
		};
		value.UseCase = new PassActionUseCase(value.Presenter as IPassActionOutput, _gameHistory);
		value.Controller = new PassActionController(value.UseCase as IPassActionInput);
		_pucContainers[typeof(IPassActionOutput)] = value;
		value = new PUCContainer
		{
			Presenter = new LoanActionPresenter(_router, _localization)
		};
		value.UseCase = new LoanActionUseCase(value.Presenter as ILoanActionOutput, _gameHistory);
		value.Controller = new LoanActionController(value.UseCase as ILoanActionInput);
		_pucContainers[typeof(ILoanActionView)] = value;
		value = new PUCContainer
		{
			Presenter = new DevelopActionPresenter(_router, _localization)
		};
		value.UseCase = new DevelopActionUseCase(value.Presenter as IDevelopActionOutput, _gameHistory);
		value.Controller = new DevelopActionController(value.UseCase as IDevelopActionInput);
		_pucContainers[typeof(IDevelopActionView)] = value;
		value = new PUCContainer
		{
			Presenter = new NetworkActionPresenter(_router, _localization)
		};
		value.UseCase = new NetworkActionUseCase(value.Presenter as INetworkActionOutput, _gameHistory);
		value.Controller = new NetworkActionController(value.UseCase as INetworkActionInput);
		_pucContainers[typeof(INetworkActionView)] = value;
		value = new PUCContainer
		{
			Presenter = new SellActionPresenter(_router, _localization)
		};
		value.UseCase = new SellActionUseCase(value.Presenter as ISellActionOutput, _gameHistory);
		value.Controller = new SellActionController(value.UseCase as ISellActionInput);
		_pucContainers[typeof(ISellActionView)] = value;
		value = new PUCContainer
		{
			Presenter = new ScoutActionPresenter(_router, _localization)
		};
		value.UseCase = new ScoutActionUseCase(value.Presenter as IScoutActionOutput, _gameHistory);
		value.Controller = new ScoutActionController(value.UseCase as IScoutActionInput);
		_pucContainers[typeof(IScoutActionView)] = value;
		value = new PUCContainer
		{
			Presenter = new RegionDetailsPresenter(_router)
		};
		value.UseCase = new RegionDetailsUseCase(value.Presenter as IRegionDetailsOutput, _gameHistory);
		value.Controller = new RegionDetailsController(value.UseCase as IRegionDetailsInput);
		_pucContainers[typeof(IRegionDetailsView)] = value;
		value = new PUCContainer
		{
			Presenter = new ActionInProgressPresenter(_router, _localization)
		};
		value.UseCase = new ActionInProgressUseCase(value.Presenter as IActionInProgressOutput);
		value.Controller = new ActionInProgressController(value.UseCase as IActionInProgressInput);
		_pucContainers[typeof(IActionInProgressView)] = value;
		value = new PUCContainer
		{
			Presenter = new GameProgressSummaryPresenter(_router, _localization)
		};
		value.UseCase = new GameProgressSummaryUseCase(value.Presenter as IGameProgressSummaryOutput, _gameHistory);
		value.Controller = new GameProgressSummaryController(value.UseCase as IGameProgressSummaryInput);
		_pucContainers[typeof(IGameProgressSummaryView)] = value;
		value = new PUCContainer
		{
			Presenter = new PlayGamePresenter(_router)
		};
		value.UseCase = new PlayGameUseCase(value.Presenter as IPlayGameOutput, _gameHistory, _networkBackendClient, Global.Instance.Analytics);
		value.Controller = new PlayGameController(value.UseCase as IPlayGameInput);
		_pucContainers[typeof(IPlayGameView)] = value;
		value = new PUCContainer
		{
			Presenter = new PlayAITurnPresenter(_router)
		};
		value.UseCase = new PlayAiTurnUseCase(value.Presenter as IPlayAITurnOutput, _gameHistory);
		value.Controller = new PlayAiTurnController(value.UseCase as IPlayAITurnInput);
		_pucContainers[typeof(IPlayAiTurnView)] = value;
		value = new PUCContainer
		{
			Presenter = new PlayHumanTurnPresenter(_router)
		};
		value.UseCase = new PlayHumanTurnUseCase(value.Presenter as IPlayHumanTurnOutput, _gameHistory);
		value.Controller = new PlayHumanTurnController(value.UseCase as IPlayHumanTurnInput);
		_pucContainers[typeof(IPlayHumanTurnView)] = value;
		value = new PUCContainer
		{
			Presenter = new PlaySpectatorTurnPresenter(_router, Global.Instance.MainThreadDispatcher)
		};
		value.UseCase = new PlaySpectatorTurnUseCase(value.Presenter as IPlaySpectatorTurnOutput, _gameHistory, _networkBackendClient);
		value.Controller = new PlaySpectatorTurnController(value.UseCase as IPlaySpectatorTurnInput);
		_pucContainers[typeof(IPlaySpectatorTurnView)] = value;
		value = new PUCContainer
		{
			Presenter = new FinishedGameReviewPresenter(_router)
		};
		value.UseCase = new FinishedGameReviewUseCase(value.Presenter as IFinishedGameReviewOutput, _gameHistory);
		value.Controller = new FinishedGameReviewController(value.UseCase as IFinishedGameReviewInput);
		_pucContainers[typeof(IFinishedGameReviewView)] = value;
		value = new PUCContainer
		{
			Presenter = new ReplayPresenter(_router, Global.Instance.MainThreadDispatcher, _gameHistory, _localization)
		};
		value.UseCase = new ReplayUseCase(value.Presenter as IReplayOutput, _gameHistory);
		value.Controller = new ReplayController(value.UseCase as IReplayInput);
		_pucContainers[typeof(IReplayView)] = value;
		value = new PUCContainer
		{
			Presenter = new CoalDeliveryPresenter(_router, _localization)
		};
		value.UseCase = new CoalDeliveryUseCase(value.Presenter as ICoalDeliveryOutput, _gameHistory);
		value.Controller = new CoalDeliveryController(value.UseCase as ICoalDeliveryInput);
		_pucContainers[typeof(ICoalDeliveryOutput)] = value;
		value = new PUCContainer
		{
			Presenter = new IronDeliveryPresenter(_router, _localization)
		};
		value.UseCase = new IronDeliveryUseCase(value.Presenter as IIronDeliveryOutput, _gameHistory);
		value.Controller = new IronDeliveryController(value.UseCase as IIronDeliveryInput);
		_pucContainers[typeof(IIronDeliveryOutput)] = value;
		value = new PUCContainer
		{
			Presenter = new BeerDeliveryPresenter(_router, _localization)
		};
		value.UseCase = new BeerDeliveryUseCase(value.Presenter as IBeerDeliveryOutput, _gameHistory);
		value.Controller = new BeerDeliveryController(value.UseCase as IBeerDeliveryInput);
		_pucContainers[typeof(IBeerDeliveryOutput)] = value;
		value = new PUCContainer
		{
			Presenter = new SellIndustryFlowPresenter(_router)
		};
		value.UseCase = new SellIndustryFlowUseCase(value.Presenter as ISellIndustryFlowOutput, _gameHistory);
		value.Controller = new SellIndustryFlowController(value.UseCase as ISellIndustryFlowInput);
		_pucContainers[typeof(ISellIndustryFlowOutput)] = value;
		value = new PUCContainer
		{
			Presenter = new EndOfEraPresenter(_router, _localization)
		};
		value.UseCase = new EndOfEraUseCase(value.Presenter as IEndOfEraOutput, _gameHistory);
		value.Controller = new EndOfEraController(value.UseCase as IEndOfEraInput);
		_pucContainers[typeof(IEndOfEraView)] = value;
		value = new PUCContainer
		{
			Presenter = new ExitToMenuPresenter(_router, _localization)
		};
		value.UseCase = new ExitToMenuUseCase(value.Presenter as IExitToMenuOutput, _gameHistory, SettingsFactory.CreateAudioSettings(), SettingsFactory.CreateLanguageSettings(), SettingsFactory.CreateGameSettings(), Global.Instance.Analytics, _serializer);
		value.Controller = new ExitToMenuController(value.UseCase as IExitToMenuInput);
		_pucContainers[typeof(IExitToMenuView)] = value;
		return _pucContainers[typeof(IGameSceneView)].Controller as GameSceneController;
	}

	private GameSceneController CreateTutorialUseCases()
	{
		PUCContainer value = _pucContainers[typeof(IGameSceneView)];
		PUCContainer value2 = new PUCContainer
		{
			Presenter = value.Presenter,
			UseCase = value.UseCase,
			Controller = value.Controller
		};
		_pucContainersArchive.Add(typeof(IGameSceneView), value2);
		value.Presenter = new TutorialGameScenePresenter(_router, _localization);
		value.UseCase = new TutorialGameSceneUseCase(value.Presenter as ITutorialGameSceneOutput, SettingsFactory.CreateGameSettings(), _gameHistory, Global.Instance.Analytics);
		value.Controller = new GameSceneController(value.UseCase as ITutorialGameSceneInput);
		_pucContainers[typeof(IGameSceneView)] = value;
		value = _pucContainers[typeof(IHandOfCardsView)];
		value2 = new PUCContainer
		{
			Presenter = value.Presenter,
			UseCase = value.UseCase,
			Controller = value.Controller
		};
		_pucContainersArchive.Add(typeof(IHandOfCardsView), value2);
		value.Presenter = new TutorialHandOfCardsPresenter(_router, _localization);
		value.UseCase = new TutorialHandOfCardsUseCase(value.Presenter as ITutorialHandOfCardsOutput, _gameHistory);
		value.Controller = new HandOfCardsController(value.UseCase as ITutorialHandOfCardsInput);
		_pucContainers[typeof(IHandOfCardsView)] = value;
		value = _pucContainers[typeof(IPlayersSummaryView)];
		value2 = new PUCContainer
		{
			Presenter = value.Presenter,
			UseCase = value.UseCase,
			Controller = value.Controller
		};
		_pucContainersArchive.Add(typeof(IPlayersSummaryView), value2);
		value.Presenter = new TutorialPlayersSummaryPresenter(_router, _localization);
		value.UseCase = new TutorialPlayersSummaryUseCase(value.Presenter as ITutorialPlayersSummaryOutput, _gameHistory);
		value.Controller = new PlayersSummaryController(value.UseCase as ITutorialPlayersSummaryInput);
		_pucContainers[typeof(IPlayersSummaryView)] = value;
		value = _pucContainers[typeof(IBoardView)];
		value2 = new PUCContainer
		{
			Presenter = value.Presenter,
			UseCase = value.UseCase,
			Controller = value.Controller
		};
		_pucContainersArchive.Add(typeof(IBoardView), value2);
		value.Presenter = new TutorialBoardPresenter(_router, _localization);
		value.UseCase = new TutorialBoardUseCase(value.Presenter as ITutorialBoardOutput, _gameHistory, SettingsFactory.CreateGameSettings());
		value.Controller = new BoardController(value.UseCase as ITutorialBoardInput);
		_pucContainers[typeof(IBoardView)] = value;
		value = _pucContainers[typeof(IReplayView)];
		value2 = new PUCContainer
		{
			Presenter = value.Presenter,
			UseCase = value.UseCase,
			Controller = value.Controller
		};
		_pucContainersArchive.Add(typeof(IReplayView), value2);
		value.Presenter = new ReplayPresenter(_router, Global.Instance.MainThreadDispatcher, _gameHistory, _localization);
		value.UseCase = new TutorialReplayUseCase(value.Presenter as IReplayOutput, _gameHistory);
		value.Controller = new ReplayController(value.UseCase as IReplayInput);
		_pucContainers[typeof(IReplayView)] = value;
		value = _pucContainers[typeof(INetworkActionView)];
		value2 = new PUCContainer
		{
			Presenter = value.Presenter,
			UseCase = value.UseCase,
			Controller = value.Controller
		};
		_pucContainersArchive.Add(typeof(INetworkActionView), value2);
		value.Presenter = new NetworkActionPresenter(_router, _localization);
		value.UseCase = new TutorialNetworkActionUseCase(value.Presenter as INetworkActionOutput, _gameHistory);
		value.Controller = new NetworkActionController(value.UseCase as INetworkActionInput);
		_pucContainers[typeof(INetworkActionView)] = value;
		value = _pucContainers[typeof(ISellActionView)];
		value2 = new PUCContainer
		{
			Presenter = value.Presenter,
			UseCase = value.UseCase,
			Controller = value.Controller
		};
		_pucContainersArchive.Add(typeof(ISellActionView), value2);
		value.Presenter = new TutorialSellActionPresenter(_router, _localization);
		value.UseCase = new TutorialSellActionUseCase(value.Presenter as ITutorialSellActionOutput, _gameHistory);
		value.Controller = new SellActionController(value.UseCase as ISellActionInput);
		_pucContainers[typeof(ISellActionView)] = value;
		value = _pucContainers[typeof(IDevelopActionView)];
		value2 = new PUCContainer
		{
			Presenter = value.Presenter,
			UseCase = value.UseCase,
			Controller = value.Controller
		};
		_pucContainersArchive.Add(typeof(IDevelopActionView), value2);
		value.Presenter = new TutorialDevelopActionPresenter(_router, _localization);
		value.UseCase = new TutorialDevelopActionUseCase(value.Presenter as ITutorialDevelopActionOutput, _gameHistory);
		value.Controller = new DevelopActionController(value.UseCase as IDevelopActionInput);
		_pucContainers[typeof(IDevelopActionView)] = value;
		value = _pucContainers[typeof(IPlayerActionsView)];
		value2 = new PUCContainer
		{
			Presenter = value.Presenter,
			UseCase = value.UseCase,
			Controller = value.Controller
		};
		_pucContainersArchive.Add(typeof(IPlayerActionsView), value2);
		value.Presenter = new TutorialPlayerActionsPresenter(_router, _localization);
		value.UseCase = new TutorialPlayerActionsUseCase(value.Presenter as ITutorialPlayerActionsOutput, _gameHistory);
		value.Controller = new PlayerActionsController(value.UseCase as ITutorialPlayerActionsInput);
		_pucContainers[typeof(IPlayerActionsView)] = value;
		return _pucContainers[typeof(IGameSceneView)].Controller as GameSceneController;
	}

	public void RestoreUseCases()
	{
		PUCContainer value = _pucContainers[typeof(IGameSceneView)];
		PUCContainer pUCContainer = _pucContainersArchive[typeof(IGameSceneView)];
		value.Presenter = pUCContainer.Presenter;
		value.UseCase = pUCContainer.UseCase;
		value.Controller = pUCContainer.Controller;
		_pucContainers[typeof(IGameSceneView)] = value;
		value = _pucContainers[typeof(IHandOfCardsView)];
		pUCContainer = _pucContainersArchive[typeof(IHandOfCardsView)];
		value.Presenter = pUCContainer.Presenter;
		value.UseCase = pUCContainer.UseCase;
		((HandOfCardsController)value.Controller).RestoreUseCase(value.UseCase as IHandOfCardsInput);
		_pucContainers[typeof(IHandOfCardsView)] = value;
	}

	public void RestoreUseCaseOfType(Type useCaseViewInterfaceType)
	{
		PUCContainer value = _pucContainers[useCaseViewInterfaceType];
		PUCContainer pUCContainer = _pucContainersArchive[useCaseViewInterfaceType];
		value.Presenter = pUCContainer.Presenter;
		value.UseCase = pUCContainer.UseCase;
		value.Controller = pUCContainer.Controller;
		_pucContainers[useCaseViewInterfaceType] = value;
	}

	private void SetupEditorGame()
	{
		if (_gameHistory.CurrentGame == null)
		{
			IRepository repository = _repositoryFactory.CreateLocalGameRepository();
			_gameHistory.Repository = repository;
			GameStartConfiguration gameStartConfiguration = new GameStartConfiguration(NumberOfPlayers.Four);
			GameMetadata gameMetadata = new GameMetadata(gameStartConfiguration, GameType.Local);
			BrassApplicationGame brassApplicationGame = repository.Create(gameMetadata);
			IList<PlayerAvatar> list = Enum.GetValues(typeof(PlayerAvatar)).Cast<PlayerAvatar>().ToList()
				.GenerateRandomElements(4);
			for (int i = 0; i < gameStartConfiguration.Players.Count; i++)
			{
				PlayerStartConfiguration playerStartConfiguration = gameStartConfiguration.Players[i];
				PlayerMetadata playerMetadata = new PlayerMetadata
				{
					GameId = brassApplicationGame.Metadata.GameId,
					NickName = $"Player {playerStartConfiguration.Color}",
					PlayerStartConfiguration = playerStartConfiguration,
					PlayerNetworkId = "Player",
					PlayerAvatar = list[i],
					AiType = ((i != 0) ? AIType.Hard : AIType.Human)
				};
				repository.JoinGame(brassApplicationGame.Metadata.GameId, null, playerMetadata);
			}
			brassApplicationGame = repository.Load(brassApplicationGame.Metadata.GameId);
			brassApplicationGame.Metadata.PlayerMetadata.ForEach(delegate(PlayerMetadata pm)
			{
				pm.AIBehaviour = AiBehaviorFactory.CreateAiBehavior(pm);
			});
			brassApplicationGame.Game.StartGame();
			brassApplicationGame.Metadata.PlayerMetadata.ForEach(delegate(PlayerMetadata pm)
			{
				pm.LastSeenEventRevision = 1;
			});
			repository.Save(brassApplicationGame);
			_gameHistory.SetGame(brassApplicationGame);
		}
	}

	public IController GetController(Type typeOfView)
	{
		IController controller = (_pucContainers.ContainsKey(typeOfView) ? _pucContainers[typeOfView].Controller : null);
		Type[] interfaces = typeOfView.GetInterfaces();
		if (controller == null && interfaces.Length != 0)
		{
			for (int i = 0; i < interfaces.Length; i++)
			{
				if (_pucContainers.ContainsKey(interfaces[i]))
				{
					controller = _pucContainers[interfaces[i]].Controller;
					break;
				}
			}
		}
		return controller;
	}

	public TUseCase GetUseCase<TUseCase>() where TUseCase : IUseCase
	{
		return (TUseCase)_pucContainers.FirstOrDefault((KeyValuePair<Type, PUCContainer> c) => c.Value.UseCase is TUseCase).Value.UseCase;
	}

	public IPresenter GetPresenter(Type typeOfView)
	{
		if (!_pucContainers.ContainsKey(typeOfView))
		{
			return null;
		}
		return _pucContainers[typeOfView].Presenter;
	}
}
