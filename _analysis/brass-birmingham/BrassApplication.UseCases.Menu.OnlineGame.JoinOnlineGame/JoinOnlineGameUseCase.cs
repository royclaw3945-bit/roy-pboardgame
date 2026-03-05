using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoardGameRules.Entities.Players;
using BoardGameRules.Utils.Logging;
using BrassApplication.AI;
using BrassApplication.Game;
using BrassApplication.GameHistory;
using BrassApplication.Network;
using BrassApplication.Persistence.RepositoryBackends;
using BrassApplication.UseCases.Common;
using BrassApplication.Utils;

namespace BrassApplication.UseCases.Menu.OnlineGame.JoinOnlineGame;

public class JoinOnlineGameUseCase : IUseCase, IJoinOnlineGameInput
{
	private readonly IJoinOnlineGameOutput _presenter;

	private readonly INetworkBackendClient _networkBackendClient;

	private GameMetadata _gameMetadata;

	private PlayerAvatar _playerAvatar;

	private readonly IGameHistory _gameHistory;

	private static readonly ILog Logger = LogFactory.BuildLogger(typeof(JoinOnlineGameUseCase));

	public JoinOnlineGameUseCase(IJoinOnlineGameOutput presenter, INetworkBackendClient networkBackendClient, IGameHistory gameHistory)
	{
		_presenter = presenter;
		_networkBackendClient = networkBackendClient;
		_gameHistory = gameHistory;
	}

	public void StartJoinProcedure(GameMetadata gameMetadata)
	{
		_gameMetadata = gameMetadata;
		UpdateView();
	}

	public void ShowChoosePortraitAndJoinView(int slot)
	{
		List<PlayerAvatar?> takenAvatars = _gameMetadata.PlayerMetadata.Select((PlayerMetadata pm) => pm?.PlayerAvatar).ToList();
		IEnumerable<PlayerAvatar> availableAvatars = ((PlayerAvatar[])Enum.GetValues(typeof(PlayerAvatar))).Where((PlayerAvatar a) => !takenAvatars.Contains(a));
		_presenter.ShowChoosePortraitAndJoinView(JoinGameClicked, availableAvatars, slot, _gameMetadata.GameStartConfiguration.Players[slot].Color);
	}

	public void JoinGameClicked(int slot, PlayerAvatar playerAvatar)
	{
		_playerAvatar = playerAvatar;
		if (_gameMetadata.NetworkGameStartConfiguration.IsPrivate)
		{
			_presenter.ShowPasswordView(JoinGame, slot);
		}
		else
		{
			JoinGame(slot, null);
		}
	}

	public void JoinGame(int slot, string password)
	{
		PlayerMetadata playerMetadata = new PlayerMetadata
		{
			AiType = AIType.Human,
			GameId = _gameMetadata.GameId,
			PlayerNetworkId = _networkBackendClient.UserManagement.UserId,
			NickName = _networkBackendClient.UserManagement.UserNickname,
			PlayerStartConfiguration = _gameMetadata.GameStartConfiguration.Players[slot],
			PlayerAvatar = _playerAvatar
		};
		_gameMetadata.PlayerMetadata[slot] = playerMetadata;
		_presenter.ShowConnectionView();
		_gameHistory.Repository.Async.JoinGameAsync(_gameMetadata.GameId, password, playerMetadata).ContinueWith(delegate(Task task)
		{
			if (task.Status.Equals(TaskStatus.RanToCompletion))
			{
				UpdateView();
				if (_gameMetadata.GameStartConfiguration.NumberOfPlayersAsInt == _gameMetadata.PlayerMetadata.Count((PlayerMetadata p) => p != null))
				{
					StartGameOnServer();
				}
				else
				{
					_presenter.HideConnectionView();
					_presenter.ShowInfoViewGameJoinSuccessful();
				}
			}
			else
			{
				_presenter.HideConnectionView();
				_gameMetadata.PlayerMetadata[slot] = null;
				if (task.Exception?.InnerException != null)
				{
					_presenter.ShowInfoViewWithError(ExceptionToTranslationKey.ConvertExceptionToTranslationKey(task.Exception.InnerException));
				}
			}
		});
	}

	private void StartGameOnServer()
	{
		if (_gameHistory.Repository.Advanced.GameStorage is ICachedGameStorage cachedGameStorage)
		{
			cachedGameStorage.FlushCaches();
		}
		_gameHistory.Repository.Async.LoadAsync(_gameMetadata.GameId).ContinueWith(delegate(Task<BrassApplicationGame> taskLoad)
		{
			if (taskLoad.Status.Equals(TaskStatus.RanToCompletion))
			{
				BrassApplicationGame result = taskLoad.Result;
				result.Game.StartGame();
				result.Metadata.PlayerMetadata.ForEach(delegate(PlayerMetadata pm)
				{
					pm.LastSeenEventRevision = 1;
				});
				_gameHistory.Repository.Async.SaveAsync(result).ContinueWith(delegate(Task taskSave)
				{
					_presenter.HideConnectionView();
					if (taskSave.Status.Equals(TaskStatus.RanToCompletion))
					{
						Logger.Debug("Online Game Started");
						_presenter.ShowInfoViewGameJoinSuccessful();
					}
					else if (taskSave.Exception?.InnerException != null)
					{
						_presenter.ShowInfoViewWithError(ExceptionToTranslationKey.ConvertExceptionToTranslationKey(taskSave.Exception.InnerException));
					}
				});
			}
			else
			{
				_presenter.HideConnectionView();
				if (taskLoad.Exception?.InnerException != null)
				{
					_presenter.ShowInfoViewWithError(ExceptionToTranslationKey.ConvertExceptionToTranslationKey(taskLoad.Exception.InnerException));
				}
			}
		});
	}

	public void BackClicked()
	{
		_presenter.HideView();
	}

	private void UpdateView()
	{
		_presenter.PresentGameConfiguration(_gameMetadata, ShowChoosePortraitAndJoinView);
	}
}
