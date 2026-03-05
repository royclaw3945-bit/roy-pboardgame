using System;
using System.Collections;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using InterfaceAdapters.Menu.Credits;
using InterfaceAdapters.Routing;
using UnityEngine;
using UnityEngine.UI;

namespace Frameworks.View.Menu;

public class CreditsView : BaseView, ICreditsView, IBaseView
{
	private CreditsController _controller;

	private ScrollRect _scrollRect;

	private readonly float scrollSpeed = 0.05f;

	private bool autoscroll;

	public override IController Controller
	{
		get
		{
			return _controller;
		}
		set
		{
			if (!(value is CreditsController controller))
			{
				throw new ArgumentException("Wrong controller type");
			}
			_controller = controller;
		}
	}

	protected override void Start()
	{
		base.Start();
		_scrollRect.verticalNormalizedPosition = 1f;
	}

	private void Update()
	{
		if (autoscroll)
		{
			_scrollRect.verticalNormalizedPosition -= Time.deltaTime * scrollSpeed;
			if (_scrollRect.verticalNormalizedPosition <= 0f)
			{
				autoscroll = false;
			}
		}
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

	public override IAnimation CreateShowViewAnimation()
	{
		_scrollRect.verticalNormalizedPosition = 1f;
		return base.CreateShowViewAnimation();
	}

	protected override void ShowViewCompleted()
	{
		base.ShowViewCompleted();
		StartCoroutine(StartScrollingCoroutine());
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		_scrollRect = base.transform.Find("SafeArea/Popup/Scroll View").GetComponent<ScrollRect>();
	}

	private IEnumerator StartScrollingCoroutine()
	{
		yield return new WaitForSeconds(1f);
		autoscroll = true;
	}
}
