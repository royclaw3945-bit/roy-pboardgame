using InterfaceAdapters.Common;
using InterfaceAdapters.Game.PlayerActions;

namespace InterfaceAdapters.Tutorials.PlayerActions;

public interface ITutorialPlayerActionsView : IPlayerActionsView, IBaseView
{
	void EnableEndTurnButton(bool enableEndTurn);

	void EnableUndoButton(bool enableUndo);

	void AddActionButtonsListeners();
}
