using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameRules.Entities.Board.Regions;
using InterfaceAdapters.Game.Board;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BorderRegionView : UIBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public RegionName Name;

	private TextMeshProUGUI _nameText;

	private List<MerchantSlotView> _merchantSlotViews;

	private Image _rewardImage;

	private Action<RegionName, float, float> _onHover = delegate
	{
	};

	private Action _onHoverExit = delegate
	{
	};

	private bool _hasBeenSet;

	private new void Awake()
	{
		PrepareReferences();
	}

	public void UpdateRegion(BorderRegionViewModel borderRegionViewModel)
	{
		for (int i = 0; i < borderRegionViewModel.MerchantSlotsViewModels.Count; i++)
		{
			_merchantSlotViews[i].UpdateSlot(borderRegionViewModel.MerchantSlotsViewModels[i]);
		}
		if (!_hasBeenSet)
		{
			_hasBeenSet = true;
			TextMeshProUGUI nameText = _nameText;
			string text = (base.gameObject.name = borderRegionViewModel.Name.ToString());
			nameText.text = text;
			_onHover = borderRegionViewModel.OnHover;
			_onHoverExit = borderRegionViewModel.OnHoverExit;
			_rewardImage.transform.GetChild(0).gameObject.SetActive(value: true);
			switch (borderRegionViewModel.BonusType)
			{
			case BonusType.VP:
				_rewardImage.sprite = Resources.Load<Sprite>("PlayerMat/Industries/IndustryElements/BbIconVpBlank");
				break;
			case BonusType.Develop:
				_rewardImage.sprite = Resources.Load<Sprite>("PlayerMat/Icons/BbIconTech");
				_rewardImage.transform.GetChild(0).gameObject.SetActive(value: false);
				break;
			case BonusType.Income:
				_rewardImage.sprite = Resources.Load<Sprite>("PlayerMat/Icons/bb_icon_income");
				break;
			default:
				_rewardImage.sprite = Resources.Load<Sprite>("PlayerMat/Icons/bb_icon_money_blank");
				break;
			}
		}
	}

	public MerchantSlotView GetMerchantSlot(string uniqueId)
	{
		return _merchantSlotViews.FirstOrDefault((MerchantSlotView m) => m.merchantSlot.UniqueId.Equals(uniqueId));
	}

	private void PrepareReferences()
	{
		_nameText = base.transform.Find("NameText").GetComponent<TextMeshProUGUI>();
		_merchantSlotViews = base.transform.GetComponentsInChildren<MerchantSlotView>().ToList();
		_rewardImage = base.transform.Find("RewardPanel/Reward").GetComponent<Image>();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		_onHover(Name, base.transform.position.x, base.transform.position.y);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		_onHoverExit();
	}
}
