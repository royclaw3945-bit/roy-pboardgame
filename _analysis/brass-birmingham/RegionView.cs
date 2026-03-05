using System;
using System.Collections.Generic;
using BoardGameRules.Entities.Board.Regions;
using DG.Tweening;
using Frameworks.Utils;
using InterfaceAdapters.Game.Board;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RegionView : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	public RegionColor Color;

	public RegionName Name;

	private Image _nameImage;

	private TextMeshProUGUI _nameText;

	public List<GameObject> _buildingSlots;

	private Action<RegionName> _actionOnClick = delegate
	{
	};

	private void Awake()
	{
		PrepareReferences();
	}

	public void UpdateRegion(RegionViewModel regionViewModel, bool highlightUpdate = false)
	{
		SetRegionInformation(regionViewModel.Name, regionViewModel.Color, regionViewModel.FriendlyName);
		if (regionViewModel.RegionsInteractive)
		{
			GetComponent<NonDrawingGraphic>().raycastTarget = true;
			_actionOnClick = regionViewModel.ActionOnClick;
		}
		else
		{
			GetComponent<NonDrawingGraphic>().raycastTarget = false;
			_actionOnClick = delegate
			{
			};
		}
		for (int num = 0; num < regionViewModel.BuildingSlotsViewModels.Count; num++)
		{
			BuildingSlotViewModel buildingSlotViewModel = regionViewModel.BuildingSlotsViewModels[num];
			buildingSlotViewModel.ActionOnRegionClick = ActionOnClickCall;
			_buildingSlots[num].GetComponent<BuildingSlotView>().UpdateSlot(buildingSlotViewModel, highlightUpdate);
		}
	}

	public void TurnOffActionOnClick()
	{
		_actionOnClick = delegate
		{
		};
	}

	public void SetRegionInformation(RegionName name, RegionColor color, string friendlyName)
	{
		_nameImage.color = ColorHelpers.RegionColorToUnityColor(color);
		_nameText.text = friendlyName;
		base.gameObject.name = name.ToString();
	}

	public void TurnOffOutlines()
	{
		foreach (GameObject buildingSlot in _buildingSlots)
		{
			buildingSlot.transform.Find("Outline").GetComponent<Image>().DOFade(0f, 0f);
		}
	}

	private void PrepareReferences()
	{
		_nameImage = base.transform.Find("NameImage").GetComponent<Image>();
		_nameText = base.transform.Find("NameImage/NameText").GetComponent<TextMeshProUGUI>();
		_buildingSlots = new List<GameObject>();
		Transform child = base.transform.Find("BuildingSlots/Row1").GetChild(0);
		if ((bool)child && child.gameObject.activeSelf)
		{
			_buildingSlots.Add(child.gameObject);
		}
		Transform child2 = base.transform.Find("BuildingSlots/Row1").GetChild(1);
		if ((bool)child2 && child2.gameObject.activeSelf)
		{
			_buildingSlots.Add(child2.gameObject);
		}
		Transform child3 = base.transform.Find("BuildingSlots/Row2").GetChild(0);
		if ((bool)child3 && child3.gameObject.activeSelf)
		{
			_buildingSlots.Add(child3.gameObject);
		}
		Transform child4 = base.transform.Find("BuildingSlots/Row2").GetChild(1);
		if ((bool)child4 && child4.gameObject.activeSelf)
		{
			_buildingSlots.Add(child4.gameObject);
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		ActionOnClickCall();
	}

	public void ActionOnClickCall()
	{
		_actionOnClick(Name);
	}
}
