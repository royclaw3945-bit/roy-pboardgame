using System;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using InterfaceAdapters.Menu.Register;
using InterfaceAdapters.Routing;
using TMPro;
using UnityEngine.EventSystems;

namespace Frameworks.View.Menu.Online;

public class RegisterView : BaseView, IRegisterView, IBaseView
{
	private TMP_InputField _emailInputField;

	private TMP_InputField _nicknameInputField;

	private TMP_InputField _passwordInputField;

	private TMP_InputField _confirmPasswordInputField;

	private RegisterController _controller;

	public override IController Controller
	{
		get
		{
			return _controller;
		}
		set
		{
			if (!(value is RegisterController controller))
			{
				throw new ArgumentException("Wrong controller type");
			}
			_controller = controller;
		}
	}

	protected override void Start()
	{
		base.Start();
		base.InputRegistration.RegisterKey(KeyEnum.Tab, this);
		base.InputRegistration.RegisterKey(KeyEnum.Return, this);
		EventSystem.current.SetSelectedGameObject(_emailInputField.gameObject);
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		_emailInputField = base.transform.Find("SafeArea/Popup/EmailInputField").GetComponent<TMP_InputField>();
		_nicknameInputField = base.transform.Find("SafeArea/Popup/NicknameInputField").GetComponent<TMP_InputField>();
		_passwordInputField = base.transform.Find("SafeArea/Popup/PasswordInputField").GetComponent<TMP_InputField>();
		_confirmPasswordInputField = base.transform.Find("SafeArea/Popup/ConfirmPasswordInputField").GetComponent<TMP_InputField>();
	}

	public void OnRegisterClicked()
	{
		_controller.RegisterClicked(_emailInputField.text, _nicknameInputField.text, _passwordInputField.text, _confirmPasswordInputField.text);
	}

	public void OnBackClicked()
	{
		_controller.BackClicked();
	}

	public override void HandleKeyDown(KeyEnum keyEnum)
	{
		base.HandleKeyDown(keyEnum);
		switch (keyEnum)
		{
		case KeyEnum.Escape:
			OnBackClicked();
			break;
		case KeyEnum.Tab:
			MoveFocus();
			break;
		case KeyEnum.Return:
			OnRegisterClicked();
			break;
		}
	}

	private void MoveFocus()
	{
		if (_emailInputField.isFocused)
		{
			EventSystem.current.SetSelectedGameObject(_nicknameInputField.gameObject);
		}
		else if (_nicknameInputField.isFocused)
		{
			EventSystem.current.SetSelectedGameObject(_passwordInputField.gameObject);
		}
		else if (_passwordInputField.isFocused)
		{
			EventSystem.current.SetSelectedGameObject(_confirmPasswordInputField.gameObject);
		}
		else if (_confirmPasswordInputField.isFocused)
		{
			EventSystem.current.SetSelectedGameObject(_emailInputField.gameObject);
		}
	}
}
