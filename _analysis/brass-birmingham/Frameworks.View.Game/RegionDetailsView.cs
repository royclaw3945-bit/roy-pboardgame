using System;
using System.Collections.Generic;
using BoardGameRules.Entities.Board.BuildingSlots;
using DG.Tweening;
using Frameworks.Utils;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.RegionDetails;
using InterfaceAdapters.Routing;
using UnityEngine;
using UnityEngine.UI;

namespace Frameworks.View.Game;

public class RegionDetailsView : BaseView, IRegionDetailsView, IBaseView
{
	private RegionDetailsController _controller;

	private GameObject _oneSlotContainer;

	private GameObject _twoSlotsContainer;

	private GameObject _threeSlotsContainer;

	private GameObject _fourSlotsContainer;

	private GameObject _backButtonGameObject;

	private Button _backButton;

	public override IController Controller
	{
		get
		{
			return _controller;
		}
		set
		{
			if (!(value is RegionDetailsController controller))
			{
				throw new ArgumentException("Wrong controller type");
			}
			_controller = controller;
		}
	}

	public void UpdateRegionDetailsView(RegionDetailsViewModel regionDetailsViewModel)
	{
		DeactivateAllContainers();
		Transform container = GetParentForRegion(regionDetailsViewModel.Slots.Count).transform;
		container.gameObject.SetActive(value: true);
		List<int> indexes = GetIndexesList(regionDetailsViewModel.Slots.Count);
		Transform transform = container.Find("RegionSlot");
		if (transform.childCount > 0)
		{
			UnityEngine.Object.Destroy(transform.GetChild(0).gameObject);
		}
		RegionView region = UnityEngine.Object.Instantiate(regionDetailsViewModel.Region as RegionView, container.Find("RegionSlot"));
		region.TurnOffOutlines();
		region.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		DOTween.Sequence().AppendCallback(delegate
		{
			Sequence s = DOTween.Sequence();
			Transform transform2 = container.Find("Divider");
			if ((bool)transform2)
			{
				transform2.localRotation = Quaternion.Euler(0f, 0f, -30f);
				transform2.DOLocalRotate(Vector3.zero, 3f * UIProperties.AnimationDuration).SetEase(Ease.OutCubic);
			}
			for (int i = 0; i < regionDetailsViewModel.Slots.Count; i++)
			{
				if (regionDetailsViewModel.Slots[indexes[i]].Count == 1)
				{
					container.Find("Slot" + indexes[i]).GetChild(1).gameObject.SetActive(value: false);
				}
				s.Insert((float)(10 + i * 5) * UIProperties.FrameDuration, region._buildingSlots[indexes[i]].transform.DOScale(1.3f, 2f * UIProperties.FrameDuration));
				s.Insert((float)(10 + i * 7) * UIProperties.FrameDuration, region._buildingSlots[indexes[i]].transform.DOScale(1f, 4f * UIProperties.FrameDuration).SetEase(Ease.OutQuad));
				for (int j = 0; j < regionDetailsViewModel.Slots[indexes[i]].Count; j++)
				{
					IndustryBuildView component = container.Find("Slot" + indexes[i]).GetChild(j).GetComponent<IndustryBuildView>();
					component.transform.localScale = Vector3.zero;
					component.SetIndustry(regionDetailsViewModel.Slots[indexes[i]][j]);
					component.BuildAction = OnBuildClicked;
					s.Insert((float)(14 + 2 * j + i * 5) * UIProperties.FrameDuration, component.transform.DOScale(1.4f, 8f * UIProperties.FrameDuration).SetEase(Ease.OutBack));
				}
			}
		});
		_backButtonGameObject.SetActive(regionDetailsViewModel.ShowBackButton);
	}

	public void OnBuildClicked(BuildingSlot buildingSlot, BuildingType buildingType)
	{
		_controller.OnBuildClicked(buildingSlot, buildingType);
	}

	public void OnBackButtonClicked()
	{
		_controller.OnBackButtonClicked();
	}

	public override void HandleKeyDown(KeyEnum keyEnum)
	{
		base.HandleKeyDown(keyEnum);
		if (keyEnum == KeyEnum.Escape)
		{
			OnBackButtonClicked();
		}
	}

	private void DeactivateAllContainers()
	{
		_oneSlotContainer.SetActive(value: false);
		_twoSlotsContainer.SetActive(value: false);
		_threeSlotsContainer.SetActive(value: false);
		_fourSlotsContainer.SetActive(value: false);
	}

	private GameObject GetParentForRegion(int numberOfSlots)
	{
		return numberOfSlots switch
		{
			1 => _oneSlotContainer, 
			2 => _twoSlotsContainer, 
			3 => _threeSlotsContainer, 
			4 => _fourSlotsContainer, 
			_ => throw new InvalidOperationException("Wrong number of industries"), 
		};
	}

	private List<int> GetIndexesList(int numberOfSlots)
	{
		return numberOfSlots switch
		{
			1 => new List<int> { 0 }, 
			2 => new List<int> { 1, 0 }, 
			3 => new List<int> { 1, 2, 0 }, 
			4 => new List<int> { 1, 3, 2, 0 }, 
			_ => throw new InvalidOperationException("Wrong number of industries"), 
		};
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		_oneSlotContainer = base.transform.Find("SafeArea/Popup/OneSlot").gameObject;
		_twoSlotsContainer = base.transform.Find("SafeArea/Popup/TwoSlots").gameObject;
		_threeSlotsContainer = base.transform.Find("SafeArea/Popup/ThreeSlots").gameObject;
		_fourSlotsContainer = base.transform.Find("SafeArea/Popup/FourSlots").gameObject;
		_backButtonGameObject = base.transform.Find("SafeArea/Popup/BackButton").gameObject;
		_backButton = _backButtonGameObject.GetComponent<Button>();
		_backButton.onClick.AddListener(OnBackButtonClicked);
	}
}
