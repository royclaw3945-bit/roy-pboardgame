using System;
using System.Collections.Generic;
using DG.Tweening;
using Frameworks.Utils;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.Actions.ScoutAction;
using UnityEngine;
using UnityEngine.UI;

namespace Frameworks.View.Game;

public class ScoutActionView : BaseView, IScoutActionView, IBaseView
{
	private ScoutActionController _controller;

	private Transform _slot0;

	private Transform _slot1;

	private Transform _wildIndustrySlot;

	private Transform _wildLocationSlot;

	private Transform _wildIndustryCard;

	private Transform _wildLocationCard;

	private List<int> _tempIndex = new List<int>();

	private List<Vector3> _tempPosition = new List<Vector3>();

	private Transform oldParent;

	[SerializeField]
	private LocationCardView _locationCardPrefab;

	[SerializeField]
	private IndustryCardView _industryCardPrefab;

	public override IController Controller
	{
		get
		{
			return _controller;
		}
		set
		{
			if (!(value is ScoutActionController controller))
			{
				throw new ArgumentException("Wrong controller type");
			}
			_controller = controller;
		}
	}

	protected override void ShowViewCompleted()
	{
		_slot0.gameObject.SetActive(value: true);
		_slot1.gameObject.SetActive(value: true);
		_wildIndustrySlot.gameObject.SetActive(value: true);
		_wildLocationSlot.gameObject.SetActive(value: true);
		Sequence s = DOTween.Sequence();
		s.Append(_wildIndustryCard.DOMove(_wildIndustrySlot.position, 5f * UIProperties.FrameDuration)).SetEase(Ease.OutQuart);
		s.Insert(2f * UIProperties.FrameDuration, _wildLocationCard.DOMove(_wildLocationSlot.position, 5f * UIProperties.FrameDuration)).SetEase(Ease.OutQuart);
		s.AppendInterval(1.5f);
		s.Append(_slot0.DOScale(0.35f * Vector3.one, 4f * UIProperties.FrameDuration));
		s.Append(_slot0.DOScale(0.3f * Vector3.one, 3f * UIProperties.FrameDuration));
	}

	public void AnimateCardToSlot(int slotIndex, Tuple<object, int> info, Action onCallback)
	{
		GameObject cardGameObject = (GameObject)info.Item1;
		cardGameObject.GetComponent<RectTransform>();
		BaseCardView baseCardView = cardGameObject.GetComponent<BaseCardView>();
		oldParent = cardGameObject.transform.parent;
		oldParent.GetComponent<HorizontalLayoutGroup>().enabled = false;
		_tempIndex.Add(info.Item2);
		_tempPosition.Add(cardGameObject.transform.position);
		Transform slot = ((slotIndex == 0) ? _slot0 : _slot1);
		Sequence sequence = DOTween.Sequence();
		sequence.AppendCallback(delegate
		{
			baseCardView.ignoreMouseOver = true;
		});
		sequence.AppendCallback(delegate
		{
			cardGameObject.transform.SetParent(slot);
		});
		sequence.Append(cardGameObject.transform.DOLocalMove(Vector3.zero, 11f * UIProperties.FrameDuration)).Join(baseCardView.HolderObject.DOLocalMove(Vector3.zero, 11f * UIProperties.FrameDuration));
		sequence.AppendInterval(7f * UIProperties.FrameDuration);
		sequence.Append(cardGameObject.transform.DOScale(Vector3.one, 7f * UIProperties.FrameDuration));
		switch (slotIndex)
		{
		case 0:
			sequence.Append(_wildIndustryCard.DOBlendableMoveBy(new Vector3(-30f, 30f, 0f), 13f * UIProperties.FrameDuration));
			sequence.AppendCallback(delegate
			{
				_slot1.GetChild(0).gameObject.SetActive(value: false);
				_slot1.GetChild(1).gameObject.SetActive(value: true);
			});
			sequence.Append(_slot1.DOScale(0.35f * Vector3.one, 4f * UIProperties.FrameDuration));
			sequence.Append(_slot1.DOScale(0.3f * Vector3.one, 3f * UIProperties.FrameDuration));
			break;
		case 1:
		{
			BaseCardView component = _wildIndustryCard.GetComponent<BaseCardView>();
			BaseCardView component2 = _wildLocationCard.GetComponent<BaseCardView>();
			component.bonusY = 140f;
			component2.bonusY = 140f;
			sequence.AppendCallback(delegate
			{
				_wildIndustryCard.SetParent(oldParent);
				_wildLocationCard.SetParent(oldParent);
				int num = ((_tempIndex[1] < _tempIndex[0]) ? 1 : 0);
				int num2 = ((_tempIndex[0] < _tempIndex[1]) ? 1 : 0);
				_wildIndustryCard.SetSiblingIndex(_tempIndex[0] - num);
				_wildLocationCard.SetSiblingIndex(_tempIndex[1] + num2);
			});
			sequence.Append(_wildLocationCard.DOBlendableMoveBy(new Vector3(-30f, 30f, 0f), 13f * UIProperties.FrameDuration));
			sequence.Append(_wildIndustryCard.DOBlendableMoveBy(new Vector3(0f, 50f, 0f), 6f * UIProperties.FrameDuration)).Join(_wildLocationCard.DOBlendableMoveBy(new Vector3(0f, 50f, 0f), 6f * UIProperties.FrameDuration));
			sequence.Append(_wildIndustryCard.DOMove(_tempPosition[0] - new Vector3(0f, 20f, 0f), 7f * UIProperties.FrameDuration)).Join(_wildLocationCard.DOMove(_tempPosition[1] - new Vector3(0f, 20f, 0f), 7f * UIProperties.FrameDuration)).Join(_wildIndustryCard.DOScale(0.4f * Vector3.one, 6f * UIProperties.FrameDuration))
				.Join(_wildLocationCard.DOScale(0.4f * Vector3.one, 6f * UIProperties.FrameDuration))
				.Join(component.HolderObject.GetComponent<RectTransform>().DOAnchorPosY(component.bonusY + 130f, 6f * UIProperties.FrameDuration))
				.Join(component2.HolderObject.GetComponent<RectTransform>().DOAnchorPosY(component2.bonusY + 130f, 6f * UIProperties.FrameDuration))
				.Join(_slot0.DOScale(Vector3.zero, 6f * UIProperties.FrameDuration))
				.Join(_slot1.DOScale(Vector3.zero, 6f * UIProperties.FrameDuration));
			sequence.Append(_wildIndustryCard.DOMove(_tempPosition[0], 6f * UIProperties.FrameDuration)).Join(_wildLocationCard.DOMove(_tempPosition[1], 6f * UIProperties.FrameDuration));
			sequence.AppendCallback(delegate
			{
				oldParent.GetComponent<HorizontalLayoutGroup>().enabled = true;
			});
			sequence.AppendInterval(1f);
			break;
		}
		}
		sequence.AppendCallback(delegate
		{
			onCallback();
		});
		sequence.Play();
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		_slot0 = base.transform.Find("SafeArea/Popup/Panel/Slot1");
		_slot1 = base.transform.Find("SafeArea/Popup/Panel/Slot2");
		_wildIndustrySlot = base.transform.Find("SafeArea/Popup/Panel/WildIndustrySlot");
		_wildLocationSlot = base.transform.Find("SafeArea/Popup/Panel/WildLocationSlot");
		_wildIndustryCard = _wildIndustrySlot.GetChild(0);
		_wildLocationCard = _wildLocationSlot.GetChild(0);
	}
}
