using System.Collections.Generic;
using BoardGameRules.Entities.Players;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Frameworks.View.Game.ReplaySummary;

public class PlayerActionLogView : MonoBehaviour
{
	private Image _playerPortrait;

	private Image _playerColor;

	private TextMeshProUGUI _action1;

	private TextMeshProUGUI _action2;

	public void UpdatePlayer(PlayerColor color, PlayerAvatar avatar, List<string> actions, string actionTranslation)
	{
		PrepareReferences();
		_playerColor.sprite = Resources.Load<Sprite>("Player/BbFramePanelPlayer" + color);
		_playerPortrait.sprite = Resources.Load<Sprite>("Player/BbFramePanelPortrait" + avatar);
		_action1.gameObject.SetActive(actions.Count > 0);
		_action2.gameObject.SetActive(actions.Count > 1);
		if (actions.Count > 0)
		{
			_action1.text = actionTranslation + " 1: <#000>" + actions[0];
		}
		if (actions.Count > 1)
		{
			_action2.text = actionTranslation + " 2: <#000>" + actions[1];
		}
	}

	private void PrepareReferences()
	{
		_playerPortrait = base.transform.Find("Avatar/PlayerPortrait").GetComponent<Image>();
		_playerColor = base.transform.Find("Avatar/PlayerColor").GetComponent<Image>();
		_action1 = base.transform.Find("Actions/Action1").GetComponent<TextMeshProUGUI>();
		_action2 = base.transform.Find("Actions/Action2").GetComponent<TextMeshProUGUI>();
	}
}
