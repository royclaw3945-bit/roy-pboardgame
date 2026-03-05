using InterfaceAdapters.Common;
using InterfaceAdapters.Game.ActionInProgressView;

namespace InterfaceAdapters.Tutorials.Actions.ActionInProgress;

public interface ITutorialActionInProgressView : IActionInProgressView, IBaseView
{
	void TurnOnCancelButton();
}
