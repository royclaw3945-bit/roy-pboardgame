using System;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using InterfaceAdapters.Menu.Login;
using InterfaceAdapters.Routing;
using TMPro;
using UnityEngine.EventSystems;

namespace Frameworks.View.Menu.Online;

public class LoginView : BaseView, ILoginView, IBaseView
{
	private TMP_InputField _emailInputField;

	private TMP_InputField _passwordInputField;

	private LoginController _controller;

	public override IController Controller
	{
		get
		{
			return _controller;
		}
		set
		{
			if (!(value is LoginController controller))
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
		_passwordInputField = base.transform.Find("SafeArea/Popup/PasswordInputField").GetComponent<TMP_InputField>();
	}

	public void OnLoginClicked()
	{
		_controller.LoginClicked(_emailInputField.text, _passwordInputField.text);
	}

	public void OnCreateClicked()
	{
		_controller.CreateClicked();
	}

	public void OnForgotPasswordClicked()
	{
		_controller.ForgotPasswordClicked();
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
			OnLoginClicked();
			break;
		}
	}

	private void MoveFocus()
	{
		if (_emailInputField.isFocused)
		{
			EventSystem.current.SetSelectedGameObject(_passwordInputField.gameObject);
		}
		else if (_passwordInputField.isFocused)
		{
			EventSystem.current.SetSelectedGameObject(_emailInputField.gameObject);
		}
	}
}
