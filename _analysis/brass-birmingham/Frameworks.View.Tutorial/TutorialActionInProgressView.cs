using Frameworks.View.Game.Actions;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.ActionInProgressView;
using InterfaceAdapters.Tutorials.Actions.ActionInProgress;
using TMPro;
using UnityEngine.UI;

namespace Frameworks.View.Tutorial;

public class TutorialActionInProgressView : ActionInProgressView, ITutorialActionInProgressView, IActionInProgressView, IBaseView
{
	public void TurnOnCancelButton()
	{
		_cancelButton.GetComponent<Button>().interactable = true;
		_cancelButton.GetChild(0).GetComponent<TextMeshProUGUI>().alpha = 1f;
		base.DestroyWhenClosed = true;
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		_cancelButton.GetComponent<Button>().interactable = false;
		base.DestroyWhenClosed = false;
	}
}
