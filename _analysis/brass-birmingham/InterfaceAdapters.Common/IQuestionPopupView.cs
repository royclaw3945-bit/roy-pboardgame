using System;

namespace InterfaceAdapters.Common;

public interface IQuestionPopupView : IBaseView
{
	void Setup(string infoText, bool isConfirmNegative, string confirmButtonText, string cancelButtonText, Action onConfirm, Action onCancel);
}
