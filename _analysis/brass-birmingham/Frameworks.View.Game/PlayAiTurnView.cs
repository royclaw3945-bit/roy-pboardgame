using System;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.PlayAiTurn;
using TMPro;

namespace Frameworks.View.Game;

public class PlayAiTurnView : BaseView, IPlayAiTurnView, IBaseView
{
	private PlayAiTurnController _controller;

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
			if (!(value is PlayAiTurnController controller))
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

	public void SetCurrentPlayerName(string playerName)
	{
		nickname.text = playerName;
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		nickname = base.transform.Find("SafeArea/Popup/ActionPanel/PlayerText").GetComponent<TextMeshProUGUI>();
	}
}
