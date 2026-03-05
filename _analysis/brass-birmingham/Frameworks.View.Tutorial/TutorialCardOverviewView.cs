using DG.Tweening;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using InterfaceAdapters.Tutorials.Tutorial;
using UnityEngine;
using UnityEngine.UI;

namespace Frameworks.View.Tutorial;

public class TutorialCardOverviewView : BaseView, ITutorialCardOverviewView, IBaseView
{
	private Image _industriesOverviewFrame;

	private Image _locationsOverviewFrame;

	private CanvasGroup _industriesOverview;

	private CanvasGroup _locationsOverview;

	public override IController Controller { get; set; }

	public void HighlightLocations()
	{
		_industriesOverviewFrame.DOFade(0f, 1f);
		_industriesOverview.DOFade(0.5f, 1f);
		_locationsOverviewFrame.DOFade(1f, 1f);
		_locationsOverview.DOFade(1f, 1f);
	}

	public void HighlightBoth()
	{
		_industriesOverviewFrame.DOFade(1f, 1f);
		_industriesOverview.DOFade(1f, 1f);
		_locationsOverviewFrame.DOFade(1f, 1f);
		_locationsOverview.DOFade(1f, 1f);
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		_industriesOverview = base.transform.Find("SafeArea/Popup/Industries").GetComponent<CanvasGroup>();
		_industriesOverviewFrame = base.transform.Find("SafeArea/Popup/Industries").GetComponent<Image>();
		_locationsOverview = base.transform.Find("SafeArea/Popup/Locations").GetComponent<CanvasGroup>();
		_locationsOverviewFrame = base.transform.Find("SafeArea/Popup/Locations").GetComponent<Image>();
	}
}
