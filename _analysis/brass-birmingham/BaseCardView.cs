using System;
using BoardGameRules.Entities.Cards;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseCardView : MonoBehaviour
{
	public BaseCard BaseCard;

	private GameObject CardCover;

	protected Image BannerImage;

	protected Image BackgroundImage;

	public Transform HolderObject;

	public Action<BaseCardView> OnMouseEnterAction = delegate
	{
	};

	public Action<BaseCardView> OnMouseExitAction = delegate
	{
	};

	public Action<BaseCard> OnMouseClickedAction = delegate
	{
	};

	private bool _isClicked;

	public float bonusY;

	public bool ignoreMouseOver;

	public virtual void Awake()
	{
		PrepareReferences();
	}

	public virtual void SetCard(BaseCard card, bool shouldHideCards)
	{
		BaseCard = card;
		CardCover.SetActive(shouldHideCards);
	}

	protected virtual void PrepareReferences()
	{
		HolderObject = base.transform.Find("Holder");
		CardCover = HolderObject.Find("CardCover").gameObject;
		BannerImage = HolderObject.Find("Banner").GetComponent<Image>();
		BackgroundImage = HolderObject.Find("ImageBackground").GetComponent<Image>();
	}

	public void OnMouseEnter()
	{
		if (!ignoreMouseOver)
		{
			OnMouseEnterAction(this);
		}
	}

	public void SetHiddenStatus(bool hidden)
	{
		CardCover.SetActive(hidden);
	}

	public bool GetHiddenStatus()
	{
		return CardCover.activeSelf;
	}

	public void OnMouseExit()
	{
		if (!ignoreMouseOver)
		{
			OnMouseExitAction(this);
		}
	}

	public void OnMouseClicked()
	{
		if (!ignoreMouseOver)
		{
			OnMouseClickedAction(BaseCard);
		}
	}
}
