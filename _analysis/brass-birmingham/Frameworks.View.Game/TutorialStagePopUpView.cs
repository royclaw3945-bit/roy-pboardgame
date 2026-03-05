using DG.Tweening;
using Frameworks.View.Animation;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using UnityEngine;

namespace Frameworks.View.Game;

public class TutorialStagePopUpView : BaseView
{
	public override IController Controller { get; set; }

	public IAnimation Open()
	{
		if (Popup == null)
		{
			PrepareReferences();
		}
		base.gameObject.SetActive(value: true);
		return CreateShowViewAnimation();
	}

	public IAnimation Close()
	{
		if (Popup == null)
		{
			PrepareReferences();
		}
		return CreateHideViewAnimation().OnComplete(delegate
		{
			base.gameObject.SetActive(value: false);
		});
	}

	public override IAnimation CreateShowViewAnimation()
	{
		_isViewVisible = true;
		EnableView();
		InitialViewSetup();
		RectTransform rectTransform = Popup.GetComponent<RectTransform>();
		CanvasGroup canvasGroup = Popup.GetComponent<CanvasGroup>();
		Sequence sequence = DOTween.Sequence();
		sequence.Append(rectTransform.DOAnchorPos(Vector2.zero, 0f));
		sequence.Append(rectTransform.DOScale(1f, 0f));
		sequence.Append(canvasGroup.DOFade(1f, 0.5f));
		sequence.AppendCallback(delegate
		{
			canvasGroup.blocksRaycasts = true;
		});
		sequence.AppendCallback(delegate
		{
			ShowViewCompleted();
		});
		sequence.OnKill(delegate
		{
			rectTransform.anchoredPosition = Vector2.zero;
			rectTransform.localScale = Vector3.one;
		});
		return new Frameworks.View.Animation.DOTweenAnimation(sequence);
	}

	public override IAnimation CreateHideViewAnimation()
	{
		_isViewVisible = false;
		Popup.GetComponent<RectTransform>();
		CanvasGroup canvasGroup = Popup.GetComponent<CanvasGroup>();
		Sequence sequence = DOTween.Sequence();
		sequence.AppendCallback(delegate
		{
			canvasGroup.blocksRaycasts = false;
		});
		sequence.Insert(0f, canvasGroup.DOFade(0f, 0.3f));
		sequence.AppendCallback(delegate
		{
			canvasGroup.interactable = false;
		});
		sequence.AppendCallback(delegate
		{
			HideViewCompleted();
		});
		sequence.OnKill(delegate
		{
			canvasGroup.alpha = 0f;
		});
		return new Frameworks.View.Animation.DOTweenAnimation(sequence);
	}
}
