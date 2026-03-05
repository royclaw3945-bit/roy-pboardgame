using System;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using InterfaceAdapters.Menu.MainMenu;
using InterfaceAdapters.Routing;
using TMPro;
using UnityEngine;
using Utils;

namespace Frameworks.View.Menu;

public class MainMenuView : BaseView, IMainMenuView, IBaseView
{
	private MainMenuController _controller;

	private TextMeshProUGUI VersionString { get; set; }

	public override IController Controller
	{
		get
		{
			return _controller;
		}
		set
		{
			if (!(value is MainMenuController controller))
			{
				throw new ArgumentException("Wrong controller type");
			}
			_controller = controller;
		}
	}

	protected override void Start()
	{
		base.Start();
		VersionString.text = Global.Instance.VersionReader.PublicVersion + "." + Global.Instance.VersionReader.BuildVersion;
	}

	protected override void InitialViewSetup()
	{
		RectTransform component = Popup.GetComponent<RectTransform>();
		component.anchoredPosition = Vector2.zero;
		component.localScale = Vector3.one;
		Popup.GetComponent<CanvasGroup>().alpha = 0f;
	}

	public void OnOfflineGameClicked()
	{
		_controller.ShowOfflineGameView();
	}

	public void OnTutorialClicked()
	{
		_controller.StartTutorial();
	}

	public void OnOnlineGamesClicked()
	{
		_controller.ShowOnlineGamesView();
	}

	public void OnTestGamesClicked()
	{
		_controller.ShowTestGamesView();
	}

	public void OnSettingsClicked()
	{
		_controller.ShowSettingsView();
	}

	public void OnCreditsClicked()
	{
		_controller.ShowCreditsView();
	}

	public void OnRulebookClicked()
	{
		_controller.ShowRulebookView();
	}

	public void OnExitClicked()
	{
		_controller.ShowQuitGameQuestion();
	}

	public void QuitTheGame()
	{
		Application.Quit();
	}

	public override void HandleKeyDown(KeyEnum keyEnum)
	{
		base.HandleKeyDown(keyEnum);
		if (keyEnum == KeyEnum.Escape)
		{
			OnExitClicked();
		}
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		VersionString = base.transform.Find("SafeArea/Popup/Version").GetComponent<TextMeshProUGUI>();
	}
}
