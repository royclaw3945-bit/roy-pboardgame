using System;
using System.Collections.Generic;
using BoardGameRules.Entities.Game;
using BoardGameRules.Entities.Players;
using BrassApplication.Game;
using BrassApplication.UseCases.Menu.OnlineGame.ChoosePortraitAndJoin;
using BrassApplication.UseCases.Menu.OnlineGame.JoinOnlineGame;
using BrassApplication.UseCases.Menu.OnlineGame.ListPlayerOnlineGamesMenu;
using Frameworks.Utils;
using InterfaceAdapters.Common;
using InterfaceAdapters.Menu.ChoosePortraitAndJoin;
using InterfaceAdapters.Menu.Connection;
using InterfaceAdapters.Menu.Online.CreateOnlineGame;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Utils;
using InterfaceAdapters.Utils.MainThreadDispatcher;

namespace InterfaceAdapters.Menu.Online.JoinOnlineGame;

public class JoinOnlineGamePresenter : IPresenter, IJoinOnlineGameOutput
{
	private readonly IRouter _router;

	private readonly ILocalization _localization;

	private readonly IMainThreadDispatcher _dispatcher;

	public JoinOnlineGamePresenter(IRouter router, ILocalization localization, IMainThreadDispatcher dispatcher)
	{
		_router = router;
		_localization = localization;
		_dispatcher = dispatcher;
	}

	public void PresentGameConfiguration(GameMetadata gameMetadata, Action<int> actionOnClick)
	{
		_dispatcher.InvokeAsync(delegate
		{
			JoinOnlineGameViewModel joinOnlineGameViewModel = new JoinOnlineGameViewModel
			{
				NumberOfPlayers = (int)gameMetadata.GameStartConfiguration.NumberOfPlayers,
				PlayersViewModels = new List<PlayerOnlineGameViewModel>(),
				Name = gameMetadata.NetworkGameStartConfiguration.GameName,
				TimePerTurn = TimeHelpers.SecondsToTimeString(gameMetadata.NetworkGameStartConfiguration.TurnTimeoutInSeconds)
			};
			IList<PlayerStartConfiguration> players = gameMetadata.GameStartConfiguration.Players;
			for (int i = 0; i < players.Count; i++)
			{
				PlayerOnlineGameViewModel playerOnlineGameViewModel = new PlayerOnlineGameViewModel();
				PlayerMetadata playerMetadataByColor = gameMetadata.GetPlayerMetadataByColor(players[i].Color);
				if (playerMetadataByColor != null)
				{
					playerOnlineGameViewModel.Nickname = playerMetadataByColor.NickName;
					playerOnlineGameViewModel.PlayerAvatar = playerMetadataByColor.PlayerAvatar;
					playerOnlineGameViewModel.HasJoined = true;
				}
				playerOnlineGameViewModel.CurrentPlayerCount = (int)gameMetadata.GameStartConfiguration.NumberOfPlayers;
				playerOnlineGameViewModel.PlayerColor = players[i].Color;
				playerOnlineGameViewModel.PlayerNumber = i;
				joinOnlineGameViewModel.PlayersViewModels.Add(playerOnlineGameViewModel);
			}
			_router.GetView<IJoinOnlineGameView>().ShowCurrentGameState(joinOnlineGameViewModel);
		});
	}

	public void HideView()
	{
		_router.HideView<IJoinOnlineGameView>();
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
			_router.HideView<IConnectionView>();
		});
	}

	public void ShowInfoViewGameJoinSuccessful()
	{
		_dispatcher.InvokeAsync(delegate
		{
			_router.GetView<IInfoPopupView>().Setup(_localization.GetTranslation("UI/GAME_JOINED_SUCCESSFULLY"), _localization.GetTranslation("UI/OK"), delegate
			{
				_router.HideView<IInfoPopupView>();
				_router.UseCaseBuilder.GetUseCase<ListPlayerOnlineGamesMenuUseCase>().UpdateView();
				_router.HideView<IJoinOnlineGameView>();
			});
			_router.ShowView<IInfoPopupView>();
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

	public void ShowPasswordView(Action<int, string> joinPrivateGameAction, int slot)
	{
		_router.GetView<IInputPrivatePasswordView>().Setup(delegate(string password)
		{
			joinPrivateGameAction(slot, password);
			_router.HideView<IInputPrivatePasswordView>();
		}, delegate
		{
			_router.HideView<IInputPrivatePasswordView>();
		}, "");
		_router.ShowView<IInputPrivatePasswordView>();
	}

	public void ShowChoosePortraitAndJoinView(Action<int, PlayerAvatar> joinGameClicked, IEnumerable<PlayerAvatar> availableAvatars, int slot, PlayerColor color)
	{
		_router.UseCaseBuilder.GetUseCase<ChoosePortraitAndJoinUseCase>().SetUp(availableAvatars, delegate(PlayerAvatar playerAvatar)
		{
			joinGameClicked(slot, playerAvatar);
		}, color);
		_router.ShowView<IChoosePortraitAndJoinView>();
	}
}
