using System;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using InterfaceAdapters.Menu.Online.CreateOnlineGame;
using InterfaceAdapters.Routing;
using TMPro;
using UnityEngine.UI;

namespace Frameworks.View.Menu.Online;

public class InputPrivatePasswordView : BaseView, IInputPrivatePasswordView, IBaseView
{
	private Button _confirmButton;

	private Button _cancelButton;

	private TMP_InputField _inputField;

	private TextMeshProUGUI _placeholderText;

	public override IController Controller { get; set; }

	protected override void Start()
	{
		base.Start();
		base.InputRegistration.RegisterKey(KeyEnum.Return, this);
		base.InputRegistration.RegisterKey(KeyEnum.Space, this);
	}

	public void Setup(Action<string> onConfirm, Action onCancel, string basicInputText)
	{
		_confirmButton.onClick.AddListener(delegate
		{
			onConfirm(_inputField.text);
		});
		_cancelButton.onClick.AddListener(delegate
		{
			onCancel();
		});
		TMP_InputField inputField = _inputField;
		string text = (_placeholderText.text = basicInputText);
		inputField.text = text;
	}

	public override void HandleKeyDown(KeyEnum keyEnum)
	{
		base.HandleKeyDown(keyEnum);
		switch (keyEnum)
		{
		case KeyEnum.Escape:
			_cancelButton.onClick.Invoke();
			_cancelButton.onClick.RemoveAllListeners();
			break;
		case KeyEnum.Return:
		case KeyEnum.Space:
			_confirmButton.onClick.Invoke();
			_confirmButton.onClick.RemoveAllListeners();
			break;
		}
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		_confirmButton = base.transform.Find("SafeArea/Popup/OkButton").GetComponent<Button>();
		_cancelButton = base.transform.Find("SafeArea/Popup/BackButton").GetComponent<Button>();
		_inputField = base.transform.Find("SafeArea/Popup/PasswordInputField").GetComponent<TMP_InputField>();
		_placeholderText = base.transform.Find("SafeArea/Popup/PasswordInputField/Text Area/Placeholder").GetComponent<TextMeshProUGUI>();
	}
}
