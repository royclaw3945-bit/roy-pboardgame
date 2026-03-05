using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BoardGameRules.Utils.Logging;
using BrassApplication.ApplicationSettings;
using BrassApplication.Game;
using BrassApplication.GameHistory;
using BrassApplication.Network;
using BrassApplication.Persistence.Repository;
using BrassApplication.UseCases.Common;
using BrassApplication.Utils;

namespace BrassApplication.UseCases.Menu.OnlineGame.ListPlayerOnlineGamesMenu;

public class ListPlayerOnlineGamesMenuUseCase : IUseCase, IListPlayerOnlineGamesMenuInput
{
	private readonly IOnlineGameMenuOutput _presenter;

	private readonly IRepositoryFactory _repositoryFactory;

	private readonly IGameHistory _gameHistory;

	private readonly INetworkBackendClient _networkBackendClient;

	private readonly INetworkCredentialsStorage _networkCredentialsStorage;

	private readonly INetworkFilterSettings _networkFilterSettings;

	private static readonly ILog Logger = LogFactory.BuildLogger(typeof(ListPlayerOnlineGamesMenuUseCase));

	private ReadOnlyCollection<GameMetadata> _joinableGames;

	private (bool show2Players, bool show3Players, bool show4Players, bool showPrivate) _loadFilterSettings;

	private bool _continueRefreshing;

	private CancellationTokenSource _tokenSource;

	public ListPlayerOnlineGamesMenuUseCase(IOnlineGameMenuOutput presenter, IRepositoryFactory repositoryFactory, IGameHistory gameHistory, INetworkBackendClient networkBackendClient, INetworkCredentialsStorage networkCredentialsStorage, INetworkFilterSettings networkFilterSettings)
	{
		_presenter = presenter;
		_repositoryFactory = repositoryFactory;
		_gameHistory = gameHistory;
		_networkBackendClient = networkBackendClient;
		_networkCredentialsStorage = networkCredentialsStorage;
		_networkFilterSettings = networkFilterSettings;
		_loadFilterSettings = LoadFilterSettings();
		_gameHistory.Repository = _repositoryFactory.CreateOnlineGameRepository();
	}

	public void OnViewCreated()
	{
		StartPeriodicViewUpdate();
	}

	public void OnViewDestroyed()
	{
		StopPeriodicViewUpdate();
	}

	private void StartPeriodicViewUpdate()
	{
		Logger.Debug("Start refreshing games");
		_continueRefreshing = true;
		_tokenSource = new CancellationTokenSource();
		Task.Run(delegate
		{
			while (_continueRefreshing)
			{
				Logger.Debug("Refreshing games");
				UpdateView();
				if (_tokenSource.Token.WaitHandle.WaitOne(15000))
				{
					break;
				}
			}
			Logger.Debug("Refreshing games thread stopped");
		}, _tokenSource.Token);
	}

	private void StopPeriodicViewUpdate()
	{
		Logger.Debug("Stop refreshing games");
		_continueRefreshing = false;
		_tokenSource.Cancel();
	}

	public void UpdateView()
	{
		UpdatePlayerGamesView(_loadFilterSettings.show2Players, _loadFilterSettings.show3Players, _loadFilterSettings.show4Players, _loadFilterSettings.showPrivate);
		UpdateOpenGamesView(_loadFilterSettings.show2Players, _loadFilterSettings.show3Players, _loadFilterSettings.show4Players, _loadFilterSettings.showPrivate);
	}

	public void SetFilters(bool show2Players, bool show3Players, bool show4Players, bool showPrivate)
	{
		_loadFilterSettings = (show2Players: show2Players, show3Players: show3Players, show4Players: show4Players, showPrivate: showPrivate);
		SaveFilterSettings(show2Players, show3Players, show4Players, showPrivate);
		_presenter.UpdateFiltersVisuals(show2Players, show3Players, show4Players, showPrivate);
	}

	private void UpdatePlayerGamesView(bool show2Players, bool show3Players, bool show4Players, bool showPrivate)
	{
		GamesFilter gamesFilter = new GamesFilter
		{
			ShowOnlyGamesThatBelongToPlayerId = _networkBackendClient.UserManagement.UserId,
			TwoPlayers = show2Players,
			ThreePlayers = show3Players,
			FourPlayers = show4Players,
			PrivateGames = showPrivate
		};
		_gameHistory.Repository.Async.GetGamesMetadataAsync(gamesFilter).ContinueWith(delegate(Task<ReadOnlyCollection<GameMetadata>> task)
		{
			ReadOnlyCollection<GameMetadata> result = task.Result;
			if (task.Status.Equals(TaskStatus.RanToCompletion))
			{
				ReadOnlyCollection<GameMetadata> readOnlyCollection = result.Where((GameMetadata metadata) => metadata.PlayerMetadata.Exists((PlayerMetadata pm) => pm?.PlayerNetworkId.Equals(_networkBackendClient.UserManagement.UserId) ?? false)).ToList().AsReadOnly();
				Logger.Debug("allGames = " + result.Count + ", playerGames = " + readOnlyCollection.Count);
				_presenter.UpdatePlayerGamesView(readOnlyCollection, _networkBackendClient.UserManagement.UserNickname, _networkBackendClient.UserManagement.UserId, LaunchGameClicked, LeaveGameClicked);
			}
			else if (task.Status.Equals(TaskStatus.Faulted) && task.Exception?.InnerException != null)
			{
				_presenter.ShowInfoViewWithError(ExceptionToTranslationKey.ConvertExceptionToTranslationKey(task.Exception.InnerException));
			}
		});
	}

	private void UpdateOpenGamesView(bool show2Players, bool show3Players, bool show4Players, bool showPrivate)
	{
		GamesFilter gamesFilter = new GamesFilter
		{
			ShowOnlyOpenGames = true,
			ShowOnlyGamesThatDoNotBelongToPlayerId = _networkBackendClient.UserManagement.UserId,
			TwoPlayers = show2Players,
			ThreePlayers = show3Players,
			FourPlayers = show4Players,
			PrivateGames = showPrivate
		};
		_gameHistory.Repository.Async.GetGamesMetadataAsync(gamesFilter).ContinueWith(delegate(Task<ReadOnlyCollection<GameMetadata>> task)
		{
			ReadOnlyCollection<GameMetadata> result = task.Result;
			if (task.Status.Equals(TaskStatus.RanToCompletion))
			{
				_joinableGames = result.Where((GameMetadata metadata) => !metadata.PlayerMetadata.Exists((PlayerMetadata pm) => pm?.PlayerNetworkId.Equals(_networkBackendClient.UserManagement.UserId) ?? false) && metadata.PlayerMetadata.Any((PlayerMetadata pm) => pm == null) && metadata.PlayerMetadata.Any((PlayerMetadata pm) => pm != null)).ToList().AsReadOnly();
				Logger.Debug("allGames = " + result.Count + ", joinableGames = " + _joinableGames.Count);
				_presenter.UpdateOpenGamesView(_joinableGames, JoinGameClicked);
			}
			else if (task.Status.Equals(TaskStatus.Faulted) && task.Exception?.InnerException != null)
			{
				_presenter.ShowInfoViewWithError(ExceptionToTranslationKey.ConvertExceptionToTranslationKey(task.Exception.InnerException));
			}
		});
	}

	public void LogoutClicked()
	{
		_networkBackendClient.UserManagement.Logout();
		_networkCredentialsStorage.DeleteNetworkCredentials();
		_presenter.ShowLogOutPopup();
	}

	public void CreateGameClicked()
	{
		_presenter.ShowCreateOnlineGameView();
	}

	public async void LeaveGameClicked(string gameId)
	{
		await _gameHistory.Repository.Async.LeaveGameAsync(gameId, _networkBackendClient.UserManagement.UserId, isThisAnAbandonRequest: false);
		UpdateView();
	}

	public void JoinGameClicked(string gameId)
	{
		GameMetadata gameMetadata = _joinableGames.First((GameMetadata gm) => gm.GameId == gameId);
		_presenter.ShowJoinOnlineGameView(gameMetadata);
	}

	private void SaveFilterSettings(bool show2Players, bool show3Players, bool show4Players, bool showPrivate)
	{
		_networkFilterSettings.SetTwoPlayersFilter(show2Players);
		_networkFilterSettings.SetThreePlayersFilter(show3Players);
		_networkFilterSettings.SetFourPlayersFilter(show4Players);
		_networkFilterSettings.SetOnlyPrivateGamesFilter(showPrivate);
	}

	private (bool show2Players, bool show3Players, bool show4Players, bool showPrivate) LoadFilterSettings()
	{
		(bool, bool, bool, bool) result = (false, false, false, false);
		result.Item1 = _networkFilterSettings.GetTwoPlayersFilter();
		result.Item2 = _networkFilterSettings.GetThreePlayersFilter();
		result.Item3 = _networkFilterSettings.GetFourPlayersFilter();
		result.Item4 = _networkFilterSettings.GetOnlyPrivateGamesFilter();
		return result;
	}

	private void LaunchGameClicked(string gameId)
	{
		StopPeriodicViewUpdate();
		_presenter.ShowConnectionView();
		_gameHistory.Repository.Async.LoadAsync(gameId).ContinueWith(delegate(Task<BrassApplicationGame> task)
		{
			_presenter.HideConnectionView();
			if (task.Status.Equals(TaskStatus.RanToCompletion))
			{
				BrassApplicationGame result = task.Result;
				if (result.Game.Revision == 0)
				{
					StartGameOnServer(result);
				}
				else
				{
					_gameHistory.SetGame(result);
					_presenter.LaunchGame(result);
				}
			}
			else if (task.Exception?.InnerException != null)
			{
				_presenter.ShowInfoViewWithError(ExceptionToTranslationKey.ConvertExceptionToTranslationKey(task.Exception.InnerException));
			}
		});
	}

	private void StartGameOnServer(BrassApplicationGame game)
	{
		_presenter.ShowConnectionView();
		game.Game.StartGame();
		game.Metadata.PlayerMetadata.ForEach(delegate(PlayerMetadata pm)
		{
			pm.LastSeenEventRevision = 1;
		});
		_gameHistory.Repository.Async.SaveAsync(game).ContinueWith(delegate(Task task)
		{
			_presenter.HideConnectionView();
			if (task.Status.Equals(TaskStatus.RanToCompletion))
			{
				Logger.Debug("Online Game Started");
				_gameHistory.SetGame(game);
				_presenter.LaunchGame(game);
			}
			else if (task.Status.Equals(TaskStatus.Faulted))
			{
				if (task.Exception?.InnerException != null)
				{
					_presenter.ShowInfoViewWithError(ExceptionToTranslationKey.ConvertExceptionToTranslationKey(task.Exception.InnerException));
				}
			}
			else
			{
				_presenter.ShowInfoViewWithError("FailedToStartOnlineGame");
			}
		});
	}

	public void BackClicked()
	{
		_presenter.HideView();
	}
}
