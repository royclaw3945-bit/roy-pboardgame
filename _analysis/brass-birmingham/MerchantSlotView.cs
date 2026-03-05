using System;
using BoardGameRules.Entities.Board.Merchants;
using DG.Tweening;
using Frameworks.Utils;
using InterfaceAdapters.Game.Board;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteInEditMode]
public class MerchantSlotView : MonoBehaviour
{
	public MerchantSlot merchantSlot;

	private Image _industryImage;

	private GameObject _beerGameObject;

	private Image _outlineImage;

	private Image _outlineBiggerObject;

	private EventTrigger _eventTrigger;

	private Action<MerchantSlot> actionOnClick = delegate
	{
	};

	private Action<MerchantSlot> actionOnHover = delegate
	{
	};

	private void Awake()
	{
		PrepareReferences();
		_eventTrigger.triggers.Clear();
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerClick;
		entry.callback.AddListener(delegate
		{
			actionOnClick(merchantSlot);
		});
		_eventTrigger.triggers.Add(entry);
		EventTrigger.Entry entry2 = new EventTrigger.Entry();
		entry2.eventID = EventTriggerType.PointerEnter;
		entry2.callback.AddListener(delegate
		{
			actionOnHover(merchantSlot);
		});
		_eventTrigger.triggers.Add(entry2);
	}

	public void UpdateSlot(MerchantSlotViewModel viewModel)
	{
		merchantSlot = viewModel.MerchantSlot;
		_industryImage.DOFade(viewModel.Highlighted ? 1f : 0.4f, UIProperties.AnimationDuration);
		_outlineImage.color = new Color(1f, 1f, 1f, viewModel.HighlightedBeer ? 1 : 0);
		if (viewModel.HighlightedBeer)
		{
			StartHighlightResourceAnimation();
		}
		_industryImage.sprite = Resources.Load<Sprite>("Board/tokens/" + viewModel.MerchantTileImageName);
		_beerGameObject.SetActive(viewModel.HasBeer);
		if (viewModel.IsInteractive)
		{
			_eventTrigger.enabled = true;
			actionOnClick = viewModel.ActionOnClick;
			actionOnHover = viewModel.ActionOnHover;
		}
		else
		{
			_eventTrigger.enabled = false;
		}
	}

	public void HideBeer()
	{
		_beerGameObject.SetActive(value: false);
	}

	public Vector3 GetBeerPosition()
	{
		return _beerGameObject.transform.position;
	}

	private void StartHighlightResourceAnimation()
	{
		Sequence sequence = DOTween.Sequence();
		sequence.AppendCallback(delegate
		{
			_outlineBiggerObject.gameObject.SetActive(value: true);
			_outlineImage.DOFade(1f, 0f);
			_outlineBiggerObject.DOFade(1f, 0f);
			_outlineImage.transform.localScale = 2.25f * Vector3.one;
			_outlineImage.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -90f));
			_outlineBiggerObject.transform.localScale = 3.2f * Vector3.one;
			_outlineBiggerObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -45f));
		});
		sequence.Append(_outlineImage.transform.DOScale(1f, 20f * UIProperties.FrameDuration)).Join(_outlineBiggerObject.transform.DOScale(1f, 20f * UIProperties.FrameDuration)).Join(_outlineImage.transform.DORotate(Vector3.zero, 20f * UIProperties.FrameDuration))
			.Join(_outlineBiggerObject.transform.DORotate(Vector3.zero, 20f * UIProperties.FrameDuration));
		sequence.AppendCallback(delegate
		{
			_outlineBiggerObject.gameObject.SetActive(value: false);
		});
		sequence.Play();
	}

	private void PrepareReferences()
	{
		_industryImage = GetComponent<Image>();
		_eventTrigger = GetComponent<EventTrigger>();
		_beerGameObject = base.transform.Find("Barrel").gameObject;
		_outlineImage = base.transform.Find("Barrel/Outline").GetComponent<Image>();
		_outlineBiggerObject = base.transform.Find("Barrel/OutlineBigger").GetComponent<Image>();
	}
}
