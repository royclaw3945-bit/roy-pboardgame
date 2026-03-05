using System;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.FinishedGameReview;

namespace Frameworks.View.Game.FinishedGameReview;

public class FinishedGameReviewView : BaseView, IFinishedGameReviewView, IBaseView
{
	private FinishedGameReviewController _controller;

	protected override bool ShowImmediately => true;

	public override IController Controller
	{
		get
		{
			return _controller;
		}
		set
		{
			if (!(value is FinishedGameReviewController controller))
			{
				throw new ArgumentException("Wrong controller type");
			}
			_controller = controller;
		}
	}

	public void OnDestroy()
	{
		_controller.OnViewDestroyed();
	}

	public void OnShowGameSummaryTableClicked()
	{
		_controller.OnShowGameSummaryTableClicked();
	}
}
