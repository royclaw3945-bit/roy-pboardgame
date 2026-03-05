using System;
using InterfaceAdapters.Common;
using InterfaceAdapters.Routing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Frameworks.View.Common;

public class QuestionPopupView : BaseView, IQuestionPopupView, IBaseView
{
	private TextMeshProUGUI _questionText;

	private TextMeshProUGUI _confirmButtonText;

	private TextMeshProUGUI _cancelButtonText;

	private Button _confirmButton;

	private Button _cancelButton;

	public override IController Controller { get; set; }

	protected override void Start()
	{
		base.Start();
		base.InputRegistration.RegisterKey(KeyEnum.Return, this);
		base.InputRegistration.RegisterKey(KeyEnum.Space, this);
	}

	public void Setup(string infoText, bool isConfirmNegative, string confirmButtonText, string cancelButtonText, Action onConfirm, Action onCancel)
	{
		_confirmButtonText.color = (isConfirmNegative ? new Color(1f, 0f, 0f) : new Color(6f / 85f, 0.4784314f, 0f));
		_questionText.text = infoText;
		_confirmButtonText.text = confirmButtonText;
		_confirmButton.onClick.AddListener(delegate
		{
			onConfirm();
		});
		_cancelButtonText.text = cancelButtonText;
		_cancelButton.onClick.AddListener(delegate
		{
			onCancel();
		});
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
		_questionText = base.transform.Find("SafeArea/Popup/QuestionText").GetComponent<TextMeshProUGUI>();
		_confirmButtonText = base.transform.Find("SafeArea/Popup/ConfirmButton/Text").GetComponent<TextMeshProUGUI>();
		_confirmButton = base.transform.Find("SafeArea/Popup/ConfirmButton").GetComponent<Button>();
		_cancelButtonText = base.transform.Find("SafeArea/Popup/CancelButton/Text").GetComponent<TextMeshProUGUI>();
		_cancelButton = base.transform.Find("SafeArea/Popup/CancelButton").GetComponent<Button>();
	}
}
