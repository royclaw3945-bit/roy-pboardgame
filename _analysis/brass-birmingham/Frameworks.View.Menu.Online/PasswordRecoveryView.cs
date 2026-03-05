using System;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using InterfaceAdapters.Menu.PasswordRecovery;
using InterfaceAdapters.Routing;
using TMPro;
using UnityEngine.EventSystems;

namespace Frameworks.View.Menu.Online;

public class PasswordRecoveryView : BaseView, IPasswordRecoveryView, IBaseView
{
	private TMP_InputField _emailInputField;

	private PasswordRecoveryController _controller;

	public override IController Controller
	{
		get
		{
			return _controller;
		}
		set
		{
			if (!(value is PasswordRecoveryController controller))
			{
				throw new ArgumentException("Wrong controller type");
			}
			_controller = controller;
		}
	}

	protected override void Start()
	{
		base.Start();
		EventSystem.current.SetSelectedGameObject(_emailInputField.gameObject);
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		_emailInputField = base.transform.Find("SafeArea/Popup/EmailInputField").GetComponent<TMP_InputField>();
	}

	public void OnSendEmailClicked()
	{
		_controller.SendEmailClicked(_emailInputField.text);
	}

	public void OnBackClicked()
	{
		_controller.BackClicked();
	}

	public override void HandleKeyDown(KeyEnum keyEnum)
	{
		base.HandleKeyDown(keyEnum);
		if (keyEnum == KeyEnum.Escape)
		{
			OnBackClicked();
		}
	}
}
