using System;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.GameScene;
using InterfaceAdapters.Routing;
using UnityEngine;

namespace Frameworks.View.Game;

public class GameSceneView : BaseView, IGameSceneView, IBaseView
{
	protected GameSceneController _controller;

	private Transform _menuButtonTransform;

	public override IController Controller
	{
		get
		{
			return _controller;
		}
		set
		{
			if (!(value is GameSceneController controller))
			{
				throw new ArgumentException("Wrong controller type");
			}
			_controller = controller;
		}
	}

	public void OnBackToMenuClicked()
	{
		_controller.OpenMenu();
	}

	public void OnMouseEnter()
	{
		_controller.OnMouseEnter(_menuButtonTransform.position.x + (float)(Screen.width / 30), _menuButtonTransform.position.y);
	}

	public void OnMouseExit()
	{
		_controller.OnMouseExit();
	}

	public override void HandleKeyDown(KeyEnum keyEnum)
	{
		base.HandleKeyDown(keyEnum);
		if (keyEnum == KeyEnum.Escape)
		{
			OnBackToMenuClicked();
		}
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		_menuButtonTransform = base.transform.Find("SafeArea/Popup/FrameMenuButton");
	}
}
