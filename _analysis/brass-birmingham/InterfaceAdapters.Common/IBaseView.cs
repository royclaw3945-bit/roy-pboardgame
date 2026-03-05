using InterfaceAdapters.Routing;

namespace InterfaceAdapters.Common;

public interface IBaseView
{
	IInputRegistration InputRegistration { get; set; }

	bool DisableMenusUnderneath { get; set; }

	bool DestroyWhenClosed { get; set; }

	IController Controller { get; set; }

	void HandleKeyDown(KeyEnum keyEnum);

	void HandleKey(KeyEnum keyEnum);

	void HandleKeyUp(KeyEnum keyEnum);

	void EnableView();

	void DisableView();

	IAnimation CreateShowViewAnimation();

	IAnimation CreateHideViewAnimation();

	IAnimation CreateShowViewImmediatelyAnimation();

	void SetInteractable(bool isInteractable);

	bool IsViewVisible();

	void MoveToTop();

	void MoveToBottom();
}
