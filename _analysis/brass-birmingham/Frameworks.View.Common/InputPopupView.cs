using System;
using InterfaceAdapters.Common;
using TMPro;
using UnityEngine.UI;

namespace Frameworks.View.Common;

public class InputPopupView : BaseView, IInputPopupView, IBaseView
{
	private TextMeshProUGUI _questionText;

	private TextMeshProUGUI _confirmButtonText;

	private TextMeshProUGUI _cancelButtonText;

	private Button _confirmButton;

	private Button _cancelButton;

	private TMP_InputField _inputField;

	private TextMeshProUGUI _placeholderText;

	public override IController Controller { get; set; }

	public void Setup(string infoText, string confirmButtonText, string cancelButtonText, Action<string> onConfirm, Action onCancel, string basicInputText)
	{
		_questionText.text = infoText;
		_confirmButtonText.text = confirmButtonText;
		_confirmButton.onClick.AddListener(delegate
		{
			onConfirm(_inputField.text);
		});
		_cancelButtonText.text = cancelButtonText;
		_cancelButton.onClick.AddListener(delegate
		{
			onCancel();
		});
		TMP_InputField inputField = _inputField;
		string text = (_placeholderText.text = basicInputText);
		inputField.text = text;
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		_questionText = base.transform.Find("SafeArea/Popup/QuestionText").GetComponent<TextMeshProUGUI>();
		_confirmButtonText = base.transform.Find("SafeArea/Popup/ConfirmButton/Text").GetComponent<TextMeshProUGUI>();
		_confirmButton = base.transform.Find("SafeArea/Popup/ConfirmButton").GetComponent<Button>();
		_cancelButtonText = base.transform.Find("SafeArea/Popup/CancelButton/Text").GetComponent<TextMeshProUGUI>();
		_cancelButton = base.transform.Find("SafeArea/Popup/CancelButton").GetComponent<Button>();
		_inputField = base.transform.Find("SafeArea/Popup/InputField").GetComponent<TMP_InputField>();
		_placeholderText = base.transform.Find("SafeArea/Popup/InputField/Text Area/Placeholder").GetComponent<TextMeshProUGUI>();
	}
}
