using System;
using DG.Tweening;
using Frameworks.View.Animation;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using InterfaceAdapters.Menu.MainMenu;
using UnityEngine;

namespace Frameworks.View.Menu;

public class SplashScreenView : BaseView, ISplashScreenView, IBaseView
{
	private SplashScreenController _controller;

	public override IController Controller
	{
		get
		{
			return _controller;
		}
		set
		{
			if (!(value is SplashScreenController controller))
			{
				throw new ArgumentException("Wrong controller type");
			}
			_controller = controller;
		}
	}

	public override IAnimation CreateShowViewAnimation()
	{
		EnableView();
		Sequence sequence = DOTween.Sequence();
		sequence.AppendInterval(1f);
		sequence.Append(Popup.GetComponent<CanvasGroup>().DOFade(0f, 0.3f));
		sequence.OnComplete(delegate
		{
			_controller.OnSplashPresentationCompleted();
		});
		return new Frameworks.View.Animation.DOTweenAnimation(sequence);
	}
}
