using System;

namespace InterfaceAdapters.Common;

public interface IInputPopupView : IBaseView
{
	void Setup(string infoText, string confirmButtonText, string cancelButtonText, Action<string> onConfirm, Action onCancel, string basicInputText);
}
