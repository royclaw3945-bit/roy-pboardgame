using System;
using DG.Tweening;
using Frameworks.Utils;
using Frameworks.View.Animation;
using InterfaceAdapters.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Frameworks.View.Common;

public class FadeView : BaseView, IFadeView, IBaseView
{
	private Image FadeImage;

	private CanvasGroup FadeGroup;

	private float duration = UIProperties.AnimationDuration;

	private readonly float fadeValue = UIProperties.DefaultFadeViewFadeValue;

	private Action onComplete = delegate
	{
	};

	private Tweener _tweener;

	public override IController Controller { get; set; }

	protected override void Awake()
	{
		PrepareReferences();
		base.transform.localScale = Vector3.zero;
	}

	protected override void Start()
	{
	}

	public virtual IFadeView FadeIn()
	{
		return Fade(0f, fadeValue);
	}

	public virtual IFadeView FadeOut()
	{
		return Fade(fadeValue, 0f);
	}

	public IFadeView Show()
	{
		base.transform.localScale = Vector3.one;
		return this;
	}

	public IFadeView Hide()
	{
		base.transform.localScale = Vector3.zero;
		return this;
	}

	public IFadeView SetColor(float r, float g, float b, float a)
	{
		FadeImage.color = new Color(r, g, b, a);
		return this;
	}

	public IFadeView SetDuration(float newDuration)
	{
		duration = newDuration;
		return this;
	}

	public IFadeView OnComplete(Action action)
	{
		onComplete = action;
		return this;
	}

	public IAnimation GetAnimation()
	{
		return new Frameworks.View.Animation.DOTweenAnimation(_tweener);
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		FadeImage = base.transform.Find("SafeArea/FadeImage").GetComponent<Image>();
		FadeGroup = base.transform.GetComponent<CanvasGroup>();
	}

	private IFadeView Fade(float startAlpha, float endAlpha)
	{
		if (_tweener != null && _tweener.IsPlaying())
		{
			_tweener.Kill();
		}
		base.transform.localScale = Vector3.one;
		_tweener = FadeGroup.DOFade(endAlpha, duration).SetEase(Ease.InOutQuad).OnComplete(delegate
		{
			if (endAlpha == 0f)
			{
				base.transform.localScale = Vector3.zero;
			}
			onComplete();
			onComplete = delegate
			{
			};
		});
		return this;
	}
}
