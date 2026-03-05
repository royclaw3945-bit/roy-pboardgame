using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.PlayerActions;

public interface IPlayerActionsView : IBaseView
{
	void UpdateActionCounter(PlayerActionsViewModel viewModel);

	void HideEndTurnButton();

	void ShowEndTurnButton();

	bool IsEndTurnButtonVisible();

	void UpdateUndoButtonState(bool isUndoActionAvailable);
}
