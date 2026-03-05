using System;
using DG.Tweening;
using Frameworks.View.Animation;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using InterfaceAdapters.Menu.Connection;
using UnityEngine;

namespace Frameworks.View.Menu.Online;

public class ConnectionView : BaseView, IConnectionView, IBaseView
{
	private ConnectionController _controller;

	public override IController Controller
	{
		get
		{
			return _controller;
		}
		set
		{
			if (!(value is ConnectionController controller))
			{
				throw new ArgumentException("Wrong controller type");
			}
			_controller = controller;
		}
	}

	public override IAnimation CreateShowViewAnimation()
	{
		if (ShowImmediately)
		{
			return CreateShowViewImmediatelyAnimation();
		}
		_isViewVisible = true;
		EnableView();
		CanvasGroup canvasGroup = Popup.GetComponent<CanvasGroup>();
		canvasGroup.alpha = 0f;
		canvasGroup.interactable = true;
		Popup.GetComponent<RectTransform>();
		Sequence sequence = DOTween.Sequence();
		if (Background != null)
		{
			sequence.Append(Background.DOFade(1f, 0.5f));
		}
		sequence.Insert(0.3f, canvasGroup.DOFade(1f, 0.5f));
		sequence.AppendCallback(delegate
		{
			canvasGroup.blocksRaycasts = true;
		});
		sequence.AppendCallback(delegate
		{
			ShowViewCompleted();
		});
		return new Frameworks.View.Animation.DOTweenAnimation(sequence);
	}
}
