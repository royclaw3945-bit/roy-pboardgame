using System;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using InterfaceAdapters.Common.CreateGame;
using InterfaceAdapters.Routing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Frameworks.View.Menu.CreateGame;

public class CreateOfflineGameView : BaseView, ICreateOfflineGameView, IBaseView
{
	private Transform _existingGameContainer;

	private Transform _existingGamePlayersContainer;

	private Transform _newGamePlayersContainer;

	private Transform _ornamentTransform;

	private Transform _characterSingleBackground;

	private Transform _characterDoubleBackground;

	private Transform _introductoryGameQuestionMark;

	private Button _startGameButton;

	private Toggle _introductoryGameBox;

	private TextMeshProUGUI _continueGameEraText;

	private TextMeshProUGUI _continueGameTurnText;

	private TextMeshProUGUI _createGameText;

	private TextMeshProUGUI _headerText;

	private TextMeshProUGUI _introductoryGameText;

	private CreateOfflineGameController _controller;

	public override IController Controller
	{
		get
		{
			return _controller;
		}
		set
		{
			if (!(value is CreateOfflineGameController controller))
			{
				throw new ArgumentException("Wrong controller type");
			}
			_controller = controller;
		}
	}

	public void ShowExistingGameState(CreateGameViewModel viewModel)
	{
		for (int i = 0; i < _existingGamePlayersContainer.childCount; i++)
		{
			_existingGamePlayersContainer.GetChild(i).GetComponent<PlayerCreateGameView>().SetPlayer(viewModel.PlayersViewModels[i]);
		}
		_continueGameEraText.text = viewModel.EraTranslation;
		_continueGameTurnText.text = viewModel.RoundTranslation;
	}

	public void ShowNewGameState(CreateGameViewModel viewModel)
	{
		for (int i = 0; i < _newGamePlayersContainer.childCount; i++)
		{
			_newGamePlayersContainer.GetChild(i).GetComponent<PlayerCreateGameView>().SetPlayer(viewModel.PlayersViewModels[i]);
		}
		_startGameButton.interactable = viewModel.StartGameButtonIsActive;
		_headerText.text = viewModel.LocalGameTranslation;
		_createGameText.text = viewModel.CreateGameTranslation;
		_introductoryGameText.text = viewModel.IntroductoryGameTranslation;
	}

	public void ShowExistingGameContainer(CreateGameViewModel viewModel)
	{
		ShowExistingGameState(viewModel);
		_existingGameContainer.gameObject.SetActive(value: true);
		_ornamentTransform.gameObject.SetActive(value: true);
		_characterDoubleBackground.gameObject.SetActive(value: true);
		_characterSingleBackground.gameObject.SetActive(value: false);
	}

	public void HideExistingGameContainer()
	{
		_existingGameContainer.gameObject.SetActive(value: false);
		_ornamentTransform.gameObject.SetActive(value: false);
		_characterDoubleBackground.gameObject.SetActive(value: false);
		_characterSingleBackground.gameObject.SetActive(value: true);
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		_existingGameContainer = base.transform.Find("SafeArea/Popup/GamesContainer/ResumeGameContainer");
		_existingGamePlayersContainer = _existingGameContainer.Find("PlayersContainer");
		_newGamePlayersContainer = base.transform.Find("SafeArea/Popup/GamesContainer/CreateGameContainer/PlayersContainer");
		_startGameButton = base.transform.Find("SafeArea/Popup/GamesContainer/CreateGameContainer/NewGameButton").GetComponent<Button>();
		_introductoryGameBox = base.transform.Find("SafeArea/Popup/GamesContainer/CreateGameContainer/IntroductoryGameContainer/IntroToggle").GetComponent<Toggle>();
		_ornamentTransform = base.transform.Find("SafeArea/Popup/GamesContainer/Ornament");
		_characterSingleBackground = base.transform.Find("SafeArea/Popup/CharacterSingleContainer");
		_characterDoubleBackground = base.transform.Find("SafeArea/Popup/CharacterDoubleContainer");
		_continueGameEraText = _existingGameContainer.Find("EraText").GetComponent<TextMeshProUGUI>();
		_continueGameTurnText = _existingGameContainer.Find("TurnText").GetComponent<TextMeshProUGUI>();
		_headerText = base.transform.Find("SafeArea/Popup/Header").GetComponent<TextMeshProUGUI>();
		_createGameText = base.transform.Find("SafeArea/Popup/GamesContainer/CreateGameContainer/CreateGameText").GetComponent<TextMeshProUGUI>();
		_introductoryGameText = base.transform.Find("SafeArea/Popup/GamesContainer/CreateGameContainer/IntroductoryGameContainer/IntroductoryGameText").GetComponent<TextMeshProUGUI>();
		_introductoryGameQuestionMark = base.transform.Find("SafeArea/Popup/GamesContainer/CreateGameContainer/IntroductoryGameContainer/QuestionMarkContainer/QuestionMark");
	}

	public void OnStartGameClicked()
	{
		_controller.StartGameClicked();
	}

	public void OnBackClicked()
	{
		_controller.BackClicked();
	}

	public void OnIntroductoryBoxClicked()
	{
		_controller.OnIntroductoryBoxClicked(_introductoryGameBox.isOn);
	}

	public void OnContinueClicked()
	{
		_controller.ContinueClicked();
	}

	public void OnIntroductoryGameQuestionMarkHoverEnter()
	{
		Vector3 position = _introductoryGameQuestionMark.position;
		_controller.ShowIntroductoryGameTooltip(position.x, position.y);
	}

	public void OnIntroductoryGameQuestionMarkHoverExit()
	{
		_controller.HideTooltip();
	}

	public override void HandleKeyDown(KeyEnum keyEnum)
	{
		base.HandleKeyDown(keyEnum);
		if (keyEnum == KeyEnum.Escape)
		{
			OnBackClicked();
		}
	}
}
