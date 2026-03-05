using System;
using BoardGameRules.Utils.Logging;
using DG.Tweening;
using Frameworks.Utils;
using Frameworks.View.Animation;
using InterfaceAdapters.Common;
using InterfaceAdapters.Routing;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Frameworks.View.Common;

public abstract class BaseView : MonoBehaviour, IBaseView
{
	public GameObject FadeViewPrefab;

	protected GameObject FadeViewGameObject;

	protected GameObject Popup;

	protected Image Background;

	protected bool _isViewVisible;

	protected ILog Logger;

	public IInputRegistration InputRegistration { get; set; }

	public IFadeView FadeView { get; set; }

	public bool DisableMenusUnderneath { get; set; }

	public bool DestroyWhenClosed { get; set; }

	public abstract IController Controller { get; set; }

	protected virtual bool ShowImmediately { get; }

	protected virtual void Awake()
	{
		PrepareReferences();
	}

	protected virtual void Start()
	{
		InputRegistration.RegisterKey(KeyEnum.Escape, this);
	}

	public void EnableView()
	{
		base.gameObject.SetActive(value: true);
	}

	public void DisableView()
	{
		base.gameObject.SetActive(value: false);
	}

	public virtual IAnimation CreateShowViewAnimation()
	{
		if (ShowImmediately)
		{
			return CreateShowViewImmediatelyAnimation();
		}
		_isViewVisible = true;
		EnableView();
		InitialViewSetup();
		RectTransform component = Popup.GetComponent<RectTransform>();
		CanvasGroup canvasGroup = Popup.GetComponent<CanvasGroup>();
		Sequence sequence = DOTween.Sequence();
		if (Background != null)
		{
			sequence.Append(Background.DOFade(1f, 0.5f));
		}
		sequence.Insert(0.3f, component.DOAnchorPos(Vector2.zero, 0.5f)).Join(canvasGroup.DOFade(1f, 0.5f)).Join(component.DOScale(1f, 0.5f));
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

	public virtual IAnimation CreateHideViewAnimation()
	{
		_isViewVisible = false;
		RectTransform component = Popup.GetComponent<RectTransform>();
		CanvasGroup canvasGroup = Popup.GetComponent<CanvasGroup>();
		Sequence sequence = DOTween.Sequence();
		sequence.AppendCallback(delegate
		{
			canvasGroup.blocksRaycasts = false;
		});
		sequence.Append(component.DOAnchorPos(Vector2.zero, 0.3f));
		sequence.Insert(0f, canvasGroup.DOFade(0f, 0.3f));
		sequence.AppendCallback(delegate
		{
			canvasGroup.interactable = false;
		});
		sequence.AppendCallback(delegate
		{
			HideViewCompleted();
		});
		if (Background != null)
		{
			sequence.Append(Background.DOFade(0f, 0.5f));
		}
		return new Frameworks.View.Animation.DOTweenAnimation(sequence);
	}

	public virtual IAnimation CreateShowViewImmediatelyAnimation()
	{
		_isViewVisible = true;
		EnableView();
		RectTransform rectTransform = Popup.GetComponent<RectTransform>();
		CanvasGroup canvasGroup = Popup.GetComponent<CanvasGroup>();
		Sequence sequence = DOTween.Sequence();
		sequence.AppendCallback(delegate
		{
			if (Background != null)
			{
				Color color = Background.color;
				Background.color = new Color(color.r, color.g, color.b, 0f);
			}
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
			canvasGroup.alpha = 1f;
			rectTransform.anchoredPosition = Vector2.zero;
			ShowViewCompleted();
		});
		return new Frameworks.View.Animation.DOTweenAnimation(sequence).Play();
	}

	public virtual void SetInteractable(bool isInteractable)
	{
		Popup.GetComponent<CanvasGroup>().interactable = isInteractable;
	}

	protected virtual void ShowViewCompleted()
	{
	}

	protected virtual void HideViewCompleted()
	{
	}

	public bool IsViewVisible()
	{
		return _isViewVisible;
	}

	public void MoveToTop()
	{
		base.transform.SetAsLastSibling();
	}

	public void MoveToBottom()
	{
		base.transform.SetAsFirstSibling();
	}

	protected virtual void InitialViewSetup()
	{
		if (Background != null)
		{
			Color color = Background.color;
			Background.color = new Color(color.r, color.g, color.b, 0f);
		}
		RectTransform component = Popup.GetComponent<RectTransform>();
		component.anchoredPosition += Vector2.up * 50f;
		component.localScale = Vector3.one * 0.95f;
		CanvasGroup component2 = Popup.GetComponent<CanvasGroup>();
		component2.alpha = 0f;
		component2.interactable = true;
	}

	public virtual void HandleKeyDown(KeyEnum keyEnum)
	{
	}

	public virtual void HandleKey(KeyEnum keyEnum)
	{
	}

	public virtual void HandleKeyUp(KeyEnum keyEnum)
	{
	}

	protected virtual void PrepareReferences()
	{
		Logger = LogFactory.BuildLogger(GetType());
		DisableMenusUnderneath = false;
		DestroyWhenClosed = true;
		InputRegistration = Global.Instance.Router;
		if ((bool)base.transform.Find("SafeArea/Popup"))
		{
			Popup = base.transform.Find("SafeArea/Popup").gameObject;
		}
		if ((bool)base.transform.Find("Popup"))
		{
			Popup = base.transform.Find("Popup").gameObject;
		}
		if ((bool)base.transform.Find("SafeArea/Background"))
		{
			Background = base.transform.Find("SafeArea/Background").gameObject.GetComponent<Image>();
		}
		if (FadeViewPrefab != null)
		{
			FadeViewGameObject = UnityEngine.Object.Instantiate(FadeViewPrefab);
			FadeViewGameObject.transform.SetParent(base.gameObject.transform);
			FadeViewGameObject.transform.SetAsFirstSibling();
			FadeViewGameObject.transform.localPosition = Vector3.zero;
			FadeViewGameObject.GetComponent<RectTransform>().sizeDelta = GetComponent<RectTransform>().sizeDelta;
			FadeView = FadeViewGameObject.GetComponent<FadeView>();
		}
	}

	private void SetPivot(RectTransform rectTransform, Vector2 pivot)
	{
		if (!(rectTransform == null))
		{
			Vector2 size = rectTransform.rect.size;
			Vector2 vector = rectTransform.localScale;
			Vector2 vector2 = rectTransform.pivot - pivot;
			Vector3 vector3 = new Vector3(vector2.x * size.x * vector.x, vector2.y * size.y * vector.y);
			rectTransform.pivot = pivot;
			rectTransform.localPosition -= vector3;
		}
	}

	private void SetAnchor(RectTransform rectTransform, Vector2 anchor)
	{
		if (!(rectTransform == null))
		{
			Vector2 vector = (rectTransform.sizeDelta = rectTransform.transform.parent.GetComponent<RectTransform>().rect.size);
			Vector2 vector2 = rectTransform.anchorMax - anchor;
			rectTransform.anchorMax = anchor;
			rectTransform.anchorMin = anchor;
			rectTransform.anchoredPosition += new Vector2(vector2.x * vector.x, vector2.y * vector.y);
		}
	}

	protected void ChangeAnchorsToCenter(RectTransform rt)
	{
		SetAnchor(rt, new Vector2(0.5f, 0.5f));
		SetPivot(rt, new Vector2(0.5f, 0.5f));
	}

	protected void ChangeAnchorsToCenterBottom(RectTransform rt)
	{
		SetAnchor(rt, new Vector2(0.5f, 0f));
		SetPivot(rt, new Vector2(0.5f, 0f));
	}

	protected void ChangeAnchorsToNearMiddleRight(RectTransform rt)
	{
		SetAnchor(rt, new Vector2(0.8f, 0.5f));
		SetPivot(rt, new Vector2(1f, 0.5f));
	}

	protected void ChangeAnchorsToNearMiddleLeft(RectTransform rt)
	{
		SetAnchor(rt, new Vector2(0.2f, 0.5f));
		SetPivot(rt, new Vector2(0f, 0.5f));
	}

	protected void FadeOutAndDisable(CanvasGroup cg)
	{
		FadeOutAndDisable(cg, delegate
		{
		});
	}

	protected void FadeOutAndDisable(CanvasGroup cg, Action onComplete)
	{
		cg.DOFade(0f, UIProperties.AnimationDuration).OnComplete(delegate
		{
			onComplete();
		});
		cg.blocksRaycasts = false;
		cg.interactable = false;
	}

	protected void FadeOutAndDisableImmediately(CanvasGroup cg)
	{
		cg.alpha = 0f;
		cg.blocksRaycasts = false;
		cg.interactable = false;
	}

	protected void FadeInAndEnable(CanvasGroup cg)
	{
		FadeInAndEnable(cg, delegate
		{
		});
	}

	protected void FadeInAndEnable(CanvasGroup cg, Action onComplete)
	{
		cg.DOFade(1f, UIProperties.AnimationDuration).OnComplete(delegate
		{
			onComplete();
		});
		cg.blocksRaycasts = true;
		cg.interactable = true;
	}
}
