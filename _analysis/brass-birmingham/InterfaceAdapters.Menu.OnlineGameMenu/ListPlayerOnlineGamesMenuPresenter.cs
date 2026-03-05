using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BoardGameRules.Entities.Common;
using BrassApplication.Game;
using BrassApplication.UseCases.Menu.OnlineGame.ListPlayerOnlineGamesMenu;
using InterfaceAdapters.Common;
using InterfaceAdapters.Common.LoadScene;
using InterfaceAdapters.Menu.Connection;
using InterfaceAdapters.Menu.MainMenu;
using InterfaceAdapters.Menu.Online.CreateOnlineGame;
using InterfaceAdapters.Menu.Online.JoinOnlineGame;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Utils;
using InterfaceAdapters.Utils.MainThreadDispatcher;

namespace InterfaceAdapters.Menu.OnlineGameMenu;

public class ListPlayerOnlineGamesMenuPresenter : IPresenter, IOnlineGameMenuOutput
{
	private readonly IRouter _router;

	private readonly ILocalization _localization;

	private readonly IMainThreadDispatcher _dispatcher;

	public ListPlayerOnlineGamesMenuPresenter(IRouter router, ILocalization localization, IMainThreadDispatcher dispatcher)
	{
		_router = router;
		_localization = localization;
		_dispatcher = dispatcher;
	}

	public void UpdatePlayerGamesView(ReadOnlyCollection<GameMetadata> playerGames, string userNickname, string userId, Action<string> launchGameClicked, Action<string> leaveGameClicked)
	{
		ListPlayerOnlineGamesMenuViewModel viewModel = new ListPlayerOnlineGamesMenuViewModel
		{
			Nickname = userNickname,
			PlayerGamesList = new List<NetworkGameViewModel>()
		};
		foreach (GameMetadata playerGame in playerGames)
		{
			if (playerGame.NetworkGameProgressInformation.CurrentGameStatus == NetworkGameProgressInformation.GameStatus.AbortedPlayerLeft)
			{
				_ = 1;
			}
			else
				_ = playerGame.NetworkGameProgressInformation.CurrentGameStatus == NetworkGameProgressInformation.GameStatus.AbortedTimeout;
			NetworkGameViewModel item = new NetworkGameViewModel
			{
				PlayersStartConfigurations = playerGame.GameStartConfiguration.Players,
				JoinedPlayersMetadata = playerGame.PlayerMetadata,
				GameId = playerGame.GameId,
				Name = playerGame.NetworkGameStartConfiguration.GameName,
				LaunchGameAction = launchGameClicked,
				LeaveGameAction = leaveGameClicked,
				IsPrivate = playerGame.NetworkGameStartConfiguration.IsPrivate,
				CurrentPlayerCount = playerGame.PlayerMetadata.Where((PlayerMetadata p) => p != null).Count(),
				MaxPlayerCount = playerGame.GameStartConfiguration.NumberOfPlayersAsInt,
				IsLoggedPlayerTurn = (playerGame.NetworkGameProgressInformation.CurrentPlayerId == userId),
				GameStatus = playerGame.NetworkGameProgressInformation.CurrentGameStatus,
				Round = string.Format("{0} {1}/{2}", _localization.GetTranslation("UI/ROUND"), playerGame.GameProgressInformation.CurrentRoundNumber, GetNumberOfRounds(playerGame.GameStartConfiguration.NumberOfPlayers)),
				Era = _localization.GetTranslation("UI/" + playerGame.GameProgressInformation.CurrentGameEra.ToString().Replace("Era", "_Era").ToUpper()),
				TimeLeft = playerGame.NetworkGameProgressInformation.TimeLeftForTurn,
				TimeLeftText = _localization.GetTranslation("UI/REMAINING_TIME_TURN"),
				CurrentPlayerColor = playerGame.GameProgressInformation.CurrentPlayer?.Color
			};
			viewModel.PlayerGamesList.Add(item);
		}
		_dispatcher.InvokeAsync(delegate
		{
			_router.GetView<IListPlayerOnlineGamesMenuView>().UpdatePlayerGamesView(viewModel);
		});
	}

	public void UpdateOpenGamesView(ReadOnlyCollection<GameMetadata> openGames, Action<string> joinGameClicked)
	{
		ListPlayerOnlineGamesMenuViewModel viewModel = new ListPlayerOnlineGamesMenuViewModel
		{
			OpenGamesList = new List<NetworkGameViewModel>()
		};
		foreach (GameMetadata openGame in openGames)
		{
			NetworkGameViewModel item = new NetworkGameViewModel
			{
				PlayersStartConfigurations = openGame.GameStartConfiguration.Players,
				JoinedPlayersMetadata = openGame.PlayerMetadata,
				GameId = openGame.GameId,
				Name = openGame.NetworkGameStartConfiguration.GameName,
				JoinGameAction = joinGameClicked,
				IsPrivate = openGame.NetworkGameStartConfiguration.IsPrivate,
				CurrentPlayerCount = openGame.PlayerMetadata.Where((PlayerMetadata p) => p != null).Count(),
				MaxPlayerCount = openGame.GameStartConfiguration.NumberOfPlayersAsInt,
				TurnTime = openGame.NetworkGameStartConfiguration.TurnTimeoutInSeconds,
				PlayerCountText = _localization.GetTranslation("UI/" + openGame.GameStartConfiguration.NumberOfPlayers.ToString().ToUpper() + "_PLAYERS")
			};
			viewModel.OpenGamesList.Add(item);
		}
		_dispatcher.InvokeAsync(delegate
		{
			_router.GetView<IListPlayerOnlineGamesMenuView>().UpdateOpenGamesView(viewModel);
		});
	}

	public void UpdateFiltersVisuals(bool show2Players, bool show3Players, bool show4Players, bool showPrivate)
	{
		_router.GetView<IListPlayerOnlineGamesMenuView>().UpdateFiltersVisuals(show2Players, show3Players, show4Players, showPrivate);
	}

	public void HideView()
	{
		_router.HideView<IListPlayerOnlineGamesMenuView>().OnComplete(delegate
		{
			_router.ShowView<IMainMenuView>();
		});
	}

	public void ShowLogOutPopup()
	{
		_router.GetView<IInfoPopupView>().Setup(_localization.GetTranslation("UI/LOGOUT_SUCCESSFULLY"), _localization.GetTranslation("UI/OK"), delegate
		{
			_router.HideView<IInfoPopupView>();
			_router.HideView<IListPlayerOnlineGamesMenuView>();
			_router.ShowView<IMainMenuView>();
		});
		_router.ShowView<IInfoPopupView>();
	}

	public void ShowCreateOnlineGameView()
	{
		_router.HideView<IListPlayerOnlineGamesMenuView>().OnComplete(delegate
		{
			_router.ShowView<ICreateOnlineGameView>();
		});
	}

	public void ShowConnectionView()
	{
		_dispatcher.InvokeAsync(delegate
		{
			_router.ShowView<IConnectionView>();
		});
	}

	public void HideConnectionView()
	{
		_dispatcher.InvokeAsync(delegate
		{
			if (_router.IsViewVisible<IConnectionView>())
			{
				_router.HideView<IConnectionView>();
			}
		});
	}

	public void ShowInfoViewWithError(string errorKey)
	{
		_dispatcher.InvokeAsync(delegate
		{
			string translation = _localization.GetTranslation("UI/ERROR_" + errorKey);
			if (string.IsNullOrEmpty(translation))
			{
				translation = _localization.GetTranslation("UI/ERROR_UnknownError");
			}
			_router.GetView<IInfoPopupView>().Setup(translation, _localization.GetTranslation("UI/OK"), delegate
			{
				_router.HideView<IInfoPopupView>();
			});
			_router.ShowView<IInfoPopupView>();
		});
	}

	public void LaunchGame(BrassApplicationGame game)
	{
		_dispatcher.InvokeAsync(delegate
		{
			_router.ShowView<ILoadSceneView>().OnComplete(delegate
			{
				_router.GetView<ILoadSceneView>().LoadScene("GameScene");
			});
		});
	}

	public void ShowJoinOnlineGameView(GameMetadata gameMetadata)
	{
		_router.ShowView<IJoinOnlineGameView>();
		_router.GetView<IJoinOnlineGameView>().SetGameToJoin(gameMetadata);
	}

	private int GetNumberOfRounds(NumberOfPlayers numberOfPlayers)
	{
		return numberOfPlayers switch
		{
			NumberOfPlayers.Two => 10, 
			NumberOfPlayers.Three => 9, 
			NumberOfPlayers.Four => 8, 
			_ => 10, 
		};
	}
}
