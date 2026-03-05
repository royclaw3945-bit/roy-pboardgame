using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoardGameRules.Entities.Common;
using BoardGameRules.Entities.Game;
using BoardGameRules.Entities.Players;
using BrassApplication.AI;
using BrassApplication.Game;
using BrassApplication.GameHistory;
using BrassApplication.Network;
using BrassApplication.UseCases.Common;
using BrassApplication.Utils;

namespace BrassApplication.UseCases.Menu.OnlineGame.CreateOnlineGame;

public class CreateOnlineGameUseCase : IUseCase, ICreateOnlineGameInput
{
	private readonly ICreateOnlineGameOutput _presenter;

	private readonly INetworkBackendClient _networkBackendClient;

	private GameMetadata _newGameMetadata;

	private int _numberOfPlayers = 4;

	private string _gameName = "";

	private string _password;

	private int _timePerTurn = 86400;

	private const int MIN = 60;

	private const int HOUR = 3600;

	private const int DAY = 86400;

	private readonly List<int> _possibleTimesPerTurn = new List<int>
	{
		300, 600, 900, 1800, 3600, 7200, 10800, 21600, 43200, 86400,
		172800, 432000, 604800, 1209600, 2592000
	};

	private readonly IGameHistory _gameHistory;

	public CreateOnlineGameUseCase(ICreateOnlineGameOutput presenter, INetworkBackendClient networkBackendClient, IGameHistory gameHistory)
	{
		_presenter = presenter;
		_networkBackendClient = networkBackendClient;
		_gameHistory = gameHistory;
	}

	public void CreateNewGameConfiguration()
	{
		List<PlayerMetadata> list = new List<PlayerMetadata>();
		_numberOfPlayers = 4;
		_gameName = _presenter.GetDefaultGameNameTranslation().Replace("<nickname>", _networkBackendClient.UserManagement.UserNickname);
		_password = "";
		_timePerTurn = 86400;
		list.Add(new PlayerMetadata
		{
			PlayerStartConfiguration = new PlayerStartConfiguration(PlayerColor.Yellow, 0),
			NickName = _networkBackendClient.UserManagement.UserNickname,
			PlayerNetworkId = _networkBackendClient.UserManagement.UserId,
			AiType = AIType.Human,
			PlayerAvatar = PlayerAvatar.Arkwright
		});
		_newGameMetadata = new GameMetadata(new GameStartConfiguration((NumberOfPlayers)_numberOfPlayers), GameType.Network)
		{
			PlayerMetadata = list,
			NetworkGameStartConfiguration = new NetworkGameStartConfiguration
			{
				GameName = _gameName,
				TurnTimeoutInSeconds = _timePerTurn
			},
			NetworkGameProgressInformation = new NetworkGameProgressInformation
			{
				CurrentGameStatus = NetworkGameProgressInformation.GameStatus.Open
			}
		};
		UpdateView();
	}

	public void ChangeTime()
	{
		List<Action> list = new List<Action>();
		foreach (int time in _possibleTimesPerTurn)
		{
			list.Add(delegate
			{
				ChangeTime(time);
			});
		}
		_presenter.ShowChangeTimeView(list);
	}

	public void ChangePassword()
	{
		_presenter.ShowChangePasswordView(ChangePassword, _password);
	}

	public void StartGame()
	{
		_presenter.ShowConnectionView();
		if (_gameName == "")
		{
			_gameName = _presenter.GetDefaultGameNameTranslation().Replace("<nickname>", _networkBackendClient.UserManagement.UserNickname);
			_newGameMetadata.NetworkGameStartConfiguration.GameName = _gameName;
		}
		_gameHistory.Repository.Async.CreateAsync(_newGameMetadata).ContinueWith(delegate(Task<BrassApplicationGame> task)
		{
			_presenter.HideConnectionView();
			if (task.Status.Equals(TaskStatus.RanToCompletion))
			{
				_presenter.ShowInfoViewGameCreateSuccessful();
			}
			else if (task.Exception?.InnerException != null)
			{
				_presenter.ShowInfoViewWithError(ExceptionToTranslationKey.ConvertExceptionToTranslationKey(task.Exception.InnerException));
			}
		});
	}

	public void Back()
	{
		_presenter.HideView();
	}

	public void ChangeGameName(string name)
	{
		_gameName = name;
		_newGameMetadata.NetworkGameStartConfiguration.GameName = _gameName;
		UpdateView();
	}

	private void ChangePassword(string password)
	{
		_password = password;
		_newGameMetadata.NetworkGameStartConfiguration.IsPrivate = !string.IsNullOrEmpty(_password);
		_presenter.LockPasswordIcon(!string.IsNullOrEmpty(_password));
		_newGameMetadata.NetworkGameStartConfiguration.PrivateGamePassword = _password;
		_presenter.HideInputView();
		UpdateView();
	}

	private void ChangeTime(int time)
	{
		_timePerTurn = time;
		_newGameMetadata.NetworkGameStartConfiguration.TurnTimeoutInSeconds = _timePerTurn;
		UpdateView();
	}

	protected void EditPortraitAndColor(PlayerColor playerColor, PlayerAvatar playerAvatar)
	{
		_presenter.ShowChoosePortraitAndColorView(playerColor, playerAvatar);
	}

	public void IncreaseNumberOfPlayers()
	{
		if (_numberOfPlayers < 4)
		{
			_numberOfPlayers++;
			_newGameMetadata.GameStartConfiguration.Players.Add(new PlayerStartConfiguration(AvailableColorsList()[0], _numberOfPlayers - 1));
			UpdateView();
			return;
		}
		throw new InvalidOperationException("Can't have more than 4 players");
	}

	public void DecreaseNumberOfPlayers()
	{
		if (_numberOfPlayers > 2)
		{
			_numberOfPlayers--;
			_newGameMetadata.GameStartConfiguration.Players.RemoveAt(_numberOfPlayers);
			UpdateView();
			return;
		}
		throw new InvalidOperationException("Can't have less than 2 players");
	}

	private List<PlayerColor> AvailableColorsList()
	{
		List<PlayerColor> list = new List<PlayerColor>();
		PlayerColor[] array = (PlayerColor[])Enum.GetValues(typeof(PlayerColor));
		foreach (PlayerColor color in array)
		{
			if (!_newGameMetadata.GameStartConfiguration.Players.Any((PlayerStartConfiguration p) => p.Color == color))
			{
				list.Add(color);
			}
		}
		return list;
	}

	public void NextColorFor(PlayerColor startingColor, PlayerColor playerColor)
	{
		PlayerColor oldColor = playerColor;
		switch (playerColor)
		{
		case PlayerColor.Blue:
			playerColor = PlayerColor.Red;
			break;
		case PlayerColor.Red:
			playerColor = PlayerColor.Violet;
			break;
		case PlayerColor.Violet:
			playerColor = PlayerColor.Yellow;
			break;
		case PlayerColor.Yellow:
			playerColor = PlayerColor.Blue;
			break;
		}
		ChangeColor(startingColor, oldColor, playerColor);
	}

	public void PreviousColorFor(PlayerColor startingColor, PlayerColor playerColor)
	{
		PlayerColor oldColor = playerColor;
		switch (playerColor)
		{
		case PlayerColor.Blue:
			playerColor = PlayerColor.Yellow;
			break;
		case PlayerColor.Red:
			playerColor = PlayerColor.Blue;
			break;
		case PlayerColor.Violet:
			playerColor = PlayerColor.Red;
			break;
		case PlayerColor.Yellow:
			playerColor = PlayerColor.Violet;
			break;
		}
		ChangeColor(startingColor, oldColor, playerColor);
	}

	private void ChangeColor(PlayerColor startingColor, PlayerColor oldColor, PlayerColor newColor)
	{
		PlayerMetadata playerMetadata = _newGameMetadata.PlayerMetadata[0];
		PlayerStartConfiguration playerStartConfiguration = _newGameMetadata.GameStartConfiguration.Players[0];
		if (startingColor != oldColor)
		{
			PlayerStartConfiguration playerStartConfiguration2 = _newGameMetadata.GameStartConfiguration.Players.FirstOrDefault((PlayerStartConfiguration p) => p.Color == startingColor);
			if (playerStartConfiguration2 != null)
			{
				int index = _newGameMetadata.GameStartConfiguration.Players.IndexOf(playerStartConfiguration2);
				_newGameMetadata.GameStartConfiguration.Players[index] = new PlayerStartConfiguration(oldColor, playerStartConfiguration2.SittingOrder);
			}
		}
		PlayerStartConfiguration playerStartConfiguration3 = _newGameMetadata.GameStartConfiguration.Players.FirstOrDefault((PlayerStartConfiguration p) => p.Color == newColor);
		_newGameMetadata.GameStartConfiguration.Players[0] = new PlayerStartConfiguration(newColor, playerStartConfiguration.SittingOrder);
		playerMetadata.PlayerStartConfiguration = _newGameMetadata.GameStartConfiguration.Players[0];
		if (playerStartConfiguration3 != null)
		{
			int index2 = _newGameMetadata.GameStartConfiguration.Players.IndexOf(playerStartConfiguration3);
			_newGameMetadata.GameStartConfiguration.Players[index2] = new PlayerStartConfiguration(startingColor, playerStartConfiguration3.SittingOrder);
		}
		_presenter.UpdateColorInPortraitAndColorView(newColor);
		UpdateView();
	}

	public void NextPortraitFor(PlayerAvatar playerAvatar)
	{
		switch (playerAvatar)
		{
		case PlayerAvatar.Arkwright:
			playerAvatar = PlayerAvatar.Bessemer;
			break;
		case PlayerAvatar.Bessemer:
			playerAvatar = PlayerAvatar.Brunel;
			break;
		case PlayerAvatar.Brunel:
			playerAvatar = PlayerAvatar.Coade;
			break;
		case PlayerAvatar.Coade:
			playerAvatar = PlayerAvatar.Owen;
			break;
		case PlayerAvatar.Owen:
			playerAvatar = PlayerAvatar.Rothschild;
			break;
		case PlayerAvatar.Rothschild:
			playerAvatar = PlayerAvatar.Stephenson;
			break;
		case PlayerAvatar.Stephenson:
			playerAvatar = PlayerAvatar.Tinsley;
			break;
		case PlayerAvatar.Tinsley:
			playerAvatar = PlayerAvatar.Watt;
			break;
		case PlayerAvatar.Watt:
			playerAvatar = PlayerAvatar.Arkwright;
			break;
		}
		ChangeAvatar(playerAvatar);
	}

	public void PreviousPortraitFor(PlayerAvatar startingAvatar, PlayerAvatar playerAvatar)
	{
		switch (playerAvatar)
		{
		case PlayerAvatar.Arkwright:
			playerAvatar = PlayerAvatar.Watt;
			break;
		case PlayerAvatar.Bessemer:
			playerAvatar = PlayerAvatar.Arkwright;
			break;
		case PlayerAvatar.Brunel:
			playerAvatar = PlayerAvatar.Bessemer;
			break;
		case PlayerAvatar.Coade:
			playerAvatar = PlayerAvatar.Brunel;
			break;
		case PlayerAvatar.Owen:
			playerAvatar = PlayerAvatar.Coade;
			break;
		case PlayerAvatar.Rothschild:
			playerAvatar = PlayerAvatar.Owen;
			break;
		case PlayerAvatar.Stephenson:
			playerAvatar = PlayerAvatar.Rothschild;
			break;
		case PlayerAvatar.Tinsley:
			playerAvatar = PlayerAvatar.Stephenson;
			break;
		case PlayerAvatar.Watt:
			playerAvatar = PlayerAvatar.Tinsley;
			break;
		}
		ChangeAvatar(playerAvatar);
	}

	private void ChangeAvatar(PlayerAvatar newAvatar)
	{
		_newGameMetadata.PlayerMetadata[0].PlayerAvatar = newAvatar;
		_presenter.UpdatePortraitInPortraitAndColorView(newAvatar);
		UpdateView();
	}

	private void UpdateView()
	{
		_presenter.PresentGameConfiguration(_newGameMetadata.GameStartConfiguration.Players, _newGameMetadata.PlayerMetadata, _numberOfPlayers, _gameName, _password, _timePerTurn, EditPortraitAndColor, IncreaseNumberOfPlayers, DecreaseNumberOfPlayers);
	}
}
