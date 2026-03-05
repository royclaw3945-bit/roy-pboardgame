using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using BoardGameRules.Entities.Players;
using BrassApplication.Game;
using BrassApplication.UseCases.Menu.OfflineGame.CreateOfflineGame;
using InterfaceAdapters.Common.LoadScene;
using InterfaceAdapters.Menu.CreateGame;
using InterfaceAdapters.Menu.MainMenu;
using InterfaceAdapters.Routing;
using InterfaceAdapters.Utils;

namespace InterfaceAdapters.Common.CreateGame;

public class CreateOfflineGamePresenter : IPresenter, ICreateOfflineGameOutput
{
	private readonly IRouter _router;

	private readonly ILocalization _localization;

	public CreateOfflineGamePresenter(IRouter router, ILocalization localization)
	{
		_router = router;
		_localization = localization;
	}

	public void PresentGameConfiguration(IEnumerable<PlayerMetadata> playerMetadata, Action<PlayerMetadata> onNextDifficultyClickedAction, Action<PlayerMetadata> onPreviousDifficultyClickedAction, Action<PlayerMetadata> onEditPortraitAndColorAction, Action<PlayerColor, string> onPlayerNameChangedAction, Action increaseNumberOfPlayers, Action decreaseNumberOfPlayers, bool canStartGame)
	{
		List<PlayerMetadata> list = playerMetadata.ToList();
		CreateGameViewModel createGameViewModel = new CreateGameViewModel
		{
			NumberOfPlayers = list.Count,
			PlayersViewModels = new List<PlayerCreateGameViewModel>(),
			StartGameButtonIsActive = canStartGame,
			LocalGameTranslation = _localization.GetTranslation("UI/LOCAL_GAME"),
			CreateGameTranslation = _localization.GetTranslation("UI/NEW_GAME"),
			IntroductoryGameTranslation = _localization.GetTranslation("UI/INTRODUCTORY_GAME")
		};
		for (int i = 0; i < 4; i++)
		{
			if (i < list.Count)
			{
				createGameViewModel.PlayersViewModels.Add(new PlayerCreateGameViewModel
				{
					PlayerMetadata = list[i],
					Nickname = list[i].NickName,
					AiType = list[i].AiType,
					AiTypeText = _localization.GetTranslation("UI/AI_TYPE_" + list[i].AiType.ToString().ToUpper()),
					PlayerColor = list[i].PlayerStartConfiguration.Color,
					PlayerNumber = i + 1,
					CurrentPlayerCount = list.Count,
					Editable = true,
					OnNextDifficultyClickedAction = onNextDifficultyClickedAction,
					OnPreviousDifficultyClickedAction = onPreviousDifficultyClickedAction,
					OnEditPortraitAndColorAction = onEditPortraitAndColorAction,
					OnPlayerNameChangedAction = onPlayerNameChangedAction,
					OnAddPlayerClickedAction = increaseNumberOfPlayers,
					OnRemovePlayerClickedAction = decreaseNumberOfPlayers
				});
			}
			else
			{
				createGameViewModel.PlayersViewModels.Add(new PlayerCreateGameViewModel
				{
					PlayerNumber = i + 1,
					CurrentPlayerCount = createGameViewModel.NumberOfPlayers,
					OnAddPlayerClickedAction = increaseNumberOfPlayers,
					OnRemovePlayerClickedAction = decreaseNumberOfPlayers,
					Editable = true
				});
			}
		}
		_router.GetView<ICreateOfflineGameView>().ShowNewGameState(createGameViewModel);
	}

	public void LaunchGame(BrassApplicationGame game)
	{
		_router.ShowView<ILoadSceneView>().OnComplete(delegate
		{
			_router.GetView<ILoadSceneView>().LoadScene("GameScene");
		});
	}

	public void ShowChoosePortraitAndColorView(PlayerMetadata playerMetadata)
	{
		_router.ShowView<IChoosePortraitAndColorView>();
		UpdatePortraitAndColorView(playerMetadata);
	}

	public void UpdatePortraitAndColorView(PlayerMetadata playerMetadata)
	{
		_router.GetView<IChoosePortraitAndColorView>().UpdateView(playerMetadata.PlayerStartConfiguration.Color, playerMetadata.PlayerAvatar);
	}

	public void ShowIntroductoryGameTooltip(float x, float y)
	{
		if (!_router.IsViewVisible<ITooltipView>())
		{
			_router.ShowView<ITooltipView>();
		}
		else
		{
			_router.MoveOnTop<ITooltipView>();
		}
		string translation = _localization.GetTranslation("UI/INTRODUCTORY_GAME_EXPLANATION");
		_router.GetView<ITooltipView>().ShowTooltip(translation, new Vector3(x, y, 0f), new Vector2(1f, 0f));
	}

	public void HideTooltip()
	{
		_router.GetView<ITooltipView>().HideTooltip();
	}

	public void AskOverwriteExistingGame(Func<BrassApplicationGame> startGameAction)
	{
		_router.ShowView<IQuestionPopupView>();
		string translation = _localization.GetTranslation("UI/DO_YOU_WANT_OVERWRITE");
		string translation2 = _localization.GetTranslation("UI/CONTINUE");
		string translation3 = _localization.GetTranslation("UI/BACK");
		_router.GetView<IQuestionPopupView>().Setup(translation, isConfirmNegative: true, translation2, translation3, delegate
		{
			startGameAction();
		}, delegate
		{
			_router.HideView<IQuestionPopupView>();
		});
	}

	public void ShowExistingGameContainer(BrassApplicationGame game)
	{
		List<PlayerMetadata> list = game.Metadata.PlayerMetadata.ToList();
		string text = game.Metadata.GameProgressInformation.CurrentGameEra.ToString();
		CreateGameViewModel createGameViewModel = new CreateGameViewModel();
		createGameViewModel.Game = game;
		createGameViewModel.NumberOfPlayers = list.Count;
		createGameViewModel.PlayersViewModels = new List<PlayerCreateGameViewModel>();
		createGameViewModel.LocalGameTranslation = _localization.GetTranslation("UI/LOCAL_GAME");
		createGameViewModel.CreateGameTranslation = _localization.GetTranslation("UI/NEW_GAME");
		createGameViewModel.EraTranslation = _localization.GetTranslation("UI/" + text.Insert(text.Length - 3, "_").ToUpper());
		createGameViewModel.RoundTranslation = _localization.GetTranslation("UI/ROUND") + " " + game.Metadata.GameProgressInformation.CurrentRoundNumber + " / " + game.Game.GameFlow.GetNumberOfRounds(game.Metadata.GameStartConfiguration.NumberOfPlayers);
		CreateGameViewModel createGameViewModel2 = createGameViewModel;
		for (int i = 0; i < 4; i++)
		{
			if (i < list.Count)
			{
				createGameViewModel2.PlayersViewModels.Add(new PlayerCreateGameViewModel
				{
					PlayerMetadata = list[i],
					Nickname = list[i].NickName,
					AiType = list[i].AiType,
					AiTypeText = _localization.GetTranslation("UI/AI_TYPE_" + list[i].AiType.ToString().ToUpper()),
					PlayerColor = list[i].PlayerStartConfiguration.Color,
					PlayerNumber = i + 1,
					CurrentPlayerCount = list.Count,
					Editable = false
				});
			}
			else
			{
				createGameViewModel2.PlayersViewModels.Add(new PlayerCreateGameViewModel
				{
					PlayerNumber = i + 1,
					CurrentPlayerCount = createGameViewModel2.NumberOfPlayers
				});
			}
		}
		_router.GetView<ICreateOfflineGameView>().ShowExistingGameContainer(createGameViewModel2);
	}

	public void HideExistingGameContainer()
	{
		_router.GetView<ICreateOfflineGameView>().HideExistingGameContainer();
	}

	public void GameLoaded(BrassApplicationGame game)
	{
		_router.ShowView<ILoadSceneView>().OnComplete(delegate
		{
			_router.GetView<ILoadSceneView>().LoadScene("GameScene");
		});
	}

	public void HideView()
	{
		_router.HideView<ICreateOfflineGameView>().OnComplete(delegate
		{
			_router.ShowView<IMainMenuView>();
		});
	}
}
