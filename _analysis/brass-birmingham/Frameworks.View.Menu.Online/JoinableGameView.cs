using System;
using InterfaceAdapters.Menu.OnlineGameMenu;
using TMPro;

namespace Frameworks.View.Menu.Online;

public class JoinableGameView : NetworkGameView
{
	private TextMeshProUGUI _turnTimeText;

	private TextMeshProUGUI _playersNumberText;

	private Action<string> _joinGameAction = delegate
	{
	};

	public override void SetGame(NetworkGameViewModel networkGameViewModel)
	{
		base.SetGame(networkGameViewModel);
		_joinGameAction = networkGameViewModel.JoinGameAction;
		int num = networkGameViewModel.TurnTime / 60;
		string text = "m";
		if (num >= 60)
		{
			num /= 60;
			text = "h";
		}
		if (num >= 24)
		{
			num /= 24;
			text = "d";
		}
		_turnTimeText.text = num + text;
		_playersNumberText.text = networkGameViewModel.PlayerCountText;
	}

	public void JoinGame()
	{
		_joinGameAction(_gameId);
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		_turnTimeText = base.transform.Find("TurnTimeText").GetComponent<TextMeshProUGUI>();
		_playersNumberText = base.transform.Find("PlayersNumberText").GetComponent<TextMeshProUGUI>();
	}
}
