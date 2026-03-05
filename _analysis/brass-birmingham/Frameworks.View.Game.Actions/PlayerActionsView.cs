using System;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.PlayerActions;
using UnityEngine;
using UnityEngine.UI;

namespace Frameworks.View.Game.Actions;

public class PlayerActionsView : BaseView, IPlayerActionsView, IBaseView
{
	private PlayerActionsController _controller;

	protected GameObject _endTurnButton;

	protected Button _undoButton;

	protected GameObject _actionButtons;

	private GameObject _actionIndicator02;

	private GameObject _actionIndicator01Active;

	private GameObject _actionIndicator02Active;

	private GameObject _actionCounters;

	protected override bool ShowImmediately => true;

	public override IController Controller
	{
		get
		{
			return _controller;
		}
		set
		{
			if (!(value is PlayerActionsController controller))
			{
				throw new ArgumentException("Wrong controller type");
			}
			_controller = controller;
		}
	}

	public void OnBuildActionClicked()
	{
		_controller.OnBuildActionClicked();
	}

	public void OnLinkActionClicked()
	{
		_controller.OnLinkActionClicked();
	}

	public void OnDevelopActionClicked()
	{
		_controller.OnDevelopActionClicked();
	}

	public void OnSellActionClicked()
	{
		_controller.OnSellActionClicked();
	}

	public void OnLoanActionClicked()
	{
		_controller.OnLoanActionClicked();
	}

	public void OnScoutActionClicked()
	{
		_controller.OnScoutActionClicked();
	}

	public void OnPassActionClicked()
	{
		_controller.OnPassActionClicked();
	}

	public void OnUndoActionClicked()
	{
		_controller.OnUndoActionClicked();
		_controller.UpdatePlayerActions();
	}

	public async void OnEndTurnClicked()
	{
		_controller.OnEndTurnClicked();
	}

	public void UpdateActionCounter(PlayerActionsViewModel playerActionsViewModel)
	{
		_actionIndicator02.SetActive(playerActionsViewModel.Is2ndActionPresent);
		_actionIndicator01Active.SetActive(playerActionsViewModel.Is1stActionAvailable);
		_actionIndicator02Active.SetActive(playerActionsViewModel.Is2ndActionAvailable);
	}

	public void ShowEndTurnButton()
	{
		_actionButtons.SetActive(value: false);
		_actionCounters.SetActive(value: false);
		_endTurnButton.SetActive(value: true);
	}

	public void HideEndTurnButton()
	{
		_actionButtons.SetActive(value: true);
		_actionCounters.SetActive(value: true);
		_endTurnButton.SetActive(value: false);
	}

	public bool IsEndTurnButtonVisible()
	{
		return _endTurnButton.activeSelf;
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		base.DestroyWhenClosed = false;
		_actionCounters = base.transform.Find("SafeArea/Popup/ActionCounters").gameObject;
		_actionIndicator02 = base.transform.Find("SafeArea/Popup/ActionCounters/ActionIndicator02").gameObject;
		_actionIndicator01Active = base.transform.Find("SafeArea/Popup/ActionCounters/ActionIndicator01/IndicatorActive").gameObject;
		_actionIndicator02Active = base.transform.Find("SafeArea/Popup/ActionCounters/ActionIndicator02/IndicatorActive").gameObject;
		_endTurnButton = base.transform.Find("SafeArea/Popup/ButtonEndTurn").gameObject;
		_undoButton = base.transform.Find("SafeArea/Popup/UndoButton").GetComponent<Button>();
		_actionButtons = base.transform.Find("SafeArea/Popup/ActionButtons").gameObject;
		HideEndTurnButton();
	}

	public void OnMouseEnter(string actionButton)
	{
		Vector3 zero = Vector3.zero;
		zero = ((!(actionButton == "Undo")) ? base.transform.Find("SafeArea/Popup/ActionButtons/" + actionButton + "Button").position : _undoButton.transform.position);
		_controller.OnMouseEnter(actionButton, zero.x, zero.y, Screen.width);
	}

	public void OnMouseExit()
	{
		_controller.OnMouseExit();
	}

	public void UpdateUndoButtonState(bool isUndoActionAvailable)
	{
		if (isUndoActionAvailable)
		{
			_undoButton.interactable = true;
		}
		else
		{
			_undoButton.interactable = false;
		}
	}
}
