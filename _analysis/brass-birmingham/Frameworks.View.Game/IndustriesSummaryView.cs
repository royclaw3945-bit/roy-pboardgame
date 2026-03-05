using System;
using System.Collections.Generic;
using Frameworks.View.Common;
using InterfaceAdapters.Common;
using InterfaceAdapters.Game.IndustriesSummary;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Frameworks.View.Game;

public class IndustriesSummaryView : BaseView, IIndustriesSummaryView, IBaseView
{
	private IndustriesSummaryController _controller;

	private Transform _industriesContainer;

	private TextMeshProUGUI _linkTilesCountText;

	private Image _linkTilesCountTokenImage;

	protected override bool ShowImmediately => true;

	public override IController Controller
	{
		get
		{
			return _controller;
		}
		set
		{
			if (!(value is IndustriesSummaryController controller))
			{
				throw new ArgumentException("Wrong controller type");
			}
			_controller = controller;
		}
	}

	public void UpdateIndustriesSummary(List<IndustryViewModel> industryViewModels, LinkCountViewModel linkCountViewModel)
	{
		for (int i = 0; i < _industriesContainer.childCount; i++)
		{
			_industriesContainer.GetChild(i).GetComponent<IndustryView>().SetIndustry(industryViewModels[i]);
		}
		_linkTilesCountTokenImage.sprite = Resources.Load<Sprite>(string.Concat("Board/Links/Token", linkCountViewModel.GameEra, linkCountViewModel.Color));
		_linkTilesCountText.text = string.Concat(linkCountViewModel.LinkCount);
	}

	public void OnIndustryClicked()
	{
		_controller.OnIndustryClicked();
	}

	public void OnMouseEnterCost(int industryFromTop)
	{
		Vector3 position = _industriesContainer.GetChild(industryFromTop).Find("LeftPanel/CostMoney").position;
		_controller.OnMouseEnter("Cost", position.x, position.y);
	}

	public void OnMouseEnterCoal(int industryFromTop)
	{
		Vector3 position = _industriesContainer.GetChild(industryFromTop).Find("LeftPanel/ResourcesCost/Coal").position;
		_controller.OnMouseEnter("Coal", position.x, position.y);
	}

	public void OnMouseEnterIron(int industryFromTop)
	{
		Vector3 position = _industriesContainer.GetChild(industryFromTop).Find("LeftPanel/ResourcesCost/Iron").position;
		_controller.OnMouseEnter("Iron", position.x, position.y);
	}

	public void OnMouseEnterVP(int industryFromTop)
	{
		Vector3 position = _industriesContainer.GetChild(industryFromTop).Find("RightPanel/VP").position;
		_controller.OnMouseEnter("VP", position.x - (float)(Screen.width / 15), position.y);
	}

	public void OnMouseEnterIncome(int industryFromTop)
	{
		Vector3 position = _industriesContainer.GetChild(industryFromTop).Find("RightPanel/Income").position;
		_controller.OnMouseEnter("Income", position.x - (float)(Screen.width / 15), position.y);
	}

	public void OnMouseEnterVPForLinks(int industryFromTop)
	{
		Vector3 position = _industriesContainer.GetChild(industryFromTop).Find("RightPanel/VPForLinks").position;
		_controller.OnMouseEnter("VPForLinks", position.x - (float)(Screen.width / 15), position.y);
	}

	public void OnMouseEnterCanalEra(int industryFromTop)
	{
		Vector3 position = _industriesContainer.GetChild(industryFromTop).Find("LeftPanel/EraCanal").position;
		_controller.OnMouseEnter("CanalEra", position.x, position.y);
	}

	public void OnMouseEnterRailEra(int industryFromTop)
	{
		Vector3 position = _industriesContainer.GetChild(industryFromTop).Find("LeftPanel/EraRail").position;
		_controller.OnMouseEnter("RailEra", position.x, position.y);
	}

	public void OnMouseEnterIndustry(int industryFromTop)
	{
		Vector3 position = _industriesContainer.GetChild(industryFromTop).Find("BlackBackground").position;
		string element = "ERROR";
		switch (industryFromTop)
		{
		case 0:
			element = "Brewery";
			break;
		case 1:
			element = "CoalMine";
			break;
		case 2:
			element = "IronWorks";
			break;
		case 3:
			element = "CottonMill";
			break;
		case 4:
			element = "Manufacture";
			break;
		case 5:
			element = "Pottery";
			break;
		}
		_controller.OnMouseEnter(element, position.x - (float)(Screen.width / 30), position.y);
	}

	public void OnMouseExit()
	{
		_controller.OnMouseExit();
	}

	protected override void PrepareReferences()
	{
		base.PrepareReferences();
		_industriesContainer = base.transform.Find("SafeArea/Popup/RightPanelBackground/IndustriesContainer");
		_linkTilesCountText = base.transform.Find("SafeArea/Popup/RightPanelBackground/IndustryLinksIndicator/BlackBackground/BackgroundLinksCount/LinksCountText").GetComponent<TextMeshProUGUI>();
		_linkTilesCountTokenImage = base.transform.Find("SafeArea/Popup/RightPanelBackground/IndustryLinksIndicator/BlackBackground/Token").GetComponent<Image>();
	}
}
