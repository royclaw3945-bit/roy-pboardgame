using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Frameworks.View.Menu.Online;

public class TimePanelView : MonoBehaviour
{
	private List<Button> _timeButtons;

	private CanvasGroup _canvasGroup;

	public void Start()
	{
		PrepareReferences();
	}

	public void UpdateTimePanel(List<Action> onClickActions)
	{
		for (int i = 0; i < _timeButtons.Count; i++)
		{
			Button button = _timeButtons[i];
			Action action = onClickActions[i];
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(delegate
			{
				action();
				HideView();
			});
		}
	}

	protected void PrepareReferences()
	{
		_timeButtons = base.transform.Find("SafeArea/Popup/TimeButtons").GetComponentsInChildren<Button>().ToList();
		_canvasGroup = base.transform.GetComponent<CanvasGroup>();
	}

	public void ShowView()
	{
		_canvasGroup.DOFade(1f, 0.5f);
		_canvasGroup.interactable = true;
		_canvasGroup.blocksRaycasts = true;
	}

	public void HideView()
	{
		_canvasGroup.DOFade(0f, 0.5f);
		_canvasGroup.interactable = false;
		_canvasGroup.blocksRaycasts = false;
	}
}
