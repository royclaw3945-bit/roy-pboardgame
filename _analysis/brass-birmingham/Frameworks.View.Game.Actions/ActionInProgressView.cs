using System;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.ActionInProgressView;
using InterfaceAdapters.Routing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Frameworks.View.Game.Actions;

public class ActionInProgressView : BaseView, IActionInProgressView, IBaseView
{
	private ActionInProgressController _controller;

	private TextMeshProUGUI _actionInfoText;

	private TextMeshProUGUI _actionCounterText;

	protected Transform _cancelButton;

	private Button _finishButton;

	private const float distanceBetweenCancelAndFinishButtons = 225f;

	protected override bool ShowImmediately => true;

	public override IController Controller
	{
		get
		{
			return _controller;
		}
		set
		{
			if (!(value is ActionInProgressController controller))
			{
				throw new ArgumentException("Wrong controller type");
			}
			_controller = controller;
		}
	}

	public void SetActionInfoText(string text)
	{
		_actionInfoText.text = text;
	}

	public void UpdateActionCounter(int currentActionNumber, int actionsMaxCount)
	{
		_actionCounterText.text = currentActionNumber + "/" + actionsMaxCount;
	}

	public void OnCancelClicked()
	{
		_controller.OnCancelClicked();
	}

	public void OnFinishClicked()
	{
		_controller.OnFinishClicked();
	}

	public override void HandleKeyDown(KeyEnum keyEnum)
	{
		base.HandleKeyDown(keyEnum);
		if (keyEnum == KeyEnum.Escape)
		{
			OnCancelClicked();
		}
	}

	public void ShowFinishButton()
	{
		_cancelButton.localPosition = new Vector3(_finishButton.transform.localPosition.x - 225f, _cancelButton.position.y, _cancelButton.position.z);
		_finishButton.gameObject.SetActive(value: true);
		_finishButton.interactable = false;
	}

	public void ActivateFinishButton()
	{
		_finishButton.interactable = true;
	}

	public void HideFinishButton()
	{
		_cancelButton.localPosition = new Vector3(_cancelButton.localPosition.x + 112.5f, _cancelButton.position.y, _cancelButton.position.z);
		_finishButton.gameObject.SetActive(value: false);
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		_actionInfoText = base.transform.Find("SafeArea/Popup/ActionInfo").GetComponent<TextMeshProUGUI>();
		_actionCounterText = base.transform.Find("SafeArea/Popup/ActionCounter/ActionText/ActionCountText").GetComponent<TextMeshProUGUI>();
		_cancelButton = base.transform.Find("SafeArea/Popup/CancelButton");
		_finishButton = base.transform.Find("SafeArea/Popup/FinishButton").GetComponent<Button>();
	}
}
