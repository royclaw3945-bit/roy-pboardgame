using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Cards;
using DG.Tweening;
using Frameworks.Utils;
using Frameworks.View.Animation;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.HandOfCards;
using UnityEngine;
using UnityEngine.UI;

public class HandOfCardsView : BaseView, IHandOfCardsView, IBaseView
{
	public float _baseCardsScale = 0.4f;

	protected float _baseYPosition = 30f;

	private float _moveOnHoverY = 80f;

	private const float yBonusOnHighlight = 250f;

	private AudioSource _audioSource;

	public AudioClip CardDiscardedClip;

	private HandOfCardsController _controller;

	private Transform _cardsContainer;

	protected RectTransform _cardsContainerRectTransform;

	private HorizontalLayoutGroup _cardsContainerLayoutGroup;

	[SerializeField]
	private LocationCardView _locationCardPrefab;

	[SerializeField]
	private IndustryCardView _industryCardPrefab;

	[SerializeField]
	private WildLocationCardView _wildLocationCardPrefab;

	[SerializeField]
	private WildIndustryCardView _wildIndustryCardPrefab;

	[SerializeField]
	private int DefaultCards;

	[SerializeField]
	private float SpacingOffset;

	[SerializeField]
	private float InitialSpacing;

	protected override bool ShowImmediately => true;

	public override IController Controller
	{
		get
		{
			return _controller;
		}
		set
		{
			if (!(value is HandOfCardsController controller))
			{
				throw new ArgumentException("Wrong controller type");
			}
			_controller = controller;
		}
	}

	public void UpdateHandCardsView(List<BaseCard> cards, bool shouldHideCards)
	{
		DestroyAllCards();
		foreach (BaseCard card in cards)
		{
			BaseCardView baseCardView = CreateGameObjectOfCard(card, shouldHideCards);
			baseCardView.Awake();
			baseCardView.SetCard(card, shouldHideCards);
		}
		ScaleCards(cards.Count);
	}

	public void EnableCardsLayoutGroup()
	{
		_cardsContainerLayoutGroup.enabled = true;
	}

	public void HighlightCards(List<BaseCard> validCardList)
	{
		BaseCardView[] componentsInChildren = _cardsContainerRectTransform.GetComponentsInChildren<BaseCardView>();
		foreach (BaseCardView card in componentsInChildren)
		{
			if (validCardList.Any((BaseCard c) => c.UniqueId.Equals(card.BaseCard.UniqueId)))
			{
				card.bonusY = 250f;
				card.HolderObject.GetComponent<RectTransform>().DOAnchorPosY(_baseYPosition + card.bonusY, UIProperties.AnimationDuration);
			}
		}
	}

	public void PlayCardDiscardedClip()
	{
		_audioSource.PlayOneShot(CardDiscardedClip);
	}

	public void ShowCards()
	{
		Sequence sequence = DOTween.Sequence();
		sequence.OnStart(delegate
		{
			for (int i = 0; i < _cardsContainerRectTransform.childCount; i++)
			{
				Transform child = _cardsContainerRectTransform.GetChild(i);
				LockCardInteractions(child.gameObject);
			}
		});
		for (int num = _cardsContainerRectTransform.childCount - 1; num >= 0; num--)
		{
			BaseCardView component = _cardsContainerRectTransform.GetChild(num).GetComponent<BaseCardView>();
			component.bonusY = 250f;
			sequence.Insert((float)(_cardsContainerRectTransform.childCount - num) * UIProperties.FrameDuration, component.HolderObject.GetComponent<RectTransform>().DOAnchorPosY(_baseYPosition + component.bonusY, 18f * UIProperties.FrameDuration).SetEase(Ease.InBack));
		}
		sequence.OnKill(delegate
		{
			for (int i = 0; i < _cardsContainerRectTransform.childCount; i++)
			{
				Transform child = _cardsContainerRectTransform.GetChild(i);
				child.GetComponent<BaseCardView>().bonusY = 250f;
				UnlockCardInteractions(child.gameObject);
			}
		});
	}

	public void HideCards()
	{
		Sequence sequence = DOTween.Sequence();
		sequence.OnStart(delegate
		{
			for (int i = 0; i < _cardsContainerRectTransform.childCount; i++)
			{
				Transform child = _cardsContainerRectTransform.GetChild(i);
				LockCardInteractions(child.gameObject);
			}
		});
		for (int num = _cardsContainerRectTransform.childCount - 1; num >= 0; num--)
		{
			BaseCardView component = _cardsContainerRectTransform.GetChild(num).GetComponent<BaseCardView>();
			component.bonusY = 0f;
			sequence.Insert(0.3f + (float)(_cardsContainerRectTransform.childCount - num) * UIProperties.FrameDuration, component.HolderObject.GetComponent<RectTransform>().DOAnchorPosY(_baseYPosition + component.bonusY, 18f * UIProperties.FrameDuration).SetEase(Ease.InBack));
		}
		sequence.OnComplete(delegate
		{
			for (int i = 0; i < _cardsContainerRectTransform.childCount; i++)
			{
				Transform child = _cardsContainerRectTransform.GetChild(i);
				UnlockCardInteractions(child.gameObject);
			}
		});
	}

	public Tuple<object, int> GetCardObjectInfo(BaseCard card)
	{
		for (int i = 0; i < _cardsContainer.childCount; i++)
		{
			BaseCardView component = _cardsContainer.GetChild(i).GetComponent<BaseCardView>();
			if (component.BaseCard.Equals(card))
			{
				return new Tuple<object, int>(component.gameObject, i);
			}
		}
		return null;
	}

	public void TurnOffInteractions()
	{
		BaseCardView[] componentsInChildren = _cardsContainerRectTransform.GetComponentsInChildren<BaseCardView>();
		foreach (BaseCardView obj in componentsInChildren)
		{
			obj.ignoreMouseOver = true;
			obj.GetComponent<CanvasGroup>().interactable = false;
			obj.GetComponent<CanvasGroup>().blocksRaycasts = false;
		}
	}

	protected void LockCardInteractions(GameObject cardObject)
	{
		cardObject.GetComponent<BaseCardView>().ignoreMouseOver = true;
		cardObject.GetComponent<CanvasGroup>().interactable = false;
		cardObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
	}

	protected void UnlockCardInteractions(GameObject cardObject)
	{
		cardObject.GetComponent<BaseCardView>().ignoreMouseOver = false;
		cardObject.GetComponent<CanvasGroup>().interactable = true;
		cardObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
	}

	public IAnimation CreateCardDiscardedAnimation(BaseCard cardToDiscard)
	{
		GameObject discardCard = null;
		for (int i = 0; i < _cardsContainer.childCount; i++)
		{
			GameObject gameObject = _cardsContainer.GetChild(i).gameObject;
			BaseCardView component = gameObject.GetComponent<BaseCardView>();
			LockCardInteractions(gameObject);
			if (component.BaseCard.UniqueId.Equals(cardToDiscard.UniqueId))
			{
				component.SetHiddenStatus(hidden: false);
				discardCard = gameObject;
			}
		}
		Sequence sequence = DOTween.Sequence();
		sequence.AppendInterval(0.1f);
		RectTransform rect = discardCard.GetComponent<BaseCardView>().HolderObject.GetComponent<RectTransform>();
		sequence.AppendCallback(PlayCardDiscardedClip);
		sequence.Append(rect.DOScale(new Vector3(_baseCardsScale, 0f, _baseCardsScale), 9f * UIProperties.FrameDuration));
		sequence.Append(DOTween.To(() => rect.parent.GetComponent<LayoutElement>().preferredWidth, delegate(float x)
		{
			rect.parent.GetComponent<LayoutElement>().preferredWidth = x;
		}, 380f, 0.4f));
		sequence.OnKill(delegate
		{
			for (int j = 0; j < _cardsContainer.childCount; j++)
			{
				GameObject cardObject = _cardsContainer.GetChild(j).gameObject;
				UnlockCardInteractions(cardObject);
			}
			Canvas component2 = GetComponent<Canvas>();
			component2.overrideSorting = false;
			component2.sortingOrder = 0;
			UnityEngine.Object.Destroy(discardCard);
		});
		return new DOTweenAnimationSequence(sequence);
	}

	public IAnimation CreateOtherPlayerCardDiscardedAnimation(BaseCard card)
	{
		Sequence sequence = DOTween.Sequence();
		sequence.AppendCallback(delegate
		{
			for (int i = 0; i < _cardsContainer.childCount; i++)
			{
				GameObject gameObject = _cardsContainer.GetChild(i).gameObject;
				BaseCardView component = gameObject.GetComponent<BaseCardView>();
				LockCardInteractions(gameObject);
				if (component.BaseCard.UniqueId.Equals(card.UniqueId))
				{
					UnityEngine.Object.Destroy(gameObject);
					break;
				}
			}
		});
		BaseCardView cardView = CreateGameObjectOfCard(card, shouldHideCards: true);
		cardView.Awake();
		cardView.SetCard(card, shouldHideCards: false);
		cardView.GetComponent<LayoutElement>().ignoreLayout = true;
		sequence.Append(ShortcutExtensions.DOMove(endValue: new Vector2(Screen.width / 2, Screen.height / 2), target: cardView.transform, duration: 0.5f));
		sequence.AppendInterval(1.5f);
		Vector2 vector = new Vector2((float)Screen.width * 1.5f, Screen.height);
		sequence.Append(ShortcutExtensions.DOLocalRotate(endValue: new Vector3(0f, 0f, 270f), target: cardView.transform, duration: 0.75f, mode: RotateMode.FastBeyond360)).Join(cardView.transform.DOMove(vector, 0.75f));
		sequence.OnKill(delegate
		{
			UnityEngine.Object.Destroy(cardView.gameObject);
		});
		return new DOTweenAnimationSequence(sequence);
	}

	public (float x, float y, float z) GetCardPosition(int cardNumber)
	{
		Vector3 position = _cardsContainer.GetChild(cardNumber).position;
		return (x: position.x, y: position.y, z: position.z);
	}

	private void DestroyAllCards()
	{
		for (int num = _cardsContainer.childCount - 1; num >= 0; num--)
		{
			UnityEngine.Object.DestroyImmediate(_cardsContainer.GetChild(num).gameObject);
		}
	}

	private BaseCardView CreateGameObjectOfCard(BaseCard card, bool shouldHideCards)
	{
		BaseCardView baseCardView = null;
		if (card != null)
		{
			if (!(card is LocationCard))
			{
				if (!(card is IndustryCard))
				{
					if (!(card is WildLocationCard))
					{
						if (card is WildIndustryCard)
						{
							baseCardView = UnityEngine.Object.Instantiate(_wildIndustryCardPrefab, _cardsContainer);
						}
					}
					else
					{
						baseCardView = UnityEngine.Object.Instantiate(_wildLocationCardPrefab, _cardsContainer);
					}
				}
				else
				{
					baseCardView = UnityEngine.Object.Instantiate(_industryCardPrefab, _cardsContainer);
				}
			}
			else
			{
				baseCardView = UnityEngine.Object.Instantiate(_locationCardPrefab, _cardsContainer);
				((LocationCardView)baseCardView).SetGetFriendlyNameFunc(_controller.GetRegionFriendlyName);
			}
		}
		baseCardView.transform.localScale = Vector3.one * _baseCardsScale;
		if (!shouldHideCards)
		{
			baseCardView.OnMouseEnterAction = OnCardMouseEnter;
			baseCardView.OnMouseExitAction = OnCardMouseExit;
			baseCardView.OnMouseClickedAction = OnCardClicked;
		}
		return baseCardView;
	}

	private void OnCardMouseEnter(BaseCardView view)
	{
		Sequence sequence = DOTween.Sequence();
		sequence.Append(view.HolderObject.GetComponent<RectTransform>().DOAnchorPosY(_baseYPosition + _moveOnHoverY + view.bonusY, UIProperties.AnimationDuration));
		sequence.Play();
	}

	private void OnCardMouseExit(BaseCardView view)
	{
		Sequence sequence = DOTween.Sequence();
		sequence.Append(view.HolderObject.GetComponent<RectTransform>().DOAnchorPosY(_baseYPosition + view.bonusY, UIProperties.AnimationDuration));
		sequence.Play();
	}

	private void OnCardClicked(BaseCard card)
	{
		_controller.OnCardClicked(card);
	}

	private void ScaleCards(int cardsCounter)
	{
		int num = DefaultCards - cardsCounter;
		_cardsContainerLayoutGroup.spacing = InitialSpacing + (float)num * SpacingOffset;
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		_cardsContainer = base.transform.Find("SafeArea/Popup/CardsContainer");
		_cardsContainerRectTransform = _cardsContainer.GetComponent<RectTransform>();
		_cardsContainerLayoutGroup = _cardsContainer.GetComponent<HorizontalLayoutGroup>();
		_audioSource = GetComponent<AudioSource>();
	}
}
