using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Game;
using BoardGameRules.Entities.Players;
using BrassApplication.AI;
using BrassApplication.Game;
using BrassApplication.UseCases.Menu.OnlineGame.CreateOnlineGame;
using Frameworks.Utils;
using InterfaceAdapters.Common;
using InterfaceAdapters.Menu.Connection;
using InterfaceAdapters.Menu.CreateGame;
using InterfaceAdapters.Menu.OnlineGameMenu;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Utils;
using InterfaceAdapters.Utils.MainThreadDispatcher;

namespace InterfaceAdapters.Menu.Online.CreateOnlineGame;

public class CreateOnlineGamePresenter : IPresenter, ICreateOnlineGameOutput
{
	private readonly IRouter _router;

	private readonly ILocalization _localization;

	private readonly IMainThreadDispatcher _dispatcher;

	public CreateOnlineGamePresenter(IRouter router, ILocalization localization, IMainThreadDispatcher dispatcher)
	{
		_router = router;
		_localization = localization;
		_dispatcher = dispatcher;
	}

	public void PresentGameConfiguration(IList<PlayerStartConfiguration> playerStartConfigurations, IList<PlayerMetadata> playersMetadata, int numberOfPlayers, string name, string password, int timePerTurn, Action<PlayerColor, PlayerAvatar> onEditPortraitAndColorAction, Action increaseNumberOfPlayers, Action decreaseNumberOfPlayers)
	{
		List<PlayerStartConfiguration> playerConfigList = playerStartConfigurations.ToList();
		CreateOnlineGameViewModel createOnlineGameViewModel = new CreateOnlineGameViewModel
		{
			NumberOfPlayers = numberOfPlayers,
			PlayersViewModels = new List<PlayerOnlineGameViewModel>(),
			Name = name,
			Password = (string.IsNullOrEmpty(password) ? _localization.GetTranslation("UI/NO_PASSWORD") : password),
			TimePerTurn = TimeHelpers.SecondsToTimeString(timePerTurn)
		};
		int i;
		for (i = 0; i < 4; i++)
		{
			if (i < numberOfPlayers)
			{
				createOnlineGameViewModel.PlayersViewModels.Add(new PlayerOnlineGameViewModel
				{
					Nickname = _localization.GetTranslation("UI/PLAYER") + " " + _localization.GetTranslation($"UI/{playerConfigList[i].Color}"),
					AiTypeText = _localization.GetTranslation("UI/AI_TYPE_" + AIType.Human.ToString().ToUpper()),
					PlayerColor = playerConfigList[i].Color,
					PlayerNumber = i + 1,
					CurrentPlayerCount = numberOfPlayers,
					OnEditPortraitAndColorAction = onEditPortraitAndColorAction,
					OnAddPlayerClickedAction = increaseNumberOfPlayers,
					OnRemovePlayerClickedAction = decreaseNumberOfPlayers
				});
				PlayerMetadata playerMetadata = playersMetadata.FirstOrDefault((PlayerMetadata pm) => pm.PlayerStartConfiguration.Color == playerConfigList[i].Color);
				if (playerMetadata != null)
				{
					PlayerOnlineGameViewModel playerOnlineGameViewModel = createOnlineGameViewModel.PlayersViewModels.Last();
					playerOnlineGameViewModel.PlayerAvatar = playerMetadata.PlayerAvatar;
					playerOnlineGameViewModel.Nickname = playerMetadata.NickName;
					playerOnlineGameViewModel.HasJoined = true;
				}
			}
			else
			{
				createOnlineGameViewModel.PlayersViewModels.Add(new PlayerOnlineGameViewModel
				{
					PlayerNumber = i + 1,
					CurrentPlayerCount = numberOfPlayers,
					OnAddPlayerClickedAction = increaseNumberOfPlayers,
					OnRemovePlayerClickedAction = decreaseNumberOfPlayers
				});
			}
		}
		_router.GetView<ICreateOnlineGameView>().ShowCurrentGameState(createOnlineGameViewModel);
	}

	public void ShowChangeTimeView(List<Action> onClickActions)
	{
		_router.GetView<ICreateOnlineGameView>().UpdateTimePanel(onClickActions);
	}

	public void ShowChangePasswordView(Action<string> actionOnConfirm, string password)
	{
		_router.GetView<IInputPrivatePasswordView>().Setup(delegate(string s)
		{
			actionOnConfirm(s);
			_router.HideView<IInputPrivatePasswordView>();
		}, delegate
		{
			_router.HideView<IInputPrivatePasswordView>();
		}, password);
		_router.ShowView<IInputPrivatePasswordView>();
	}

	public void HideView()
	{
		_router.HideView<ICreateOnlineGameView>().OnComplete(delegate
		{
			_router.ShowView<IListPlayerOnlineGamesMenuView>();
		});
	}

	public void HideInputView()
	{
		_router.HideView<IInputPopupView>();
	}

	public void ShowConnectionView()
	{
		_router.ShowView<IConnectionView>();
	}

	public void HideConnectionView()
	{
		_dispatcher.InvokeAsync(delegate
		{
			_router.HideView<IConnectionView>();
		});
	}

	public void ShowInfoViewGameCreateSuccessful()
	{
		_dispatcher.InvokeAsync(delegate
		{
			_router.GetView<IInfoPopupView>().Setup(_localization.GetTranslation("UI/GAME_CREATED_SUCCESSFULLY"), _localization.GetTranslation("UI/OK"), delegate
			{
				_router.HideView<IInfoPopupView>();
				_router.HideView<ICreateOnlineGameView>();
				_router.ShowView<IListPlayerOnlineGamesMenuView>();
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

	public void UpdateColorInPortraitAndColorView(PlayerColor playerColor)
	{
		_router.GetView<IChoosePortraitAndColorView>().UpdateColor(playerColor);
	}

	public void UpdatePortraitInPortraitAndColorView(PlayerAvatar avatar)
	{
		_router.GetView<IChoosePortraitAndColorView>().UpdateAvatar(avatar);
	}

	public void ShowChoosePortraitAndColorView(PlayerColor playerColor, PlayerAvatar playerAvatar)
	{
		_router.ShowView<IChoosePortraitAndColorView>();
		_router.GetView<IChoosePortraitAndColorView>().UpdateView(playerColor, playerAvatar, isOnlineGame: true);
	}

	public void LockPasswordIcon(bool isPrivate)
	{
		_router.GetView<ICreateOnlineGameView>().LockPasswordIcon(isPrivate);
	}

	public string GetDefaultGameNameTranslation()
	{
		return _localization.GetTranslation("UI/DEFAULT_GAME_NAME");
	}
}
