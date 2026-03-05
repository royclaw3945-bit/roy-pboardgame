using System;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.PlaySpectatorTurn;
using TMPro;

namespace Frameworks.View.Game;

public class PlaySpectatorTurnView : BaseView, IPlaySpectatorTurnView, IBaseView
{
	private PlaySpectatorTurnController _controller;

	private TextMeshProUGUI nickname;

	protected override bool ShowImmediately => true;

	public override IController Controller
	{
		get
		{
			return _controller;
		}
		set
		{
			if (!(value is PlaySpectatorTurnController controller))
			{
				throw new ArgumentException("Wrong controller type");
			}
			_controller = controller;
		}
	}

	private new void Awake()
	{
		PrepareReferences();
	}

	private void OnDestroy()
	{
		_controller.OnViewDestroyed();
	}

	public void SetCurrentPlayerName(string playerName)
	{
		nickname.text = playerName;
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		base.DestroyWhenClosed = false;
		nickname = base.transform.Find("SafeArea/Popup/ActionPanel/PlayerText").GetComponent<TextMeshProUGUI>();
	}
}
