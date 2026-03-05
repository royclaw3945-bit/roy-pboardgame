using System;
using BrassApplication.Game;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using InterfaceAdapters.Menu.Online.JoinOnlineGame;
using InterfaceAdapters.Routing;
using TMPro;
using UnityEngine;

namespace Frameworks.View.Menu.Online;

public class JoinOnlineGameView : BaseView, IJoinOnlineGameView, IBaseView
{
	private Transform _playersContainer;

	private TextMeshProUGUI _nameText;

	private JoinOnlineGameController _controller;

	public override IController Controller
	{
		get
		{
			return _controller;
		}
		set
		{
			if (!(value is JoinOnlineGameController controller))
			{
				throw new ArgumentException("Wrong controller type");
			}
			_controller = controller;
		}
	}

	public void SetGameToJoin(GameMetadata gameMetadata)
	{
		_controller.SetGameToJoin(gameMetadata);
	}

	public void ShowCurrentGameState(JoinOnlineGameViewModel viewModel)
	{
		for (int i = 0; i < _playersContainer.childCount; i++)
		{
			Transform child = _playersContainer.GetChild(i);
			bool flag = i < viewModel.NumberOfPlayers;
			child.gameObject.SetActive(flag);
			if (flag)
			{
				PlayerCreateOnlineGameView component = child.GetComponent<PlayerCreateOnlineGameView>();
				component.SetPlayerForJoin(viewModel.PlayersViewModels[i]);
				component.SetJoinAction(OnJoinGameClicked);
			}
		}
		_nameText.text = viewModel.Name;
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		_playersContainer = base.transform.Find("SafeArea/Popup/JoinGameContainer/PlayersContainer");
		_nameText = base.transform.Find("SafeArea/Popup/JoinGameContainer/GameName").GetComponent<TextMeshProUGUI>();
	}

	public void OnJoinGameClicked(int slot)
	{
		_controller.JoinGameClicked(slot);
	}

	public void OnBackClicked()
	{
		_controller.BackClicked();
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
