using InterfaceAdapters.Common;

namespace InterfaceAdapters.Game.ActionInProgressView;

public interface IActionInProgressView : IBaseView
{
	void SetActionInfoText(string text);

	void UpdateActionCounter(int currentActionNumber, int actionsMaxCount);

	void ShowFinishButton();

	void ActivateFinishButton();

	void HideFinishButton();
}
