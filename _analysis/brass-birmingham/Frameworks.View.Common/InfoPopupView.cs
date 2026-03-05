using System;
using InterfaceAdapters.Common;
using InterfaceAdapters.Routing;
using TMPro;
using UnityEngine.UI;

namespace Frameworks.View.Common;

public class InfoPopupView : BaseView, IInfoPopupView, IBaseView
{
	private TextMeshProUGUI _infoText;

	private TextMeshProUGUI _confirmButtonText;

	protected Button _confirmButton;

	public override IController Controller { get; set; }

	protected override void Start()
	{
		base.Start();
		base.InputRegistration.RegisterKey(KeyEnum.Return, this);
		base.InputRegistration.RegisterKey(KeyEnum.Space, this);
	}

	public void Setup(string infoText, string confirmButtonText, Action onConfirm)
	{
		_infoText.text = infoText;
		_confirmButtonText.text = confirmButtonText;
		_confirmButton.onClick.AddListener(delegate
		{
			onConfirm();
		});
	}

	public override void HandleKeyDown(KeyEnum keyEnum)
	{
		base.HandleKeyDown(keyEnum);
		if (keyEnum == KeyEnum.Return || keyEnum == KeyEnum.Escape || keyEnum == KeyEnum.Space)
		{
			_confirmButton.onClick.Invoke();
			_confirmButton.onClick.RemoveAllListeners();
		}
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		_infoText = base.transform.Find("SafeArea/Popup/InfoText").GetComponent<TextMeshProUGUI>();
		_confirmButtonText = base.transform.Find("SafeArea/Popup/ConfirmButton/Text").GetComponent<TextMeshProUGUI>();
		_confirmButton = base.transform.Find("SafeArea/Popup/ConfirmButton").GetComponent<Button>();
	}
}
