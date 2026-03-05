using System;
using System.Collections.Generic;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using InterfaceAdapters.Menu.Online.CreateOnlineGame;
using InterfaceAdapters.Routing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Frameworks.View.Menu.Online;

public class CreateOnlineGameView : BaseView, ICreateOnlineGameView, IBaseView
{
	private Transform _playersContainer;

	private TMP_InputField _nameText;

	private TextMeshProUGUI _roundTimeText;

	private Image _passwordIcon;

	private TimePanelView _timePanelView;

	private CreateOnlineGameController _controller;

	public override IController Controller
	{
		get
		{
			return _controller;
		}
		set
		{
			if (!(value is CreateOnlineGameController controller))
			{
				throw new ArgumentException("Wrong controller type");
			}
			_controller = controller;
		}
	}

	public void ShowCurrentGameState(CreateOnlineGameViewModel viewModel)
	{
		for (int i = 0; i < _playersContainer.childCount; i++)
		{
			_playersContainer.GetChild(i).GetComponent<PlayerCreateOnlineGameView>().SetPlayer(viewModel.PlayersViewModels[i]);
		}
		_nameText.text = viewModel.Name;
		_roundTimeText.text = viewModel.TimePerTurn;
	}

	public void LockPasswordIcon(bool isPrivate)
	{
		if (isPrivate)
		{
			_passwordIcon.sprite = Resources.Load<Sprite>("PlayerMat/Icons/BbIconLockOn");
		}
		else
		{
			_passwordIcon.sprite = Resources.Load<Sprite>("PlayerMat/Icons/BbIconLock");
		}
	}

	public void UpdateTimePanel(List<Action> onClickActions)
	{
		_timePanelView.UpdateTimePanel(onClickActions);
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		_playersContainer = base.transform.Find("SafeArea/Popup/CreateGameContainer/PlayersContainer");
		_nameText = base.transform.Find("SafeArea/Popup/CreateGameContainer/GameName").GetComponent<TMP_InputField>();
		_roundTimeText = base.transform.Find("SafeArea/Popup/CreateGameContainer/RoundTimeText").GetComponent<TextMeshProUGUI>();
		_passwordIcon = base.transform.Find("SafeArea/Popup/CreateGameContainer/PrivateGameButton/LockIcon").GetComponent<Image>();
		_timePanelView = base.transform.Find("SafeArea/TimePanelView").GetComponent<TimePanelView>();
	}

	public void OnStartGameClicked()
	{
		_controller.ChangeNameClicked(_nameText.text);
		_controller.StartGameClicked();
	}

	public void OnChangeTimeClicked()
	{
		_timePanelView.ShowView();
		_controller.ChangeTimeClicked();
	}

	public void OnChangePasswordClicked()
	{
		_controller.ChangePasswordClicked();
	}

	public void OnBackClicked()
	{
		_controller.BackClicked();
	}

	public void OnChangeName()
	{
		_controller.ChangeNameClicked(_nameText.text);
	}

	public void OnChangeNameButtonClicked()
	{
		_nameText.ActivateInputField();
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
