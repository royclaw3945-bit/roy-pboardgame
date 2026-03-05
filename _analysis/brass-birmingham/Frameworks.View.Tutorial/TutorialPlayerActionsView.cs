using Frameworks.View.Game.Actions;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.PlayerActions;
using InterfaceAdapters.Tutorials.PlayerActions;
using UnityEngine.UI;

namespace Frameworks.View.Tutorial;

public class TutorialPlayerActionsView : PlayerActionsView, ITutorialPlayerActionsView, IPlayerActionsView, IBaseView
{
	public void EnableEndTurnButton(bool enableEndTurn)
	{
		_endTurnButton.GetComponent<Button>().enabled = enableEndTurn;
	}

	public void EnableUndoButton(bool enableUndo)
	{
		_undoButton.enabled = enableUndo;
	}

	public void AddActionButtonsListeners()
	{
		_actionButtons.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(base.OnBuildActionClicked);
		_actionButtons.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(base.OnLinkActionClicked);
		_actionButtons.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(base.OnDevelopActionClicked);
		_actionButtons.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(base.OnSellActionClicked);
		_actionButtons.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(base.OnLoanActionClicked);
		_actionButtons.transform.GetChild(5).GetComponent<Button>().onClick.AddListener(base.OnScoutActionClicked);
		_actionButtons.transform.GetChild(6).GetComponent<Button>().onClick.AddListener(base.OnPassActionClicked);
	}
}
