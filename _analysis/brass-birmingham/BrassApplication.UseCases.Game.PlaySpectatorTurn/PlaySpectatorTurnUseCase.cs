using System.Threading;
using System.Threading.Tasks;
using BoardGameRules.Entities.Players;
using BoardGameRules.Utils.Logging;
using BrassApplication.Game;
using BrassApplication.GameHistory;
using BrassApplication.Network;
using BrassApplication.Persistence.RepositoryBackends;
using BrassApplication.UseCases.Common;
using BrassApplication.UseCases.Game.Replay;

namespace BrassApplication.UseCases.Game.PlaySpectatorTurn;

public class PlaySpectatorTurnUseCase : IUseCase, IPlaySpectatorTurnInput, IReplayDelegate
{
	private readonly ILog Logger = LogFactory.BuildLogger(typeof(PlaySpectatorTurnUseCase));

	private readonly IPlaySpectatorTurnOutput _presenter;

	private readonly IGameHistory _gameHistory;

	private readonly INetworkBackendClient _networkBackendClient;

	private bool _continuePooling;

	private PlayerMetadata _replayWatchingPlayerMetadata;

	private int _lastSeenGameRevision;

	public PlaySpectatorTurnUseCase(IPlaySpectatorTurnOutput presenter, IGameHistory gameHistory, INetworkBackendClient networkBackendClient)
	{
		_presenter = presenter;
		_gameHistory = gameHistory;
		_networkBackendClient = networkBackendClient;
	}

	public void StartSpectatorTurn(PlayerMetadata replayWatchingPlayerMetadata)
	{
		Logger.Debug("Starting Spectator Turn");
		_replayWatchingPlayerMetadata = replayWatchingPlayerMetadata;
		_presenter.ReplayUnseenActions(replayWatchingPlayerMetadata, this);
	}

	public void OnViewDestroyed()
	{
		Logger.Debug("OnViewDestroyed");
		StopPoolingEventsFromServer();
	}

	public void ReplayStarted()
	{
		Logger.Debug("ReplayStarted");
		StopPoolingEventsFromServer();
		_presenter.ShowSkipToLatestTurnView();
	}

	public void ReplayCompleted()
	{
		Logger.Debug("ReplayCompleted");
		Player currentPlayer = _gameHistory.CurrentGame.Game.GameProgressInformation.CurrentPlayer;
		PlayerMetadata playerMetadataByColor = _gameHistory.CurrentGame.Metadata.GetPlayerMetadataByColor(currentPlayer.Color);
		_presenter.ShowWaitingForOnlinePlayerView(playerMetadataByColor);
		_presenter.DisableBuildingFromTheBoard();
		_presenter.ShowSpectatorCards(_replayWatchingPlayerMetadata);
		_presenter.ShowSpectatorIndustries(_replayWatchingPlayerMetadata);
		_lastSeenGameRevision = _gameHistory.CurrentGame.Game.Revision;
		StartPoolingEventsFromServer();
	}

	private void StartPoolingEventsFromServer()
	{
		Logger.Debug("Starting pooling events from server");
		_continuePooling = true;
		Task.Run(delegate
		{
			while (_continuePooling)
			{
				Logger.Debug("Requesting to load game");
				if (_gameHistory.Repository.Advanced.GameStorage is ICachedGameStorage cachedGameStorage)
				{
					cachedGameStorage.FlushCaches();
				}
				BrassApplicationGame brassApplicationGame = _gameHistory.Repository.Load(_gameHistory.CurrentGame.Metadata.GameId);
				Player currentPlayer = brassApplicationGame.Game.GameProgressInformation.CurrentPlayer;
				string playerNetworkId = brassApplicationGame.Metadata.GetPlayerMetadataByColor(currentPlayer.Color).PlayerNetworkId;
				string userId = _networkBackendClient.UserManagement.UserId;
				int revision = brassApplicationGame.Game.Revision;
				Logger.Debug("Current player id = {0}, logged in player id = {1}", playerNetworkId, userId);
				if (playerNetworkId.Equals(userId))
				{
					Logger.Debug("It's logged player turn again!");
					_gameHistory.SetGame(brassApplicationGame);
					StopPoolingEventsFromServer();
					_presenter.EnableBuildingFromTheBoard();
					_presenter.PlayGame();
				}
				else if (revision != _lastSeenGameRevision)
				{
					Logger.Debug("There are new events, replay them");
					StopPoolingEventsFromServer();
					_gameHistory.SetGame(brassApplicationGame);
					_presenter.PlayGame();
				}
				else
				{
					Logger.Debug("Keep waiting for logged player turn.");
					Thread.Sleep(15000);
					Logger.Debug("End Keep waiting for logged player turn.");
				}
			}
			Logger.Debug("Pooling thread stopped");
		});
	}

	private void StopPoolingEventsFromServer()
	{
		Logger.Debug("Stopping pooling events from server");
		_continuePooling = false;
	}
}
