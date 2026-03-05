using System;
using InterfaceAdapters.Common;

namespace InterfaceAdapters.Menu.Online.CreateOnlineGame;

public interface IInputPrivatePasswordView : IBaseView
{
	void Setup(Action<string> onConfirm, Action onCancel, string basicInputText);
}
