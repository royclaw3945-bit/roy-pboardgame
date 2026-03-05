using System;
using BoardGameRules.Entities.Board.BuildingSlots;
using InterfaceAdapters.Game.IndustriesSummary;
using TMPro;
using UnityEngine;

public class IndustryBuildView : IndustryView
{
	private GameObject _buildButton;

	private TextMeshProUGUI _reasonsText;

	private TextMeshProUGUI _nameText;

	private BuildingSlot _buildingSlot;

	public Action<BuildingSlot, BuildingType> BuildAction = delegate
	{
	};

	public new virtual void Awake()
	{
		PrepareReferences();
	}

	public override void SetIndustry(IndustryViewModel industryViewModel)
	{
		base.SetIndustry(industryViewModel);
		_buildButton.SetActive(industryViewModel.Active);
		_reasonsText.gameObject.SetActive(!industryViewModel.Active);
		_reasonsText.text = industryViewModel.CantBuildReason;
		_nameText.text = industryViewModel.Name;
		_buildingSlot = industryViewModel.BuildingSlot;
		BuildingType = industryViewModel.BuildingType;
	}

	public void OnBuildClicked()
	{
		BuildAction(_buildingSlot, BuildingType);
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		_buildButton = base.transform.Find("BuildButton").gameObject;
		_reasonsText = base.transform.Find("ReasonText").GetComponent<TextMeshProUGUI>();
		_nameText = base.transform.Find("Name").GetComponent<TextMeshProUGUI>();
	}
}
