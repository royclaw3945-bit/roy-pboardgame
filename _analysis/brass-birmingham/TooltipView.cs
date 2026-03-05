using System;
using System.Numerics;
using DG.Tweening;
using Frameworks.Utils;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipView : BaseView, ITooltipView, IBaseView
{
	private TextMeshProUGUI _infoText;

	private Image _background;

	private Sequence _sequence;

	private RectTransform _popup;

	public override IController Controller { get; set; }

	protected override void Awake()
	{
		base.Awake();
		_sequence = DOTween.Sequence();
	}

	public void ShowTooltip(string text, System.Numerics.Vector3 position, System.Numerics.Vector2 pivot)
	{
		ShowTooltip(delegate
		{
			_infoText.text = text;
			Canvas.ForceUpdateCanvases();
			_background.rectTransform.pivot = new UnityEngine.Vector2(pivot.X, pivot.Y);
			base.gameObject.transform.position = new UnityEngine.Vector3(position.X, position.Y, position.Z - 10f);
		});
	}

	private void ShowTooltip(Action Setup)
	{
		if (_sequence != null)
		{
			_sequence.Kill();
		}
		_sequence = DOTween.Sequence();
		_sequence.AppendCallback(delegate
		{
			HideTooltipInstantly();
			Setup();
			ShowTooltipWithAnimation();
		});
		_sequence.Play();
	}

	public void HideTooltip()
	{
		if (_sequence != null)
		{
			_sequence.Kill();
		}
		_sequence = DOTween.Sequence();
		_sequence.AppendCallback(HideTooltipWithAnimation);
		_sequence.Play();
	}

	private void ShowTooltipWithAnimation()
	{
		GetComponent<CanvasGroup>().DOFade(1f, UIProperties.AnimationDuration);
	}

	private void HideTooltipWithAnimation()
	{
		GetComponent<CanvasGroup>().DOFade(0f, UIProperties.AnimationDuration);
	}

	private void HideTooltipInstantly()
	{
		GetComponent<CanvasGroup>().alpha = 0f;
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		_infoText = base.transform.Find("SafeArea/Popup/Background/Info").GetComponent<TextMeshProUGUI>();
		_background = base.transform.Find("SafeArea/Popup/Background").GetComponent<Image>();
		_popup = base.transform.Find("SafeArea/Popup").GetComponent<RectTransform>();
		base.DestroyWhenClosed = false;
	}
}
