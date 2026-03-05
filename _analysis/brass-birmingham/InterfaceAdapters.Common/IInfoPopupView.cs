using System;

namespace InterfaceAdapters.Common;

public interface IInfoPopupView : IBaseView
{
	void Setup(string infoText, string confirmButtonText, Action onConfirm);
}
